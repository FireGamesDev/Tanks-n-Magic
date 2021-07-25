using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MinimapFaceToCam : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if(PlayerPrefs.GetString("IsGreen", "false").Equals("false"))
        {
            transform.Rotate(0.0f, 0.0f, 180.0f, Space.Self);
        }
    }
}
