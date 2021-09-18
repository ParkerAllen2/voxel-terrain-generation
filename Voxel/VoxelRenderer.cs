using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class VoxelRenderer : MonoBehaviour
    {
        public uint Debug_Id;
        public uint Debug_meshIndex;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;

        public void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void RenderVoxel(uint id, uint meshIndex)
        {
            Debug_Id = id;
            Debug_meshIndex = meshIndex;
            if (meshIndex == 0 || id == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            Mesh mesh = meshFilter.mesh;
            mesh.Clear();
            mesh.vertices = VoxelMeshData.GetVerticies(meshIndex);
            mesh.triangles = VoxelMeshData.GetTriangles(meshIndex);
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
    }
}
