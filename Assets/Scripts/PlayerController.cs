using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class PlayerController : MonoBehaviour
{
    private Rigidbody _compRigidbody;
    public float OriginalSpeed;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 120.0f;
    public GameObject[] leftWheels;
    public GameObject[] rightWheels;
    public float Life = 10;
    public float wheelRotationSpeed = 200.0f;
    private float moveInput;
    private float rotationInput;
    //public AudioSource audioSource;
    //public AudioClip damageSound;
    [SerializeField] private string gameOverScene = "Derrota";
    [SerializeField] private float delayBeforeSceneChange = 2.0f;
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
        if (OnPlayerInstantiated != null)
        {
            OnPlayerInstantiated(this);
        }
        /*  if (audioSource == null)
          {
              audioSource = gameObject.AddComponent<AudioSource>();
          }
          */
    }

    void Update()
    {
        RotationWeels(moveInput, rotationInput);
        if (Life <= 0)
        {
            SceneManager.LoadScene(gameOverScene);
        }

    }
    void FixedUpdate()
    {
        MoveTank(moveInput);

        RotationTank(rotationInput);
    }

    void MoveTank(float input)
    {
        Vector3 forwardMovement = transform.forward * input * moveSpeed;// 2 multiplicaciones y 1 asignaci�n
        _compRigidbody.linearVelocity = forwardMovement;// 1 acceso y 1 asignaci�n
    }
    //Detallado: 2+1+1+1 = 5
    //Asintotico: O(1)
    void RotationTank(float input)
    {
        float rotation = input * rotationSpeed * Mathf.Deg2Rad; // 1 multiplicaci�n, 1 acceso a constante y 1 asignaci�n
        Vector3 angularVelocity = new Vector3(0.0f, rotation, 0.0f);// 1 creaci�n de objeto, 1 acceso y 1 asignaci�n
        _compRigidbody.angularVelocity = angularVelocity; // 1 acceso y 1 asignaci�n
    }
    //Detallado: 1+1+1+1+1+1=6
    //Asintotico: O(1)
    void RotationWeels(float moveInput, float rotationInput)
    {
        float wheelRotation = moveInput * wheelRotationSpeed * Time.deltaTime;// 1 multiplicaci�n y 1 asignaci�n


        for (int i = 0; i < leftWheels.Length; i++)// 1 por inicializaci�n + N (1 comparaci�n + 1 incremento + 1 acceso al arreglo)
        {
            if (leftWheels[i] != null)// 1 comparaci�n y 1 acceso al arreglo
            {
                leftWheels[i].transform.Rotate(wheelRotation - rotationInput * wheelRotationSpeed * Time.deltaTime, 0.0f, 0.0f);// 2 multiplicaciones + 1 resta + 1 acceso al arreglo + 1 llamada al m�todo Rotate
            }
        }


        for (int i = 0; i < rightWheels.Length; i++)// 1 por inicializaci�n + M (1 comparaci�n + 1 incremento + 1 acceso al arreglo)
        {
            if (rightWheels[i] != null) // 1 comparaci�n y 1 acceso al arreglo
            {
                rightWheels[i].transform.Rotate(wheelRotation + rotationInput * wheelRotationSpeed * Time.deltaTime, 0.0f, 0.0f);// 2 multiplicaciones + 1 suma + 1 acceso al arreglo + 1 llamada al m�todo Rotate
            }
        }
    }
    //Detallado: 1 + (1 + N(7)) + (1 + M(7)) = 3 + 7N + 7M
    //Asintotico: O(n)
    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }
    public void Rotation(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<float>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene("LoseScene");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            SceneManager.LoadScene("LoseScene");
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
    
    public void OnEnemyDestroyed()
{
    SceneManager.LoadScene("WinScene");
}
}