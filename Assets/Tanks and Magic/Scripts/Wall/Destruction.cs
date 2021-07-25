using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Destruction : MonoBehaviour
{
    public PhotonView PV;

    public GameObject phase1;
    public GameObject phase2;
    public GameObject destructedVersion;

    private int phase_Counter = 1;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Prewiew>() != null)
        {
            return;
        }

        if (other.gameObject.name.Equals("Charged_Shell(Clone)"))
        {
            phase_Counter = 3;
        }

        if (PV.IsMine)
        {
            if (phase_Counter == 3)
            {
                PhotonNetwork.Instantiate(destructedVersion.name, transform.position, Quaternion.identity);
            }

            PV.RPC("Phases", RpcTarget.AllBuffered, phase_Counter);
        }
    }

    [PunRPC]
    private void Phases(int phase)
    {
        if (phase == 1)
        {
            phase1.SetActive(true);
            phase_Counter++;
            return;
        }
        if (phase == 2)
        {
            phase2.SetActive(true);
            phase_Counter++;
            return;
        }
        if (phase == 3)
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
