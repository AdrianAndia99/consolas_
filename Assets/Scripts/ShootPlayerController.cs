using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootPlayerController : MonoBehaviour
{
    public float launchSpeed = 50.0f;
    public GameObject bullet;
    private PlayerInput playerInput;

    public AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 1);

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SetupForSecondPlayer();
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
        Quaternion spawnRotation = Quaternion.identity;

        Vector3 localDirection = transform.TransformDirection(Vector3.forward);

        float modifiedLaunchSpeed = launchSpeed * speedCurve.Evaluate(Time.timeSinceLevelLoad % speedCurve[speedCurve.length - 1].time);
        Vector3 velocity = localDirection * modifiedLaunchSpeed;

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
        // Opcional: tecla alternativa para testing
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SpawnBullet();
        }
    }
}