using UnityEngine;

public class FloatingPowerUp : MonoBehaviour
{
    public float floatAmplitude = 0.5f;   
    public float floatFrequency = 1f;   

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
