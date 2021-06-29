using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;

public class ServerMessages : MonoBehaviour, IChatClientListener
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
        Debug.Log("Connected to Chat");

        this.chatClient.Subscribe(currentChannelName);
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
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
        {
            SendWelcomeMessage();
        }
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

    public void OnDisconnected()
    {

    }

    #endregion

    private ChatClient chatClient;

    [SerializeField] private string nickName;
    [SerializeField] private string currentChannelName;

    [SerializeField] TMPro.TMP_Text CurrentChannelText = null;

    private void Awake()
    {
        nickName = PlayerPrefs.GetString("Username");
        currentChannelName = PhotonNetwork.CurrentRoom.Name;
    }

    private void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.MessageLimit = 100;

        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(nickName));
    }

    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
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

    public void LeaveMatch()
    {
        chatClient.Disconnect();
    }

    //Disconnect
    void OnApplicationQuit()
    {
        chatClient.Disconnect();
    }

    #region Server Messages
    public void SendWelcomeMessage()
    {
        SendChatMessage("<color=green> joined to the game</color>");
    }

    public void SendLeaveMessage(string username)
    {
        SendChatMessage("<color=red>" + username + " left from the game</color>");
    }
    #endregion
}
