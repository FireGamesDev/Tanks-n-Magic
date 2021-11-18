using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;

namespace TankScripts
{
    public class TankHealth : MonoBehaviourPunCallbacks
    {
        public float m_StartingHealth = 10f;               // The amount of health each tank starts with.
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
        public Image m_FillImage;                           // The image component of the slider.
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        public GameObject m_BustedTankPrefab;
        

        public float m_CurrentHealth;                      // How much health the tank currently has.
        private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?

        [SerializeField] private PostProcessVolume m_PostProcessVolume = null;

        [SerializeField] public BuildSystem buildSystemScript = null;

        public override void OnEnable()
        {
            // When the tank is enabled, reset the tank's health and whether or not it's dead.
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            // Update the health slider's value and color.
            SetHealthUI();

            base.OnEnable();
        }

        [PunRPC] public void TakeDamage (float amount)
        {
            ModifyHealth(amount);
        }

        [PunRPC]
        public void Heal(int amount)
        {
            while (m_CurrentHealth + amount > m_StartingHealth)
            {
                amount--;
            }

            m_CurrentHealth += amount;

            // Change the UI elements appropriately.
            SetHealthUI();
        }

        private void ModifyHealth(float amount)
        {
            // Reduce current health by the amount of damage done.
            m_CurrentHealth -= amount;

            // Change the UI elements appropriately.
            SetHealthUI();

            if (photonView.IsMine)
            {
                OnDamaged();
            }

            // If the current health is at or below zero and it has not yet been registered, call OnDeath.
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                if (buildSystemScript)
                {
                    buildSystemScript.DestroyPreview();
                }

                if (photonView.IsMine)
                {
                    OnDeath();
                }
            }
        }


        private void SetHealthUI ()
        {
            // Set the slider's value appropriately.
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        private void OnDamaged()
        {
            if (m_PostProcessVolume != null)
            {
                Vignette vignette_red;
                m_PostProcessVolume.profile.TryGetSettings(out vignette_red);
                vignette_red.intensity.value = Mathf.Lerp(0.4f, 0.3f, 0.1f);
                StartCoroutine("Damaged", vignette_red);
            }
        }

        IEnumerator Damaged(Vignette vignette)
        {
            yield return new WaitForSeconds(0.3f);
            vignette.intensity.value = Mathf.Lerp(0f, 0.4f, 0.1f);
        }


        public void OnDeath()
        {
            // Set the flag so that this function is only called once.
            m_Dead = true;

            if (photonView.IsMine)
            {
                photonView.RPC("ExplosionParticles", RpcTarget.AllBuffered);
            }

            PhotonNetwork.Instantiate(m_BustedTankPrefab.name, transform.position, transform.rotation, 0);

            if (gameObject.tag != "PurpleBot" && gameObject.tag != "GreenBot" && photonView.IsMine)
            {
                if (gameObject.tag == "LobbyBot")
                {
                    DestroyTheTank();
                    return;
                }

                GameHandler gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
                if (gameObject.CompareTag("GreenTank"))
                {
                    gameHandler.Respawn("green");
                }
                if (gameObject.CompareTag("PurpleTank"))
                {
                    gameHandler.Respawn("purple");
                }
            }

            
        }

        public void DestroyTheTank()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Lobby"))
            {
                Altar.minionCount -= 1;
            }

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }

        [PunRPC]
        private void ExplosionParticles()
        {
            PhotonNetwork.Instantiate(m_ExplosionPrefab.name, transform.position, transform.rotation, 0).GetComponent<ParticleSystem>();
        }
    }
}