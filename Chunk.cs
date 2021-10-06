using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    GameObject meshObject;
    Vector3 position;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    LODMesh lodMesh;
    Biome biome;

    bool generateCollider;

    public Chunk(Biome biome, Vector3 coord, Transform parent, Material mat, bool generateCollider)
    {
        DebugCounter.AddDebug("Chunk Created");
        this.generateCollider = generateCollider;
        position = coord;
        this.biome = biome;

        MapGenerator.RequestMapData(biome, coord, OnMapDataReceived);

        meshObject = new GameObject("Chunk");
        meshObject.transform.position = coord;
        meshObject.transform.SetParent(parent);

        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        if(generateCollider)
        {
            meshCollider = meshObject.AddComponent<MeshCollider>();
        }

        meshRenderer.material = mat;

        lodMesh = new LODMesh(1, UpdateTerrainChunk);
    }

    void OnMapDataReceived(MapData mapData)
    {
        DebugCounter.AddDebug("Map Received");
        lodMesh.RequestMesh(biome, mapData);
    }

    public void UpdateTerrainChunk()
    {
        meshFilter.sharedMesh = lodMesh.mesh;
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataRecieve(MeshData meshData)
        {
            DebugCounter.AddDebug("Mesh Received");
            Debug.Log("Mesh Received");
            mesh = meshData.ReducedMesh;
            updateCallback();
        }

        public void RequestMesh(Biome biome, MapData mapData)
        {
            hasRequestedMesh = true;
            MeshGenerator.RequestMeshData(biome, mapData, OnMeshDataRecieve);
        }
    }
}
