using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager_V2 : MonoBehaviour
{

    public float sightRadius = 3;
    private double sRSqrd = 9;

    public float despawnRadius = 4;
    private double dRSqrd = 16;

    public bool livingMap = true;

    public TileManager_V2 tileManager;

    Dictionary<(int, int), MapTile> map = new Dictionary<(int, int), MapTile>();
    Dictionary<(int, int), MapTile> visableMap = new Dictionary<(int, int), MapTile>();

    public Transform playerTransform;
    public PlayerMovement_v2 pm;

    public TileGrowth tileGrowth;

    float lastTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = new TileManager_V2();

        // set up growth map
        tileGrowth = new TileGrowth();
        tileGrowth.Equations.Add(new RaidialDisplacement(playerTransform, 1.5));
        //tileGrowth.Equations.Add(new RaidialDisplacement(playerTransform, 6));
        //tileGrowth.Equations.Add(new RayDisplacement(playerTransform, 1,1,5,false));


        sRSqrd = sightRadius * sightRadius;
        despawnRadius = despawnRadius > sightRadius ? despawnRadius : sightRadius + 1;
        dRSqrd = despawnRadius * despawnRadius;
    }

    bool flip = false;

    // Update is called once per frame
    void Update()
    {
        FillTilesInRadius(playerTransform.position.x, playerTransform.position.y);
        DestroyTilesBeyondRadius(playerTransform.position.x, playerTransform.position.y);

        
        if (livingMap && Time.time - lastTime > .5)
        {
            //MoveWallsTowards(playerTransform.position.x, playerTransform.position.y);

            if (!flip)
            {
                MoveWallsTowards(playerTransform.position.x, playerTransform.position.y);
                flip = true;
            }
            else
            {
                RemoveWallsTowards(playerTransform.position.x, playerTransform.position.y);
                flip = false;
            }
            lastTime = Time.time;
        }

        (int, int) cord = PlayerCord();
        MapTile tile;
        if (map.TryGetValue(cord, out tile))
        {
            if (tile.mapAndType.Item1 == 0)
                pm.canMove = true;
            else
                pm.canMove = false;
        }

    }

    public (int, int ) PlayerCord()
    {
        return ((int)Math.Floor(playerTransform.position.x), (int)Math.Floor(playerTransform.position.y));
    }

    public void SpawnTile(int x, int y)
    {
        if (visableMap.ContainsKey((x, y)))
            return;

        if (!map.ContainsKey((x, y)))
            map.Add((x, y), new MapTile());

        map[(x, y)].mapAndType = tileManager.SpawnTile(x, y);
        map[(x, y)].visable = true;

        visableMap.Add((x, y), map[(x, y)]);
    }

    public int GetTileCode(int x, int y, int mapNum)
    {
        MapTile tile;
        int tilecode = 0;
        tilecode |= map.TryGetValue((x, y + 1), out tile) && tile.mapAndType.Item1 == mapNum ? 1 : 0;
        tilecode |= map.TryGetValue((x + 1, y), out tile) && tile.mapAndType.Item1 == mapNum ? 2 : 0;
        tilecode |= map.TryGetValue((x, y - 1), out tile) && tile.mapAndType.Item1 == mapNum ? 4 : 0;
        tilecode |= map.TryGetValue((x - 1, y), out tile) && tile.mapAndType.Item1 == mapNum ? 8 : 0;

        tilecode |= map.TryGetValue((x + 1, y + 1), out tile) && tile.mapAndType.Item1 == mapNum ? 16 : 0;
        tilecode |= map.TryGetValue((x + 1, y - 1), out tile) && tile.mapAndType.Item1 == mapNum ? 32 : 0;
        tilecode |= map.TryGetValue((x - 1, y - 1), out tile) && tile.mapAndType.Item1 == mapNum ? 64 : 0;
        tilecode |= map.TryGetValue((x - 1, y + 1), out tile) && tile.mapAndType.Item1 == mapNum ? 128 : 0;

        return tilecode;
    }

    public void RecalcTileCodes(int x, int y, int mapNum)
    {
        RecalcTileCode(x, y + 1, mapNum, 4);
        RecalcTileCode(x + 1, y, mapNum, 8);
        RecalcTileCode(x, y - 1, mapNum, 1);
        RecalcTileCode(x - 1, y, mapNum, 2);
        RecalcTileCode(x + 1, y + 1, mapNum, 64);
        RecalcTileCode(x + 1, y - 1, mapNum, 128);
        RecalcTileCode(x - 1, y - 1, mapNum, 16);
        RecalcTileCode(x - 1, y + 1, mapNum, 32);
    }

    private void RecalcTileCode(int x, int y, int mapNum, int mask)
    {
        MapTile tile;
        int tilecode;

        if (map.TryGetValue((x, y), out tile))
        {
            tilecode = tile.mapAndType.Item1 == mapNum ? tile.mapAndType.Item2 |= mask : tile.mapAndType.Item2 &= ~mask;
            tileManager.SpawnTile(x, y, tile.mapAndType.Item1, tilecode);
        }
    }

    public void SpawnTile(int x, int y, int mapNum)
    {
        if (visableMap.ContainsKey((x, y)))
            return;

        if (!map.ContainsKey((x, y)))
            map.Add((x, y), new MapTile());

        int tilecode = GetTileCode(x, y, mapNum);

        map[(x, y)].mapAndType = tileManager.SpawnTile(x, y, mapNum, tilecode);
        map[(x, y)].visable = true;

        visableMap.Add((x, y), map[(x, y)]);

        RecalcTileCodes(x, y, mapNum);
    }

    public bool IsFloor(float x,float y)
    {
        (int, int) cord = ((int)Math.Floor(x), (int)Math.Floor(y));

        MapTile mt;
        if (map.TryGetValue(cord, out mt) && mt.mapAndType.Item1 == 0)
            return true;

        return false;
    }

    public void BreakWall(int x, int y)
    {
        DestroyTile((x, y));
        SpawnTile(x, y, 0);
    }

    public void BreakWall(float fx, float fy)
    {
        int x = Mathf.FloorToInt(fx);
        int y = Mathf.FloorToInt(fy);

        DestroyTile((x, y));
        SpawnTile(x, y, 0);
    }

    public void DestroyTile((int, int) cord)
    {
        if (map.ContainsKey(cord))
            if (map[cord].visable)
            {
                tileManager.DestroyTile(map[cord].mapAndType, cord);
                map[cord].visable = false;

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
        System.Random rnd = new System.Random();
        for (double i = -sightRadius; i <= sightRadius; ++i)
            for (double j = -sightRadius; j <= sightRadius; ++j)
            {
                if (i * i + j * j <= sRSqrd)
                {
                    var cord = ((int)(i + x), (int)(j + y));
                    if (!visableMap.ContainsKey(cord))
                        SpawnTile(cord.Item1, cord.Item2);
                }
            }
    }

    public void MoveWallsTowards(double x, double y)
    {
        int dx, dy, px, py, numSpawn = 1;
        x = Math.Floor(x);
        y = Math.Floor(y);

        System.Random rnd = new System.Random();

        //numSpawn = rnd.Next(numSpawn);

        SortedList< (int, int),double> hi = new SortedList<(int, int), double>(numSpawn);

        foreach (KeyValuePair<(int, int), MapTile> pair in visableMap)
        {
            px = pair.Key.Item1;
            py = pair.Key.Item2;

            dx = (x - px) > 0 ? 1 : -1;
            dy = (y - py) > 0 ? 1 : -1;

            if (pair.Value.mapAndType.Item1 == 0)
            {
                if ((pair.Value.mapAndType.Item2 & 15) < 15)
                {
                    var t = tileGrowth.GetGrowthChance(px, py);

                    //t *= rnd.NextDouble();

                    if (hi.Count < numSpawn)
                        hi.Add(pair.Key, t);
                    else if (hi.Count > 0 && t > hi.ElementAt(0).Value)
                    {

                        hi.RemoveAt(0);
                        hi.Add(pair.Key, t);
                    }
                }
            }
        }
        foreach (KeyValuePair<(int, int), double> p in hi)
        {
            if (p.Key == PlayerCord()) continue;

            DestroyTile(p.Key);
            SpawnTile(p.Key.Item1, p.Key.Item2, 1);
        }
    }

    public void RemoveWallsTowards(double x, double y)
    {
        int dx, dy, px, py, numSpawn = 1;
        x = Math.Floor(x);
        y = Math.Floor(y);

        System.Random rnd = new System.Random();

        //numSpawn = rnd.Next(numSpawn);

        SortedList<(int, int), double> low = new SortedList<(int, int), double>(numSpawn);
        //Debug.Log("start");
        foreach (KeyValuePair<(int, int), MapTile> pair in visableMap)
        {
            px = pair.Key.Item1;
            py = pair.Key.Item2;

            dx = (x - px) > 0 ? 1 : -1;
            dy = (y - py) > 0 ? 1 : -1;

            if (pair.Value.mapAndType.Item1 == 1)
            {
                if ((pair.Value.mapAndType.Item2 & 15) < 15)
                {
                    var t = tileGrowth.GetGrowthChance(px, py);

                    //t *= rnd.NextDouble();

                    if (low.Count < numSpawn)
                        low.Add(pair.Key,t);
                    else if (low.Count > 0 && t < low.ElementAt(numSpawn-1).Value)
                    {

                        low.RemoveAt(numSpawn -1);
                        low.Add(pair.Key,t);
                        //Debug.Log((low.ElementAt(0).Value, low.ElementAt(numSpawn - 1).Value));
                    }
                }
            }
        }
        foreach (KeyValuePair<(int, int), double> p in low)
        {
            DestroyTile(p.Key);
            SpawnTile(p.Key.Item1, p.Key.Item2, 0);
        }
    }
}
