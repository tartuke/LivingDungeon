using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    int spawnCount = 1;

    [SerializeField]
    GameObject spawnObject;

    [SerializeField] //set to zero to spawn once
    int spawnTime = 0;

    [SerializeField]
    int spawnRadius = 10;

    // Start is called before the first frame update
    void Start()
    {
        if(spawnObject != null) StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);

        GameObject temp;

        for(int i = 0; i < spawnCount; i++)
        {
            temp = Instantiate(spawnObject, 
                        new Vector3(transform.position.x + Random.Range(-spawnRadius, spawnRadius), transform.position.y + Random.Range(-spawnRadius, spawnRadius), transform.position.z), 
                        transform.rotation);
            Enemy enemy = temp.GetComponent<Enemy>();
            //if (enemy != null) enemy.SetStats = 
        }

        if (spawnTime != 0) StartCoroutine("Spawn");
    }
}
