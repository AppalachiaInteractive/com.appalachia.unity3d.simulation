using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.AmbientOcclusion
{
    [Serializable]
    public class AmbientOcclusionSamplePoint
    {
        public float area;
        public float density;
        public bool flag;
        public Vector3 position;
        public float radius;

        public float GetOcclusionAmount(Vector3 testPoint, Vector3 testDirection)
        {
            Vector3 delta = position - testPoint;
            float ds = delta.sqrMagnitude;
            float d2 = Mathf.Max(0.0f, ds - area);
            
            if (ds > Mathf.Epsilon)
            {
                delta.Normalize();
            }

            return (
                    1.0f - (1.0f /
                        Mathf.Sqrt((area / d2) + 1)
                    )
                ) *
                Mathf.Clamp01(4.0f * Vector3.Dot(testDirection, delta));
        }

        public AmbientOcclusionSamplePoint(Vector3 pos, float radius, float density)
        {
            position = pos;
            this.radius = radius;
            area = radius * radius;
            this.density = density;
        }
    }
}
