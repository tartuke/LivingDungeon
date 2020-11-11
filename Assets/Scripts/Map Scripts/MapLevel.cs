using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Mathematics;
using UnityEngine;

namespace Map
{
    public static class Level
    {
        public static void Save(LevelData input, int level)
        {
            FileStream file = File.Create(FilePath(level));
            var bformatter = new BinaryFormatter();
            bformatter.Serialize(file, input);
            file.Close();
        }

        public static LevelData Load(int level)
        {
            if (File.Exists(FilePath(level)))
            {
                Debug.Log("Loading level " + level);
                FileStream file = File.Open(FilePath(level), FileMode.Open);
                var bformatter = new BinaryFormatter();
                object data = bformatter.Deserialize(file);
                file.Close();
                return (LevelData)data;
            }
            else
                return LevelGenerator.Generate(level);
        }

        public static string FilePath(int level)
        {
            return Application.persistentDataPath + "/LevelData/level_" + level;
        }
    }

    [Serializable]
    public struct TileData
    {
        public int map;
        public int tilecode;
        public float baseValue;
        public float value;
        public float pDis;
        public float minPDis;
    }

    [Serializable]
    public struct LevelData
    {
        public int width;
        public int height;
        public int levelNum;
        public TileData[] tileData;
        public int pX;
        public int pY;

        public int Of(int x, int y)
        {
            x = math.clamp(x, 0, width-1);
            y = math.clamp(y, 0, height - 1);
            return x + y * width;
        }
    }
}