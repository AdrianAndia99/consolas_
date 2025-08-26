using UnityEngine;
using System.Collections;
public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DestroyBullet();
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
