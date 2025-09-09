using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SpenController : MonoBehaviour
{
    public float spinSpeed = 90.0f;
    public float Direction;

    private PlayerInput playerInput;
    private Gamepad secondGamepad;

    private Vector2 rotationInput;
    private float currentHorizontalRotation = 0f;
    private float currentVerticalRotation = 0f;

    public int maxVerticalAngle = 80;
    public int minVerticalAngle = -30;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        currentHorizontalRotation = transform.localEulerAngles.y;
        currentVerticalRotation = transform.localEulerAngles.x;
        AssignSecondGamepad();
    }

    void Update()
    {
       RotateCannon();
    }

    void RotateCannon()
    {
        
        currentHorizontalRotation += rotationInput.x * spinSpeed * Time.deltaTime;

        
        currentVerticalRotation -= rotationInput.y * spinSpeed * Time.deltaTime;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle);

       
        transform.localRotation = Quaternion.Euler(currentVerticalRotation, currentHorizontalRotation, 0f);
    }

    void AssignSecondGamepad()
    {
        var gamepads = Gamepad.all;
        if (gamepads.Count >= 2 && playerInput != null)
        {

            secondGamepad = gamepads[1];
            playerInput.SwitchCurrentControlScheme("Gamepad", secondGamepad);
            Debug.Log("Segundo mando asignado para rotación: " + secondGamepad.name);
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<Vector2>();
    }


    public void CheckGamepadConnection()
    {
        if (secondGamepad == null || !secondGamepad.added)
        {
            AssignSecondGamepad();
        }
    }
}