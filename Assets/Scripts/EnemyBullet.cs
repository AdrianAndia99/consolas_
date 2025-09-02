using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 1;
   

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Causar daño al jugador
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Life -= damage;
     
            }

            // Efecto de impacto


            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("EnemyBullet"))
        {

            Destroy(gameObject);
        }
    }
}