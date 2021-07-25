using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

namespace TankScripts
{
    public class ShellExplosion : MonoBehaviourPunCallbacks
    {
        public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
        public AudioSource m_ricochetSFX;                // Reference to the audio that will play on explosion.
        public float damage = 1;
        public float m_Bulletspeed;

        public bool m_isCharged = false;

        private Shake shake; //cam shake

        [Header("Ricochet")]
        public AudioClip ricochetSFX;

        private Vector3 shootDir;

        private float rot;
        private bool ricochet = true;

        [Header("Kill Feed")]
        public GameObject killerObject;
        private GameObject killedObject;
        //kill feed colors
        private Color killerColor = Color.white;
        private Color killedColor = Color.white;

        private int killsofPlayer;
        private GameObject toplist;

        [HideInInspector] public bool isTowerShell = false;

        private void Start()
        {
            if (GameObject.FindGameObjectWithTag("ScreenShake") != null) {
                shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<Shake>(); //Screen shake
            }

            if (m_isCharged)
            {
                int rand = Random.Range(2, 7);
                photonView.RPC("SetDamage", RpcTarget.AllBuffered, rand);
            }
        }

        private void Update()
        {
            if (!isTowerShell)
            {
                Move();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "PurpleTank" && gameObject.tag == "PurpleBullet")         //teammate can't damage (TankShooting 51, AIShoot 20)
            {
                return;
            }

            if (other.tag == "GreenTank" && gameObject.tag == "GreenBullet")         //teammate can't damage (TankShooting 51, AIShoot 19)
            {
                return;
            }

            if (other.tag == "PurpleBot" && gameObject.tag == "PurpleBotBullet")         //bots can't damage each other (TankShooting 51, AIShoot 19)
            {
                return;
            }

            if (other.tag == "GreenBot" && gameObject.tag == "GreenBotBullet")         //bots can't damage each other (TankShooting 51, AIShoot 19)
            {
                return;
            }

            if (other.tag.Contains("Bullet") && gameObject.tag.Contains("Bullet")) //if the object are bullets don't collide with each other
            {
                return;
            }

            if (other.tag == "Flag" || other.tag == "Preview" || other.tag == "Heal" || other.tag == "BustedTank")
            {
                return;
            }

            if (ricochet && other.tag != "PurpleBot" && other.tag != "GreenBot" 
                && other.tag != "PurpleBotBullet" && other.tag != "GreenBotBullet" && other.tag != "PurpleTank"
                && other.tag != "GreenTank" && other.tag != "GreenAltar"
                && other.tag != "PurpleAltar" && other.tag != "Heal")
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("Ricochet", RpcTarget.AllBuffered);
                }
                return;
            }

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = other.GetComponent<TankHealth>();

            if (photonView.IsMine)
            {
                if (!SceneManager.GetActiveScene().name.Equals("Lobby"))
                {
                    if(targetHealth == null)
                    {
                        if (gameObject.CompareTag("GreenBullet") && other.gameObject.CompareTag("GreenAltar"))
                        {
                            return;
                        }
                        if (gameObject.CompareTag("PurpleBullet") && other.gameObject.CompareTag("PurpleAltar"))
                        {
                            return;
                        }
                    }

                    if (other.gameObject && targetHealth)
                    {
                        if (gameObject.CompareTag("GreenBotBullet"))
                        {
                            if (!other.gameObject.tag.Contains("Green"))
                            {
                                if (isTowerShell && other.gameObject.CompareTag("PurpleBot"))
                                {
                                    damage = 1f;
                                }
                                else if (other.GetComponent<TankScripts.TankShooting>())
                                {
                                    other.GetComponent<TankScripts.TankShooting>().SetTowerHitted();
                                }
                                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                            }
                            else return;
                        }
                        if (gameObject.CompareTag("PurpleBotBullet"))
                        {
                            if (!other.gameObject.tag.Contains("Purple"))
                            {
                                if (isTowerShell && other.gameObject.CompareTag("GreenBot"))
                                {
                                    damage = 1f;
                                }
                                else if (other.GetComponent<TankScripts.TankShooting>())
                                {
                                    other.GetComponent<TankScripts.TankShooting>().SetTowerHitted();
                                }
                                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                            }
                            else return;
                        }
                        if (gameObject.CompareTag("GreenBotBulletPlayer"))
                        {
                            if (!other.gameObject.tag.Contains("Green"))
                            {
                                if (other.gameObject.CompareTag("PurpleTank"))
                                {
                                    return;
                                }
                                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                            }
                            else return;
                        }
                        if (gameObject.CompareTag("PurpleBotBulletPlayer"))
                        {
                            if (!other.gameObject.tag.Contains("Purple"))
                            {
                                if (other.gameObject.CompareTag("GreenTank"))
                                {
                                    return;
                                }
                                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                            }
                            else return;
                        }
                        if (gameObject.CompareTag("GreenBullet"))
                        {
                            if (!other.gameObject.tag.Contains("Green"))
                            {
                                if (other.gameObject.CompareTag("PurpleBot"))
                                {
                                    return;
                                }
                                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                            }
                            else return;
                        }
                        if (gameObject.CompareTag("PurpleBullet"))
                        {
                            if (!other.gameObject.tag.Contains("Purple"))
                            {
                                if (other.gameObject.CompareTag("GreenBot"))
                                {
                                    return;
                                }
                                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                            }
                            else return;
                        }
                    }
                }
            }

