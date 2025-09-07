using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _compRigidbody;
    public float OriginalSpeed;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 120.0f;
    public GameObject[] leftWheels;
    public GameObject[] rightWheels;
    public int playerNumber = 0;

    // Cambiamos a propiedad con backing field
    private float _life = 10f;
    public float Life
    {
        get => _life;
        set
        {
            _life = Mathf.Clamp(value, 0, maxLife);
            Debug.Log($"Jugador {playerNumber} - Vida cambiada a: {_life}");
            OnLifeChanged?.Invoke(_life);
        }
    }

    public float maxLife = 10f;
    public float wheelRotationSpeed = 200.0f;
    private float moveInput;
    private float rotationInput;

    [Header("Scene Management")]
    public string winSceneName = "Victoria";
    public string loseSceneName = "Derrota";

    [Header("Game Manager Integration")]
    public bool useGameManager = true;

    private bool isInTrigger = false;
    public float reducedPushBackFactor = 0.5f;

    public static event Action<PlayerController> OnPlayerInstantiated;
    public event Action<float> OnLifeChanged;

    void Awake()
    {
        _compRigidbody = GetComponent<Rigidbody>();
        if (_compRigidbody == null)
        {
            Debug.LogError("Rigidbody no encontrado en: " + gameObject.name);
        }
    }
    private void Start()
    {
        OriginalSpeed = moveSpeed;
        maxLife = Life;

        // REEMPLAZA con el nuevo método no obsoleto
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.OnPlayerInstantiated(this);
        }

        OnLifeChanged?.Invoke(Life);
    }

    void Update()
    {
        RotationWeels(moveInput, rotationInput);
        CheckLife();
    }

    void FixedUpdate()
    {
        MoveTank(moveInput);
        RotationTank(rotationInput);
    }

    void CheckLife()
    {
        if (Life <= 0)
        {
            HandleDefeat();
        }
    }

    void HandleDefeat()
    {
        Debug.Log($"Jugador {playerNumber} derrotado!");

       
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (useGameManager && gm != null)
        {
            gm.OnPlayerEliminated(this);
        }
        else
        {
            
            SceneManager.LoadScene(loseSceneName);
        }

        enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(EliminationEffect());
    }

    public void OnEnemyDestroyed()
    {
        if (useGameManager)
        {
            return;
        }
        else
        {
            SceneManager.LoadScene(winSceneName);
        }
    }

    IEnumerator EliminationEffect()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float effectTime = 2f;
        float elapsed = 0f;

        while (elapsed < effectTime)
        {
            foreach (Renderer r in renderers)
            {
                r.enabled = !r.enabled;
            }
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Jugador {playerNumber} recibe {damage} de daño");
        Life -= damage;
    }

    public void Heal(float healAmount)
    {
        Debug.Log($"Jugador {playerNumber} recibe {healAmount} de curación");
        Life += healAmount;
    }

    void MoveTank(float input)
    {
        Vector3 forwardMovement = transform.forward * input * moveSpeed;
        _compRigidbody.linearVelocity = forwardMovement;
    }

    void RotationTank(float input)
    {
        float rotation = input * rotationSpeed * Mathf.Deg2Rad;
        Vector3 angularVelocity = new Vector3(0.0f, rotation, 0.0f);
        _compRigidbody.angularVelocity = angularVelocity;
    }

    void RotationWeels(float moveInput, float rotationInput)
    {
        float wheelRotation = moveInput * wheelRotationSpeed * Time.deltaTime;

        for (int i = 0; i < leftWheels.Length; i++)
        {
            if (leftWheels[i] != null)
            {
                leftWheels[i].transform.Rotate(wheelRotation - rotationInput * wheelRotationSpeed * Time.deltaTime, 0.0f, 0.0f);
            }
        }

        for (int i = 0; i < rightWheels.Length; i++)
        {
            if (rightWheels[i] != null)
            {
                rightWheels[i].transform.Rotate(wheelRotation + rotationInput * wheelRotationSpeed * Time.deltaTime, 0.0f, 0.0f);
            }
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void Rotation(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<float>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            EnemyBullet bullet = other.GetComponent<EnemyBullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PushBackReducer"))
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PushBackReducer"))
        {
            isInTrigger = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(2f);
        }
    }
}