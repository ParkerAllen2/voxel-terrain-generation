using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    //Node[,,] nextLOD = new Node[2,2,2];
    Node[] nextLOD = new Node[8];

    public uint value = 0;
    bool visible;
    byte faceFlags;

    public uint Subdivide(float[,,] nodeValues, int depth, uint x, uint y, uint z)
    {
        // check if at max depth
        if(depth == 0)
        {
            // check coords in bounds
            if (nodeValues.GetLength(0) < x)
            {
                value = (uint)nodeValues[x, y, z];
                return value;
            }
        }

        //create nodes
        for(int i = 0; i < nextLOD.Length; i++)
        {
            nextLOD[i] = new Node();

            //subdivide again
            uint newX = x + (uint)(i & 1);
            uint newY = y + (uint)(i >> 1 & 1);
            uint newZ = z + (uint)(i >> 2 & 1);
            nextLOD[i].Subdivide(nodeValues, depth - 1, newX, newY, newZ);
        }

        /*for(uint i = 0; i < nextLOD.GetLength(0); i++)
        {
            for(uint j = 0; j < nextLOD.GetLength(1); j++)
            {
                for(uint k = 0; k < nextLOD.GetLength(2); k++)
                {
                    nextLOD[i, j, k] = new Node();

                    //subdivide again
                    nextLOD[i, j, k].Subdivide(nodeValues, depth - 1, x + i, y + j, z + k);
                }
            }
        }*/

        return CalculateValueFromNextLOD();
    }

    uint CalculateValueFromNextLOD()
    {
        var groups = nextLOD.GroupBy(v => v.value);
        int maxCount = groups.Max(g => g.Count());
        uint mode = groups.First(g => g.Count() == maxCount).Key;
        return mode;
    }

    void CalculateVisibleFaces()
    {

    }

    void GetAdjacent()
    {

    }
}
