using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    public static class FrondGeometryGenerator
    {
        // ReSharper disable once FunctionComplexityOverflow
        public static void GenerateFrondGeometry(
            LODGenerationOutput output,
            LevelOfDetailSettings settings,
            InputMaterialCache inputMaterialCache,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            List<SplineHeightSample> heightSamples)
        {
            using (BUILD_TIME.GEO_GEN.GenerateFrondGeometry.Auto())
            {
                var frondMinRange = Mathf.Min(
                    hierarchy.geometry.frond.frondRange.Value.x,
                    hierarchy.geometry.frond.frondRange.Value.y
                );
                var frondMaxRange = Mathf.Max(
                    hierarchy.geometry.frond.frondRange.Value.x,
                    hierarchy.geometry.frond.frondRange.Value.y
                );

                var frondActualMin = Mathf.Clamp(frondMinRange, 0.0f, shape.breakOffset);
                var frondActualMax = Mathf.Clamp(frondMaxRange, 0.0f, shape.breakOffset);

                if ((hierarchy.geometry.frond.frondCount <= 0) || (Math.Abs(frondMinRange - frondMaxRange) < float.Epsilon))
                {
                    return;
                }

                var needStartPoint = true;
                var needEndPoint = true;

                for (var i = 0; i < heightSamples.Count; i++)
                {
                    var t = heightSamples[i].height;

                    if (t < frondMinRange)
                    {
                        heightSamples.RemoveAt(i);
                        i--;
                    }
                    else if (Math.Abs(t - frondMinRange) < float.Epsilon)
                    {
                        needStartPoint = false;
                    }
                    else if (Math.Abs(t - frondMaxRange) < float.Epsilon)
                    {
                        needEndPoint = false;
                    }
                    else if (t > frondMaxRange)
                    {
                        heightSamples.RemoveAt(i);
                        i--;
                    }
                }

                if (needStartPoint)
                {
                    heightSamples.Insert(0, SplineHeightSample.At(frondMinRange));
                }

                if (needEndPoint)
                {
                    heightSamples.Add(SplineHeightSample.At(frondMaxRange));
                }

                var materialID = inputMaterialCache?.GetInputMaterialData(
                            hierarchy.geometry.frond.frondMaterial,
                            TreeMaterialUsage.Frond
                        )
                        ?.materialID ??
                    -1;

                var capStart = 1.0f - shape.capRange;

                for (var frondIndex = 0; frondIndex < hierarchy.geometry.frond.frondCount; frondIndex++)
                {
                    var crease = hierarchy.geometry.frond.frondCrease * 90.0f * Mathf.Deg2Rad;
                    var angle = (((hierarchy.geometry.frond.frondRotation * 360.0f) +
                                ((frondIndex * 180.0f) / hierarchy.geometry.frond.frondCount)) -
                            90.0f) *
                        Mathf.Deg2Rad;

                    var angleA = -angle - crease;
                    var angleB = angle - crease;

                    var directionA = new Vector3(Mathf.Sin(angleA), 0.0f, Mathf.Cos(angleA));
                    var normalA = new Vector3(directionA.z, 0.0f, -directionA.x);

                    var directionB = new Vector3(Mathf.Sin(angleB), 0.0f, -Mathf.Cos(angleB));
                    var normalB = new Vector3(-directionB.z, 0.0f, directionB.x);

                    for (var sampleIndex = 0; sampleIndex < heightSamples.Count; sampleIndex++)
                    {
                        var sample = heightSamples[sampleIndex].height;

                        var sampleTime = (sample - frondActualMin) / (frondActualMax - frondActualMin);

                        var sample2 = sample;
                        if (sample > capStart)
                        {
                            sample2 = capStart;

                            var capAngle = Mathf.Acos(Mathf.Clamp01((sample - capStart) / shape.capRange));
                            var sinCapAngle = Mathf.Sin(capAngle);
                            var cosCapAngle = Mathf.Cos(capAngle) * hierarchy.limb.capSmoothing;

                            directionA = new Vector3(
                                Mathf.Sin(angleA) * sinCapAngle,
                                cosCapAngle,
                                Mathf.Cos(angleA) * sinCapAngle
                            );

                            normalA = new Vector3(directionA.z, directionA.y, -directionA.x);

                            directionB = new Vector3(
                                Mathf.Sin(angleB) * sinCapAngle,
                                cosCapAngle,
                                -Mathf.Cos(angleB) * sinCapAngle
                            );

                            normalB = new Vector3(-directionB.z, directionB.y, directionB.x);
                        }

                        var normalMid = new Vector3(0, 0, -1);

                        var position = SplineModeler.GetPositionAtTime(shape.spline, sample2);
                        var rotation = SplineModeler.GetRotationAtTime(shape.spline, sample);

                        var radius = Mathf.Clamp01(hierarchy.geometry.frond.frondWidth.Value.EvaluateScaled(sample)) *
                            shape.effectiveScale;

                        var matrix = shape.effectiveMatrix * Matrix4x4.TRS(position, rotation, Vector3.one);

                        if (settings.doubleSidedLeafGeometry)
                        {
                            for (float side = -1; side < 2; side += 2)
                            {
                                var v0 = new TreeVertex() /*TreeVertex.Get()*/;
                                var v1 = new TreeVertex() /*TreeVertex.Get()*/;
                                var v2 = new TreeVertex() /*TreeVertex.Get()*/;
                                var v3 = new TreeVertex() /*TreeVertex.Get()*/;

                                output.AddVertex(v0);
                                output.AddVertex(v1);
                                output.AddVertex(v2);
                                output.AddVertex(v3);

                                v0.Set(shape, sample);
                                v0.position = matrix.MultiplyPoint(directionA * radius);
                                v0.normal = matrix.MultiplyVector(normalA * side).normalized;
                                v0.tangent = TangentGenerator.CreateTangent(shape, rotation, v0.normal);
                                v0.tangent.w = -side;
                                v0.raw_uv0 = new Vector2(1.0f, sampleTime);

                                v1.Set(shape, sample);
                                v1.position = matrix.MultiplyPoint(Vector3.zero);
                                v1.normal = matrix.MultiplyVector(normalMid * side).normalized;
                                v1.tangent = TangentGenerator.CreateTangent(shape, rotation, v1.normal);
                                v1.tangent.w = -side;
                                v1.raw_uv0 = new Vector2(0.5f, sampleTime);

                                v2.Set(shape, sample);
                                v2.position = matrix.MultiplyPoint(Vector3.zero);
                                v2.normal = matrix.MultiplyVector(normalMid * side).normalized;
                                v2.tangent = TangentGenerator.CreateTangent(shape, rotation, v2.normal);
                                v2.tangent.w = -side;
                                v2.raw_uv0 = new Vector2(0.5f, sampleTime);

                                v3.Set(shape, sample);
                                v3.position = matrix.MultiplyPoint(directionB * radius);
                                v3.normal = matrix.MultiplyVector(normalB * side).normalized;
                                v3.tangent = TangentGenerator.CreateTangent(shape, rotation, v3.normal);
                                v3.tangent.w = -side;
                                v3.raw_uv0 = new Vector2(0.0f, sampleTime);
                            }

                            if (sampleIndex > 0)
                            {
                                var voffset = output.vertices.Count;

                                var tri0 = new TreeTriangle();
                                var tri1 = new TreeTriangle();
                                var tri2 = new TreeTriangle();
                                var tri3 = new TreeTriangle();
                                var tri4 = new TreeTriangle();
                                var tri5 = new TreeTriangle();
                                var tri6 = new TreeTriangle();
                                var tri7 = new TreeTriangle();

                                tri0.Set(
                                    shape,
                                    materialID,
                                    voffset - 4,
                                    voffset - 3,
                                    voffset - 11,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri1.Set(
                                    shape,
                                    materialID,
                                    voffset - 4,
                                    voffset - 11,
                                    voffset - 12,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri2.Set(
                                    shape,
                                    materialID,
                                    voffset - 8,
                                    voffset - 7,
                                    voffset - 15,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri3.Set(
                                    shape,
                                    materialID,
                                    voffset - 8,
                                    voffset - 15,
                                    voffset - 16,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri4.Set(
                                    shape,
                                    materialID,
                                    voffset - 2,
                                    voffset - 9,
                                    voffset - 1,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri5.Set(
                                    shape,
                                    materialID,
                                    voffset - 2,
                                    voffset - 10,
                                    voffset - 9,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri6.Set(
                                    shape,
                                    materialID,
                                    voffset - 6,
                                    voffset - 13,
                                    voffset - 5,
                                    TreeMaterialUsage.Frond,
                                    false
                                );
                                tri7.Set(
                                    shape,
                                    materialID,
                                    voffset - 6,
                                    voffset - 14,
                                    voffset - 13,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri0.FlipFace();
                                tri1.FlipFace();
                                tri6.FlipFace();
                                tri7.FlipFace();

                                output.AddTriangle(tri0);
                                output.AddTriangle(tri1);
                                output.AddTriangle(tri2);
                                output.AddTriangle(tri3);
                                output.AddTriangle(tri4);
                                output.AddTriangle(tri5);
                                output.AddTriangle(tri6);
                                output.AddTriangle(tri7);
                            }
                        }
                        else
                        {
                            var v0 = new TreeVertex() /*TreeVertex.Get()*/;
                            var v1 = new TreeVertex() /*TreeVertex.Get()*/;
                            var v2 = new TreeVertex() /*TreeVertex.Get()*/;

                            output.AddVertex(v0);
                            output.AddVertex(v1);
                            output.AddVertex(v2);

                            v0.Set(shape, sample);
                            v0.position = matrix.MultiplyPoint(directionA * radius);
                            v0.normal = matrix.MultiplyVector(normalA).normalized;
                            v0.raw_uv0 = new Vector2(0.0f, sampleTime);

                            v1.Set(shape, sample);
                            v1.position = matrix.MultiplyPoint(Vector3.zero);
                            v1.normal = matrix.MultiplyVector(Vector3.back).normalized;
                            v1.raw_uv0 = new Vector2(0.5f, sampleTime);

                            v2.Set(shape, sample);
                            v2.position = matrix.MultiplyPoint(directionB * radius);
                            v2.normal = matrix.MultiplyVector(normalB).normalized;
                            v2.raw_uv0 = new Vector2(1.0f, sampleTime);

                            if (sampleIndex > 0)
                            {
                                var voffset = output.vertices.Count;

                                var tri0 = new TreeTriangle();
                                var tri1 = new TreeTriangle();
                                var tri2 = new TreeTriangle();
                                var tri3 = new TreeTriangle();

                                tri0.Set(
                                    shape,
                                    materialID,
                                    voffset - 2,
                                    voffset - 3,
                                    voffset - 6,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri1.Set(
                                    shape,
                                    materialID,
                                    voffset - 2,
                                    voffset - 6,
                                    voffset - 5,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri2.Set(
                                    shape,
                                    materialID,
                                    voffset - 2,
                                    voffset - 4,
                                    voffset - 1,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                tri3.Set(
                                    shape,
                                    materialID,
                                    voffset - 2,
                                    voffset - 5,
                                    voffset - 4,
                                    TreeMaterialUsage.Frond,
                                    false
                                );

                                output.AddTriangle(tri0);
                                output.AddTriangle(tri1);
                                output.AddTriangle(tri2);
                                output.AddTriangle(tri3);
                            }
                        }
                    }
                }
            }
        }
    }
}