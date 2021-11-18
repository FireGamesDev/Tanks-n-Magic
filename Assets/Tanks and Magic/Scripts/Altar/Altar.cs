using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class Altar : MonoBehaviour
{
    public const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 2f;

    private float altar_Health = 100f;
    private float damage;
    public Image barImage;
    public Image barDamageImage;
    public float damagedHealthShrinkTimer = 2f;
    public float shrinkSpeed = 1f;
    private Animator anim;

    public PhotonView photonView;

    public TMPro.TMP_Text leavingText;

    public bool died = false;

    [Header("SpawnMinion")]
    public GameObject minionPrefab;
    public GameObject minionSpawnPoint;
    public GameObject[] minionSpawnPoints;

    [Header("Crystals")]
    public GameObject crystal1;
    public GameObject crystal2;
    public GameObject crystal3;
    public GameObject crystal4;

    public static int minionCount = 0;

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            InvokeRepeating("SpawnWave", 5f, 30f);
            return;
        }

        crystal1.SetActive(true);
        crystal2.SetActive(true);
        crystal3.SetActive(true);
        crystal4.SetActive(true);

        if(PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            damage = 2;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            damage = 1;
        }

        anim = GetComponent<Animator>();
        barImage.fillAmount = 1f;
        barDamageImage.fillAmount = barImage.fillAmount;

        InvokeRepeating("SpawnWave", 5f, 30f);
    }

    private void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Lobby"))
        {
            return;
        }

        damagedHealthShrinkTimer -= Time.deltaTime;
        if(damagedHealthShrinkTimer < 0)
        {
            if(barImage.fillAmount < barDamageImage.fillAmount)
            {
                barDamageImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }

        if (died)
        {
            Death();
            StartCoroutine(WinOrLose());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "PurpleAltar" && other.tag == "GreenBullet") //the bullet has the same tag what shoots it (e.g. GreenTank --> GreenBullet) (TankShooting 51)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("TakeDamage", RpcTarget.All);
            }
            anim.SetTrigger("isDamaged");
            damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        }
        else if (gameObject.tag == "GreenAltar" && other.tag == "PurpleBullet")
        {
            if (photonView.IsMine)
            {
                photonView.RPC("TakeDamage", RpcTarget.All);
            }
            anim.SetTrigger("isDamaged");
            damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        }
        else return;
    }

    public void SetHealth(float healthNormailzed)
    {
        barImage.fillAmount -= healthNormailzed / 100;
    }

    [PunRPC]
    private void TakeDamage()
    {
        altar_Health -= damage;

        SetHealth(damage);

        if(altar_Health < 75f)
        {
            crystal1.SetActive(false);
        }
        if (altar_Health < 50f)
        {
            crystal2.SetActive(false);
        }
        if (altar_Health < 25f)
        {
            crystal3.SetActive(false);
        }

        if (altar_Health <= 0f)
        {
            crystal4.SetActive(false);
            Death();
            StartCoroutine(WinOrLose());
        }
    }

    private IEnumerator WinOrLose()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        GameObject[] winners;
        GameObject[] losers;

        if (gameObject.CompareTag("GreenAltar"))
        {
            winners = GameObject.FindGameObjectsWithTag("PurpleTank");
            losers = GameObject.FindGameObjectsWithTag("GreenTank");

            Decide(winners, losers);
        }
        else
        {
            winners = GameObject.FindGameObjectsWithTag("GreenTank");
            losers = GameObject.FindGameObjectsWithTag("PurpleTank");

            Decide(winners, losers);
        }

        leavingText.gameObject.SetActive(true);
        leavingText.text = "Leaving in 5";
        yield return new WaitForSeconds(1f);
        leavingText.text = "Leaving in 4";
        yield return new WaitForSeconds(1f);
        leavingText.text = "Leaving in 3";
        yield return new WaitForSeconds(1f);
        leavingText.text = "Leaving in 2";
        yield return new WaitForSeconds(1f);
        leavingText.text = "Leaving in 1";
        yield return new WaitForSeconds(1f);


        GameObject[] playersGreen = GameObject.FindGameObjectsWithTag("GreenTank");
        GameObject[] playersPurple = GameObject.FindGameObjectsWithTag("PurpleTank");
        GameObject[] bots = FindGameObjectsWithTags("PurpleBot", "GreenBot");

        foreach (GameObject p in playersGreen)
        {
            Destroy(p);
        }

        foreach (GameObject p in playersPurple)
        {
            Destroy(p);
        }

        foreach (GameObject b in bots)
        {
            Destroy(b);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    GameObject[] FindGameObjectsWithTags(params string[] tags)
    {
        var all = new List<GameObject>();

        foreach (string tag in tags)
        {
            all.AddRange(GameObject.FindGameObjectsWithTag(tag).ToList());
        }

        return all.ToArray();
    }

    private void Decide(GameObject[] winners, GameObject[] losers)
    {
        if (winners != null)
        {
            foreach (GameObject player in winners)
            {
                player.GetComponent<Player>().Win();
            }
        }

        if (losers != null)
        {
            foreach (GameObject player in losers)
            {
                player.GetComponent<Player>().Lose();
            }
        }
    }

    public GameObject smokePrefab;

    private ParticleSystem smokeParticles;        // The particle system the will play when the tank is destroyed.

    private void Death()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        smokeParticles = PhotonNetwork.Instantiate(smokePrefab.name, new Vector3(transform.position.x, 
            transform.position.y, transform.position.z), Quaternion.Euler(-90, 0, 0)).GetComponent<ParticleSystem>();

        smokeParticles.Play();

        if (photonView.IsMine)
        {
            StartCoroutine(Destroy());
        }
    }

    private IEnumerator Destroy()
    {
        // Once the particles have finished, destroy the gameobject they are on.
        ParticleSystem.MainModule mainModule = smokeParticles.main;
        yield return new WaitForSeconds(mainModule.duration);
        PhotonNetwork.Destroy(smokeParticles.gameObject);
    }

    private void SpawnWave()
    {
        StartCoroutine(SpawnMinion());
    }

    private IEnumerator SpawnMinion()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Lobby"))
            {
                for (int i = 0; i < 4; i++)
                {
                    if(minionCount > 10)
                    {
                        break;
                    }
                    else
                    {
                        minionCount++;
                        minionSpawnPoint = minionSpawnPoints[Random.Range(0, minionSpawnPoints.Length - 1)];
                        PhotonNetwork.InstantiateRoomObject(minionPrefab.name, minionSpawnPoint.transform.position, minionSpawnPoint.transform.rotation);
                    }
                    yield return new WaitForSeconds(1f);
                }
                
            } else
            {
                PhotonNetwork.InstantiateRoomObject(minionPrefab.name, minionSpawnPoint.transform.position, minionSpawnPoint.transform.rotation);
                yield return new WaitForSeconds(1f);
                PhotonNetwork.InstantiateRoomObject(minionPrefab.name, minionSpawnPoint.transform.position, minionSpawnPoint.transform.rotation);
                yield return new WaitForSeconds(1f);
                PhotonNetwork.InstantiateRoomObject(minionPrefab.name, minionSpawnPoint.transform.position, minionSpawnPoint.transform.rotation);
                yield return new WaitForSeconds(1f);
                PhotonNetwork.InstantiateRoomObject(minionPrefab.name, minionSpawnPoint.transform.position, minionSpawnPoint.transform.rotation);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
