using UnityEngine;

public class enemy : MonoBehaviour
{
	public float moveSpeed = 3f;
	public float detectionRange = 10f;
	public float randomMoveTime = 2f;

	private Vector3 randomDirection;
	private float randomMoveTimer;
	private Transform player;
	private bool chasingPlayer = false;

	void Start()
	{
		randomMoveTimer = randomMoveTime;
		ChooseRandomDirection();
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null)
			player = playerObj.transform;
	}

	void Update()
	{
		if (player != null)
		{
			float distanceToPlayer = Vector3.Distance(transform.position, player.position);
			if (distanceToPlayer <= detectionRange)
			{
				chasingPlayer = true;
			}
			else
			{
				chasingPlayer = false;
			}
		}

		if (chasingPlayer && player != null)
		{
			Vector3 direction = (player.position - transform.position).normalized;
			transform.position += direction * moveSpeed * Time.deltaTime;
		}
		else
		{
			randomMoveTimer -= Time.deltaTime;
			if (randomMoveTimer <= 0f)
			{
				ChooseRandomDirection();
				randomMoveTimer = randomMoveTime;
			}
			transform.position += randomDirection * moveSpeed * Time.deltaTime;
		}
	}

	void ChooseRandomDirection()
	{
		float angle = Random.Range(0f, 360f);
		randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bullet"))
		{
			PlayerController player = FindObjectOfType<PlayerController>();
			if (player != null)
			{
				player.OnEnemyDestroyed();
			}
			Destroy(gameObject);
		}
	}
}