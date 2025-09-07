using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
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

    [Header("Ground Settings")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float heightOffset = 0.5f;

    [Header("Patrol Settings")]
    public float minPatrolTime = 2f;
    public float maxPatrolTime = 5f;

    private Transform[] players;
    private Transform currentTarget;
    private Vector3 patrolCenter;
    private Vector3 targetPosition;
    private float nextFireTime;
    private float patrolTimer;
    private bool isChasing = false;
    private bool isAttacking = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("PushBackReducer");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        patrolCenter = transform.position;
        ChooseNewPatrolPoint();
        StartCoroutine(UpdateTargetCoroutine());
    }

    void Update()
    {
        // Mantener en el suelo
        MaintainGroundPosition();

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= attackRange && !isAttacking)
            {
                StartAttacking();
            }
            else if (distanceToTarget <= detectionRange && distanceToTarget > attackRange)
            {
                ChasePlayer();
            }
            else if (distanceToTarget > detectionRange && isChasing)
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

    IEnumerator UpdateTargetCoroutine()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(1f); // Actualizar objetivo cada segundo
        }
    }
    void MaintainGroundPosition()
    {
        // Raycast hacia abajo para mantenerse en el suelo
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, groundCheckDistance + 0.5f, groundLayer))
        {
            // Ajustar posición para estar exactamente sobre el suelo
            Vector3 newPosition = hit.point + Vector3.up * heightOffset;
            transform.position = newPosition;

            // Ajustar rotación para seguir el terreno
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }
    void UpdateTarget()
    {
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }
        }

        currentTarget = closestPlayer;
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f)
        {
            ChooseNewPatrolPoint();
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Mantener movimiento horizontal

        // Movimiento suave
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(targetPosition.x, transform.position.y, targetPosition.z),
            patrolSpeed * Time.deltaTime);

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

        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            direction.y = 0;

            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z),
                chaseSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
        }
    }

    void StartAttacking()
    {
        isChasing = false;
        isAttacking = true;

        // Detener movimiento al atacar
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
        if (bulletPrefab != null && firePoint != null && currentTarget != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Calcular dirección con un poco de predicción
                Vector3 predictedPosition = currentTarget.position + currentTarget.GetComponent<Rigidbody>().linearVelocity * 0.3f;
                Vector3 shootDirection = (predictedPosition - firePoint.position).normalized;

                bulletRb.linearVelocity = shootDirection * bulletSpeed;
            }

            // Destruir la bala después de un tiempo
            Destroy(bullet, 5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            HandleDestruction();
            Destroy(other.gameObject);
        }
    }

    void HandleDestruction()
    {
        // Notificar a todos los jugadores que un enemigo fue destruído
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in allPlayers)
        {
            player.OnEnemyDestroyed();
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar rangos en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolCenter, patrolRange);
    }
}