using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class Chunk : MonoBehaviour
    {
        const int threadGroupSize = 8;

        public VoxelRenderer voxelPrefab;
        public List<VoxelRenderer> voxelPool;
        public NoiseMap noiseMap;
        public VoxelMarcher voxelMarcher;
        public VoxelMarcherSlow voxelMarcherSlow;

        //[Range(1, 32)]
        public int numVoxelsPerAxis;
        int numVoxelsPerAxisSqr;
        public float spacing;
        float boundsSize;
        Vector3 position;
        public Vector3 offset;

        public float minDensity;

        private void Start()
        {
            numVoxelsPerAxisSqr = numVoxelsPerAxis * numVoxelsPerAxis;
            boundsSize = numVoxelsPerAxis * spacing;
            position = transform.position;
            voxelPool = new List<VoxelRenderer>();

            GenerateVoxels();
            //GenerateVoxelsSlow();
        }

        private void Update()
        {
            //GeneratePoints();
        }

        public void GenerateVoxelsSlow()
        {
            int numPoints = numVoxelsPerAxisSqr * numVoxelsPerAxis;
            int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);

            ComputeBuffer voxelIdBuffer = new ComputeBuffer(numPoints, sizeof(uint));

            noiseMap.Generate(voxelIdBuffer, numVoxelsPerAxis, boundsSize, position, offset, spacing, minDensity, numThreadsPerAxis);
            uint[] voxelIds = new uint[numPoints];
            voxelIdBuffer.GetData(voxelIds);

            uint[] meshIndexs = voxelMarcherSlow.Generate(voxelIds, numVoxelsPerAxis);

            voxelIdBuffer.Release();

            RenderCubes(voxelIds, meshIndexs);
        }

        public void GenerateVoxels()
        {
            int numPoints = numVoxelsPerAxisSqr * numVoxelsPerAxis;
            int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);

            ComputeBuffer voxelIdBuffer = new ComputeBuffer(numPoints, sizeof(uint));
            ComputeBuffer meshIndexBuffer = new ComputeBuffer(numPoints, sizeof(uint));

            noiseMap.Generate(voxelIdBuffer, numVoxelsPerAxis, boundsSize, position, offset, spacing, minDensity, numThreadsPerAxis);
            uint[] voxelIds = new uint[numPoints];
            voxelIdBuffer.GetData(voxelIds);

            voxelMarcher.Generate(voxelIdBuffer, meshIndexBuffer, numVoxelsPerAxis, numThreadsPerAxis);
            uint[] meshIndexs = new uint[numPoints];
            meshIndexBuffer.GetData(meshIndexs);

            voxelIdBuffer.Release();
            meshIndexBuffer.Release();

            RenderCubes(voxelIds, meshIndexs);
        }

        void RenderCubes(uint[] voxelIds, uint[] meshIndexs)
        {
            int numVoxels = voxelIds.Length;
            ManageVoxelPool(numVoxels);

            int i;
            for(i = 0; i < numVoxels; i++)
            {
                /*uint data = voxelData[i];

                uint id = data & 0x3FFFFFF;
                byte sides = (byte)(~data >> 26);*/
                voxelPool[i].RenderVoxel(voxelIds[i], meshIndexs[i]);
            }

            while(i < voxelPool.Count)
            {
                voxelPool[i].gameObject.SetActive(false);
                i++;
            }
        }

        void ManageVoxelPool(int numVoxels)
        {
            for (int i = voxelPool.Count; i < numVoxels; i++)
            {
                voxelPool.Add(Instantiate(voxelPrefab, IndexTo3DSpace(i), Quaternion.identity));
            }
        }

        Vector3 IndexTo3DSpace(int index)
        {
            float x = index % numVoxelsPerAxis * spacing;
            float y = (index / numVoxelsPerAxis) % numVoxelsPerAxis * spacing;
            float z = index / numVoxelsPerAxisSqr * spacing;
            return new Vector3(x, y, z);
        }
    }
}

