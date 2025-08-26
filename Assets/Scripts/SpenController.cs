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

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        AssignSecondGamepad();
    }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Direction * Time.deltaTime);
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
        Direction = context.ReadValue<float>();
    }

  
    public void CheckGamepadConnection()
    {
        if (secondGamepad == null || !secondGamepad.added)
        {
            AssignSecondGamepad();
        }
    }
}