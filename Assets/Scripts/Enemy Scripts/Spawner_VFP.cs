using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_VFP : MonoBehaviour
{
    [SerializeField]
    int spawnCount = 1;
    [SerializeField]
    int maxSpawned = 25;

    [SerializeField]
    GameObject spawnObject;

    [SerializeField] //set to zero to spawn once
    float spawnTime = 0.1f;

    [SerializeField]
    int maxSpawnRadius = 10;
    [SerializeField]
    int minSpawnRadius = 3;

    [SerializeField]
    bool isSpawningFromPlayer = true;
    [SerializeField]
    bool isRepeating = false;
    public bool isPaused = true;

    public static int numSpawned = 0;

    Transform playerTransform;
    Transform enemyEmpty;
    MapManager_V2 mapManager;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mapManager = GameObject.FindObjectOfType<MapManager_V2>();
        enemyEmpty = Instantiate(new GameObject("Enemies").transform);
    }

    public void StartSpawn()
    {
        if (spawnObject != null) StartCoroutine("Spawn");
    }

    public void freeNum()
    {
        numSpawned--;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);

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
                    int xNeg = (Random.Range(0, 100) > 50) ? 1 : -1;
                    int yNeg = (Random.Range(0, 100) > 50) ? 1 : -1;

                    float randX = Random.Range(0, maxSpawnRadius - minSpawnRadius);
                    float randY = Random.Range(0, maxSpawnRadius - minSpawnRadius);

                    x = xOffset + (minSpawnRadius + randX) * xNeg;
                    y = yOffset + (minSpawnRadius + randY) * yNeg;

                    if(count++ > 5) break;
                
                } while (!mapManager.IsFloor(x, y));

                temp = Instantiate(spawnObject, new Vector3(x, y, 0), new Quaternion(), enemyEmpty);

                mapManager.tileGrowth.Equations.Add(new PointDisplacement(temp.transform, 1, 1, false));

                numSpawned++;
            }
        }


        if (spawnTime != 0 && !isPaused && isRepeating) StartCoroutine("Spawn");
    }
}
