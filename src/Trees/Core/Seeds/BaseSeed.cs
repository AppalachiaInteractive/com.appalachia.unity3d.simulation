#region

using System;
using UnityEngine;
using Random = System.Random;

#endregion

namespace Appalachia.Simulation.Trees.Core.Seeds
{
    [Serializable]
    public abstract class BaseSeed : ISeed
    {
        public const int HIGH_ELEMENT = 999999;

        private Random _random;

        [SerializeField, HideInInspector]
        protected int _seed;

        protected BaseSeed(int seed)
        {
            _seed = seed;

            // ReSharper disable once VirtualMemberCallInConstructor
            SetInternalSeed(_seed);
        }

        public virtual int EffectiveSeed => Mathf.Clamp(_seed, 0, HIGH_ELEMENT);

        public void SetInternalSeed(double seed)
        {
            SetInternalSeed((int) seed);
        }

        public virtual void SetInternalSeed(int seed)
        {
            seed = Mathf.Clamp(seed, 0, HIGH_ELEMENT);
            _seed = seed;

            Reset();
        }

        public float RandomValue(float min = 0f, float max = 1f)
        {
            for (var i = 0; i < 10; i++)
            {
                _random.NextDouble();
            }

            var value = min + ((float) _random.NextDouble() * (max - min));

            return value;
        }

        public int RandomValue(int min, int max)
        {
            var value = (int) RandomValue(min, (float) max);

            return value;
        }

        public void Reset()
        {
            _random = new Random(EffectiveSeed);
        }

        public float Noise2(float x, float y, float scale)
        {
            return Noise2(x, y, scale, scale);
        }
        
        public float Noise2(float x, float y, float scaleX, float scaleY)
        {
            return SeedNoise.Noise2(EffectiveSeed, HIGH_ELEMENT, x, y, scaleX, scaleY);
        }

        public float Noise3(float x, float y, float z, float scaleX, float scaleY, float scaleZ, float offset)
        {
            return SeedNoise.Noise3(EffectiveSeed, HIGH_ELEMENT, x, y, z, scaleX, scaleY, scaleZ, offset);
        }
        
        public float Noise3(float x, float y, float z, float scale, float offset)
        {
            return Noise3(x, y, z, scale, scale, scale, offset);
        }

        public float Noise3(Vector3 pos, Vector3 scale, float offset)
        {
            return Noise3(pos.x, pos.y, pos.z, scale.x, scale.y, scale.z, offset);
        }

        public float Noise3(Vector3 pos, float scale, float offset)
        {
            return Noise3(pos.x, pos.y, pos.z, scale, scale, scale, offset);
        }

    }
}