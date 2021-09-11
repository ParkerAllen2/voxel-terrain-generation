using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMarcherSlow : MonoBehaviour
{
    public uint[] Generate(uint[] voxelIds, int numVoxelsPerAxis)
    {
        int[] adjacents = GenerateAdjacents(numVoxelsPerAxis);
        uint[] meshIndexs = new uint[voxelIds.Length];
        int boundsSizeIndex = numVoxelsPerAxis - 1;

        int index = 0;
        for(int z = 0; z < numVoxelsPerAxis; z++)
        {
            for (int y = 0; y < numVoxelsPerAxis; y++)
            {
                for (int x = 0; x < numVoxelsPerAxis; x++)
                {
                    meshIndexs[index] = CalculateMeshIndex(voxelIds, adjacents, index, boundsSizeIndex, x, y, z);
                    index++;
                }
            }
        }
        return meshIndexs;
    }

    uint CalculateMeshIndex(uint[] voxelIds, int[] adjacent, int index, int boundsSizeIndex, int x, int y, int z)
    {
        if (voxelIds[index] == 0)
        {
            return 0;
        }

        uint meshIndex = 0;
        //top
        if (y == boundsSizeIndex || voxelIds[index + adjacent[0]] == 0)
        {
            meshIndex |= 0x1;
            //meshIndex |= 0x8000000;
        }
        //bot
        if (y == 0 || voxelIds[index + adjacent[1]] == 0)
        {
            meshIndex |= 0x2;
            //meshIndex |= 0x4000000;
        }
        //right
        if (x == boundsSizeIndex || voxelIds[index + adjacent[2]] == 0)
        {
            meshIndex |= 0x4;
            //meshIndex |= 0x20000000;
        }
        //left
        if (x == 0 || voxelIds[index + adjacent[3]] == 0)
        {
            meshIndex |= 0x8;
            //meshIndex |= 0x10000000;
        }
        //front
        if (z == boundsSizeIndex || voxelIds[index + adjacent[4]] == 0)
        {
            meshIndex |= 0x10;
            //meshIndex |= 0x80000000;
        }
        //back
        if (z == 0 || voxelIds[index + adjacent[5]] == 0)
        {
            meshIndex |= 0x20;
            //meshIndex |= 0x40000000;
        }

        return meshIndex;
    }

    int[] GenerateAdjacents(int numVoxelsPerAxis)
    {
        int sizeSqr = numVoxelsPerAxis * numVoxelsPerAxis;
        int[] adjacent = new int[6];
        adjacent[0] = numVoxelsPerAxis;
        adjacent[1] = -numVoxelsPerAxis;
        adjacent[2] = 1;
        adjacent[3] = -1;
        adjacent[4] = sizeSqr;
        adjacent[5] = -sizeSqr;

        return adjacent;
    }
}
