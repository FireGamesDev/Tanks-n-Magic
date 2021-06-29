using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Multiplayer;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public PhotonView photonView;
    public TMPro.TMP_Text username;

    public GameObject chatInputField;

    private GameObject chatFeed;
    private GameObject chatExit;
    private GameObject chatManagerObj;
    private ChatManager chatManager;
    private bool isChatting = false;

    private GameObject winText;
    private GameObject loseText;

    private bool found;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!photonView.IsMine)
        {
            return;
        }

        SetCam();

        username.color = Color.white;

        //PlayersList
        if (photonView.IsMine)
        {
            if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                photonView.RPC("AddNewPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, GetPlayerColor());
            }
        }

        isChatting = false;
    }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            username.text = PhotonNetwork.NickName;
        }
        else
        {
            username.text = photonView.Owner.NickName;
        }
    }

    private void Update()
    {
        if (winText == null || loseText == null)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                winText = GameObject.Find("WinText");
                loseText = GameObject.Find("LoseText");
            }
        }

        if (!photonView.IsMine)
        {
            return;
        }

        if (!found)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                if (photonView.IsMine)
                {
                    SetCam();
                }
                found = true;
            }
        }

        //set the chat
        if (chatInputField != null && chatFeed != null && chatManager != null && chatExit != null)
        {
            if (photonView.IsMine)
            {
                if ((Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl)) && !isChatting)
                {
                    chatManagerObj.SetActive(true);
                    Chat.ChatInputField.SetActive(true);
                    Chat.ChatInputField.GetComponent<TMPro.TMP_InputField>().text = "";
                    Chat.ChatInputField.GetComponent<TMPro.TMP_InputField>().ActivateInputField();
                    chatFeed.GetComponent<Mask>().showMaskGraphic = true;
                    chatExit.GetComponent<Mask>().showMaskGraphic = true;
                    Chat.ChatInputField.GetComponent<Mask>().showMaskGraphic = true;
                    isChatting = true;
                    return;
                }
                if ((Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Escape)) && isChatting && Chat.ChatInputField.GetComponent<Mask>().showMaskGraphic)
                {
                    chatFeed.GetComponent<Mask>().showMaskGraphic = false;
                    chatExit.GetComponent<Mask>().showMaskGraphic = false;
                    Chat.ChatInputField.GetComponent<Mask>().showMaskGraphic = false;
                    Chat.ChatInputField.SetActive(false);
                    isChatting = false;
                    return;
                }
            }
        }
        else
        {
            chatInputField = Chat.ChatInputField;
            if(chatInputField != null)
            {
                chatInputField.GetComponent<Mask>().showMaskGraphic = false;
            }

            chatFeed = GameObject.Find("ChatFeed");
            chatExit = GameObject.Find("HowToExit");

            chatManager = GameObject.Find("ChatManager").GetComponent<ChatManager>();
            chatManagerObj = GameObject.Find("ChatManager");

            chatFeed.GetComponent<Mask>().showMaskGraphic = false;
            chatExit.GetComponent<Mask>().showMaskGraphic = false;
            Chat.Placeholder.GetComponent<Mask>().showMaskGraphic = false;

            if (photonView.IsMine)
            {
                chatManager.GetChatColors(GetPlayerColor());
            }
        }
    }

    private bool GetPlayerColor()
    {
        if (gameObject.CompareTag("GreenTank"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [PunRPC]
    private void AddNewPlayer(string nickname, bool isGreen)
    {
        Color color = Color.magenta;
        if (isGreen)
        {
            color = Color.green;
        }

        StartCoroutine(Wait(nickname, color));
    }

    private IEnumerator Wait(string nickname, Color color)
    {
        while (PlayersInLobby.instance == null)
        {
            yield return new WaitForSeconds(1f);
        }
        PlayersInLobby.instance.AddNewPlayer(nickname, color);
        PlayersCounter.instance.UpdatePlayersInLobbyCount();
    }

    public void Win()
    {
        if (photonView.IsMine)
        {
            if (!winText)
            {
                winText = GameObject.Find("WinText");
            }

            winText.GetComponent<TMPro.TMP_Text>().text = "You won!";
            ParticleSystem[] winParticles = winText.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem p in winParticles)
            {
                p.Play();
            }

            AudioSource[] sfx = winText.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource p in sfx)
            {
                p.Play();
            }

            SetCam();

            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public void Lose()
    {
        if (photonView.IsMine)
        {
            if (!loseText)
            {
                loseText = GameObject.Find("LoseText");
            }
            loseText.GetComponent<TMPro.TMP_Text>().text = "You lost!";
            ParticleSystem[] loseParticles = loseText.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in loseParticles)
            {
                p.Play();
            }

            AudioSource[] sfx = loseText.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource p in sfx)
            {
                p.Play();
            }

            SetCam();

            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public void WinOrLose()
    {
        if (GameObject.FindGameObjectWithTag("GreenAltar").GetComponent<Altar>().died)
        {
            if (gameObject.CompareTag("GreenTank"))
            {
                Lose();
            }
            else Win();
        }
        if (GameObject.FindGameObjectWithTag("PurpleAltar").GetComponent<Altar>().died)
        {
            if (gameObject.CompareTag("PurpleTank"))
            {
                Lose();
            }
            else Win();
        }
    }

    private void SetCam()
    {
        GameObject.FindGameObjectWithTag("MainCamera")
            .GetComponent<LevDev.Cameras.LD_BaseCamera>().m_Target = this.gameObject.transform;

        if (GameObject.FindGameObjectWithTag("PurpleAltar") && GameObject.FindGameObjectWithTag("PurpleAltar").GetComponent<Altar>().died)
        {
            GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<LevDev.Cameras.LD_TopDown_Camera>().m_Target
                = GameObject.FindGameObjectWithTag("PurpleAltar").transform;
        }

        if (GameObject.FindGameObjectWithTag("GreenAltar") && GameObject.FindGameObjectWithTag("GreenAltar").GetComponent<Altar>().died)
        {
            GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<LevDev.Cameras.LD_TopDown_Camera>().m_Target
                = GameObject.FindGameObjectWithTag("GreenAltar").transform;
        }
    }
}
