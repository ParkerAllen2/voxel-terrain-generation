using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator
{
    public static void RequestMapData(Biome biome, Vector3 center, Action<MapData> callback)
    {
        DebugCounter.AddDebug("Map Requested");
        ThreadStart threadStart = delegate
        {
            MapDataThread(biome, center, callback);
        };
        new Thread(threadStart).Start();
    }

    static void MapDataThread(Biome biome, Vector3 coord, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(biome, coord);
        Updater.Instance.EnqueueMapDataInfo(callback, mapData);
    }

    static MapData GenerateMapData(Biome biome, Vector3 coord)
    {
        float max = float.MinValue;
        float min = float.MaxValue;

        int[] data = new int[WorldSettings.Instance.NumVoxelsPerAxis * WorldSettings.Instance.NumVoxelsPerAxisSqr];
        for(int i = 0; i < data.Length; i++)
        {
            Vector3 voxelCoord = coord + WorldSettings.Instance.ConvertIndexToVector3(i) * WorldSettings.Instance.Spacing;
            float density = 0;
            foreach (IMapFunction func in biome.mapFunctions)
            {
                density += func.CalcDensityAtPoint(voxelCoord);
            }
            data[i] = biome.DensityToVoxelId(density);

            max = Mathf.Max(density, max);
            min = Mathf.Min(density, min);
        }
        Debug.Log("Max Density: " + max);
        Debug.Log("Min Density: " + min);

        return new MapData(data);
    }
}

public struct MapData
{
    public int[] data;

    public MapData(int[] mapData)
    {
        data = mapData;
    }
}
