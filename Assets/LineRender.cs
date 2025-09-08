using UnityEngine;

public class LineRender : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Line Settings")]
    public float lineLength = 5f; 
    public float startWidth = 0.1f;
    public float endWidth = 0.0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);

        lineRenderer.SetPosition(1, transform.position + transform.forward * lineLength);
    }
}
