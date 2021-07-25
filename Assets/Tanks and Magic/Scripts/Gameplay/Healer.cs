using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Healer : MonoBehaviour
{
    public GameObject healCubePrefab;

    private void Start()
    {
        StartCoroutine(SpawnCube());
    }

    public void Respawn()
    {
        StartCoroutine(SpawnCube());
    }

    private IEnumerator SpawnCube()
    {
        yield return new WaitForSeconds(30f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(healCubePrefab.name, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity)
                .transform.parent = this.gameObject.transform;
        }
    }
}
