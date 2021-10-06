using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IMapFunction : ScriptableObject
{
    public virtual void Initialize() { }

    public virtual float CalcDensityAtPoint(Vector3 voxelCoord)
    {
        return 1;
    }
}
