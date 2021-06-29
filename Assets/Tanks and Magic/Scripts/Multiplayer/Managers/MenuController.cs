using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject UsernameMenu = null;

    [SerializeField] private TMPro.TMP_InputField UsernameInput = null;
    [SerializeField] private TMPro.TMP_InputField CreateGameInput = null;

    [SerializeField] private GameObject StartButton = null;
    [SerializeField] private GameObject errorText = null;
    [SerializeField] private GameObject connectingText = null;
    [SerializeField] private GameObject playMenu = null;
    [SerializeField] private GameObject createButton = null;
    [SerializeField] private GameObject noRooms = null;

    public Animator transitionAnim;

    private int maxPlayers = 4;

    private void Start()
    {
        UsernameInput.characterLimit = 10;
        CreateGameInput.characterLimit = 10;

        noRooms.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
        connectingText.gameObject.SetActive(false);
        createButton.SetActive(true);
    }

    public void ChangeUserNameInput()
    {
        if(UsernameInput.text.Length >= 3)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
    }

    public void SetUserName()
    {
        UsernameMenu.SetActive(false);
        PlayerPrefs.SetString("Username", UsernameInput.text);
        PhotonNetwork.NickName = PlayerPrefs.GetString("Username");
    }

    public void Play()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (PlayerPrefs.GetString("Username", "Tank").Equals("Tank"))
        {
            UsernameMenu.SetActive(true);
        }
        else
        {
            playMenu.SetActive(true);
            PhotonNetwork.NickName = PlayerPrefs.GetString("Username");
        }
    }

    public void CreateGame()
    {
        if (string.IsNullOrEmpty(CreateGameInput.text))
        {
            return;
        }
        PhotonNetwork.JoinOrCreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = (byte)maxPlayers }, null);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(SceneTransition());
    }

    private IEnumerator SceneTransition()
    {
        transitionAnim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(message.Equals("Game closed"))
        {
            message = "Game already started";
        }
        errorText.GetComponent<TMPro.TMP_Text>().text = "Failed to Join: " + message;
        errorText.SetActive(true);
        StartCoroutine("HideErrorMessage");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.GetComponent<TMPro.TMP_Text>().text = "Room Creation Failed: " + message;
        errorText.SetActive(true);
        StartCoroutine("HideErrorMessage");
    }

    IEnumerator HideErrorMessage()
    {
        yield return new WaitForSeconds(5f);
        errorText.SetActive(false);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();

        connectingText.gameObject.SetActive(true);
        createButton.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    #region RoomList
    public GameObject content;
    public GameObject roomPrefab;
    private List<RoomInfo> roomList;

    private void ClearRoomList()
    {
        Transform contents = content.transform;
        foreach (Transform go in contents) Destroy(go.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_List)
    {
        roomList = p_List;
        ClearRoomList();

        Transform contents = content.transform;

        foreach (RoomInfo room in roomList)
        {
            GameObject newRoomButton = Instantiate(roomPrefab, contents) as GameObject;
            newRoomButton.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = room.Name;
            newRoomButton.transform.Find("Players").GetComponent<TMPro.TMP_Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
            if (room.IsOpen)
            {
                newRoomButton.transform.Find("Status").GetComponent<TMPro.TMP_Text>().text = "Join";
            }
            else newRoomButton.transform.Find("Status").GetComponent<TMPro.TMP_Text>().text = "Running";


            newRoomButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
        }

        if(roomList.Count == 0)
        {
            noRooms.SetActive(true);
        }
        else
        {
            noRooms.SetActive(false);
        }

        base.OnRoomListUpdate(roomList);
    }

    private void JoinRoom(Transform p_button)
    {
        string t_roomName = p_button.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text;
        PhotonNetwork.JoinRoom(t_roomName);
    }
    #endregion
}
