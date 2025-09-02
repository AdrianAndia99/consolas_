using UnityEngine;
using UnityEngine.InputSystem;
public class CameraTankController : MonoBehaviour
{
    public Transform tankTransform;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (tankTransform == null) return;

        Vector3 desiredPosition = tankTransform.position + tankTransform.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(tankTransform);
    }
}