using UnityEngine;

public class BulletPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PushBackReducer"))
        {
            other.gameObject.GetComponentInChildren<ShootPlayerController>().UpdateBullet();
            Destroy(gameObject);
        }
    }
}
