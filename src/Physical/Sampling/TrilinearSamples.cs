using System;
using Appalachia.Core.Collections.Native;
using Appalachia.Utility.Constants;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Sampling
{
    [Serializable]
    public struct TrilinearSamples<T> : IDisposable
        where T : struct
    {
        public TrilinearSamples(Bounds b, int3 subdivisions, Allocator allocator = Allocator.Persistent)
        {
            this.subdivisions = subdivisions;
            count = subdivisions.x * subdivisions.y * subdivisions.z;

            points = new NativeArray<float3>(count, allocator);
            values = new NativeArray<T>(count, allocator);
            isCreated = true;
            isPopulated = false;

            localToWorld = default;
            sampleStrengths = default;
            idx000 = default;
            idx001 = default;
            idx010 = default;
            idx011 = default;
            idx100 = default;
            idx101 = default;
            idx110 = default;
            idx111 = default;

            var step = (float3) b.size / subdivisions;

            for (var x = 0; x < subdivisions.x; x++)
            for (var y = 0; y < subdivisions.y; y++)
            for (var z = 0; z < subdivisions.z; z++)
            {
                var index = GetIndex(x, y, z);

                var point = new float3(step.x * x, step.y * y, step.z * z);

                points[index] = point;
            }
        }

        public bool isCreated;
        public bool isPopulated;

        public int3 subdivisions;
        public int count;
        public float4x4 localToWorld;
        
        public NativeArray<float3> points;
        public NativeArray<T> values;

        public float3 sampleStrengths;
        public int3 idx000;
        public int3 idx001;
        public int3 idx010;
        public int3 idx011;
        public int3 idx100;
        public int3 idx101;
        public int3 idx110;
        public int3 idx111;
        
        public T c000 => Get(idx000);
        public T c001 => Get(idx001);
        public T c010 => Get(idx010);
        public T c011 => Get(idx011);
        public T c100 => Get(idx100);
        public T c101 => Get(idx101);
        public T c110 => Get(idx110);
        public T c111 => Get(idx111);

        public T Sample(float3 time, ITrilinearSampler<T> sampler)
        {
            SetSampleIndices(time);

            return sampler.Sample(this);
        }
        
        public void Dispose()
        {
            isCreated = false;
            isPopulated = false;
            
            points.SafeDispose();
            values.SafeDispose();
        }

        public T Get(int3 index)
        {
            var flatIndex = GetIndex(index.x, index.y, index.z);

            return values[flatIndex];
        }
        
        public T Get(int x, int y, int z)
        {
            var index = GetIndex(x, y, z);
            
            return values[index];
        }
        
        public int GetIndex(int3 index)
        {
            return (subdivisions.y * subdivisions.z * index.x) + (subdivisions.z * index.y) + index.z;
        }
        
        public int GetIndex(int x, int y, int z)
        {
            return (subdivisions.y * subdivisions.z * x) + (subdivisions.z * y) + z;
        }

        public void ReverseIndex(int index, out int x, out int y, out int z)
        {
            x = index / (subdivisions.y * subdivisions.z);
            y = (index - (subdivisions.y * subdivisions.z * x)) / subdivisions.z;
            z = index % subdivisions.z;
        }

        private void SetSampleIndices(float3 time)
        {
            var exactIndexPosition = new float3(time.x * subdivisions.x, time.y * subdivisions.y, time.z * subdivisions.z);

            var floor = (int3)math.floor(exactIndexPosition);
            var remai = exactIndexPosition % float3c.one;
            var ceili = (int3)math.ceil(exactIndexPosition);
                
            sampleStrengths.x = (remai.x - floor.x) / (ceili.x - floor.x);
            sampleStrengths.y = (remai.y - floor.y) / (ceili.y - floor.y);
            sampleStrengths.z = (remai.z - floor.z) / (ceili.z - floor.z);
            
            idx000 = new int3(floor.x, floor.y, floor.z);
            idx001 = new int3(floor.x, floor.y, ceili.z);
            idx010 = new int3(floor.x, ceili.y, floor.z);
            idx011 = new int3(floor.x, ceili.y, ceili.z);
            idx100 = new int3(ceili.x, floor.y, floor.z);
            idx101 = new int3(ceili.x, floor.y, ceili.z);
            idx110 = new int3(ceili.x, ceili.y, floor.z);
            idx111 = new int3(ceili.x, ceili.y, ceili.z);
        }
    }
}
