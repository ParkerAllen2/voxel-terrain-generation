using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    Node rootNode;

    public int LOD;

    int maxDepth;
    int initialSize;

    public void createOctree(float[,,] nodeValues)
    {
        //find lowest power of 2 less than max axis
        //max depth = power
        maxDepth = CalculateDepth(nodeValues.GetLength(0) - 1);
        //create root node
        //subdivide(depth)
    }

    public void RenderMesh()
    {

    }

    int CalculateDepth(int maxAxis)
    {
        int count = 0;
        while (maxAxis != 0)
        {
            count++;
            maxAxis = maxAxis >> 1;
        }
        return count;
    }
}
