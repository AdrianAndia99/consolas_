using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootPlayerController : MonoBehaviour
{


    public float launchSpeed = 50.0f;
    public GameObject bullet;
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;
    public LineRenderer trajectoryLine;
    
    private PlayerInput playerInput;
    private SpenController cannonController;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        cannonController = GetComponent<SpenController>();
        SetupForSecondPlayer();
        
        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = trajectoryPoints;
        }
    }

    void SetupForSecondPlayer()
    {
        if (playerInput != null)
        {
            var gamepads = Gamepad.all;
            if (gamepads.Count >= 2)
            {
                playerInput.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
                Debug.Log("Segundo mando asignado al jugador 2");
            }
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = transform.rotation;

        // Calcular dirección basada en la rotación del cañón
        Vector3 launchDirection = transform.forward;
        Vector3 velocity = launchDirection * launchSpeed;

        GameObject spawnedBullet = Instantiate(bullet, spawnPosition, spawnRotation);
        Rigidbody rigidbody = spawnedBullet.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.linearVelocity = velocity;
        }

        Debug.Log("Segundo jugador disparó!");
    }

    void Update()
    {
        // Actualizar visualización de trayectoria
        UpdateTrajectoryVisualization();

        // Tecla alternativa para testing
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SpawnBullet();
        }
    }

    void UpdateTrajectoryVisualization()
    {
        if (trajectoryLine == null) return;

        Vector3 launchDirection = transform.forward;
        Vector3 initialVelocity = launchDirection * launchSpeed;
        Vector3 currentPosition = transform.position;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * trajectoryTimeStep;
            
            // Fórmula de movimiento parabólico: posición = posición_inicial + velocidad_inicial * tiempo + 0.5 * gravedad * tiempo²
            Vector3 pointPosition = currentPosition + 
                                   initialVelocity * time + 
                                   0.5f * Physics.gravity * time * time;

            trajectoryLine.SetPosition(i, pointPosition);

            // Detener si choca con algo
            if (i > 0)
            {
                Vector3 previousPosition = trajectoryLine.GetPosition(i - 1);
                if (Physics.Linecast(previousPosition, pointPosition))
                {
                    trajectoryLine.positionCount = i + 1;
                    break;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualización alternativa con Gizmos
        if (!Application.isPlaying) return;

        Vector3 launchDirection = transform.forward;
        Vector3 initialVelocity = launchDirection * launchSpeed;
        Vector3 currentPosition = transform.position;

        Gizmos.color = Color.red;
        
        for (int i = 0; i < trajectoryPoints - 1; i++)
        {
            float time1 = i * trajectoryTimeStep;
            float time2 = (i + 1) * trajectoryTimeStep;
            
            Vector3 point1 = currentPosition + 
                            initialVelocity * time1 + 
                            0.5f * Physics.gravity * time1 * time1;
            
            Vector3 point2 = currentPosition + 
                            initialVelocity * time2 + 
                            0.5f * Physics.gravity * time2 * time2;

            Gizmos.DrawLine(point1, point2);
        }
    }
}