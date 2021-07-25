using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIFaceToCamera : MonoBehaviourPunCallbacks
{
    private GameObject playerCam;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        playerCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if (playerCam)
        {
            transform.LookAt(transform.position + playerCam.transform.rotation * Vector3.forward,
               playerCam.transform.rotation * Vector3.up);
        } else playerCam = GameObject.FindGameObjectWithTag("MainCamera");
    }
}
