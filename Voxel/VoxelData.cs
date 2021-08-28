using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class VoxelData
    {
        [Range(0, 63)]
        public readonly byte sideFlags;
        public readonly Chunk Chunk;
    }
}
