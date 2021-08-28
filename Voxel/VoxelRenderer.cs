using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class VoxelRenderer : MonoBehaviour
    {
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        public Material meshMaterial;

        public void AddMeshComponents()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        public void RenderVoxel(byte sideFlags)
        {
            meshRenderer.material = meshMaterial;
            Mesh mesh = meshFilter.mesh;
            mesh.Clear();
            mesh.vertices = VoxelMeshData.GetVerticies(sideFlags);
            mesh.triangles = VoxelMeshData.GetTriangles(sideFlags);
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
    }
}
