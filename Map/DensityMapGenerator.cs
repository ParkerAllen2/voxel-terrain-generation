using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class DensityMapGenerator : MonoBehaviour
    {
        public ComputeShader densityShader;

        protected List<ComputeBuffer> buffersToRelease;

        public virtual ComputeBuffer Generate(ComputeBuffer voxelIdBuffer, int numVoxelsPerAxis, float boundsSize, Vector3 center, Vector3 offset, float spacing, float minDensity, int numThreadsPerAxis)
        {

            densityShader.SetBuffer(0, "voxelIds", voxelIdBuffer);
            densityShader.SetInt("numVoxelsPerAxis", numVoxelsPerAxis);
            densityShader.SetFloat("boundsSize", boundsSize);
            densityShader.SetVector("center", center);
            densityShader.SetVector("offset", offset);
            densityShader.SetFloat("spacing", spacing);
            densityShader.SetFloat("minDensity", minDensity);

            densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

            if (buffersToRelease != null)
            {
                foreach (var b in buffersToRelease)
                {
                    b.Release();
                }
            }

            return voxelIdBuffer;
        }
    }
}
