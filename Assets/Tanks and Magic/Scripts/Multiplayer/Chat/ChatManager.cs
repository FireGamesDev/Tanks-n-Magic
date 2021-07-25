using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    #region Photon Chat Callbacks

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnConnected()
    {
        this.chatClient.Subscribe(currentChannelName);
    }

    public void OnDisconnected()
    {
        
    }


    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ShowChannel(channelName);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }

    #endregion

    private ChatClient chatClient;

    [SerializeField] private string nickName;
    [SerializeField] private string currentChannelName;

    [SerializeField] TMPro.TMP_InputField chatInput = null;
    [SerializeField] TMPro.TMP_Text CurrentChannelText = null;
    [SerializeField] TMPro.TMP_Text commandSuccess = null;

    private void Awake()
    {
        nickName = PlayerPrefs.GetString("Username");
        currentChannelName = PhotonNetwork.CurrentRoom.Name;
    }

    private void Start()
    {
        Chat.ChatInputField = GameObject.Find("ChatInputField");
        Chat.Placeholder = GameObject.Find("Placeholder");

        //hide the chat, but it isn't inactive
        Chat.ChatInputField.GetComponent<Mask>().showMaskGraphic = false;
        GameObject.Find("HowToExit").GetComponent<Mask>().showMaskGraphic = false;
        GameObject.Find("ChatFeed").GetComponent<Mask>().showMaskGraphic = false;
        Chat.Placeholder.GetComponent<Mask>().showMaskGraphic = false;
    }

    private void Setup()
    {
        Chat.ChatInputField.GetComponent<Mask>().showMaskGraphic = false;
        GameObject.Find("HowToExit").GetComponent<Mask>().showMaskGraphic = false;
        GameObject.Find("ChatFeed").GetComponent<Mask>().showMaskGraphic = false;
        Chat.Placeholder.GetComponent<Mask>().showMaskGraphic = false;

        chatClient = new ChatClient(this);
        chatClient.MessageLimit = 100;

        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(nickName));
    }

    private void Update()
    {
        if(chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SendChatMessage(chatInput.text);
            chatInput.text = "";
            chatInput.GetComponent<TMPro.TMP_InputField>().ActivateInputField();
        }
    }

	private void SendChatMessage(string inputLine)
	{
        if (string.IsNullOrEmpty(inputLine))
		{
			return;
		}
        if (inputLine.StartsWith("/"))
        {
            if (PlayerPrefs.GetString("Username", "Player") != "Beni" && PlayerPrefs.GetString("Username", "Player") != "Levi")
            {
                return;
            }
            inputLine = inputLine.Substring(1);
            string[] texts = inputLine.Split(',');
            string command = texts[0];
            if(command == "givetitle")
            {
                string to = texts[1];
                string title = texts[2];
                GetComponent<PhotonView>().RPC("GiveTitle", RpcTarget.All, to, title);
                commandSuccess.text = to + " is now a " + title;
                commandSuccess.gameObject.SetActive(true);
                GetComponent<PhotonView>().RPC("UpdatePlayerTitle", RpcTarget.AllBuffered, to, title);
                StartCoroutine(hideSuccessMessage());
            }
            return;
        }
        chatClient.PublishMessage(currentChannelName, inputLine);
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel;

        bool found = chatClient.TryGetChannel(channelName, out channel);

        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        CurrentChannelText.text = channel.ToStringMessages();
    }

    public void GetChatColors(bool isGreen)
    {
        Color nameColor = Color.magenta;
        if (isGreen)
        {
            nameColor = Color.green;
        }

        if (nameColor == Color.magenta)
        {
            nickName = "<color=purple>" + PlayerPrefs.GetString("Username") + "</color>";
        }
        else nickName = "<color=green>" + PlayerPrefs.GetString("Username") + "</color>";

        Setup();
    }

    private IEnumerator hideSuccessMessage()
    {
        yield return new WaitForSeconds(5);
        commandSuccess.gameObject.SetActive(false);
    }

    [PunRPC]
    private void GiveTitle(string to, string title)
    {
        if (PlayerPrefs.GetString("Username", "Player") == to)
        {
            PlayerPrefs.SetString("Title", title);
            print(title + " " + to);
        }
    }

    [PunRPC]
    private void UpdatePlayerTitle(string to, string title)
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("GreenTank"))
        {
            if(player.GetComponent<Player>().username.text == to)
            {
                player.GetComponent<Player>().title.text = title;
                return;
            }
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("PurpleTank"))
        {
            if (player.GetComponent<Player>().username.text == to)
            {
                player.GetComponent<Player>().title.text = title;
                return;
            }
        }
    }
}
