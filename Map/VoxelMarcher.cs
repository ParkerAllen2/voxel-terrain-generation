using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class VoxelMarcher : MonoBehaviour
    {
        public ComputeShader voxelMarcher;

        protected List<ComputeBuffer> buffersToRelease;

        public ComputeBuffer Generate(ComputeBuffer voxelIdBuffer, ComputeBuffer meshIndexBuffer, int numVoxelsPerAxis, int numThreadsPerAxis)
        {
            buffersToRelease = new List<ComputeBuffer>();

            voxelMarcher.SetBuffer(0, "voxelIds", voxelIdBuffer);
            voxelMarcher.SetBuffer(0, "meshIndexs", meshIndexBuffer);
            voxelMarcher.SetBuffer(0, "adjacent", GenerateAdjacentBuffer(numVoxelsPerAxis));
            voxelMarcher.SetInt("numVoxelsPerAxis", numVoxelsPerAxis);
            voxelMarcher.SetFloat("boundsSizeIndex", numVoxelsPerAxis - 1);

            voxelMarcher.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            if (buffersToRelease != null)
            {
                foreach (var b in buffersToRelease)
                {
                    b.Release();
                }
            }

            return meshIndexBuffer;
        }

        ComputeBuffer GenerateAdjacentBuffer(int numVoxelsPerAxis)
        {
            int sizeSqr = numVoxelsPerAxis * numVoxelsPerAxis;
            int[] adjacent = new int[6];
            adjacent[0] = numVoxelsPerAxis;
            adjacent[1] = -numVoxelsPerAxis;
            adjacent[2] = 1;
            adjacent[3] = -1;
            adjacent[4] = sizeSqr;
            adjacent[5] = -sizeSqr;

            ComputeBuffer adjacentBuffer = new ComputeBuffer(6, sizeof(float) * 3);
            adjacentBuffer.SetData(adjacent);
            buffersToRelease.Add(adjacentBuffer);
            return adjacentBuffer;
        }
    }
}
