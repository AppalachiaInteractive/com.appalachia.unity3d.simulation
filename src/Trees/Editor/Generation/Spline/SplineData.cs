using System;
using System.Collections.Generic;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Spline
{
    [Serializable]
    public sealed class SplineData
    {
        public List<SplineDataPoint> points = new List<SplineDataPoint>();

        public float tension = 0.5f; // 0.0f = linear, 0.5f = catmull-rom spline, 1.0f = over shoot

        public void SetPointCount(int c)
        {
            for (var i = points.Count-1; i >= c; i--)
            {
                points.RemoveAt(i);
            }
        }

        public void RemovePoint(int c)
        {
            if ((c < 0) || (c >= points.Count)) return;

            points.RemoveAt(c);
        }

        public void AddPoint(Vector3 pos, float timeInSeconds)
        {
            var node = new SplineDataPoint(pos, timeInSeconds);
            
            points.Add(node);
        }

        public void Reset()
        {
            points.Clear();
        }

        public override string ToString()
        {
            return
                $"Spline | Points: {points.Count} | Length: {(points[points.Count - 1].point - points[0].point).magnitude}";
        }
    }
}