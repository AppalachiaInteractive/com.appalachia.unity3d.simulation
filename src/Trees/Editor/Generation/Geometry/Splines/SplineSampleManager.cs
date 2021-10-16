using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    public static class SplineSampleManager
    {
        public static List<SplineHeightSample> GetSamplePoints(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float adaptiveQuality)
        {
            using (BUILD_TIME.GEO_GEN.GetSamplePoints.Auto())
            {
                var samplePoints = new List<SplineHeightSample>();
                if (shape.spline == null)
                {
                    return samplePoints;
                }
                
                var length = SplineModeler.GetApproximateLength(shape.spline);

                // starting point for cap smoothing
                var capStartPoint = 1.0f - shape.capRange;

                for (var i = 0; i < shape.spline.points.Count; i++)
                {
                    if (shape.spline.points[i].time > capStartPoint)
                    {
                        samplePoints.Add(SplineHeightSample.At(capStartPoint));
                        break;
                    }

                    samplePoints.Add(SplineHeightSample.At(shape.spline.points[i].time));
                }

                var children = shapes.GetShapesByParentShape(shape.shapeID);

                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var targetSample = child.offset;
                    
                        if (child.type == TreeComponentType.Branch)
                        {
                            var childRadius = child.effectiveSize;

                            var radiusOffsetBoost = 2*(childRadius / length);
                            targetSample = child.offset + radiusOffsetBoost;
                        }
                    
                        var point = child.type == TreeComponentType.Branch
                            ? SplineHeightSample.Critical(targetSample)
                            : SplineHeightSample.At(targetSample);

                        samplePoints.Add(point);
                    }
                }
                

                samplePoints.Sort();

                if (samplePoints.Count < 2)
                {
                    return samplePoints;
                }

                var radius = shape.effectiveSize;

                var thresholdA = Mathf.Lerp(0.999f, 0.99999f, adaptiveQuality);
                var thresholdB = Mathf.Lerp(0.5f, 0.985f, adaptiveQuality);
                var thresholdR = Mathf.Lerp(0.3f * radius, 0.1f * radius, adaptiveQuality);

                var subdivisionRemaining = 200;

                var last = 0;
                while (last < (samplePoints.Count - 1))
                {
                    for (var i = last; i < (samplePoints.Count - 1); i++)
                    {
                        var a = SplineModeler.GetRotationAtTime(shape.spline, samplePoints[i].height);
                        var b = SplineModeler.GetRotationAtTime(shape.spline, samplePoints[i + 1].height);

                        var upA = a * Vector3.up;
                        var upB = b * Vector3.up;
                        var rightA = a * Vector3.right;
                        var rightB = b * Vector3.right;
                        var frontA = a * Vector3.forward;
                        var frontB = b * Vector3.forward;

                        var radiusA = SplineModeler.GetRadiusWithCollarAtTime(hierarchies, shape, hierarchy, samplePoints[i].height);
                        var radiusB = SplineModeler.GetRadiusWithCollarAtTime(hierarchies, shape, hierarchy, samplePoints[i + 1].height);

                        var needSubDivision = (Vector3.Dot(upA, upB) < thresholdB) ||
                            (Vector3.Dot(rightA, rightB) < thresholdB) ||
                            (Vector3.Dot(frontA, frontB) < thresholdB);

                        if (Mathf.Abs(radiusA - radiusB) > thresholdR)
                        {
                            needSubDivision = true;
                        }

                        if (needSubDivision)
                        {
                            subdivisionRemaining--;
                            if (subdivisionRemaining > 0)
                            {
                                var mid = (samplePoints[i].height + samplePoints[i + 1].height) * 0.5f;

                                samplePoints.Insert(i + 1, SplineHeightSample.At(mid));

                                break;
                            }
                        }

                        last = i + 1;
                    }
                }

                for (var i = 0; i < (samplePoints.Count - 2); i++)
                {
                    var pointA = samplePoints[i];
                    var pointB = samplePoints[i + 1];
                    var pointC = samplePoints[i + 2];

                    if (pointB.critical)
                    {
                        continue;
                    }
                    
                    var a = SplineModeler.GetPositionAtTime(shape.spline, pointA.height);
                    var b = SplineModeler.GetPositionAtTime(shape.spline, pointB.height);
                    var c = SplineModeler.GetPositionAtTime(shape.spline, pointC.height);

                    var radiusA = SplineModeler.GetRadiusWithCollarAtTime(hierarchies, shape, hierarchy, pointA.height);
                    var radiusB = SplineModeler.GetRadiusWithCollarAtTime(hierarchies, shape, hierarchy, pointB.height);
                    var radiusC = SplineModeler.GetRadiusWithCollarAtTime(hierarchies, shape, hierarchy, pointC.height);

                    var dirAB = (b - a).normalized;
                    var dirAC = (c - a).normalized;

                    var removeMidPoint = Vector3.Dot(dirAB, dirAC) >= thresholdA;

                    if (Mathf.Abs(radiusA - radiusB) > thresholdR)
                    {
                        removeMidPoint = false;
                    }

                    if (Mathf.Abs(radiusB - radiusC) > thresholdR)
                    {
                        removeMidPoint = false;
                    }

                    if (removeMidPoint)
                    {
                        samplePoints.RemoveAt(i + 1);
                        i--;
                    }
                }

                if (samplePoints[samplePoints.Count - 1].height < 1.0f)
                {
                    samplePoints.Add(SplineHeightSample.At(1.0f));
                }
                else
                {
                    var sp = samplePoints[samplePoints.Count - 1];
                    sp.height = 1.0f;
                    samplePoints[samplePoints.Count - 1] = sp;
                }

                /*if (shape.capRange > 0.0f)
                {
                    var capLoops = 1 + Mathf.CeilToInt(shape.capRange * 16.0f * adaptiveQuality);

                    for (var i = 0; i < capLoops; i++)
                    {
                        var angle = ((float) (i + 1) / capLoops) * Mathf.PI * 0.5f;
                        var smooth = Mathf.Sin(angle);
                        var capPoint = capStartPoint + (shape.capRange * smooth);

                        samplePoints.Add(SplineHeightSample.At(capPoint));
                    }

                    /*samplePoints.Sort();#1#
                }*/

                return samplePoints;
            }
        }

        public static void ResampleSplineChildren(
            IShapeRead shapes,
            LevelOfDetailSettings settings,
            BarkShapeData shape,
            List<SplineHeightSample> heightSamples)
        {
            using (BUILD_TIME.GEO_GEN.ResampleSplineChildren.Auto())
            {
                var length = SplineModeler.GetApproximateLength(shape.spline);
                
                var resampleChildren = settings.resampleSplineAtChildren;

                if (settings.resampleOnTrunkOnly && (shape.type != TreeComponentType.Trunk))
                {
                    resampleChildren = false;
                }

                if (!resampleChildren)
                {
                    return;
                }

                var children = shapes.GetShapesByParentShape(shape.shapeID);

                if (children == null)
                {
                    return;
                }

                foreach (var child in children)
                {
                    var process = false;

                    switch (child.type)
                    {
                        case TreeComponentType.Root:
                        case TreeComponentType.Trunk:
                            process = false;
                            break;
                        case TreeComponentType.Branch:
                            process = true;
                            break;
                        case TreeComponentType.Leaf:
                            process = settings.resampleLeaves;
                            break;
                        case TreeComponentType.Fruit:
                        case TreeComponentType.Knot:
                        case TreeComponentType.Fungus:
                            process = settings.resampleAddOns;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!child.exportGeometry)
                    {
                        process = false;
                    }

                    if ((child.offset > settings.childResampleHeightLimit) || (child.offset > .95f))
                    {
                        process = false;
                    }

                    if (!process)
                    {
                        continue;
                    }

                    var matched = false;

                    var targetSample = child.offset;
                    
                    if (child.type == TreeComponentType.Branch)
                    {
                        var radius = child.effectiveSize;

                        var radiusOffsetBoost = 2*(radius / length);
                        targetSample = child.offset + radiusOffsetBoost;
                    }
                    
                    foreach (var sample in heightSamples)
                    {
                        if (Mathf.Abs(sample.height - targetSample) < settings.childResampleThreshold)
                        {
                            matched = true;
                            break;
                        }
                    }

                    if (matched)
                    {
                        continue;
                    }

                    heightSamples.Add(SplineHeightSample.At(targetSample));
                }
            }
        }
     
        public static List<SplineHeightSample> CollapseHeightSamples(List<SplineHeightSample> samples)
        {
            var result = new List<SplineHeightSample>();

            for (var i = 0; i < samples.Count; i++)
            {
                var sample = samples[i];
                
                if (i == 0)
                {
                    result.Add(sample);
                    continue;
                }

                var previous = samples[i - 1];

                var gap = sample.height - previous.height;

                if (gap < .01f)
                {
                    continue;
                }

                result.Add(sample);
            }

            var last = result[result.Count - 1];
            last.height = 1;
            result[result.Count - 1] = last;

            return result;
        }
    }
}
