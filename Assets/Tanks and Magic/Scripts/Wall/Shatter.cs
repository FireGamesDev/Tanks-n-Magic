using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shatter : MonoBehaviour
{
    public GameObject particlePrefab;

    private AudioSource ExplosionAudio;
    private ParticleSystem ExplosionParticles;

    public PhotonView photonView;

    private void OnEnable()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        ExplosionParticles = PhotonNetwork.Instantiate(particlePrefab.name, transform.position, Quaternion.identity, 0).GetComponent<ParticleSystem>();
        ExplosionParticles.transform.position = transform.position;

        // Get a reference to the audio source on the instantiated prefab.
        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource>();

        // Play the particle system of the tank exploding.
        ExplosionParticles.Play();

        // Play the explosion sound effect.
        ExplosionAudio.Play();

        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        // Once the particles have finished, destroy the gameobject they are on.
        ParticleSystem.MainModule mainModule = ExplosionParticles.main;
        yield return new WaitForSeconds(mainModule.duration);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(ExplosionParticles.gameObject);
        }

        // Destroy the shattered pieces after 5 second
        yield return new WaitForSeconds(5f - mainModule.duration);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
