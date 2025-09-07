using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 7f;
    public float patrolRange = 15f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float bulletSpeed = 15f;

    [Header("Patrol Settings")]
    public float minPatrolTime = 2f;
    public float maxPatrolTime = 5f;

    private Transform player;
    private Vector3 patrolCenter;
    private Vector3 targetPosition;
    private float nextFireTime;
    private float patrolTimer;
    private bool isChasing = false;
    private bool isAttacking = false;

    void Start()
    {
        // Buscar al jugador (asumiendo que tiene el tag "Player")
        player = GameObject.FindGameObjectWithTag("PushBackReducer").transform;

        patrolCenter = transform.position;
        ChooseNewPatrolPoint();
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartAttacking();
            }
            else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else if (distanceToPlayer > detectionRange && isChasing)
            {
                StopChasing();
            }

            if (isAttacking && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f)
        {
            ChooseNewPatrolPoint();
        }

        // Movimiento hacia el punto de patrulla
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Mantener movimiento horizontal

        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(targetPosition.x, transform.position.y, targetPosition.z),
            patrolSpeed * Time.deltaTime);

        // Rotación hacia la dirección del movimiento
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }

    void ChooseNewPatrolPoint()
    {
        // Elegir un punto aleatorio dentro del rango de patrulla
        Vector2 randomCircle = Random.insideUnitCircle * patrolRange;
        targetPosition = patrolCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
        patrolTimer = Random.Range(minPatrolTime, maxPatrolTime);
    }

    void ChasePlayer()
    {
        isChasing = true;
        isAttacking = false;

        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(player.position.x, transform.position.y, player.position.z),
                chaseSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
        }
    }

    void StartAttacking()
    {
        isChasing = false;
        isAttacking = true;
        // El enemigo se queda en su posición mientras dispara
    }

    void StopChasing()
    {
        isChasing = false;
        isAttacking = false;
        patrolCenter = transform.position; // Establecer nuevo centro de patrulla
        ChooseNewPatrolPoint();
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                Vector3 shootDirection = (player.position - firePoint.position).normalized;
                bulletRb.linearVelocity = shootDirection * bulletSpeed;
            }

            Destroy(bullet, 5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolCenter, patrolRange);
    }
}