using Appalachia.Core.Math.Noise;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Seeds
{
    public static class SeedNoise
    {
        public static float Noise2(int seed, int highElement, float x, float y, float scaleX, float scaleY)
        {
            var o = seed * (1.0 / highElement);
            //o *= scale;
            
            x *= scaleX;
            y *= scaleY;

            var value = Mathf.PerlinNoise((float) (x + o), (float) (y + o));

            return value;
        }

        public static float Noise3(int seed, int highElement, float x, float y, float z, float scaleX, float scaleY, float scaleZ, float offset)
        {
            var o = (seed * (1.0 / highElement)) + offset;
            //o *= scale;

            x *= scaleX;
            y *= scaleY;
            z *= scaleZ;

            var value = Perlin.Noise((float) (x + o), (float) (y + o), (float) (z + o));

            value += .5f;

            value = Mathf.Clamp01(value);

            return value;
        }
    }
}
