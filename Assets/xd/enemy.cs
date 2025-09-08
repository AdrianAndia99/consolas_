using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRange = 10f;
    public float attackRange = 7f;
    public float patrolRange = 15f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float bulletSpeed = 15f;

    private Transform[] players;
    private Transform currentTarget;
    private Vector3 patrolCenter;
    private float nextFireTime;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("PushBackReducer");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
            players[i] = playerObjects[i].transform;

        patrolCenter = transform.position;
        ChooseNewPatrolPoint();

        StartCoroutine(UpdateTargetCoroutine());
    }

    void Update()
    {
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true; 
                transform.LookAt(currentTarget);

                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
            else if (distance <= detectionRange)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.position); 
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    IEnumerator UpdateTargetCoroutine()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateTarget()
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Transform player in players)
        {
            if (player == null) continue;

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = player;
            }
        }

        currentTarget = closest;
    }

    void Patrol()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
            ChooseNewPatrolPoint();
    }

    void ChooseNewPatrolPoint()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRange;
        Vector3 patrolPoint = patrolCenter + new Vector3(rnd.x, 0, rnd.y);
        agent.SetDestination(patrolPoint);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && currentTarget != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (currentTarget.position - firePoint.position).normalized;
                rb.linearVelocity = dir * bulletSpeed;
            }
            Destroy(bullet, 5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}