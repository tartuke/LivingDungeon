using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileSet
{
    protected Dictionary<int, TileBase> Tiles;

    public bool Extended = false;

    public virtual TileBase GetTile(int num)
    {
        TileBase tile;

        Tiles.TryGetValue(num, out tile);

        return tile;
    }
}

[Serializable]
public class WallTileSet : TileSet
{
    public override TileBase GetTile(int tilecode)
    {
        int code = ((tilecode & 255) == 255) ? 255 :
                    ((tilecode & 239) == 239) ? 239 :
                    ((tilecode & 223) == 223) ? 223 :
                    ((tilecode & 207) == 207) ? 207 :
                    ((tilecode & 205) == 205) ? 205 :
                    ((tilecode & 191) == 191) ? 191 :
                    ((tilecode & 127) == 127) ? 127 :
                    ((tilecode & 111) == 111) ? 111 :
                    ((tilecode & 110) == 110) ? 110 :
                    ((tilecode & 78) == 78) ? 78 :
                    ((tilecode & 77) == 77) ? 77 :
                    ((tilecode & 76) == 76) ? 76 :
                    ((tilecode & 63) == 63) ? 63 :
                    ((tilecode & 55) == 55) ? 55 :
                    ((tilecode & 46) == 46) ? 46 :
                    ((tilecode & 39) == 39) ? 39 :
                    ((tilecode & 38) == 38) ? 38 :
                    ((tilecode & 159) == 159) ? 14 :
                    ((tilecode & 155) == 155) ? 10 : tilecode%16;

        TileBase tile;

        Tiles.TryGetValue(code, out tile);

        return tile;
    }
    public WallTileSet()
    {
        Extended = true;

        Tiles = new Dictionary<int, TileBase>();

        Tiles[0] = Resources.Load<Tile>("Tiles/Wall-0_0");
        Tiles[1] = Resources.Load<Tile>("Tiles/Wall-0_1");
        Tiles[2] = Resources.Load<Tile>("Tiles/Wall-0_2");
        Tiles[3] = Resources.Load<Tile>("Tiles/Wall-0_3");
        Tiles[4] = Resources.Load<Tile>("Tiles/Wall-0_4");
        Tiles[5] = Resources.Load<Tile>("Tiles/Wall-0_5");
        Tiles[6] = Resources.Load<Tile>("Tiles/Wall-0_6");
        Tiles[7] = Resources.Load<Tile>("Tiles/Wall-0_7");
        Tiles[8] = Resources.Load<Tile>("Tiles/Wall-0_8");
        Tiles[9] = Resources.Load<Tile>("Tiles/Wall-0_9");
        Tiles[10] = Resources.Load<Tile>("Tiles/Wall-0_10");
        Tiles[11] = Resources.Load<Tile>("Tiles/Wall-0_11");
        Tiles[12] = Resources.Load<Tile>("Tiles/Wall-0_12");
        Tiles[13] = Resources.Load<Tile>("Tiles/Wall-0_13");
        Tiles[14] = Resources.Load<Tile>("Tiles/Wall-0_14");
        Tiles[15] = Resources.Load<Tile>("Tiles/Wall-0_15");
        Tiles[39] = Resources.Load<Tile>("Tiles/Wall-0_16");
        Tiles[55] = Resources.Load<Tile>("Tiles/Wall-0_17");
        Tiles[77] = Resources.Load<Tile>("Tiles/Wall-0_18");
        Tiles[110] = Resources.Load<Tile>("Tiles/Wall-0_19");
        Tiles[111] = Resources.Load<Tile>("Tiles/Wall-0_20");
        Tiles[205] = Resources.Load<Tile>("Tiles/Wall-0_21");
        Tiles[255] = Resources.Load<Tile>("Tiles/Wall-0_22");
        Tiles[38] = Resources.Load<Tile>("Tiles/Wall-0_24");
        Tiles[76] = Resources.Load<Tile>("Tiles/Wall-0_25");
        Tiles[207] = Resources.Load<Tile>("Tiles/Wall-0_26");
        Tiles[63] = Resources.Load<Tile>("Tiles/Wall-0_27");
        Tiles[239] = Resources.Load<Tile>("Tiles/Wall-0_28");
        Tiles[127] = Resources.Load<Tile>("Tiles/Wall-0_29");
        Tiles[223] = Resources.Load<Tile>("Tiles/Wall-0_30");
        Tiles[191] = Resources.Load<Tile>("Tiles/Wall-0_31");
        Tiles[46] = Resources.Load<Tile>("Tiles/Wall-0_32");
        Tiles[78] = Resources.Load<Tile>("Tiles/Wall-0_33");
    }

    
}

[Serializable]
public class FloorTileSet : TileSet
{
    public override TileBase GetTile(int tilecode)
    {
        TileBase tile;

        Tiles.TryGetValue(tilecode%16, out tile);

        return tile;
    }
    public FloorTileSet()
    {
        Tiles = new Dictionary<int, TileBase>();

        Tiles[0] = Resources.Load<Tile>("Tiles/Floor-0_0");
        Tiles[1] = Resources.Load<Tile>("Tiles/Floor-0_8");
        Tiles[2] = Resources.Load<Tile>("Tiles/Floor-0_9");
        Tiles[3] = Resources.Load<Tile>("Tiles/Floor-0_5");
        Tiles[4] = Resources.Load<Tile>("Tiles/Floor-0_3");
        Tiles[5] = Resources.Load<Tile>("Tiles/Floor-0_8");
        Tiles[6] = Resources.Load<Tile>("Tiles/Floor-0_0");
        Tiles[7] = Resources.Load<Tile>("Tiles/Floor-0_5");
        Tiles[8] = Resources.Load<Tile>("Tiles/Floor-0_2");
        Tiles[9] = Resources.Load<Tile>("Tiles/Floor-0_7");
        Tiles[10] = Resources.Load<Tile>("Tiles/Floor-0_1");
        Tiles[11] = Resources.Load<Tile>("Tiles/Floor-0_6");
        Tiles[12] = Resources.Load<Tile>("Tiles/Floor-0_2");
        Tiles[13] = Resources.Load<Tile>("Tiles/Floor-0_7");
        Tiles[14] = Resources.Load<Tile>("Tiles/Floor-0_1");
        Tiles[15] = Resources.Load<Tile>("Tiles/Floor-0_6");
    }
}

[Serializable]
public class HoleTileSet : TileSet
{
    public override TileBase GetTile(int tilecode)
    {
        TileBase tile;

        Tiles.TryGetValue(0, out tile);

        return tile;
    }
    public HoleTileSet()
    {
        Tiles = new Dictionary<int, TileBase>();
        Tiles[0] = Resources.Load<Tile>("Tiles/Wall_8");
    }
}