using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Smoke_Out_Of_Tank : MonoBehaviour
{
    public GameObject smokePrefab;

    private ParticleSystem smokeParticles;        // The particle system the will play when the tank is destroyed.

    public PhotonView photonView;

    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it.
            smokeParticles = PhotonNetwork.Instantiate(smokePrefab.name, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(-90, 0, 0)).GetComponent<ParticleSystem>();

            smokeParticles.Play();

            StartCoroutine(Destroy());
        }
    }

    private IEnumerator Destroy()
    {
        // Once the particles have finished, destroy the gameobject they are on.
        ParticleSystem.MainModule mainModule = smokeParticles.main;
        yield return new WaitForSeconds(mainModule.duration);
        PhotonNetwork.Destroy(smokeParticles.gameObject);

        yield return new WaitForSeconds(5f - mainModule.duration);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
