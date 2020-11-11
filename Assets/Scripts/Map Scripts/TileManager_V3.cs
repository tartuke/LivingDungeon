using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

public class TileManager_V3
{
    public float stepSize = .14f;
    public float seed = 5;
    public float totalRange = 2;

    internal Tilemap[] tilemaps;
    public string[] Tilemaps = { "Floor", "Walls" };
    private TileSet[] tileSets = { new FloorTileSet(), new WallTileSet() };
    public float[] range = { 1, 1 };

    public TileManager_V3()
    {
        if (tilemaps == null || tilemaps.Length != Tilemaps.Length)
        {
            tilemaps = new Tilemap[Tilemaps.Length];
            for (int i = 0; i < Tilemaps.Length; i++)
            {
                if (tilemaps[i] == null)
                    tilemaps[i] = GameObject.Find(Tilemaps[i]).GetComponent<Tilemap>();
            }
        }
    }

    public float noiseFunct(int x, int y, int type=0)
    {
        switch (type)
        {
            case 0:
                return (noise.cnoise(new float2(stepSize * x, stepSize * y)) + 1) / 2;
            case 1:
                float v = Mathf.Sin(x) / 3 + 1;
                return (noise.cnoise(new float2(stepSize * x, stepSize * y)) + 1) / 2 * v;
            default:
                break;
        }
        return (noise.cnoise(new float2(stepSize * x, stepSize * y)) + 1) / 2;
    }

    public int GetTileCode(int x, int y, bool isWall)
    {
        var flip = !isWall;
        int tilecode = 0;
        tilecode |= flip ^ noiseFunct(x, y + 1, 0) > .5 ? 1 : 0;
        tilecode |= flip ^ noiseFunct(x + 1, y, 0) > .5 ? 2 : 0;
        tilecode |= flip ^ noiseFunct(x, y - 1, 0) > .5 ? 4 : 0;
        tilecode |= flip ^ noiseFunct(x - 1, y, 0) > .5 ? 8 : 0;
        tilecode |= flip ^ noiseFunct(x + 1, y + 1, 0) > .5 ? 16 : 0;
        tilecode |= flip ^ noiseFunct(x + 1, y - 1, 0) > .5 ? 32 : 0;
        tilecode |= flip ^ noiseFunct(x - 1, y - 1, 0) > .5 ? 64 : 0;
        tilecode |= flip ^ noiseFunct(x - 1, y + 1, 0) > .5 ? 128 : 0;

        return tilecode;
     }

    public (int,int) SpawnTile(int x, int y)
    {
        if (tilemaps == null || tilemaps.Length != Tilemaps.Length)
        {
            totalRange = 0;
            tilemaps = new Tilemap[Tilemaps.Length];
            for (int t = 0; t < Tilemaps.Length; t++)
            {
                totalRange += range[t];
            }
        }

        bool isWall = noiseFunct(x,y,0) > .5;

        int tilecode = GetTileCode(x, y, isWall);

        //float threshold = noiseFunct(x, y, 0);

        //threshold *= totalRange;

        //float tileLevel = range[0];
        //int i = 0;
        //for (; i < range.Length - 1 && tileLevel < threshold; i++)
        //    tileLevel += range[i + 1];

        //i = (i) % tilemaps.Length;

        int i = isWall ? 1 : 0;

        if (tilemaps[i] == null)
            tilemaps[i] = GameObject.Find(Tilemaps[i]).GetComponent<Tilemap>();

        tilemaps[i].SetTile(new Vector3Int(x, y, 0), tileSets[i].GetTile(tilecode));

        return (i, tilecode);
    }

    public (int, int) SpawnTile(int x, int y ,int map , int tilecode)
    {
        tilemaps[map].SetTile(new Vector3Int(x, y, 0), tileSets[map].GetTile(tilecode));

        return (map, tilecode);
    }

    public void DestroyTile((int,int) tile, (int,int) cord )
    {
        tilemaps[tile.Item1].SetTile(new Vector3Int(cord.Item1, cord.Item2, 0), null);
    }

    public void DestroyTile(int map, int x, int y)
    {
        tilemaps[map].SetTile(new Vector3Int(x, y, 0), null);
    }
}
