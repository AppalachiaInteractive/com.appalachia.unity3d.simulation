using System;
using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Spline
{
    [Serializable]
    public class SplineDataPoint : AppalachiaSimpleBase
    {
        public Vector3 normal;

        public Vector3 point;
        public Quaternion rotation;
        public Vector3 tangent;
        public float time;

        public SplineDataPoint(Vector3 point, float time)
        {
            this.point = point;
            this.time = time;
        }
    }
}