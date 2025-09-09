using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    public float multiplicadorSpeed = 2f;
    public float duraccion = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PushBackReducer"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AplicarSpeed(multiplicadorSpeed, duraccion);
            }
            Destroy(gameObject);  
        }
    }
}
