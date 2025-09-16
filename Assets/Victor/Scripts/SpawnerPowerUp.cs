using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class SpawnerPowerUp : MonoBehaviour
{
    public GameObject[] powerUps;      
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
    
    private void SpawnOnePowerUp()
    {
        Vector3 position = GetRandomPosition();
        Instantiate(powerUps[Random.Range(0,powerUps.Length)],
            new Vector3(position.x,1.3f,position.z), Quaternion.identity);
    }
    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 50;
        print(randomDirection);
        randomDirection += transform.position;
        print(randomDirection);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 100, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}
