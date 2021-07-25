using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankScripts;
using Photon.Pun;

namespace TankAI
{
    public class AIShoot : MonoBehaviourPunCallbacks
    {
        public GameObject m_Shell;                   // Prefab of the shell.
        [SerializeField] public GameObject towerShell = null;
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public Transform m_FireTransform;
        private float lifeTime = 2f;

        public float damage;

        private GameObject bullet;

        public void Fire(bool isTower, Transform enemyPosition)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (isTower)
                {
                    bullet = PhotonNetwork.Instantiate(towerShell.name, m_FireTransform.position, m_FireTransform.rotation, 0);
                    bullet.GetComponent<ShellExplosion>().isTowerShell = true;
                }
                else
                {
                    bullet = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation, 0);
                }

                bullet.GetComponent<ShellExplosion>().killerObject = gameObject; // kill feed get the reference who killed the tank
                bullet.GetComponent<ShellExplosion>().damage = this.damage;

                if (gameObject.CompareTag("GreenBot"))
                {
                    bullet.tag = "GreenBotBullet";
                }

                if (gameObject.CompareTag("PurpleBot"))
                {
                    bullet.tag = "PurpleBotBullet";
                }

                Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
                    m_FireTransform.parent.parent.GetComponent<Collider>());

                StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime));
            }

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
        }

        private IEnumerator DestroyBulletAfterTime(GameObject m_Shell, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if(m_Shell != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(m_Shell);
                }
            }
        }
    }
}
