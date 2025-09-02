using UnityEngine;

public class LineRender : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);

        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z + 5));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.0f;
    }
}
