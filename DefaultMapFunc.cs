using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DefaultMapFunc : IMapFunction
{
    public int octaves = 8;
    public float lacunarity = 2;
    public float persistence = .5f;
    public float noiseScale = 10;
    Vector3[] offsets;

    public override void Initialize()
    {
        System.Random prng = new System.Random(WorldSettings.Instance.Seed);
        offsets = new Vector3[octaves];
        float offsetRange = 1000;
        for (int i = 0; i < octaves; i++)
        {
            offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
        }
    }

    public override float CalcDensityAtPoint(Vector3 voxelCoord)
    {
        Vector3 pos = voxelCoord;
        float noise = 0;
        float frequency = noiseScale / 100;
        float amplitude = 1;
        return PerlinNoise3D(pos * frequency);
        for (int i = 0; i < octaves; i++)
        {
            float n = PerlinNoise3D(pos * frequency + offsets[i]);
            float v = 1 - n / 2;
            noise += v * amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return noise;
    }

    float PerlinNoise3D(Vector3 position)
    {
        float x = position.x;
        float y = position.y;
        float z = position.z;
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
