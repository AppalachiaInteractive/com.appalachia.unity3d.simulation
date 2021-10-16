using System.Collections.Generic;
using System.Linq;

namespace Appalachia.Simulation.Trees.Core.Seeds
{
    public class VirtualSeed : ISeed
    {
        public VirtualSeed(params BaseSeed[] args)
        {
            _seeds = args?.ToList();
        }

        private List<BaseSeed> _seeds;
        
        public float RandomValue(float min = 0, float max = 1)
        {
            var val = 0f;
            
            foreach (var seed in _seeds)
            {
                val += seed.RandomValue(min, max);
            }

            return val / _seeds.Count;
        }

        public int RandomValue(int min, int max)
        {
            var val = 0f;
            
            foreach (var seed in _seeds)
            {
                val += seed.RandomValue(min, max);
            }

            return (int) (val / _seeds.Count);
        }

        public void Reset()
        {
            foreach (var seed in _seeds)
            {
                seed.Reset();
            }
        }

        public float Noise2(float x, float y, float scale)
        {
            return Noise2(x, y, scale, scale);
        }

        public float Noise2(float x, float y, float scaleX, float scaleY)
        {var val = 0f;
            
            foreach (var seed in _seeds)
            {
                val += seed.Noise2(x, y, scaleX, scaleY);
            }

            return val / _seeds.Count;
        }

        public float Noise3(float x, float y, float z, float scale, float offset)
        {
            return Noise3(x, y, z, scale, scale, scale, offset);
        }

        public float Noise3(
            float x,
            float y,
            float z,
            float scaleX,
            float scaleY,
            float scaleZ,
            float offset)
        {
            var val = 0f;
            
            foreach (var seed in _seeds)
            {
                val += seed.Noise3(x, y, z, scaleX, scaleY, scaleZ, offset);
            }

            return val / _seeds.Count;
        }
    }
}
