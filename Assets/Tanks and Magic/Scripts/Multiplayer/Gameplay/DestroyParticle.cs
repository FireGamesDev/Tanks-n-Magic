using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyParticle : MonoBehaviour
{
    public PhotonView photonView;
    public AudioSource sfx;
    public ParticleSystem particle;

    private void Start()
    {
        particle.Play();
        sfx.Play();

        if (photonView.IsMine)
        {
            StartCoroutine(ParticleDestroy());
        }
    }

    private IEnumerator ParticleDestroy()
    {
        // Once the particles have finished, destroy the gameobject they are on.
        ParticleSystem.MainModule mainModule = gameObject.GetComponent<ParticleSystem>().main;
        yield return new WaitForSeconds(mainModule.duration);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
