using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayersCounter : MonoBehaviour
{
    public static PlayersCounter instance;
    public TMPro.TMP_Text playerCounterText;

    private void Start()
    {
        instance = this;
    }

    public void UpdatePlayersInLobbyCount()
    {
        playerCounterText.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
