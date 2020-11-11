using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]MapManager mapManager;


        public EntityInfo[] EntityTypes;

        GameObject enemyEmpty;
        int Level = -1;

        // Start is called before the first frame update
        void Awake()
        {
            mapManager.OnLevelLoadCallback += LoadEntities;
            mapManager.OnLevelJoinCallback += SpawnEntities;

            enemyEmpty = new GameObject("Enemies");
        }

        void Start()
        {
            foreach (EntityInfo entity in EntityTypes)
            {
                StartCoroutine(Spawn(entity));
            }
        }

        private void LoadEntities(int level)
        {
            Reset();
            foreach (EntityInfo entity in EntityTypes)
            {
                for (entity.count = 0; entity.count < entity.Inital(level); entity.count++)
                {
                    SpawnOne(entity.prefab);
                }
            }
        }

        private void SpawnEntities(int level)
        {
            Level = level;
        }

        private void Reset()
        {
            Destroy(enemyEmpty);
            enemyEmpty = new GameObject("Enemies");
            foreach (EntityInfo entity in EntityTypes)
                entity.count = 0;
        }

        IEnumerator Spawn(EntityInfo entity)
        {
            if (Level == -1) yield return null;

            yield return new WaitForSeconds(1/entity.Rate(Level));

            if (entity.count < entity.Max(Level))
            {
                SpawnOne(entity.prefab);
                entity.count++;
            }

            yield return Spawn(entity);
;
        }

        void SpawnOne(GameObject prefab)
        {
            GameObject temp;

            LevelData Ld = mapManager.LevelData;

            float randX = UnityEngine.Random.Range(0, Ld.width);
            float randY = UnityEngine.Random.Range(0, Ld.height);

            Vector3 pos = mapManager.FindFloor(new Vector3(randX, randY));

            temp = Instantiate(prefab, pos, new Quaternion(), enemyEmpty.transform);

            mapManager.tileGrowth.Equations.Add(new PointDisplacement(temp.transform, 1, 1, .2));

            Debug.Log(temp);
        }

        [Serializable]
        public class EntityInfo
        {
            public GameObject prefab;
            public int count;
            [SerializeField] private AnimationCurve inital;
            [SerializeField] private AnimationCurve max;
            [SerializeField] private AnimationCurve rate;

            public int Inital(int level)
            {
                return (int)inital.Evaluate(level);
            }
            public int Max(int level)
            {
                return (int)max.Evaluate(level);
            }
            public float Rate(int level)
            {
                return rate.Evaluate(level);
            }
        }
    }

}

