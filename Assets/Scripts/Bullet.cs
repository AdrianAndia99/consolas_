using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3.0f;
    public GameObject explosionEffect;
    public TrailRenderer bulletTrail;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Configurar el Trail Renderer si existe
        if (bulletTrail != null)
        {
            bulletTrail.emitting = true;
        }

        StartCoroutine(DestroyBullet());
    }

    void FixedUpdate()
    {
        
        if (rb != null)
        {
            rb.AddForce(Physics.gravity, ForceMode.Acceleration);
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTime);
        DestroyBulletWithEffect();
    }

    void OnCollisionEnter(Collision collision)
    {
        DestroyBulletWithEffect();
    }

    void DestroyBulletWithEffect()
    {
        
        if (bulletTrail != null)
        {
            bulletTrail.emitting = false;
            bulletTrail.transform.parent = null; 
            Destroy(bulletTrail.gameObject, bulletTrail.time); 
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}