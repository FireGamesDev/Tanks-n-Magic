using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameHandler : MonoBehaviourPunCallbacks
{
    public Transform greenSpawnPoint;
    public Transform purpleSpawnPoint;

    public GameObject respawnCountdown;
    public GameObject greenTank_PlayerPrefab;
    public GameObject purpleTank_PlayerPrefab;

    public Animator transitionAnim;

    private float differentSpawnPoint = 0f;

    private ServerMessages serverMessagesManager;

    public GameObject toplistContent;
    public GameObject playerPrefab;

    public PhotonView pv;

    private int respawnTime;

    private void Start()
    {
        Chat.ChatInputField = GameObject.Find("ChatInputField");
        Chat.Placeholder = GameObject.Find("Placeholder");

        Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>().showMaskGraphic = false;

        serverMessagesManager = GameObject.Find("ServerMessages").GetComponent<ServerMessages>();
        GameObject[] purplePlayers = GameObject.FindGameObjectsWithTag("PurpleTank");
        GameObject[] greenPlayers = GameObject.FindGameObjectsWithTag("GreenTank");

        if(purplePlayers != null)
        {
            foreach (GameObject player in purplePlayers)
            {
                player.transform.position = new Vector3(purpleSpawnPoint.transform.position.x, purpleSpawnPoint.transform.position.y, purpleSpawnPoint.transform.position.z - differentSpawnPoint);
                differentSpawnPoint += 4f;
            }
        }

        differentSpawnPoint = 0f;

        foreach (GameObject player in greenPlayers)
        {
            player.transform.position = new Vector3(greenSpawnPoint.transform.position.x, greenSpawnPoint.transform.position.y, greenSpawnPoint.transform.position.z - differentSpawnPoint);
            differentSpawnPoint += 4f;
        }

        SetupToplist();
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        serverMessagesManager.LeaveMatch();

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        StartCoroutine(SceneTransition());
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            serverMessagesManager.SendLeaveMessage(otherPlayer.NickName);
            pv.RPC("RemovePlayer", RpcTarget.All, otherPlayer.NickName);

            StartCoroutine(Wait());
        }
        base.OnPlayerLeftRoom(otherPlayer);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(4f);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if (GameObject.FindGameObjectsWithTag("GreenTank").Length == 0)
            {
                photonView.RPC("DieAltar", RpcTarget.All, true);
            }
            if (GameObject.FindGameObjectsWithTag("PurpleTank").Length == 0)
            {
                photonView.RPC("DieAltar", RpcTarget.All, false);
            }
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 &&
            (GameObject.FindGameObjectsWithTag("GreenTank") == null
            || GameObject.FindGameObjectsWithTag("PurpleTank") == null))
        {
            if (GameObject.FindGameObjectsWithTag("GreenTank") == null)
            {
                photonView.RPC("DieAltar", RpcTarget.All, true);
            }
            if (GameObject.FindGameObjectsWithTag("PurpleTank") == null)
            {
                photonView.RPC("DieAltar", RpcTarget.All, false);
            }
        }
    }

    [PunRPC]
    private void DieAltar(bool isGreen)
    {
        if (isGreen)
        {
            GameObject.FindGameObjectWithTag("GreenAltar").GetComponent<Altar>().died = true;
        } else GameObject.FindGameObjectWithTag("PurpleAltar").GetComponent<Altar>().died = true;
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    private IEnumerator SceneTransition()
    {
        transitionAnim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Menu");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Respawn(string playerColor)
    {
        respawnCountdown.SetActive(true);
        StartCoroutine(RespawnTimer(playerColor));
    }

    private IEnumerator RespawnTimer(string playerColor)
    {
        if (playerColor.Equals("green"))
        {
            GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<LevDev.Cameras.LD_TopDown_Camera>().m_Target
                = GameObject.FindGameObjectWithTag("GreenAltar").transform;
        }

        if (playerColor.Equals("purple"))
        {
            GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<LevDev.Cameras.LD_TopDown_Camera>().m_Target
                = GameObject.FindGameObjectWithTag("PurpleAltar").transform;
        }

        TMPro.TMP_Text countdownText = respawnCountdown.GetComponentInChildren<TMPro.TMP_Text>();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            respawnTime = 15;
        }
        else respawnTime = 10;

        for (int i = 0; i < respawnTime; i++)
        {
            countdownText.text = "Respawn in " + (respawnTime - i);
            yield return new WaitForSeconds(1);
        }
        
        respawnCountdown.SetActive(false);

        if (playerColor.Equals("green"))
        {
            float randomValue = Random.Range(-3f, 3f);
            GameObject tank = PhotonNetwork.Instantiate(greenTank_PlayerPrefab.name, new Vector3(greenSpawnPoint.position.x + randomValue, this.transform.position.y, greenSpawnPoint.position.z + randomValue),
            Quaternion.identity, 0);

            tank.GetComponent<Player>().WinOrLose();
        }

        if (playerColor.Equals("purple"))
        {
            float randomValue = Random.Range(-3f, 3f);
            GameObject tank = PhotonNetwork.Instantiate(purpleTank_PlayerPrefab.name, new Vector3(purpleSpawnPoint.position.x + randomValue, this.transform.position.y, purpleSpawnPoint.position.z + randomValue),
            Quaternion.identity, 0);

            tank.GetComponent<Player>().WinOrLose();
        }
    }

    private void ClearTopList()
    {
        Transform contents = toplistContent.transform;
        foreach (Transform go in contents) Destroy(go.gameObject);
    }

    private void SetupToplist()
    {
        ClearTopList();

        Transform contents = toplistContent.transform;

        foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            GameObject newPlayer = Instantiate(playerPrefab, contents) as GameObject;
            newPlayer.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = p.NickName;
            newPlayer.transform.Find("KillCount").GetComponent<TMPro.TMP_Text>().text = "0";
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("Kills", 0);
            p.SetCustomProperties(hash);
        }
    }

    [PunRPC]
    private void RemovePlayer(string nickName)
    {
        Transform contents = toplistContent.transform;

        foreach (Transform o in contents)
        {
            if (o.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text.Equals(nickName))
            {
                Destroy(o.gameObject);
            }
        }
    }
}
