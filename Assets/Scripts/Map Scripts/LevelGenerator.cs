using Unity.Mathematics;
using UnityEngine;

namespace Map
{
    public static class LevelGenerator
    {
        public static LevelData Generate(int level)
        {
            var Ld = new LevelData();

            Ld.width = 128;
            Ld.height = 128;

            Ld.levelNum = level;

            Ld.tileData = new TileData[Ld.width * Ld.height];

            for (int i = 0; i < Ld.width; i++)
            {
                for (int j = 0; j < Ld.height; j++)
                {
                    float baseValue;
                    // is edge
                    if (i == 0 || i == Ld.width-1 || j == 0 || j == Ld.height -1)
                    {
                        baseValue = int.MaxValue/2;
                    } else
                    { 
                        baseValue = noiseFunct(i + Ld.width*level, j + Ld.height*level, .14f, level%3);
                    }
                    var index = Ld.Of(i, j);
                    //Debug.Log((i, j));

                    Ld.tileData[index].baseValue = baseValue;
                    Ld.tileData[index].value = baseValue;
                }
            }

            //CalculateAllTileCodes(Ld);

            return Ld;
        }


        public static int CalculateTileCode(LevelData Ld, int x, int y)
        {
            int tilecode = 0;
            int map = Ld.tileData[Ld.Of(x, y)].map;

            tilecode |= Ld.tileData[Ld.Of(x, y + 1)].map == map ? 1 : 0;
            tilecode |= Ld.tileData[Ld.Of(x + 1, y)].map == map ? 2 : 0;
            tilecode |= Ld.tileData[Ld.Of(x, y - 1)].map == map ? 4 : 0;
            tilecode |= Ld.tileData[Ld.Of(x - 1, y)].map == map ? 8 : 0;
            tilecode |= Ld.tileData[Ld.Of(x + 1, y + 1)].map == map ? 16 : 0;
            tilecode |= Ld.tileData[Ld.Of(x + 1, y - 1)].map == map ? 32 : 0;
            tilecode |= Ld.tileData[Ld.Of(x - 1, y - 1)].map == map ? 64 : 0;
            tilecode |= Ld.tileData[Ld.Of(x - 1, y + 1)].map == map ? 128 : 0;

            Ld.tileData[Ld.Of(x, y)].tilecode = tilecode;

            return tilecode;
        }

        public static void CalculateAllTileCodes(LevelData Ld)
        {
            for (int i = 0; i < Ld.width; i++)
            {
                for (int j = 0; j < Ld.height; j++)
                {
                    CalculateTileCode(Ld, i, j);
                }
            }
        }

        public static float noiseFunct(int x, int y,float stepSize, int type = 0)
        {
            float v;
            switch (type)
            {
                case 0:
                    return (noise.cnoise(new float2(stepSize * x, stepSize * y)) + 1) / 2;
                case 1:
                    v = math.sin(x) / 3 + 1;
                    v = (noise.cnoise(new float2(stepSize * x, stepSize * y)) + 1) / 2 * v;
                    return math.tan((v - .5f) * 2) / 4 + .4f;
                case 2:
                    v = noise.cellular(new float2(stepSize / 1f * x, stepSize / 1f * y)).x;
                    return math.tan((v-.5f)*2)/5+.47f;
                default:
                    break;
            }
            return (noise.cnoise(new float2(stepSize * x, stepSize * y)) + 1) / 2;
        }
    }
}