using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealCube : MonoBehaviour
{
    public GameObject healParticlesPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.CompareTag("GreenTank") || other.CompareTag("PurpleTank"))
            {
                PhotonNetwork.Instantiate(healParticlesPrefab.name, transform.position ,Quaternion.identity);

                int healAmount = Random.Range(1, 7);

                other.gameObject.GetComponent<PhotonView>().RPC("Heal", RpcTarget.AllBuffered, healAmount);

                GetComponentInParent<Healer>().Respawn();
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}
