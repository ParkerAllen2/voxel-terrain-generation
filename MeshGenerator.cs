using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeshGenerator
{
    public static void RequestMeshData(Biome biome, MapData mapData, Action<MeshData> callback)
    {
        DebugCounter.AddDebug("Mesh Requested");
        ThreadStart threadStart = delegate
        {
            MeshDataThread(biome, mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    static void MeshDataThread(Biome biome, MapData mapData, Action<MeshData> callback)
    {
        MeshData meshdata = GenerateMeshData(biome, mapData);
        Updater.Instance.EnqueueMeshDataInfo(callback, meshdata);
    }

    static MeshData GenerateMeshData(Biome biome, MapData mapData)
    {
        MeshData meshData = new MeshData();
        int[] data = mapData.data;
        int numVoxelsPerAxis = WorldSettings.Instance.NumVoxelsPerAxis;
        int maxBound = numVoxelsPerAxis - 1;

        for(int i = 0; i < data.Length; i++)
        {
            if (data[i] == 0)
            {
                continue;
            }
            Vector3 index3 = WorldSettings.Instance.ConvertIndexToVector3(i);
            uint meshIndex = 0;
            //right
            if(index3.x == maxBound || data[i + WorldSettings.Instance.Adjacents[0]] == 0)
            {
                meshIndex |= 1;
            }
            //left
            if (index3.x == 0 || data[i + WorldSettings.Instance.Adjacents[1]] == 0)
            {
                meshIndex |= 2;
            }
            //top
            if (index3.y == maxBound || data[i + WorldSettings.Instance.Adjacents[2]] == 0)
            {
                meshIndex |= 4;
            }
            //bot
            if (index3.y == 0 || data[i + WorldSettings.Instance.Adjacents[3]] == 0)
            {
                meshIndex |= 8;
            }
            //front
            if (index3.z == maxBound || data[i + WorldSettings.Instance.Adjacents[4]] == 0)
            {
                meshIndex |= 16;
            }
            //back
            if (index3.z == 0 || data[i + WorldSettings.Instance.Adjacents[5]] == 0)
            {
                meshIndex |= 32;
            }
            meshData.AddVoxel(meshIndex, i);
        }
        return meshData;
    }
}

public class MeshData
{
    Dictionary<int, VoxelData> voxelDataMap = new Dictionary<int, VoxelData>();

    public void AddVoxel(uint meshIndex, int mapIndex)
    {
        int[] triangles = VoxelMeshTableData.GetTriangles(meshIndex, mapIndex);
        VoxelData voxelMesh = new VoxelData(triangles);
        voxelDataMap.Add(mapIndex, voxelMesh);
    }

    public Mesh ReducedMesh
    {
        get
        {
            //<Vector3,  
            Dictionary<int, int> verticeMap = new Dictionary<int, int>();
            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();

            int triangleIndex = 0;
            foreach(KeyValuePair<int, VoxelData> entry in voxelDataMap)
            {
                foreach(int index in entry.Value.triangles)
                {
                    int vertexIndex;
                    if (verticeMap.TryGetValue(index, out vertexIndex))
                    {
                        triangles.Add(vertexIndex);
                    }
                    else
                    {
                        verticeMap.Add(index, triangleIndex);
                        triangles.Add(triangleIndex);
                        vertices.Add(WorldSettings.Instance.VerticeTable[index]);
                        triangleIndex++;
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();
            return mesh;
        }
    }

    struct VoxelData
    {
        public int[] triangles;

        public VoxelData(int[] triangles)
        {
            this.triangles = triangles;
        }
    }
}