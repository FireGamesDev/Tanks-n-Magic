using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ping : MonoBehaviour
{
    public TMPro.TMP_Text ping;
    private float time;
    private float maxTime = 5f;

    private void Update()
    {
        time -= Time.deltaTime;
        
        if(time <= 0f)
        {
            ping.text = "Ping: " + PhotonNetwork.GetPing();
            time = maxTime;
        }
    }
}
