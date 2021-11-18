using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Photon.Pun;

public class AI : MonoBehaviourPunCallbacks
{
    public NavMeshAgent agent;
    public LayerMask whatIsGround;
    public LayerMask whatIsPurpleEnemy;
    public LayerMask whatIsGreenEnemy;
    private LayerMask layer;

    public bool isTower = false;
    private List<GameObject> enemies = new List<GameObject>();

    private Transform enemyPosition;

    private bool purpleBot = false;
    private bool lobbyBot = false;

    [Header("Patrolling")]
    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;

    [Header("Attacking")]
    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    [HideInInspector] public TankAI.AIShoot tankShootScript;

    [Header("States")]
    //States
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [SerializeField] public UnityEngine.UI.Image attackRangeSphere;

    private void Awake()
    {
        tankShootScript = GetComponent<TankAI.AIShoot>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (gameObject.CompareTag("PurpleBot"))
        {
            purpleBot = true;
        }

        if (gameObject.CompareTag("LobbyBot"))
        {
            lobbyBot = true;
        }
    }

    private void Update()
    {
        //Check for sight and attack range
        if (purpleBot)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPurpleEnemy);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPurpleEnemy);
        }
        else if (lobbyBot)
        {
            layer = whatIsGreenEnemy | whatIsPurpleEnemy;
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, layer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, layer);
        }
        else
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsGreenEnemy);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsGreenEnemy);
        }

        if (isTower)
        {
            if (playerInAttackRange)
            {
                if (purpleBot && GetClosestEnemy().CompareTag("GreenTank"))
                {
                    attackRangeSphere.color = new Color(255f, 0f, 0f, 0.02f);
                }
                else if (!purpleBot && GetClosestEnemy().CompareTag("PurpleTank"))
                {
                    attackRangeSphere.color = new Color(255f, 0f, 0f, 0.02f);
                }
                else attackRangeSphere.color = new Color(0f, 0f, 0f, 0.2f);
            }
            else
            {
                attackRangeSphere.color = new Color(0f, 0f, 0f, 0.2f);
            }
        }

        if (!isTower)
        {
            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) Chase();
        }
        if (playerInSightRange && playerInAttackRange) Attacking();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        if (GetClosestEnemy())
        {
            walkPoint = GetClosestEnemy().transform.position;
        }

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void Chase()
    {
        enemyPosition = GetClosestEnemy().transform;
        if(enemyPosition != null)
        {
            agent.SetDestination(enemyPosition.position);
        }
    }

    private void Attacking()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        enemyPosition = GetClosestEnemy().transform;

        transform.LookAt(enemyPosition);

        if (!alreadyAttacked)
        {
            if (isTower)
            {
                tankShootScript.Fire(true, enemyPosition);
            } else tankShootScript.Fire(false, null);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    GameObject GetClosestEnemy()
    {
        if (lobbyBot)
        {
            enemies.AddRange(FindGameObjectsWithTags("GreenTank", "GreenBot"));
            enemies.AddRange(FindGameObjectsWithTags("PurpleTank", "PurpleBot"));
        }
        if (purpleBot)
        {
            enemies.AddRange(FindGameObjectsWithTags("GreenTank", "GreenBot"));
        }
        else enemies.AddRange(FindGameObjectsWithTags("PurpleTank", "PurpleBot"));

        // If no enemies found at all directly return nothing
        // This happens if there simply is no object tagged "Enemy" in the scene
        if (enemies.Count == 0)
        {
            return null;
        }

        GameObject closest;

        // If there is only exactly one anyway skip the rest and return it directly
        if (enemies.Count == 1)
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
