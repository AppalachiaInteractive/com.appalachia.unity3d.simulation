#region

using System;
using System.Collections.Generic;
using Appalachia.Core.Collections.NonSerialized;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Recursion;
using Appalachia.Spatial.Octree;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Appalachia.Simulation.Trees.Generation.AmbientOcclusion
{
    public static class AmbientOcclusionGenerator
    {
        public static BoundsOctree<AmbientOcclusionSamplePoint> GenerateAOSamplePoints(
            TreeStage individual,
            IHierarchyRead readStructure)
        {
            using (BUILD_TIME.AMB_OCC_GEN.GenerateAOSamplePoints.Auto())
            {
                var points = new List<AmbientOcclusionSamplePoint>();
                var bounds = new Bounds();

                individual.shapes.RecurseSplines(readStructure, data => { GenerateBarkAOSamplePoints(data, ref bounds, points); });

                individual.shapes.RecurseLeaves(readStructure, data => { GenerateLeafAOSamplePoints(data, ref bounds, points); });

                return GenerateAOOctree(bounds, points);
            }
        }
        
        public static BoundsOctree<AmbientOcclusionSamplePoint> GenerateAOSamplePoints(TreeBranch branch)
        {
            using (BUILD_TIME.AMB_OCC_GEN.GenerateAOSamplePoints.Auto())
            {
                var points = new List<AmbientOcclusionSamplePoint>();
                var bounds = new Bounds();

                branch.shapes.RecurseSplines(branch.hierarchies,
                    data => { GenerateBarkAOSamplePoints(data, ref bounds, points); }
                );

                branch.shapes.RecurseLeaves(branch.hierarchies,
                    data => { GenerateLeafAOSamplePoints(data, ref bounds, points); }
                );

                return GenerateAOOctree(bounds, points);
            }
        }

        private static BoundsOctree<AmbientOcclusionSamplePoint> GenerateAOOctree(
            Bounds bounds,
            List<AmbientOcclusionSamplePoint> points)
        {
            var tree = new BoundsOctree<AmbientOcclusionSamplePoint>(OctreeStyle.EightDivisions, bounds);

            foreach (var point in points)
            {
                var aoBounds = new Bounds(point.position, Vector3.one * point.radius);
                tree.Add(aoBounds, point);
            }

            return tree;
        }

        private static void GenerateBarkAOSamplePoints(
            BarkRecursionData data,
            ref Bounds bounds,
            List<AmbientOcclusionSamplePoint> points)
        {
            using (BUILD_TIME.AMB_OCC_GEN.GenerateAOSamplePoints.Auto())
            {
                var shape = data.shape;

                var hierarchy = data.hierarchy;
                var spline = data.shape.spline;
                var geometry = hierarchy.geometry;

                var minStepSize = 0.5f;

                var mo = geometry.geometryMode;
                
                var includeFronds = (mo == BranchGeometryMode.Frond) || (mo == BranchGeometryMode.BranchFrond);
                
                var includeBranch =  (mo == BranchGeometryMode.Branch) || (mo == BranchGeometryMode.BranchFrond);

                var scale = shape.effectiveScale;
                var totalHeight = SplineModeler.GetApproximateLength(spline);

                if (totalHeight < minStepSize)
                {
                    totalHeight = minStepSize;
                }

                var minStep = minStepSize / totalHeight;

                var t = 0.0f;
                while (t < 1.0f)
                {
                    var pos = shape.effectiveMatrix.MultiplyPoint(SplineModeler.GetPositionAtTime(spline, t));

                    var rad = 0.0f;

                    if (includeBranch)
                    {
                        rad = SplineModeler.GetRadiusAtTime(shape, hierarchy, t) * 0.95f;
                    }

                    if (includeFronds)
                    {
                        rad = Mathf.Max(rad, 0.95f * (geometry.frond.frondWidth.Value.EvaluateScaled(t) * scale));

                        // push position down..
                        pos.y -= rad;
                    }

                    if (rad > 0.01f)
                    {
                        var point = new AmbientOcclusionSamplePoint(pos, rad, 1.0f);
                        var aoBounds = new Bounds(pos, Vector3.one * rad);

                        bounds.Encapsulate(aoBounds);

                        points.Add(point);
                    }

                    t += Mathf.Max(minStep, rad / totalHeight);
                }
            }
        }

        private static void GenerateLeafAOSamplePoints(
            LeafRecursionData data,
            ref Bounds bounds,
            List<AmbientOcclusionSamplePoint> points)
        {
            using (BUILD_TIME.AMB_OCC_GEN.GenerateAOSamplePoints.Auto())
            {
                var shape = data.shape;
                var scaleFactor = 0.75f;

                var pos = shape.effectiveMatrix.MultiplyPoint(Vector3.zero);
                var rad = shape.effectiveScale * scaleFactor;

                var point = new AmbientOcclusionSamplePoint(pos, rad, 0.5f);
                var aoBounds = new Bounds(pos, Vector3.one * rad);

                bounds.Encapsulate(aoBounds);

                points.Add(point);
            }
        }

        public static void ApplyDefaultAmbientOcclusion(LODGenerationOutput output)
        {
            using (BUILD_TIME.AMB_OCC_GEN.ApplyDefaultAmbientOcclusion.Auto())
            {
                foreach (var vertex in output.vertices)
                {
                    vertex.SetAmbientOcclusion(1f);
                }
            }
        }

        public static void ApplyAmbientOcclusion(
            LODGenerationOutput output,
            BoundsOctree<AmbientOcclusionSamplePoint> samplePoints,
            AmbientOcclusionStyle style,
            float density,
            float raytracingSamples,
            float raytracingRange)
        {
            using (BUILD_TIME.AMB_OCC_GEN.ApplyAmbientOcclusion.Auto())
            {
                foreach (var plane in output.billboards)
                {
                    SetAmbientOcclusionForBillboard(
                        plane,
                        samplePoints,
                        style,
                        density,
                        raytracingSamples,
                        raytracingRange
                    );
                }

                foreach (var vertex in output.vertices)
                {
                    if (vertex.billboard)
                    {
                        continue;
                    }

                    var sampled = GetAmbientOcclusionAtPoint(
                        vertex.position,
                        vertex.normal,
                        samplePoints,
                        style,
                        density,
                        raytracingSamples,
                        raytracingRange
                    );

                    vertex.SetAmbientOcclusion(sampled);
                }
            }
        }

        private static void SetAmbientOcclusionForBillboard(
            BillboardPlane plane,
            BoundsOctree<AmbientOcclusionSamplePoint> samplePoints,
            AmbientOcclusionStyle style,
            float density,
            float raytracingSamples,
            float raytracingRange)
        {
            using (BUILD_TIME.AMB_OCC_GEN.SetAmbientOcclusionForBillboard.Auto())
            {
                var pushR = Vector3.right * plane.effectiveScale;
                var pushF = Vector3.forward * plane.effectiveScale;

                var a = 0.0f;

                a = GetAmbientOcclusionAtPoint(
                    plane.v0.position + pushR,
                    Vector3.right,
                    samplePoints,
                    style,
                    density,
                    raytracingSamples,
                    raytracingRange
                );
                a += GetAmbientOcclusionAtPoint(
                    plane.v0.position - pushR,
                    -Vector3.right,
                    samplePoints,
                    style,
                    density,
                    raytracingSamples,
                    raytracingRange
                );
                a += GetAmbientOcclusionAtPoint(
                    plane.v0.position + pushF,
                    Vector3.forward,
                    samplePoints,
                    style,
                    density,
                    raytracingSamples,
                    raytracingRange
                );
                a += GetAmbientOcclusionAtPoint(
                    plane.v0.position - pushF,
                    -Vector3.forward,
                    samplePoints,
                    style,
                    density,
                    raytracingSamples,
                    raytracingRange
                );

                a /= 4.0f;

                plane.v0.SetAmbientOcclusion(a);
                plane.v1.SetAmbientOcclusion(a);
                plane.v2.SetAmbientOcclusion(a);
                plane.v3.SetAmbientOcclusion(a);
            }
        }

        private static float GetAmbientOcclusionAtPoint(
            Vector3 testPoint,
            Vector3 testDirection,
            BoundsOctree<AmbientOcclusionSamplePoint> samplePoints,
            AmbientOcclusionStyle style,
            float density,
            float raytracingSamples,
            float raytracingRange)
        {
            using (BUILD_TIME.AMB_OCC_GEN.GetAmbientOcclusionAtPoint.Auto())
            {
                if (samplePoints == null)
                {
                    return 1.0f;
                }

                var total = 0.0f;
                //var results = new List<OctreeNode<Bounds, AmbientOcclusionSamplePoint>>();
                var keys = new NonSerializedList<Bounds>(1024, noTracking:true);
                var values = new NonSerializedList<AmbientOcclusionSamplePoint>(1024, noTracking:true);

                switch (style)
                {
                    case AmbientOcclusionStyle.Simplified:
                    {
                        var bounds = new Bounds(testPoint, Vector3.one);

                        samplePoints.GetAll(OctreeQueryMode.InsideOrIntersecting, bounds, keys, values);
                        //samplePoints.GetAll(OctreeQueryMode.Intersecting, bounds, results);

                        for(var i = 0; i < values.Count; i++)
                        //foreach (var result in results)
                        {
                            //var key = keys[i];
                            var point = values[i];
                            //var point = result.value;
                            var occlusion = point.GetOcclusionAmount(testPoint, testDirection) * point.density * 0.25f;
                            total += occlusion;
                        }

                        return 1.0f - Mathf.Clamp01(Mathf.Clamp01(total) * density);
                    }

                    case AmbientOcclusionStyle.Raytraced:
                    {
                        var ray = new Ray(testPoint + (testDirection * 0.1f), testDirection);
                        var t = 3.0f;
                        var mint = 3.0f;

                        // trace
                        for (var s = 0; s < raytracingSamples; s++)
                        {
                            var direction = Random.onUnitSphere;
                            if (Vector3.Dot(direction, testDirection) < 0.0f)
                            {
                                direction *= -1.0f;
                            }

                            ray.direction = direction;

                            t = raytracingRange;
                            mint = raytracingRange;

                            keys.Clear();
                            values.Clear();
                            //results.Clear();

                            samplePoints.GetRayHits(ray, keys, values);

                            for(var r = 0; r < values.Count; r++)
                            {
                                var result = values[r];
                                // weight according to density..
                                t = Mathf.Lerp(raytracingRange, t, result.density);
                                //t = Mathf.Lerp(raytracingRange, t, result.value.density);
                                mint = Mathf.Min(t, mint);
                            }

                            total += mint / raytracingRange / raytracingSamples;
                        }

                        return total;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}

/*// raytracing model
float t = 3.0f;
float mint = 3.0f;
Vector3 hit = Vector3.zero;
Ray ray = new Ray(testPoint + testDirection * 0.1f, testDirection);

// cull
for (int i = 0; i < samplePoints.Count; i++)
{
   var point = samplePoints[i];

   Vector3 delta = ray.origin - point.position;

   point.flag = delta.sqrMagnitude <
       ((point.radius + settings.raytracingRange) * (point.radius + settings.raytracingRange));
}

// trace
for (int s = 0; s < settings.raytracingSamples; s++)
{
   Vector3 direction = Random.onUnitSphere;
   if (Vector3.Dot(direction, testDirection) < 0.0f)
   {
       direction *= -1.0f;
   }

   ray.direction = direction;

   t = settings.raytracingRange;
   mint = settings.raytracingRange;
   for (int i = 0; i < samplePoints.Count; i++)
   {
       var point = samplePoints[i];
       if (point.flag)
       {
           if (MathUtils.IntersectRaySphere(ray, point.position, point.radius, ref t, ref hit))
           {
               // weight according to density..
               t = Mathf.Lerp(settings.raytracingRange, t, point.density);
               mint = Mathf.Min(t, mint);
           }
       }
   }

   total += (mint / settings.raytracingRange) / settings.raytracingSamples;
}

return total;*/
