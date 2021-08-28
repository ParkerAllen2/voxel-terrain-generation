using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class Voxel
    {
        public VoxelType voxelType;
        public VoxelData voxelData;
        public VoxelRenderer voxelRenderer;

        public VoxelRenderer voxelRendererPrefab;

        public void RenderVoxel(Vector3 position)
        {
            if (voxelType.empty)
            {
                return;
            }
            voxelRenderer = GameObject.Instantiate(voxelRendererPrefab, position, Quaternion.identity);
        }
    }
}
