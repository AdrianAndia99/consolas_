using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnerPowerUp : MonoBehaviour
{
    public GameObject[] powerUps;      
    public Transform[] spawnPoints;      
    public float spawnInterval = 3f;     

    void Start()
    {
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    IEnumerator SpawnPowerUpsRoutine()
    {
        while (true)
        {
            SpawnOnePowerUp();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOnePowerUp()
    {
        List<Transform> puntosLibres = new List<Transform>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Collider[] colliderOcupado = Physics.OverlapSphere(spawnPoints[i].position, 0.5f);
            bool ocupado = false;

            for (int j = 0; j < colliderOcupado.Length; j++)
            {
                if (colliderOcupado[j].CompareTag("PowerUp"))
                {
                    ocupado = true;
                    break;
                }
            }

            if (!ocupado)
            {
                puntosLibres.Add(spawnPoints[i]);
            }
        }

        if (puntosLibres.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, puntosLibres.Count);
        Transform spawnPoint = puntosLibres[randomIndex];

        int randomPowerIndex = Random.Range(0, powerUps.Length);

        Instantiate(powerUps[randomPowerIndex], spawnPoint.position, Quaternion.identity);
    }
}
