using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_Boids : MonoBehaviour
{
    [SerializeField]
    int spawnCount = 1;
    [SerializeField]
    int maxSpawned = 25;

    int numSpawned = 0;

    [SerializeField]
    GameObject spawnObject;

    [SerializeField]
    int spawnTime = 0;

    [SerializeField]
    bool isRepeating = false;
    [SerializeField]
    bool isSpawningFromPlayer = true;

    public bool isPaused = true;

    [SerializeField]
    int maxSpawnRadius = 10;
    [SerializeField]
    int minSpawnRadius = 3;

    Transform playerTransform;
    Transform enemyEmpty;

    [SerializeField]
    MapManager_V2 mapManager;
    [SerializeField]
    BOID_Manager boidManager;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        enemyEmpty = Instantiate(new GameObject("Enemies").transform);
    }

    public void freeNum()
    {
        //Debug.Log(numSpawned);
        numSpawned--;
    }

    public void StartSpawn()
    {
        if (spawnObject != null) StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(0,spawnTime));

        GameObject temp;
        float xOffset = transform.position.x;
        float yOffset = transform.position.y;

        if (isSpawningFromPlayer)
        {
            xOffset = playerTransform.position.x;
            yOffset = playerTransform.position.y;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            if (numSpawned <= maxSpawned)
            {
                yield return new WaitForEndOfFrame();
                float x, y;
                int count = 0;
                do
                {
                    if (Random.Range(0, 100) > 50) x = xOffset + (minSpawnRadius + Random.Range(0, maxSpawnRadius - minSpawnRadius));
                    else x = xOffset - (minSpawnRadius + Random.Range(0, maxSpawnRadius - minSpawnRadius));

                    if (Random.Range(0, 100) > 50) y = yOffset + (minSpawnRadius + Random.Range(0, maxSpawnRadius - minSpawnRadius));
                    else y = yOffset - (minSpawnRadius + Random.Range(0, maxSpawnRadius - minSpawnRadius));

                } while (!mapManager.IsFloor(x, y));

                temp = Instantiate(spawnObject, new Vector3(x, y, 0), new Quaternion(), enemyEmpty);

                mapManager.tileGrowth.Equations.Add(new PointDisplacement(temp.transform, 1, 1, false));
                numSpawned++;
                if (count++ > 5) break;
            }
        }

        boidManager.Setup();

        // if spawntime is 0 you'll crash the game
        if (isRepeating && spawnTime != 0 && !isPaused) StartCoroutine("Spawn");
    }
}