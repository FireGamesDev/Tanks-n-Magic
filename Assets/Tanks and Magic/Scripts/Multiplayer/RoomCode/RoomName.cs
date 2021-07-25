using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomName : MonoBehaviour
{
    public TMPro.TMP_Text roomNameDisplay;

    private void Start()
    {
        GetRoomName(PhotonNetwork.CurrentRoom.Name);
    }

    private void GetRoomName(string roomName)
    {
        roomNameDisplay.text = "Room Name: " + roomName;
    }
}
