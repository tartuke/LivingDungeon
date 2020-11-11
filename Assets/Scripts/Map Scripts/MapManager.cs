using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {

        public static MapManager instance;

        void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("More than one instance of MapManager found");
            }
            instance = this;
        }

        public float sightRadius = 3;
        private double sRSqrd = 9;

        public float despawnRadius = 4;
        private double dRSqrd = 16;

        public int wallHealth = 20;

        public bool livingMap = true;
        public float growthFactor = .5f;

        public TileManager tileManager;

        Dictionary<(int, int), MapTile> visableMap = new Dictionary<(int, int), MapTile>();

        LevelData levelData;
        LevelData nextLevelData;

        public LevelData LevelData => levelData;

        public bool loadingNextLevel = false;

        public TileGrowth tileGrowth;
        public Transform playerTransform;
        public PlayerMovement_v2 pm;

        public ComputeShader Shader;
        public MapComputeManager mapComputeManager;

        float lastTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            tileManager = new TileManager();

            levelData = Level.Load(0);

            MapComputeManager.Shader = Shader;
            mapComputeManager = new MapComputeManager(levelData);

            // set up growth map
            tileGrowth = new TileGrowth();
            tileGrowth.Equations.Add(new RaidialDisplacement(playerTransform, 1.5,1,2));
            //tileGrowth.Equations.Add(new RaidialDisplacement(playerTransform, 6));
            //tileGrowth.Equations.Add(new RayDisplacement(playerTransform, 1,1,5,false));


            sRSqrd = sightRadius * sightRadius;
            despawnRadius = despawnRadius > sightRadius ? despawnRadius : sightRadius + 1;
            dRSqrd = despawnRadius * despawnRadius;

            OnLevelLoadCallback.Invoke(0);
        }

        bool flip = false;

        // Update is called once per frame
        void Update()
        {
            (levelData.pX,levelData.pY) = PlayerCord();

            mapComputeManager.Compute(levelData);

            DestroyTilesBeyondRadius(playerTransform.position.x, playerTransform.position.y);
            FillTilesInRadius(playerTransform.position.x, playerTransform.position.y);


            if (livingMap && !loadingNextLevel && Time.time - lastTime > .5)
            {
                MoveWallsTowards(playerTransform.position.x, playerTransform.position.y);

                lastTime = Time.time;
            }
            var pos = FindFloor(playerTransform.position);
            var mag = (playerTransform.position - pos).magnitude;
            if (Math.Floor(mag) >= 1)
            {
                HealthStat h = playerTransform.gameObject.GetComponent<StatControler>()?.GetStatOfType(StatType.HealthStat) as HealthStat;
                if (h != null)
                    h.addValue(-1 * h.BaseValue * .05f * mag);
            }
            playerTransform.position = pos;


            (int, int) cord = PlayerCord();
            MapTile tile;
            if (visableMap.TryGetValue(cord, out tile))
            {
                if (tile.mapAndType.Item1 == 0)
                    pm.canMove = true;
                else
                    pm.canMove = false;
            }

        }
        public delegate void OnLevelLoad(int level);
        public OnLevelLoad OnLevelLoadCallback;

        public delegate void OnLevelJoin(int level);
        public OnLevelJoin OnLevelJoinCallback;

        public void LoadNextLevel()
        {
            loadingNextLevel = true;
            nextLevelData = Level.Load(levelData.levelNum + 1);
            OnLevelLoadCallback.Invoke(levelData.levelNum + 1);
        }

        public void JoinNextLevel()
        {
            if (loadingNextLevel)
            {
                levelData = nextLevelData;
                Update();
                loadingNextLevel = false;
                OnLevelJoinCallback.Invoke(levelData.levelNum);
            }

        }

        private void OnDestroy()
        {
            mapComputeManager.OnDestroy();
        }

        public (int, int ) PlayerCord()
        {
            return ((int)Math.Floor(playerTransform.position.x), (int)Math.Floor(playerTransform.position.y));
        }

        public void SpawnTile(int x, int y)
        {
            MapTile tile = new MapTile();

            int map = levelData.tileData[levelData.Of(x, y)].map;
            int tilecode = (x < 0 || x >= levelData.width || y < 0 || y >= levelData.height) ? 255 : levelData.tileData[levelData.Of(x, y)].tilecode;

            tile.mapAndType = tileManager.SpawnTile(map,tilecode, x, y);
            tile.visable = true;

            visableMap[(x, y)] = tile;
        }

        public void DamageTile(float x, float y, float damage)
        {
            (int _x, int _y) = ((int)Math.Floor(x), (int)Math.Floor(y));

            var v = levelData.tileData[levelData.Of(_x, _y)].value;

            // apply damage
            levelData.tileData[levelData.Of(_x, _y)].value -= 1/ (wallHealth / (damage * Math.Min(1, v)) + 1);
        }

        public bool IsFloor(Vector2 p)
        {
            return IsFloor(p.x, p.y);
        }

        public bool IsFloor(float x,float y)
        { 
            (int _x, int _y) = ((int)Math.Floor(x), (int)Math.Floor(y));
            
            if (levelData.tileData[levelData.Of(_x,_y)].map == 0)
                return true;

            return false;
        }

        public Vector3 FindFloor(Vector3 pos)
        {
            Vector3 v = new Vector3();
            float f = 1;
            while (!IsFloor(pos + v) && !IsHole(pos + v))
            {
                v.x = UnityEngine.Random.value - .5f;
                v.y = UnityEngine.Random.value - .5f;
                v = v * f;
                f += .1f;
            }
            return pos + v;
        }

        public bool IsHole(Vector2 p)
        {
            return IsHole(p.x, p.y);
        }

        public bool IsHole(float x, float y)
        {
            (int _x, int _y) = ((int)Math.Floor(x), (int)Math.Floor(y));

            if (levelData.tileData[levelData.Of(_x, _y)].map == -1)
                return true;

            return false;
        }

        public void DestroyTile((int, int) cord)
        {
            if (visableMap.ContainsKey(cord))
            { 
                tileManager.DestroyTile(visableMap[cord].mapAndType, cord);

                visableMap.Remove(cord);
            }
        }

        private void DestroyTilesBeyondRadius(double x, double y)
        {

            KeyValuePair<(int, int), MapTile>[] query = visableMap.Where(p => Math.Pow((p.Key.Item1 - x), 2) + Math.Pow((p.Key.Item2 - y), 2) > dRSqrd).ToArray();

            foreach (KeyValuePair<(int, int), MapTile> p in query)
                DestroyTile(p.Key);
        }

        private void FillTilesInRadius(double x, double y)
        {
            TileData tileData;
            MapTile tile;
            for (double i = -sightRadius; i <= sightRadius; ++i)
                for (double j = -sightRadius; j <= sightRadius; ++j)
                {
                    if (i * i + j * j <= sRSqrd)
                    {
                        var cord = ((int)(i + x), (int)(j + y));
                        if (!visableMap.ContainsKey(cord))
                            SpawnTile(cord.Item1, cord.Item2);
                        else
                        {
                            tile = visableMap[cord];
                            tileData = levelData.tileData[levelData.Of(cord.Item1, cord.Item2)];
                            if (tile.mapAndType.Item1 != tileData.map ||
                                tile.mapAndType.Item2 != tileData.tilecode)
                            {
                                DestroyTile(cord);

                                SpawnTile(cord.Item1, cord.Item2);
                            }

                        }
                    }
                }
        }


        public void MoveWallsTowards(double x, double y)
        {
            float deadRaidius = .75f;
            x = Math.Floor(x);
            y = Math.Floor(y);

            (int px, int py) = PlayerCord();

            System.Random rnd = new System.Random();

            //numSpawn = rnd.Next(numSpawn);

            //SortedList<(int, int), double> hi = new SortedList<(int, int), double>(numSpawn);

            foreach (KeyValuePair<(int, int), MapTile> pair in visableMap)
            {
                (int _x, int _y) = pair.Key;

                if ((pair.Value.mapAndType.Item2 & 15) < 15 )//
                {
                    var t = tileGrowth.GetGrowthChance(_x, _y);


                    var curVal = levelData.tileData[levelData.Of(_x, _y)].value;
                    var toVal = levelData.tileData[levelData.Of(_x, _y)].baseValue*t.x + t.y;
                    levelData.tileData[levelData.Of(_x, _y)].value += (float)(toVal - curVal) * growthFactor;
                }
            }

        }
    }
}
/*
 * &&
                    (_x > px + deadRaidius || _x < px - deadRaidius ||
                    _y > py + deadRaidius || _y < py - deadRaidius)
*/