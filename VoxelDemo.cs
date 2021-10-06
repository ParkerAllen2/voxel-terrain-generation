using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelDemo : MonoBehaviour
{
    [Range(0, 63)]
    public uint faceFlags = 0;
    uint previousFlags = 0;
    public int mapIndex = 0;
    int previousIndex;
    public VoxelData2 voxelData;
    public int[] trianglesV;
    public Vector3[] verticesV;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if(previousFlags != faceFlags || previousIndex != mapIndex)
        {
            SetVoxelData();
            meshFilter.sharedMesh = ReducedMesh();
            previousFlags = faceFlags;
            previousIndex = mapIndex;
        }
    }

    public void SetVoxelData()
    {
        int[] triangles = VoxelMeshTableData.GetTriangles(faceFlags, mapIndex);
        voxelData = new VoxelData2(triangles);
    }

    Mesh ReducedMesh()
    {
        Dictionary<int, int> verticeMap = new Dictionary<int, int>();
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();

        int triangleIndex = 0;
        foreach (int index in voxelData.triangles)
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

        Mesh mesh = new Mesh();
        verticesV = mesh.vertices = vertices.ToArray();
        trianglesV = mesh.triangles = triangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        return mesh;
    }

    [System.Serializable]
    public struct VoxelData2
    {
        public int[] triangles;

        public VoxelData2(int[] triangles)
        {
            this.triangles = triangles;
        }
    }
}
