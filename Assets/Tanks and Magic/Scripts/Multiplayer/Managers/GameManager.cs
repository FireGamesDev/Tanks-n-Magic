using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject greenTank_PlayerPrefab;
    public GameObject purpleTank_PlayerPrefab;
    public GameObject startGame_Button;
    public GameObject sceneCamera;
    public GameObject purpleTeam_Button;
    public GameObject greenTeam_Button;

    public Transform greenSpawnPoint;
    public Transform purpleSpawnPoint;

    [SerializeField] public GameObject errorMessage;

    private ServerMessages serverMessagesManager;

    public Animator transitionAnim;

    public PhotonView pv;

    private void Awake()
    {
        Cursor.visible = true;
    }

    private void Start()
    {
        Chat.ChatInputField = GameObject.Find("ChatInputField");
        Chat.Placeholder = GameObject.Find("Placeholder");

        //same time to load the scene
        PhotonNetwork.AutomaticallySyncScene = true;

        serverMessagesManager = GameObject.Find("ServerMessages").GetComponent<ServerMessages>();
    }

    public void IamPurpleTank()
    {
        PlayerPrefs.SetString("IsGreen", "false");
        float randomValue = Random.Range(-5f, 5f);

        PhotonNetwork.Instantiate(purpleTank_PlayerPrefab.name, new Vector3(purpleSpawnPoint.position.x + randomValue, this.transform.position.y, purpleSpawnPoint.position.z + randomValue),
            Quaternion.identity, 0);

        sceneCamera.SetActive(false);
        purpleTeam_Button.SetActive(false);
        greenTeam_Button.SetActive(false);

        pv.RPC("StartButton", RpcTarget.MasterClient);
    }

    public void IamGreenTank()
    {
        PlayerPrefs.SetString("IsGreen", "true");
        float randomValue = Random.Range(-5f, 5f);

        PhotonNetwork.Instantiate(greenTank_PlayerPrefab.name, new Vector3(greenSpawnPoint.position.x + randomValue, this.transform.position.y, greenSpawnPoint.position.z + randomValue),
            Quaternion.identity, 0);

        sceneCamera.SetActive(false);
        purpleTeam_Button.SetActive(false);
        greenTeam_Button.SetActive(false);

        pv.RPC("StartButton", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void StartButton()
    {
        startGame_Button.SetActive(true);
    }

    public void LoadGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            StartCoroutine(SceneTransitionToGame());
        }
        else
        {
            errorMessage.SetActive(true);
            errorMessage.GetComponent<TMPro.TMP_Text>().text = "Minimum 2 Players required to Load Arena!";
            StartCoroutine(HideMessage());
        }
    }

    private IEnumerator SceneTransitionToGame()
    {
        if (transitionAnim)
        {
            transitionAnim.SetTrigger("FadeIn");
        }
        yield return new WaitForSeconds(1.5f);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Aram");
        }
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        serverMessagesManager.LeaveMatch();

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        StartCoroutine(SceneTransition());
    }

    private IEnumerator SceneTransition()
    {
        if (transitionAnim)
        {
            transitionAnim.SetTrigger("FadeIn");
        }
        yield return new WaitForSeconds(1.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(5);
        errorMessage.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                startGame_Button.SetActive(false);
            }
            if (otherPlayer.NickName.Contains("/"))
            {
                string[] names = otherPlayer.NickName.Split('/');
                serverMessagesManager.SendLeaveMessage(names[0]);
                photonView.RPC("RemovePlayer", RpcTarget.All, names[0]);
            }
            else
            {
                serverMessagesManager.SendLeaveMessage(otherPlayer.NickName);
                photonView.RPC("RemovePlayer", RpcTarget.All, otherPlayer.NickName);
            }
        }
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGame_Button.SetActive(false);
        }
        base.OnPlayerEnteredRoom(newPlayer);
    }

    [PunRPC]
    private void RemovePlayer(string nickName)
    {
        Multiplayer.PlayersInLobby.RemovePlayer(nickName);
    }
}