            if (shake != null)
            {
                shake.CamShake(); // Screen shake
            }

            if (photonView.IsMine)
            {
                PhotonNetwork.Instantiate(m_ExplosionParticles.gameObject.name, transform.position, transform.rotation);
            }

            //kill feed
            if (other.GetComponent<TankHealth>() != null)
            {
                if (other.GetComponent<TankHealth>().m_CurrentHealth <= 0f)
                {
                    killedObject = other.gameObject;

                    if (killerObject && killedObject)
                    {
                        UpdateKillFeed(killerObject.GetComponent<PhotonView>().ViewID, killedObject.GetComponent<PhotonView>().ViewID);
                    }
                }
            }

            // Destroy the shell.
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        //necessary method for kill feed
        public void UpdateKillFeed(int killerViewID, int killedViewID)
        {
            photonView.RPC("KillFeedRPC", RpcTarget.AllBuffered, killerViewID, killedViewID);
        }

        private void Move()
        {
            shootDir = transform.forward;
            transform.position += shootDir * m_Bulletspeed * Time.deltaTime;

            // Ricochet
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 0.5f * m_Bulletspeed + .1f))
            {
                Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
                rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
            }
        }

        [PunRPC]
        private void Ricochet()
        {
            if (m_isCharged)
            {
                m_ricochetSFX.PlayOneShot(ricochetSFX);
            }
            transform.eulerAngles = new Vector3(0, rot, 0);
        }

        [PunRPC]
        private void KillFeedRPC(int killerViewID, int killedViewID)
        {
            if (!SceneManager.GetActiveScene().name.Equals("Aram"))
            {
                return;
            }

            GameObject killer;
            GameObject killed;

            if (PhotonView.Find(killerViewID))
            {
                killer = PhotonView.Find(killerViewID).gameObject;
            }
            else return;
            if (PhotonView.Find(killedViewID))
            {
                killed = PhotonView.Find(killedViewID).gameObject;
            }
            else return;

            //Coloring the names
            if (killer.CompareTag("PurpleTank"))
            {
                killerColor = Color.magenta;
            }
            if (killer.CompareTag("GreenTank"))
            {
                killerColor = Color.green;
            }

            if (killed.CompareTag("PurpleTank"))
            {
                killedColor = Color.magenta;
            }
            if (killed.CompareTag("GreenTank"))
            {
                killedColor = Color.green;
            }

            if (killer.CompareTag("PurpleBot") || killer.CompareTag("GreenBot"))
            {
                killed.GetComponent<TankHealth>().DestroyTheTank();
                return;
            }
            else if (killed.CompareTag("PurpleBot") || killed.CompareTag("GreenBot"))
            {
                killed.GetComponent<TankHealth>().DestroyTheTank();
                return;
            } 
            else
            {
                string killername;
                string killedname;
                if (killer.GetComponent<PhotonView>().Owner.NickName.Contains("/"))
                {
                    killername = killer.GetComponent<PhotonView>().Owner.NickName.Split('/')[0];
                }
                else
                {
                    killername = killer.GetComponent<PhotonView>().Owner.NickName;
                }
                if (killed.GetComponent<PhotonView>().Owner.NickName.Contains("/"))
                {
                    killedname = killed.GetComponent<PhotonView>().Owner.NickName.Split('/')[0];
                }
                else
                {
                    killedname = killed.GetComponent<PhotonView>().Owner.NickName;
                }
                KillFeed.instance.AddNewKillListing(killername, killedname, 0, killerColor, killedColor);
                killed.GetComponent<TankHealth>().DestroyTheTank();
            }

            if (killer && !killer.CompareTag("PurpleBot") && !killer.CompareTag("GreenBot"))
            {
                Hashtable hash = new Hashtable();
                killsofPlayer = System.Convert.ToInt32(killer.GetComponent<PhotonView>().Owner.CustomProperties["Kills"]);
                hash.Add("Kills", killsofPlayer + 1);
                killer.GetComponent<PhotonView>().Owner.SetCustomProperties(hash);

                if (photonView.IsMine)
                {
                    if (killer.GetComponent<PhotonView>().Owner.NickName.Contains("/"))
                    {
                        string[] names = killer.GetComponent<PhotonView>().Owner.NickName.Split('/');
                        photonView.RPC("UpdateToplist", RpcTarget.All, names[0], killsofPlayer + 1);
                    }
                    else
                    {
                        photonView.RPC("UpdateToplist", RpcTarget.All, killer.GetComponent<PhotonView>().Owner.NickName, killsofPlayer + 1);
                    }

                    //leaderboard increase kill count
                    if (killer.GetComponent<PhotonView>().Owner.NickName == photonView.Owner.NickName)
                    {
                        PlayerPrefs.SetInt("Kills", PlayerPrefs.GetInt("Kills", 0) + 1);
                    }
                }
            }
        }

        [PunRPC]
        private void SetDamage(int rand)
        {
            m_Bulletspeed -= 15f;
            damage += rand;
        }

        [PunRPC]
        private void UpdateToplist(string username, int killCount)
        {
            toplist = GameObject.Find("ToplistContent");
            Transform contents = toplist.transform;
            foreach (Transform go in contents)
            {
                if (go.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text.Equals(username))
                {
                    go.transform.Find("KillCount").GetComponent<TMPro.TMP_Text>().text = killCount.ToString();
                    return;
                }
            };
        }
    }
}