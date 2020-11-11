using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;


public static class MapThresholds
{
    public static float Wall = .5f;
    public static float Floor = .2f;
}

[CreateAssetMenu(fileName = "TileManager",menuName ="My Game/TileManager")]
public class TileManager : ScriptableObject
{
    public float stepSize;
    public float seed;
    public float totalRange;

    public string[] Tilemaps;
    internal Tilemap[] tilemaps;
    public TileBase[] tiles;
    public float[] range;
    (float, float) minMax;

    //public void Awake()
    //{
    //    tilemaps = new Tilemap[Tilemaps.Length];
    //    for (int t = 0; t < Tilemaps.Length; t++)
    //    {
    //        tilemaps[t] = GameObject.Find(Tilemaps[t]).GetComponent<Tilemap>();
    //        totalRange += range[t];
    //    }
    //}

    private float noiseFunct(int x, int y, int type=0)
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

        float threshold = noiseFunct(x,y,0);
        //float threshold = (Mathf.PerlinNoise(stepSize*x+seed, stepSize*y+seed)%1);
        minMax.Item1 = math.min(threshold, minMax.Item1);
        minMax.Item2 = math.max(threshold, minMax.Item2);
        threshold *= totalRange;

        float tileLevel = range[0];
        int i = 0;
        for (; i < range.Length -1 && tileLevel < threshold; i++)
            tileLevel += range[i+1];

        int optionNum = 0;

        i = (i)%tilemaps.Length;

        if (tilemaps[i] == null)
            tilemaps[i] = GameObject.Find(Tilemaps[i]).GetComponent<Tilemap>();

        tilemaps[i].SetTile(new Vector3Int(x, y, 0), tiles[i]);
       
        return (i, optionNum);
    }

    public (int, int) SpawnTile(int x, int y ,int map)
    {
        tilemaps[map].SetTile(new Vector3Int(x, y, 0), tiles[map]);
        return (map, 0);
    }

    public void DestroyTile((int,int) tile, (int,int) cord )
    {
        tilemaps[tile.Item1].SetTile(new Vector3Int(cord.Item1, cord.Item2, 0), null);
    }
}


