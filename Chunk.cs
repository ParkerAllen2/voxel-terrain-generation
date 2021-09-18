using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace voxel_marching
{
    public class Chunk : MonoBehaviour
    {
        const int threadGroupSize = 8;

        public VoxelRenderer voxelPrefab;
        List<VoxelRenderer> voxelPool;

        // Computes
        DensityMapGenerator densityMap;
        MarchingVoxel marchingVoxel;

        // Settings
        [Min(1)]
        public int numVoxelsPerAxis;
        [Min(.001f)]
        public float spacing = 1;
        public float minDensity;
        public Vector3 offset;
        int numVoxels;
        int numThreadsPerAxis;
        float boundsSize;
        Vector3 position;

        private void Start()
        {
            Setup();
            CalculateSettings();
            StartCoroutine(GenerateDensityMap());
        }

        void Setup()
        {
            densityMap = GetComponent<DensityMapGenerator>();
            marchingVoxel = GetComponent<MarchingVoxel>();
            voxelPool = new List<VoxelRenderer>();
        }

        void CalculateSettings()
        {
            boundsSize = numVoxelsPerAxis * spacing;
            position = transform.position;
            numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
            numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);
            ManageVoxelPool();
        }

        IEnumerator GenerateDensityMap()
        {
            ComputeBuffer voxelIdBuffer = new ComputeBuffer(numVoxels, sizeof(uint));

            densityMap.Generate(voxelIdBuffer, numVoxelsPerAxis, boundsSize, position, offset, spacing, minDensity, numThreadsPerAxis);

            AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(voxelIdBuffer);

            yield return new WaitUntil(() => request.done);

            Debug.Log("Chunk Map Generated!");
            StartCoroutine(March(voxelIdBuffer));
        }

        IEnumerator March(ComputeBuffer voxelIdBuffer)
        {
            ComputeBuffer voxelDataBuffer = new ComputeBuffer(numVoxels, sizeof(uint));

            marchingVoxel.Generate(voxelIdBuffer, voxelDataBuffer, numVoxelsPerAxis, numThreadsPerAxis);

            NativeArray<uint> voxelData = new NativeArray<uint>(numVoxels, Allocator.TempJob);
            AsyncGPUReadbackRequest request = AsyncGPUReadback.RequestIntoNativeArray(ref voxelData, voxelDataBuffer);
            //AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(voxelDataBuffer);

            yield return new WaitUntil(() => request.done);

            RenderVoxels(request.GetData<uint>().ToArray());

            voxelData.Dispose();
            voxelDataBuffer.Dispose();
            voxelIdBuffer.Dispose();

            Debug.Log("Voxel Data Ready!");
        }

        void RenderVoxels(uint[] voxelData)
        {
            int numVoxels = voxelData.Length;

            int i;
            for (i = 0; i < numVoxels; i++)
            {
                uint data = voxelData[i];

                uint id = data & 0x3FFFFFF;
                byte meshIndex = (byte)(data >> 26);
                voxelPool[i].RenderVoxel(id, meshIndex);
            }

            while (i < voxelPool.Count)
            {
                voxelPool[i].gameObject.SetActive(false);
                i++;
            }
            Debug.Log("Voxels Rendered!");
        }

        void ManageVoxelPool()
        {
            int numVoxelsPerAxisSqr = numVoxelsPerAxis * numVoxelsPerAxis;
            for (int i = voxelPool.Count; i < numVoxels; i++)
            {
                voxelPool.Add(Instantiate(voxelPrefab, IndexTo3DSpace(i, numVoxelsPerAxisSqr), Quaternion.identity));
            }
        }

        Vector3 IndexTo3DSpace(int index, int numVoxelsPerAxisSqr)
        {
            float x = index % numVoxelsPerAxis * spacing;
            float y = (index / numVoxelsPerAxis) % numVoxelsPerAxis * spacing;
            float z = index / numVoxelsPerAxisSqr * spacing;
            return new Vector3(x, y, z);
        }
    }
}
