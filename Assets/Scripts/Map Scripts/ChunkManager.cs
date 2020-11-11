using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    private int chunkSize = 16; // must be power of 2
    private int numChunks = 8; // must be power of 2

    private WrapingCollection2D<TileData> mapData;
    private WrapingCollection2D<ChunkData> chunks;

    public int playerLoadLevel = 0;
    public int maxLoadLevel = 4;

    public Transform playerTransform;

    public Vector2Int liveChunk;

    public TileManager_V3 tileManager;

    private void Start()
    {
        tileManager = new TileManager_V3();

        mapData = new WrapingCollection2D<TileData>(chunkSize * numChunks);
        chunks = new WrapingCollection2D<ChunkData>(numChunks);

        UpdateChuncks(PlayerChunck());
    }

    private void Update()
    {
        Vector2Int playerChunck = PlayerChunck();
        if (liveChunk != playerChunck)
        {
            UpdateChuncks(playerChunck);
            liveChunk = playerChunck;
        }
    }

    // get the cords of the chunk the player is in
    public Vector2Int PlayerChunck()
    {
        int x = ((int)math.floor(playerTransform.position.x)) / chunkSize;
        int y = ((int)math.floor(playerTransform.position.x)) / chunkSize;

        return new Vector2Int(x, y);
    }

    // update chunks if the player moves to a new chunk
    public void UpdateChuncks(Vector2Int playerChunk)
    {
        int x, y, level;

        Queue<Vector2Int> loadQ = new Queue<Vector2Int>();
        Queue<Vector2Int> unloadQ = new Queue<Vector2Int>();

        for (int i = -(numChunks >> 1); i < numChunks >> 1; i++)
        {
            for (int j = -(numChunks >> 1); j < numChunks >> 1; j++)
            {
                level = playerLoadLevel + math.max(math.abs(i), math.abs(j));

                
                x = playerChunk.x + i;
                y = playerChunk.y + j;

                Debug.Log("chunck " + (x, y) + " level: " + level);

                ChunkData cD = chunks[x, y];

                var pos = new Vector2Int(x, y);

                // set new chunk
                if (cD.chunckCord == null)
                {
                    cD.loadLevel = maxLoadLevel;
                    cD.chunckCord = pos;
                }
               

                // reset old chunck
                if (chunks[x, y].chunckCord != pos)
                {
                    cD.chunckCord = pos;
                    cD.loadLevel = maxLoadLevel;
                }

                // set desired level
                cD.update = level;

                // send to Queue
                if (cD.update < cD.loadLevel)
                {
                    loadQ.Enqueue(cD.chunckCord);
                }
                else if (cD.update > cD.loadLevel + 1)
                {
                    unloadQ.Enqueue(cD.chunckCord);
                }

                chunks[x, y] = cD;
            }
        }

        LoadChunks(loadQ);
        UnloadChunks(unloadQ);
    }

    private void LoadChunks(Queue<Vector2Int> loadQ)
    {

        // calculate base values
        Load(loadQ, 4, 4, LoadBaseValues);
        // calculate tile codes
        Load(loadQ, 4, 3, LoadTileCodes);

        // spawn tiles
        Load(loadQ, 3, 3, LoadTiles);
        // spawn enemies
        Load(loadQ, 3, 2, LoadEnemies);

        // make walls and enemies start updating
        Load(loadQ, 2, 1, LoadLogic);

        // everything is loaded
        
    }

    public delegate void Loader(int x, int y);

    private void Load(Queue<Vector2Int> loadQ, int find, int set , Loader load)
    {
        var count = loadQ.Count;
        ChunkData cD;
        Vector2Int cord;
        while (count > 0)
        {
            Debug.Log(loadQ.Count);
            cord = loadQ.Dequeue();
            cD = chunks[cord.x, cord.y];

            if (cD.loadLevel >= find)
            {
                int x, y;
                var offsetX = cD.chunckCord.x * chunkSize;
                var offsetY = cD.chunckCord.y * chunkSize;

                // loop through tiles
                for (int i = 0; i < chunkSize; i++)
                {
                    for (int j = 0; j < chunkSize; j++)
                    {
                        x = offsetX + i;
                        y = offsetY + j;

                        // call load function
                        load(x, y);
                    }
                }
                cD.loadLevel = set;
            }

            if (cD.loadLevel > cD.update)
            {
                Debug.Log("enqueue " + cord);
                loadQ.Enqueue(cord);
            }

            count--;
        }
    }

    // set base values
    private void LoadBaseValues(int x, int y)
    {
        
        TileData tD = mapData[x,y];
        Debug.Log("value added to " + (x, y));
        tD.baseValue = tileManager.noiseFunct(x,y);
        tD.value = tD.baseValue;

        mapData[x,y] = tD;
    }

    // set tile codes
    private void LoadTileCodes(int x, int y)
    {
        TileData tD = mapData[x,y];

        tD.map = (tD.value > .5) ? 1 : 0;
        tD.tileCode = CalculateTileCode(x,y, tD.map == 1);
        Debug.Log("tile code " + tD.tileCode + " added to " + (x, y));
        mapData[x,y] = tD;
    }

    // spawn tiles
    private void LoadTiles(int x ,int y)
    {
        TileData tD = mapData[x, y];

        tileManager.SpawnTile(x,y, tD.map, tD.tileCode);
        Debug.Log("tile spawned at " + (x, y));
        mapData[x,y] = tD;
    }

    // spawn enemies
    private void LoadEnemies(int x, int y)
    {
        TileData tD = mapData[x, y];

        // load enemies

        mapData[x, y] = tD;
    }

    // start logic
    private void LoadLogic(int x, int y)
    {
        TileData tD = mapData[x, y];

        // load logic

        mapData[x, y] = tD;
    }

    private void UnloadChunks(Queue<Vector2Int> unloadQ)
    {
        // stop walls and enimies from updating
        Unloader(unloadQ, 1, 2, UnloadLogic);
        // despawn walls
        Unloader(unloadQ, 2, 2, UnloadEnemies);
        //  despawn enemies
        Unloader(unloadQ, 2, 3, UnloadTiles);
        // save off values
        Unloader(unloadQ, 3, 4, UnloadBaseValues);
        // everything is despawned
    }

    private void UnloadLogic(int x, int y)
    {
        TileData tD = mapData[x, y];

        // unload logic

        mapData[x, y] = tD;
    }

    private void UnloadEnemies(int x, int y)
    {
        TileData tD = mapData[x, y];

        // unload enemies

        mapData[x, y] = tD;
    }

    private void UnloadTiles(int x, int y)
    {
        TileData tD = mapData[x, y];

        // unload tiles
        tileManager.DestroyTile(tD.map, x, y);

        mapData[x, y] = tD;
    }

    private void UnloadBaseValues(int x, int y)
    {
        TileData tD = mapData[x, y];

        // unload values

        mapData[x, y] = tD;
    }

    private void Unloader(Queue<Vector2Int> unloadQ, int find, int set, Loader unload)
    {
        var count = unloadQ.Count;
        ChunkData cD;
        Vector2Int cord;
        while (count > 0)
        {
            cord = unloadQ.Dequeue();
            cD = chunks[cord.x, cord.y];

            if (cD.loadLevel <= find)
            {
                int x, y;
                var offsetX = cD.chunckCord.x * chunkSize;
                var offsetY = cD.chunckCord.y * chunkSize;

                // loop through tiles
                for (int i = 0; i < chunkSize; i++)
                {
                    for (int j = 0; j < chunkSize; j++)
                    {
                        x = offsetX + i;
                        y = offsetY + j;

                        // call unload function
                        unload(x, y);
                    }
                }
                cD.loadLevel = set;
            }

            if (cD.loadLevel < cD.update)
                unloadQ.Enqueue(cord);

            count--;
        }
    }

    public int CalculateTileCode(int x, int y, bool isWall)
    {
        var flip = !isWall;
        int tilecode = 0;
        tilecode |= flip ^ mapData[x, y + 1].value > .5 ? 1 : 0;
        tilecode |= flip ^ mapData[x + 1, y].value > .5 ? 2 : 0;
        tilecode |= flip ^ mapData[x, y - 1].value > .5 ? 4 : 0;
        tilecode |= flip ^ mapData[x - 1, y].value > .5 ? 8 : 0;
        tilecode |= flip ^ mapData[x + 1, y + 1].value > .5 ? 16 : 0;
        tilecode |= flip ^ mapData[x + 1, y - 1].value > .5 ? 32 : 0;
        tilecode |= flip ^ mapData[x - 1, y - 1].value > .5 ? 64 : 0;
        tilecode |= flip ^ mapData[x - 1, y + 1].value > .5 ? 128 : 0;

        return tilecode;
    }
}

struct TileData
{
    public int map;
    public float baseValue;
    public float value;
    public int tileCode;
}

class WrapingCollection2D<T>
{
    private T[,] arr;
    private int mask;

    public WrapingCollection2D(int length)
    {
        arr = new T[length,length];
        mask = length - 1;
    }

    public T this[int i,int j]
    {
        get => arr[i&mask,j&mask];
        set => arr[i&mask,j&mask] = value;
    }
}
