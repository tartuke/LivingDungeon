using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOID_Manager : MonoBehaviour
{
    const int threadGroupSize = 1024;

    [SerializeField]
    ComputeShader compute;
    [SerializeField]
    float range = 10;

    BOID_Entity[] boids = null;

    float perceptionRadius;
    float avoidanceRadius;

    Transform player;
    Vector2 playerPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Setup()
    {
        StartCoroutine("Delay");
    }

    IEnumerator Delay()
    {
        boids = FindObjectsOfType<BOID_Entity>();

        if (boids.Length == 0) boids = null;

        if (boids == null)
        {
            yield return new WaitForEndOfFrame();
            
            Debug.Log("Refresh");
        }
        else
        {
            perceptionRadius = boids[0].perceptionRadius;
            avoidanceRadius = boids[0].avoidanceRadius;
        }

        StartCoroutine("Delay");
    }

    void Update()
    {
        //Debug.Log(SystemInfo.supportsComputeShaders);
        if (boids != null && SystemInfo.supportsComputeShaders)
        {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++)
            {
                if (boids[i] != null)
                {
                    boidData[i].position = boids[i].position;
                    boidData[i].direction = boids[i].forward;
                }
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Length);
            compute.SetFloat("viewRadius", perceptionRadius);
            compute.SetFloat("avoidRadius", avoidanceRadius);

            compute.SetFloat("playerX", player.position.x);
            compute.SetFloat(" playerY", player.position.y);
            compute.SetFloat("range", 20f);

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {
                if (boidData[i].outOfRange == 1)
                {
                    //Version 2 too difficult
                    //Destroy(boids[i].gameObject);
                    //boids[i] = null;
                    //Debug.Log("Garbage Boid Collected");
                }
                else if(boids[i] != null)
                {
                    boids[i].index = i;
                    boids[i].avgFlockHeading = boidData[i].flockHeading;
                    boids[i].centreOfFlockmates = boidData[i].flockCentre;
                    boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                    boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                    boids[i].UpdateBoid();
                }
            }

            boidBuffer.Release();
        }
    }

    public void DestroyBoid(int i)
    {
        //Destroy(boids[i].gameObject);
        //boids[i] = null;
        FindObjectOfType<Spawner_Boids>().freeNum();
    }

    public struct BoidData
    {
        public float outOfRange;

        public Vector2 position;
        public Vector2 direction;

        public Vector2 flockHeading;
        public Vector2 flockCentre;
        public Vector2 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 2 * 5 + sizeof(int) + sizeof(float);
            }
        }
    }
}