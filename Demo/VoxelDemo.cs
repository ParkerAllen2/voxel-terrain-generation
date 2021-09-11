using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class VoxelDemo : MonoBehaviour
    {
        VoxelRenderer voxelRenderer;
        public float stepTime;

        [Range(0, 63)]
        public byte faces;

        private void Start()
        {
            StartCoroutine(FlagStepper());
        }

        void OnValidate()
        {
            if (!Application.isPlaying)
            {
                voxelRenderer.RenderVoxel(faces, 1);
            }
        }

        IEnumerator FlagStepper()
        {
            byte b = 0;
            while(b < 64)
            {
                voxelRenderer.RenderVoxel(b, 1);
                b++;
                yield return new WaitForSeconds(stepTime);
            }
            StartCoroutine(FlagStepper());
        }
    }
}
