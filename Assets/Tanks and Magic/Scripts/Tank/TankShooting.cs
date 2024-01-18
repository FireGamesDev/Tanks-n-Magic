using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace TankScripts
{
    public class TankShooting : MonoBehaviour
    {
        public GameObject m_Shell;                   // Prefab of the shell.
        public GameObject m_BotShell;
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public float lifeTime;
        private float shootCooldown;
        public float coolDown;
        public float coolDownCharged;
        private bool anotherShoot;                  //Makes the double shoot possible
        public bool canShoot;                // if the player pauses the game can't shoot, and if Build
        public AudioClip shootSFX;

        private UnityEngine.UI.Mask chatInputMask;
        
        public PhotonView photonView;

        [Header("ChargeShot")]
        public AudioClip chargeShootSFX;
        public GameObject chargedShellPrefab;

        [Header("ShootLight")]
        public GameObject tankTurretLight;

        private GameObject bullet;
        [Header("ReticleAnim")]
        [SerializeField] public Animator reticleAnim;

        [Header("TowerTarget")]
        [SerializeField] public Animator reticleAnim1;
        [SerializeField] public GameObject towerTargetted;


        [SerializeField] public ParticleSystem muzzleFlash = null;
        [SerializeField] public ParticleSystem muzzleFlashRed = null;

        [Header("Mobile")]
        [SerializeField] private VariableJoystick _shootingJoystick;

        private bool isMobile = false;

        private UnityEngine.UI.Image chargedBulletCooldown;

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            isMobile = CheckMobile.IsMobile;

            CheckMobile.Instance.chargedBulletButton.onClick.AddListener(delegate { ShootChargedShot(); });

            canShoot = true;
            shootCooldown = coolDown;
            anotherShoot = false;

            TurretLightOn();

            if (GameObject.Find("ChargeBullet") == null)
            {
                chargedBulletCooldown = GameObject.Find("ChargeBulletButton").GetComponent<UnityEngine.UI.Image>();
            }
            else
            {
                chargedBulletCooldown = GameObject.Find("ChargeBullet").GetComponent<UnityEngine.UI.Image>();
            }
            
            chatInputMask = Chat.ChatInputField.GetComponent<UnityEngine.UI.Mask>();
        }

        private void Update()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                if (GameObject.FindGameObjectWithTag("GameHandler"))
                {
                    if (GameObject.FindGameObjectWithTag("GameHandler").GetComponent<GameHandler>().stopped)
                    {
                        return;
                    }
                }
            }

            if (GameObject.Find("ResumeMenu"))
            {
                return;
            }

            if (photonView.IsMine)
            {
                if (chatInputMask.showMaskGraphic)
                {
                    return;
                }

                if (chargedBulletCooldown)
                {
                    chargedBulletCooldown.fillAmount = coolDown / coolDown - shootCooldown / coolDown;
                }
                else chargedBulletCooldown = GameObject.Find("ChargeBullet").GetComponent<UnityEngine.UI.Image>();

                if (shootCooldown <= 0 || anotherShoot)
                {
                    TurretLightOn();
                }
                else TurretLightOff();

                shootCooldown -= Time.deltaTime;

                HandleInputs();
            }
        }

        private void HandleInputs()
        {
            if (isMobile)
            {
                if (_shootingJoystick.Direction != Vector2.zero)
                {
                    if (canShoot && shootCooldown <= 0)
                    {
                        if (reticleAnim)
                        {
                            reticleAnim.SetTrigger("isShooting");
                        }
                        Fire(true);
                        m_ShootingAudio.PlayOneShot(shootSFX);
                        shootCooldown = coolDown;
                        anotherShoot = true;
                    }
                }

                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                bool shootAgain = anotherShoot && shootCooldown <= coolDown - 0.1f;
                if (canShoot && shootCooldown <= 0 || shootAgain)
                {
                    if (reticleAnim)
                    {
                        if (!(canShoot && shootCooldown <= 0))
                        {
                            reticleAnim.SetTrigger("DoubleShoot");
                        }
                        else
                        {
                            reticleAnim.SetTrigger("isShooting");
                        }
                    }
                    Fire(true);
                    m_ShootingAudio.PlayOneShot(shootSFX);
                    shootCooldown = coolDown;
                    anotherShoot = true;

                    if (shootAgain)
                    {
                        anotherShoot = false;
                    }
                }
                if (canShoot && anotherShoot && shootCooldown <= coolDown - 0.1f)
                {
                    if (reticleAnim)
                    {
                        
                    }
                    Fire(true);
                    m_ShootingAudio.PlayOneShot(shootSFX);
                    shootCooldown = coolDown;
                    anotherShoot = false;
                }
            }

            /*
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (canShoot && shootCooldown <= 0)
                {
                    if (reticleAnim)
                    {
                        reticleAnim.SetTrigger("isShooting");
                    }
                    Fire(false);
                    m_ShootingAudio.PlayOneShot(shootSFX);
                    shootCooldown = coolDown;
                    anotherShoot = true;
                }
                if (canShoot && anotherShoot && shootCooldown <= coolDown - 0.1f)
                {
                    if (reticleAnim)
                    {
                        reticleAnim.SetTrigger("DoubleShoot");
                    }
                    Fire(false);
                    m_ShootingAudio.PlayOneShot(shootSFX);
                    shootCooldown = coolDown;
                    anotherShoot = false;
                }
            }
            */
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ShootChargedShot();
            }
        }

        private void ShootChargedShot()
        {
            if (shootCooldown <= 0)
            {
                ChargeShoot();
                m_ShootingAudio.PlayOneShot(chargeShootSFX);
                shootCooldown = coolDownCharged;
            }
        }

        private void Fire(bool isPlayerBullet)
        {
            if (photonView.IsMine)
            {

                if (isPlayerBullet)
                {
                    if (muzzleFlash)
                    {
                        muzzleFlash.Play();
                    }

                    bullet = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation, 0);
                    StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime));
                }
                else
                {
                    if (muzzleFlashRed)
                    {
                        muzzleFlashRed.Play();
                    }
                    bullet = PhotonNetwork.Instantiate(m_BotShell.name, m_FireTransform.position, m_FireTransform.rotation, 0);
                    StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime - 1f));
                }
            }

            bullet.GetComponent<ShellExplosion>().killerObject = gameObject;

            Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
                m_FireTransform.parent.parent.GetComponent<Collider>());
        }

        private void ChargeShoot()
        {
            if (reticleAnim)
            {
                reticleAnim.SetTrigger("DoubleShoot");
            }

            if (photonView.IsMine)
            {
                bullet = PhotonNetwork.Instantiate(chargedShellPrefab.name, m_FireTransform.position, m_FireTransform.rotation, 0);
                bullet.GetComponent<ShellExplosion>().killerObject = gameObject; // kill feed get the reference who killed the tank
            }

            StartCoroutine(DestroyBulletAfterTime(bullet, lifeTime));

            bullet.GetComponent<ShellExplosion>().m_isCharged = true;

            Physics.IgnoreCollision(bullet.GetComponent<Collider>(),
                m_FireTransform.parent.parent.GetComponent<Collider>());
        }

        private IEnumerator DestroyBulletAfterTime(GameObject m_Shell, float delay)
        {
            yield return new WaitForSeconds(delay);

            if(m_Shell != null)
            {
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(m_Shell);
                }
            }
        }

        private void TurretLightOn()
        {
            tankTurretLight.SetActive(true);
        }

        private void TurretLightOff()
        {
            tankTurretLight.SetActive(false);
        }

        public void SetTowerTargeting()
        {
            towerTargetted.SetActive(true);
        }

        public void DisableTowerTargeting()
        {
            towerTargetted.SetActive(false);
        }

        public void SetTowerHitted()
        {
            if(reticleAnim1 != null && reticleAnim1.runtimeAnimatorController != null && reticleAnim1.isActiveAndEnabled)
            {
                reticleAnim1.SetTrigger("DoubleShoot");
            }
        }
    }
}