using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace voxel_marching
{
    public class Vector3Byte
    {
        public readonly byte x;
        public readonly byte y;
        public readonly byte z;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Vector3Byte))
            {
                return false;
            }

            Vector3Byte v = (Vector3Byte)obj;
            return this.x == v.x && this.y == v.y && this.z == v.z;
        }
    }
}
