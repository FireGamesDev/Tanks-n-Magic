using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class TowerShell : MonoBehaviour
{
    public float m_Bulletspeed;
    private GameObject[] enemies;

    private float maxTurnSpeed = 270f; // degrees per second
    private Vector3 currentDirection;
    private Vector3 directionToTarget;
    private Vector3 resultingDirection;

    private bool isPurpleBotBullet = false;

    private GameObject closestEnemy;

    private void Start()
    {
        if (gameObject.CompareTag("PurpleBotBullet"))
        {
            isPurpleBotBullet = true;
        }

        closestEnemy = GetClosestEnemy();
    }

    private void Update()
    {
        if(closestEnemy == null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        } else
        {
            transform.position += transform.forward * m_Bulletspeed * Time.deltaTime;

            directionToTarget = closestEnemy.transform.position - transform.position;
            currentDirection = transform.forward;
            resultingDirection = Vector3.RotateTowards(currentDirection, directionToTarget, maxTurnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1f);
            transform.rotation = Quaternion.LookRotation(resultingDirection);
        }
    }

    GameObject GetClosestEnemy()
    {
        if (isPurpleBotBullet)
        {
            enemies = FindGameObjectsWithTags("GreenTank", "GreenBot");
        }
        else enemies = FindGameObjectsWithTags("PurpleTank", "PurpleBot");

        // If no enemies found at all directly return nothing
        // This happens if there simply is no object tagged "Enemy" in the scene
        if (enemies.Length == 0)
        {
            Debug.LogWarning("No enemies found!", this);
            return null;
        }

        GameObject closest;

        // If there is only exactly one anyway skip the rest and return it directly
        if (enemies.Length == 1)
        {
            closest = enemies[0];
            return closest;
        }

        // Otherwise: Take the enemies
        closest = enemies
            // Order them by distance (ascending) => smallest distance is first element
            .OrderBy(go => (transform.position - go.transform.position).sqrMagnitude)
            // Get the first element
            .First();

        return closest;
    }

    //Find tags
    GameObject[] FindGameObjectsWithTags(params string[] tags)
    {
        var all = new List<GameObject>();

        foreach (string tag in tags)
        {
            all.AddRange(GameObject.FindGameObjectsWithTag(tag).ToList());
        }

        return all.ToArray();
    }
}
