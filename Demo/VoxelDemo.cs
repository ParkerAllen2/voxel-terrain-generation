using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class VoxelDemo : MonoBehaviour
    {
        public Material meshMaterial;
        VoxelRenderer voxelRenderer;
        public float stepTime;

        private void Start()
        {
            voxelRenderer = gameObject.AddComponent<VoxelRenderer>();
            voxelRenderer.AddMeshComponents();
            voxelRenderer.meshMaterial = meshMaterial;
            StartCoroutine(FlagStepper());
        }

        IEnumerator FlagStepper()
        {
            byte b = 0;
            while(b < 64)
            {
                voxelRenderer.RenderVoxel(b);
                b++;
                yield return new WaitForSeconds(stepTime);
            }
            StartCoroutine(FlagStepper());
        }
    }
}
