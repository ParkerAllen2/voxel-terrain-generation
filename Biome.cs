using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Biome : ScriptableObject
{

    public List<IMapFunction> mapFunctions;
    [SerializeField] List<VoxelDensity> voxelDensityLevels;

    private void Awake()
    {
        voxelDensityLevels.OrderBy(d => d.density);
    }

    public int DensityToVoxelId(float density)
    {
        foreach(VoxelDensity vd in voxelDensityLevels)
        {
            if(density > vd.density)
            {
                return vd.id;
            }
        }
        return 0;
    }

    public void InitializeMapFunctions()
    {
        foreach (IMapFunction func in mapFunctions)
        {
            func.Initialize();
        }
    }

    [System.Serializable]
    struct VoxelDensity
    {
        public int id;
        public float density;
    }
}
