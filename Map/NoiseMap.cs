using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class NoiseMap : DensityMapGenerator
    {
        public int seed;
        public int octaves = 8;
        public float lacunarity = 2;
        public float persistence = .5f;
        public float noiseScale = 10;

        public override ComputeBuffer Generate(ComputeBuffer voxelIdBuffer, int numVoxelsPerAxis, float boundsSize, Vector3 center, Vector3 offset, float spacing, float minDensity, int numThreadsPerAxis)
        {
            buffersToRelease = new List<ComputeBuffer>();

            System.Random prng = new System.Random(seed);
            Vector3[] offsets = new Vector3[octaves];
            float offsetRange = 1000;
            for(int i = 0; i < octaves; i++)
            {
                offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
            }

            ComputeBuffer offsetsBuffer = new ComputeBuffer(octaves, sizeof(float) * 3);
            offsetsBuffer.SetData(offsets);
            buffersToRelease.Add(offsetsBuffer);

            densityShader.SetBuffer(0, "offsets", offsetsBuffer);
            densityShader.SetInt("octaves", octaves);
            densityShader.SetFloat("lacunarity", lacunarity);
            densityShader.SetFloat("persistance", persistence);
            densityShader.SetFloat("noiseScale", noiseScale);

            return base.Generate(voxelIdBuffer, numVoxelsPerAxis, boundsSize, center, offset, spacing, minDensity, numThreadsPerAxis);
        }
    }
}
