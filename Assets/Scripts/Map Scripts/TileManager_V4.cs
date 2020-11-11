using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public static class MapThresholds
    {
        public static float Wall = .5f;
        public static float Floor = .2f;
    }

    public class TileManager
    {
        internal Tilemap[] tilemaps;
        public string[] Tilemaps = {"Floor", "Walls", "Holes" };
        private TileSet[] tileSets = { new FloorTileSet(), new WallTileSet(), new HoleTileSet() };

        public TileManager()
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

        public int CalcMap(float v)
        {
            return (v > .5) ? 1 : 0;
        }

        public (int,int) SpawnTile(TileData tileData, int x, int y)
        {
            int i = tileData.map;//CalcMap(tileData.value);
            int tilecode = tileData.tilecode;

            if (i == -1)
            {
                i = 2;
                tilecode = 0;
            }

            if (tilemaps[i] == null)
                tilemaps[i] = GameObject.Find(Tilemaps[i]).GetComponent<Tilemap>();

            tilemaps[i].SetTile(new Vector3Int(x, y, 0), tileSets[i].GetTile(tilecode));

            return (i, tilecode);
        }
        public (int, int) SpawnTile(int map, int tilecode, int x, int y)
        {
            int i = map;//CalcMap(tileData.value);

            if (i == -1)
            {
                i = 2;
                tilecode = 0;
            }

            if (tilemaps[i] == null)
                tilemaps[i] = GameObject.Find(Tilemaps[i]).GetComponent<Tilemap>();

            tilemaps[i].SetTile(new Vector3Int(x, y, 0), tileSets[i].GetTile(tilecode));

            return (i, tilecode);
        }

        public void DestroyTile((int,int) tile, (int,int) cord )
        {
            if (tile.Item1 == -1) return;
            tilemaps[tile.Item1].SetTile(new Vector3Int(cord.Item1, cord.Item2, 0), null);
        }

        public void DestroyTile(int map, int x, int y)
        {
            if (map == -1) return;
            tilemaps[map].SetTile(new Vector3Int(x, y, 0), null);
        }
    }
}
