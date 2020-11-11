using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapComputeManager
    {
        public static ComputeShader Shader;

        ComputeBuffer resultBuffer;
        int kernelCSMap;
        int kernelCSTileCode;
        int kernelCSPDis;
        uint threadGroupSize;
        LevelData levelData;

        public MapComputeManager(LevelData data)
        {
            levelData = data;

            //program we're executing
            kernelCSMap = Shader.FindKernel("CSMap");
            kernelCSTileCode = Shader.FindKernel("CSTileCode");
            kernelCSPDis = Shader.FindKernel("CSPDis");
            Shader.GetKernelThreadGroupSizes(kernelCSMap, out threadGroupSize, out _, out _);

            //buffer on the gpu in the ram
            resultBuffer = new ComputeBuffer(levelData.tileData.Length, sizeof(int)*2+sizeof(float)*4);
            

        }

        public void Compute(LevelData data)
        {
            levelData = data;

            Shader.SetInt("width", levelData.width);
            Shader.SetInt("height", levelData.height);
            Shader.SetFloat("thresholdWall", MapThresholds.Wall);
            Shader.SetFloat("thresholdFloor", MapThresholds.Floor);
            Shader.SetInt("pX", levelData.pX);
            Shader.SetInt("pY", levelData.pY);

            resultBuffer.SetData(levelData.tileData);

            Shader.SetBuffer(kernelCSMap, "tileData", resultBuffer);
            Shader.SetBuffer(kernelCSTileCode, "tileData", resultBuffer);
            Shader.SetBuffer(kernelCSPDis, "tileData", resultBuffer);

            int threadGroups = (int)((levelData.tileData.Length + (threadGroupSize - 1)) / threadGroupSize);

            Shader.Dispatch(kernelCSMap, threadGroups, 1, 1);
            Shader.Dispatch(kernelCSTileCode, threadGroups, 1, 1);
            Shader.Dispatch(kernelCSPDis, threadGroups, 1, 1);
            resultBuffer.GetData(levelData.tileData);
        }

        public void OnDestroy()
        {
            resultBuffer.Dispose();
        }
    }
}
