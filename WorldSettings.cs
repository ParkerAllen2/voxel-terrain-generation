using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : Singleton<WorldSettings>
{
    [SerializeField] int seed;
    [SerializeField] int numVoxelsPerAxis;
    [Min(.001f)]
    [SerializeField] float chunkSize;

    int numVoxelsPerAxisSqr;
    float spacing;
    int[] adjacents;
    Vector3[] verticeTable;
    int[] voxelCornersTable;

    protected WorldSettings() { }

    public override void Awake()
    {
        base.Awake();
        spacing = chunkSize / numVoxelsPerAxis;
        numVoxelsPerAxisSqr = numVoxelsPerAxis * numVoxelsPerAxis;
        int numVoxelsPerAxisCube = numVoxelsPerAxisSqr * numVoxelsPerAxis;

        adjacents = new int[] {
            1,
            -1,
            numVoxelsPerAxis,
            -numVoxelsPerAxis,
            numVoxelsPerAxisSqr,
            -numVoxelsPerAxisSqr
        };
        InitializeVerticeTable();
        InitializeCornerVerticeTable();
        Debug.Log("World Settings Initialized!");
    }

    void InitializeVerticeTable()
    {
        int numVerticesPerAxis = numVoxelsPerAxis + 1;
        verticeTable = new Vector3[numVerticesPerAxis * numVerticesPerAxis * numVerticesPerAxis];
        int index = 0;
        for(int z = 0; z < numVerticesPerAxis; z++)
        {
            for (int y = 0; y < numVerticesPerAxis; y++)
            {
                for (int x = 0; x < numVerticesPerAxis; x++)
                {
                    verticeTable[index] = new Vector3(x, y, z) * spacing;
                    index++;
                }
            }
        }
    }

    void InitializeCornerVerticeTable()
    {
        int numVerticesPerAxis = numVoxelsPerAxis + 1;
        int numVerticesPerAxisSqr = numVerticesPerAxis * numVerticesPerAxis;
        voxelCornersTable = new int[]
        {
            0,
            1,
            numVerticesPerAxis,
            numVerticesPerAxis + 1,
            numVerticesPerAxisSqr,
            numVerticesPerAxisSqr + 1,
            numVerticesPerAxisSqr + numVerticesPerAxis,
            numVerticesPerAxisSqr + numVerticesPerAxis +1
        };
    }

    public Vector3 ConvertIndexToVector3(int index)
    {
        int x = index % numVoxelsPerAxis;
        int y = (index / numVoxelsPerAxis) % numVoxelsPerAxis;
        int z = index / numVoxelsPerAxisSqr;
        return new Vector3(x, y, z);
    }

    public int Seed
    {
        get { return seed; }
    }

    public int NumVoxelsPerAxis
    {
        get { return numVoxelsPerAxis; }
    }

    public float ChunkSize
    {
        get { return chunkSize; }
    }

    public int NumVoxelsPerAxisSqr
    {
        get { return numVoxelsPerAxisSqr; }
    }

    public float Spacing
    {
        get { return spacing; }
    }

    public int[] Adjacents
    {
        get { return adjacents; }
    }

    public Vector3[] VerticeTable
    {
        get { return verticeTable; }
    }

    public int[] VoxelCornersTable
    {
        get { return voxelCornersTable; }
    }
}
