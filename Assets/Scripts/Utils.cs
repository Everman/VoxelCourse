using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    private static int maxHeight = World.maxHeight;
    
    private static float dirt_smooth = World.dirt_smooth;
    private static int dirt_octaves = World.dirt_octaves;
    private static float dirt_persistence = World.dirt_persistence;

    private static float stone_smooth = World.dirt_smooth;
    private static int stone_octaves = World.dirt_octaves;
    private static float stone_persistence = World.dirt_persistence;

    public static int GenerateHeight( float x, float y) {
        float height = Map(0, maxHeight, 0, 1, FractalBrownianMotion(x * dirt_smooth, y * dirt_smooth, dirt_octaves, dirt_persistence));
        return (int)height;
    }

    public static int GenerateStoneHeight(float x, float y) {
        float height = Map(0, maxHeight-5, 0, 1, FractalBrownianMotion(x * stone_smooth, y * stone_smooth, stone_octaves, stone_persistence));
        return (int)height;
    }

    public static float FractalBrownianMotion3D(float x, float y, float z, float sm, int oct) {


        float XY = FractalBrownianMotion(x * sm, y * sm, oct, 0.5f);
        float YZ = FractalBrownianMotion(y * sm, z * sm, oct, 0.5f);
        float XZ = FractalBrownianMotion(x * sm, z * sm, oct, 0.5f);

        float YX = FractalBrownianMotion(y * sm, x * sm, oct, 0.5f);
        float ZY = FractalBrownianMotion(z * sm, y * sm, oct, 0.5f);
        float ZX = FractalBrownianMotion(z * sm, x * sm, oct, 0.5f);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6.0f;
    }

    private static float Map(float newmin, float newmax, float originmin, float originmax, float value) {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(originmin, originmax, value));
    }

    private static float FractalBrownianMotion( float x, float z, int oct, float pers) {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        float offset = 32000f;
        for (int i = 0; i < oct; i++) {
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;
            
            maxValue += amplitude;
            
            amplitude *= pers;
            frequency *= 2;
        }
        return total/maxValue;
    }
}
