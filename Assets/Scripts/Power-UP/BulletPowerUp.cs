using UnityEngine;

public class BulletPowerUp : MonoBehaviour
{
    [SerializeField] private int numBullet;
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Player"))
        {
            // other.gameObject.GetComponentInChildren<ShootPlayerController>().UpdateBullet(numBullet);
        }
    }
    public void UpdateBullet(int numBullet)
    {
        this.numBullet += numBullet;
    }
}
