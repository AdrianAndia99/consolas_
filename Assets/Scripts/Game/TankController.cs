using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class TankController : MonoBehaviour
{
    private float inputMove;
    private float inputRotate;
    private Rigidbody rb;
    [SerializeField] private float speedMove;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, inputMove * speedMove);
    }
    private void Update()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.y += inputRotate;
        transform.localEulerAngles = currentRotation;
    }
    public void Move(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<float>();
    }
    public void MoveRotate(InputAction.CallbackContext context)
    {
        inputRotate = context.ReadValue<float>();
    }
}
