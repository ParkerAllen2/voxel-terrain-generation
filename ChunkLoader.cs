using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public Biome biome;
    public Vector3Int numChunks = Vector3Int.one;
    public Material mat;
    public bool generateColliders;

    List<Chunk> chunks;
    Dictionary<Vector3Int, Chunk> existingChunks;
    Queue<Chunk> recycleableChunks;

    private void Start()
    {
        biome.InitializeMapFunctions();
        InitChunkDataStructures();
        InitChunks();
        //DebugCounter.LogAll();
    }

    void InitChunkDataStructures()
    {
        recycleableChunks = new Queue<Chunk>();
        chunks = new List<Chunk>();
        existingChunks = new Dictionary<Vector3Int, Chunk>();
    }

    void InitChunks()
    {
        for (int x = 0; x < numChunks.x; x++)
        {
            for (int y = 0; y < numChunks.y; y++)
            {
                for (int z = 0; z < numChunks.z; z++)
                {
                    Vector3 coord = new Vector3(x, y, z) * WorldSettings.Instance.ChunkSize;
                    chunks.Add(CreateChunk(coord));
                }
            }
        }
    }

    Chunk CreateChunk(Vector3 coord)
    {
        Chunk newChunk = new Chunk(biome, coord, transform, mat, generateColliders);
        return newChunk;
    }
}
