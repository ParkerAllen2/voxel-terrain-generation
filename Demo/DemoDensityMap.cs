using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class DemoDensityMap : MonoBehaviour
    {
        const int threadGroupSize = 8;

        public Vector4[] voxelData;
        public NoiseMap noiseMap;

        [Range(1, 32)]
        public int numPointsPerAxis;
        public float spacing;
        float boundsSize;
        Vector3 position;
        public Vector3 offset;

        public float minDensity;

        ComputeBuffer voxelBuffer;

        private void Start()
        {
            boundsSize = numPointsPerAxis * spacing;
            position = transform.position;
            GeneratePoints();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                GeneratePoints();
            }
        }

        public void GeneratePoints()
        {
            if(voxelBuffer != null)
            {
                voxelBuffer.Release();
            }

            int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis; 
            int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / (float)threadGroupSize);

            voxelBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4, ComputeBufferType.Append);

            noiseMap.Generate(voxelBuffer, numPointsPerAxis, boundsSize, position, offset, spacing, minDensity, numThreadsPerAxis);

            voxelData = new Vector4[numPoints];
            voxelBuffer.GetData(voxelData);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 1, 1);
            foreach(Vector4 v4 in voxelData)
            {
                if(v4.w > minDensity)
                    //Gizmos.DrawSphere(new Vector3(v4.x, v4.y, v4.z), .3f);
                    Gizmos.DrawCube(new Vector3(v4.x, v4.y, v4.z), Vector3.one * spacing);
            }
        }

        void DisplayMinMax()
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (Vector4 v4 in voxelData)
            {
                min = Mathf.Min(min, v4.w);
                max = Mathf.Max(max, v4.w);
            }
            print("min: " + min + " max: " + max);
        }
    }
}
