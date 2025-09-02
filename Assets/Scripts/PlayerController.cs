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
    public float Life = 10;
    public float maxLife = 10;
    public float wheelRotationSpeed = 200.0f;
    private float moveInput;
    private float rotationInput;

    [Header("Scene Management")]
    public string winSceneName = "Victoria"; // Escena cuando ganas
    public string loseSceneName = "Derrota"; // Escena cuando pierdes

    [Header("Game Manager Integration")]
    public bool useGameManager = true; // Si usas el GameManager

    private bool isInTrigger = false;
    public float reducedPushBackFactor = 0.5f;

    public static event Action<PlayerController> OnPlayerInstantiated;
    public event Action<float> OnLifeChanged;

    void Awake()
    {
        _compRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        OriginalSpeed = moveSpeed;
        maxLife = Life; // Guardar vida máxima

        if (OnPlayerInstantiated != null)
        {
            OnPlayerInstantiated(this);
        }
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
        if (useGameManager && GameManager.Instance != null)
        {
            // Notificar al GameManager que este jugador fue eliminado
            GameManager.Instance.OnPlayerEliminated(this);
        }
        else
        {
            SceneManager.LoadScene(loseSceneName);
        }

        // Desactivar controles pero mantener visible
        enabled = false;
        GetComponent<Collider>().enabled = false;

        // Efecto visual de eliminación
        StartCoroutine(EliminationEffect());
    }

    public void OnEnemyDestroyed()
    {
        if (useGameManager && GameManager.Instance != null)
        {
            // El GameManager maneja la victoria global
            return;
        }
        else
        {
            // Victoria directa si no hay GameManager
            SceneManager.LoadScene(winSceneName);
        }
    }
    IEnumerator EliminationEffect()
    {
        // Parpadeo y efecto de eliminación
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
        Life -= damage;
        Life = Mathf.Clamp(Life, 0, maxLife);

        // Notificar cambio de vida
        if (OnLifeChanged != null)
        {
            OnLifeChanged(Life);
        }

        // Si usas GameManager, notificar daño

    }

    public void Heal(float healAmount)
    {
        Life += healAmount;
        Life = Mathf.Clamp(Life, 0, maxLife);

        if (OnLifeChanged != null)
        {
            OnLifeChanged(Life);
        }
    }

    // Resto de tus métodos existentes...
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
            // Recibir daño de bala enemiga
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
            // Daño por colisión con enemigo
            TakeDamage(2f);
        }
    }
}