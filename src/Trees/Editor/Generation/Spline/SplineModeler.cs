using System;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Shape;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Spline
{
    public static class SplineModeler
    {
        public static void UpdateSpline(
            BarkHierarchyData hierarchy,
            BarkShapeData shape,
            BarkShapeData parentShape,
            ISeed seed)
        {
            using (BUILD_TIME.SPLN_MOD.UpdateSpline.Auto())
            {
                if (shape.spline == null)
                {
                    var spline = new SplineData();
                    shape.spline = spline;
                }

                var totalHeight = shape.effectiveLength;

                var splineStepSizeStart = 1f;
                var splineStepSizeEnd = 1f;
                var splineStepCount = (int) Mathf.Round(totalHeight);

                if ((hierarchy.curvature.exotropism == null) || (hierarchy.curvature.exotropism.accessor.curve == null) || (hierarchy.curvature.exotropism.accessor.curve.length == 0))
                {
                    hierarchy.curvature.exotropism = TreeProperty.fCurve(0f, 0f, .25f, 0f);
                }
                if (hierarchy.geometry.splineStepMode == SplineStepMode.Adaptive)
                {
                    var y = shape.effectiveMatrix.MultiplyPoint(Vector3.zero).y;

                    var yTime = Mathf.Clamp01(y / 20f);

                    splineStepSizeStart = Mathf.Lerp(1f, 3f, yTime);
                    splineStepSizeEnd = splineStepSizeStart + (totalHeight * .1f);
                }
                else if (hierarchy.geometry.splineStepMode == SplineStepMode.Fixed)
                {
                    if (hierarchy.geometry.splineStepSize.defaultValue == 0f)
                    {
                        hierarchy.geometry.splineStepSize = TreeProperty.New(1f);
                    }

                    splineStepSizeStart = hierarchy.geometry.splineStepSize.Value;
                    splineStepSizeEnd = hierarchy.geometry.splineStepSize.Value;
                }
                else
                {
                    if ((hierarchy.geometry.splineStepSizeV2.defaultValue.x == 0f) &&
                        (hierarchy.geometry.splineStepSizeV2.defaultValue.y == 0f))
                    {
                        hierarchy.geometry.splineStepSizeV2 = TreeProperty.v2(1f, 3f);
                    }

                    splineStepSizeStart = hierarchy.geometry.splineStepSizeV2.Value.x;
                    splineStepSizeEnd = hierarchy.geometry.splineStepSizeV2.Value.y;
                }

                if (splineStepCount == 0)
                {
                    splineStepCount = 1;
                }


                var position = Vector3.zero;

                hierarchy.curvature.crookLikelihood.CheckInitialization(TreeProperty.fCurve(0.0f, 1.0f, 1.0f));

                hierarchy.curvature.crookAbruptness.CheckInitialization(.33f);

                hierarchy.curvature.crookMemory.CheckInitialization(.9f);

                var phototropismCurve = hierarchy.curvature.phototropism.Value;
                var exotrophyCurve = hierarchy.curvature.exotropism.Value;
                var crookednessCurve = hierarchy.curvature.crookedness.Value;
                var croookLikelihoodCurve = hierarchy.curvature.crookLikelihood.Value;
                var curlinessCurve = hierarchy.curvature.curliness.Value;

                var phototropism = MathUtils.QuaternionFromMatrix(shape.matrix.inverse) *
                    Quaternion.Euler(0.0f, shape.radialAngle, 0.0f);
                var gravitropism = MathUtils.QuaternionFromMatrix(shape.matrix.inverse) *
                    Quaternion.Euler(-180.0f, shape.radialAngle, 0.0f);

                var exotropism = Quaternion.identity;
                var endotropism = Quaternion.identity;

                if (parentShape != null)
                {
                    exotropism = MathUtils.QuaternionFromMatrix(shape.matrix.inverse) *
                        MathUtils.QuaternionFromMatrix(parentShape.matrix);

                    var inwardRot = (int) (shape.variationSeed * 4.0f);
                    
                    endotropism = exotropism *
                        (inwardRot switch
                        {
                            0 => Quaternion.Euler(-180.0f, shape.radialAngle, 0.0f),
                            1 => Quaternion.Euler(180.0f,  shape.radialAngle, 0.0f),
                            2 => Quaternion.Euler(0f,      shape.radialAngle, -180.0f),
                            _ => Quaternion.Euler(0f,      shape.radialAngle, 180.0f)
                        });
                }

                var curlinessPlus = Quaternion.Euler(0, shape.radialAngle, 180);
                var curlinessMinus = Quaternion.Euler(0, shape.radialAngle, -180);

                shape.spline.Reset();
                shape.spline.AddPoint(position, 0.0f);

                var crookTargetX = 0f;
                var crookTargetZ = 0f;
                var crookX = 0f;
                var crookZ = 0f;

                var currentHeight = 0.0f;

                /*for (var i = 0; i < count; i++)
                 
                {
                    stepPosition = stepSize;
                    
                    if (i == (count - 1))
                    {
                        stepPosition = totalHeight - currentHeight;
                    }
                    
                    currentHeight += stepPosition;
                */

                while (currentHeight < totalHeight)
                {
                    var time = currentHeight / totalHeight;
                    var stepSize = Mathf.Lerp(splineStepSizeStart, splineStepSizeEnd, time);

                    if ((currentHeight + stepSize) > totalHeight)
                    {
                        stepSize = totalHeight - currentHeight;
                    }

                    currentHeight += stepSize;

                    var currentTime = currentHeight / totalHeight;

                    var crookLikelihood = croookLikelihoodCurve.EvaluateScaledInverse01(currentTime);
                    var crookChance = seed.RandomValue();

                    if (crookChance < crookLikelihood)
                    {
                        crookTargetX = seed.RandomValue() - 0.5f;
                        crookTargetZ = seed.RandomValue() - 0.5f;
                    }
                    else
                    {
                        crookTargetX *= hierarchy.curvature.crookMemory.Value;
                        crookTargetZ *= hierarchy.curvature.crookMemory.Value;
                    }

                    crookX = Mathf.Lerp(crookX, crookTargetX, hierarchy.curvature.crookAbruptness.Value);
                    crookZ = Mathf.Lerp(crookZ, crookTargetZ, hierarchy.curvature.crookAbruptness.Value);

                    var crookedness = Quaternion.Euler(180.0f * crookX, 0.0f, 180.0f * crookZ);

                    var curlValue = curlinessCurve.EvaluateClamped(currentTime, -1.0f, 1.0f);
                    var phototropismValue = phototropismCurve.EvaluateClamped(currentTime, -1.0f, 1.0f);
                    var exotrophyValue = exotrophyCurve.EvaluateClamped(currentTime, -1.0f, 1.0f);

                    var curlinessPlusT = Mathf.Clamp01(curlValue) * curlinessCurve.value;
                    var curlinessMinusT = Mathf.Clamp01(-curlValue) * curlinessCurve.value;
                    var phototrophyT = Mathf.Clamp01(phototropismValue) * phototropismCurve.value;
                    var gravityT = Mathf.Clamp01(-phototropismValue) * phototropismCurve.value;
                    var exotropismT = Mathf.Clamp01(exotrophyValue) * exotrophyCurve.value;
                    var endotropismT = Mathf.Clamp01(-exotrophyValue) * exotrophyCurve.value;
                    var crookednessT = crookednessCurve.EvaluateScaled01(currentTime);

                    var adjRot = Quaternion.identity;
                    adjRot = Quaternion.Slerp(adjRot, curlinessPlus, curlinessPlusT);
                    adjRot = Quaternion.Slerp(adjRot, curlinessMinus, curlinessMinusT);
                    adjRot = Quaternion.Slerp(adjRot, phototropism, phototrophyT);
                    adjRot = Quaternion.Slerp(adjRot, gravitropism, gravityT);
                    adjRot = Quaternion.Slerp(adjRot, exotropism, exotropismT);
                    adjRot = Quaternion.Slerp(adjRot, endotropism, endotropismT);
                    adjRot = Quaternion.Slerp(adjRot, adjRot * crookedness, crookednessT);

                    // advance position
                    position += adjRot * (new Vector3(0.0f, stepSize, 0.0f));


                    shape.spline.AddPoint(position, currentTime);
                }

                UpdateTime(shape.spline);
                UpdateRotations(shape.spline);


                if (hierarchy.geometry.forked)
                {
                    shape.capRange = 0.0f;
                }
                else if (hierarchy.limb.capSmoothing < 0.01f)
                {
                    shape.capRange = 0.0f;
                }
                else
                {
                    var capRadius = hierarchy.geometry.radiusCurve.EvaluateClamped01(1.0f) * shape.effectiveSize;

                    var approximateHeight = Mathf.Max(GetApproximateLength(shape.spline), 0.00001f);

                    shape.capRange = (capRadius / approximateHeight) * hierarchy.limb.capSmoothing * 2.0f;
                }

                if (shape.spline.points.Count == 0)
                {
                    throw new NotSupportedException("Bad spline point count");
                }
            }
        }

        public static float GetApproximateLength(SplineData spline)
        {
            using (BUILD_TIME.SPLN_MOD.GetApproximateLength.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2) return 0.0f;

                var lengthEstimate = 0.0f;
                for (var i = 1; i < spline.points.Count; i++)
                {
                    var delta = (spline.points[i - 1].point - spline.points[i].point).magnitude;
                    lengthEstimate += delta;
                }

                return lengthEstimate;
            }
        }

        public static void UpdateTime(SplineData spline)
        {
            using (BUILD_TIME.SPLN_MOD.UpdateTime.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2)
                {
                    throw new NotSupportedException("Bad spline count");
                }

                var totalDistance = GetApproximateLength(spline);

                var currentDistance = 0.0f;
                spline.points[0].time = currentDistance;
                for (var i = 1; i < spline.points.Count; i++)
                {
                    var delta = (spline.points[i - 1].point - spline.points[i].point).magnitude;
                    currentDistance += delta;
                    spline.points[i].time = currentDistance / totalDistance;
                }
            }
        }

        public static void UpdateRotations(SplineData spline)
        {
            using (BUILD_TIME.SPLN_MOD.UpdateRotations.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2)
                {
                    throw new NotSupportedException("Bad spline count");
                }

                var matrix = Matrix4x4.identity;

                spline.points[0].rotation = Quaternion.identity;
                spline.points[0].tangent = new Vector3(0, 1, 0);
                spline.points[0].normal = new Vector3(0, 0, 1);

                for (var i = 1; i < spline.points.Count; i++)
                {
                    Vector3 up;

                    if (i == (spline.points.Count - 1))
                    {
                        up = spline.points[i].point - spline.points[i - 1].point;
                    }
                    else
                    {
                        var distA = Vector3.Distance(spline.points[i].point, spline.points[i - 1].point);
                        var distB = Vector3.Distance(spline.points[i].point, spline.points[i + 1].point);
                        up = ((spline.points[i].point - spline.points[i - 1].point) / distA) +
                            ((spline.points[i + 1].point - spline.points[i].point) / distB);
                    }

                    up.Normalize();

                    matrix.SetColumn(1, up);
                    if (Mathf.Abs(Vector3.Dot(up, matrix.GetColumn(0))) > 0.9999f)
                    {
                        matrix.SetColumn(0, new Vector3(0, 1, 0));
                    }

                    var c2 = Vector3.Cross(matrix.GetColumn(0), up).normalized;
                    matrix.SetColumn(2, c2);
                    matrix = MathUtils.OrthogonalizeMatrix(matrix);

                    spline.points[i].rotation = MathUtils.QuaternionFromMatrix(matrix);
                    spline.points[i].normal = matrix.GetColumn(2);
                    spline.points[i].tangent = matrix.GetColumn(1);

                    if (Quaternion.Dot(spline.points[i].rotation, spline.points[i - 1].rotation) < 0.0f)
                    {
                        spline.points[i].rotation.x = -spline.points[i].rotation.x;
                        spline.points[i].rotation.y = -spline.points[i].rotation.y;
                        spline.points[i].rotation.z = -spline.points[i].rotation.z;
                        spline.points[i].rotation.w = -spline.points[i].rotation.w;
                    }
                }
            }
        }

        private static Quaternion GetRotationInternal(SplineData spline, int idxFirstpoint, float t)
        {
            using (BUILD_TIME.SPLN_MOD.GetRotationInternal.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2)
                {
                    throw new NotSupportedException("Bad spline count");
                }

                var t2 = t * t;
                var t3 = t2 * t;

                var Q0 = spline.points[Mathf.Max(idxFirstpoint - 1, 0)].rotation;
                var Q1 = spline.points[idxFirstpoint].rotation;
                var Q2 = spline.points[idxFirstpoint + 1].rotation;
                var Q3 = spline.points[Mathf.Min(idxFirstpoint + 2, spline.points.Count - 1)].rotation;

                var T1 = new Quaternion(
                    spline.tension * (Q2.x - Q0.x),
                    spline.tension * (Q2.y - Q0.y),
                    spline.tension * (Q2.z - Q0.z),
                    spline.tension * (Q2.w - Q0.w)
                );
                var T2 = new Quaternion(
                    spline.tension * (Q3.x - Q1.x),
                    spline.tension * (Q3.y - Q1.y),
                    spline.tension * (Q3.z - Q1.z),
                    spline.tension * (Q3.w - Q1.w)
                );

                var Blend1 = ((2 * t3) - (3 * t2)) + 1;
                var Blend2 = (-2 * t3) + (3 * t2);
                var Blend3 = (t3 - (2 * t2)) + t;
                var Blend4 = t3 - t2;

                var q = new Quaternion();
                q.x = (Blend1 * Q1.x) + (Blend2 * Q2.x) + (Blend3 * T1.x) + (Blend4 * T2.x);
                q.y = (Blend1 * Q1.y) + (Blend2 * Q2.y) + (Blend3 * T1.y) + (Blend4 * T2.y);
                q.z = (Blend1 * Q1.z) + (Blend2 * Q2.z) + (Blend3 * T1.z) + (Blend4 * T2.z);
                q.w = (Blend1 * Q1.w) + (Blend2 * Q2.w) + (Blend3 * T1.w) + (Blend4 * T2.w);
                var mag = Mathf.Sqrt((q.x * q.x) + (q.y * q.y) + (q.z * q.z) + (q.w * q.w));
                q.x /= mag;
                q.y /= mag;
                q.z /= mag;
                q.w /= mag;
                return q;
            }
        }

        private static Vector3 GetPositionInternal(SplineData spline, int idxFirstpoint, float t)
        {
            using (BUILD_TIME.SPLN_MOD.GetPositionInternal.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2)
                {
                    throw new NotSupportedException("Bad spline count");
                }

                var t2 = t * t;
                var t3 = t2 * t;

                var P0 = spline.points[Mathf.Max(idxFirstpoint - 1, 0)].point;
                var P1 = spline.points[idxFirstpoint].point;
                var P2 = spline.points[idxFirstpoint + 1].point;
                var P3 = spline.points[Mathf.Min(idxFirstpoint + 2, spline.points.Count - 1)].point;

                var T1 = spline.tension * (P2 - P0);
                var T2 = spline.tension * (P3 - P1);

                var Blend1 = ((2 * t3) - (3 * t2)) + 1;
                var Blend2 = (-2 * t3) + (3 * t2);
                var Blend3 = (t3 - (2 * t2)) + t;
                var Blend4 = t3 - t2;

                return (Blend1 * P1) + (Blend2 * P2) + (Blend3 * T1) + (Blend4 * T2);
            }
        }

        public static Quaternion GetRotationAtTime(SplineData spline, float timeParam)
        {
            using (BUILD_TIME.SPLN_MOD.GetRotationAtTime.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2) return Quaternion.identity;

                if (timeParam <= spline.points[0].time) return spline.points[0].rotation;
                if (timeParam >= spline.points[spline.points.Count - 1].time)
                    return spline.points[spline.points.Count - 1].rotation;

                int c;
                for (c = 1; c < spline.points.Count; c++)
                {
                    if (spline.points[c].time > timeParam) break;
                }

                var idx = c - 1;
                var param = (timeParam - spline.points[idx].time) /
                    (spline.points[idx + 1].time - spline.points[idx].time);

                return GetRotationInternal(spline, idx, param);
            }
        }

        public static Vector3 GetPositionAtTime(SplineData spline, float timeParam)
        {
            using (BUILD_TIME.SPLN_MOD.GetPositionAtTime.Auto())
            {
                if (spline == null)
                {
                    throw new NotSupportedException("Spline missing!");
                }

                if (spline.points.Count < 2) return Vector3.zero;

                if (timeParam <= spline.points[0].time) return spline.points[0].point;
                if (timeParam >= spline.points[spline.points.Count - 1].time)
                    return spline.points[spline.points.Count - 1].point;

                int c;
                for (c = 1; c < spline.points.Count; c++)
                {
                    if (spline.points[c].time > timeParam) break;
                }

                var idx = c - 1;
                var param = (timeParam - spline.points[idx].time) /
                    (spline.points[idx + 1].time - spline.points[idx].time);

                return GetPositionInternal(spline, idx, param);
            }
        }

        public static float GetSurfaceAngleAtTime(
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            using (BUILD_TIME.SPLN_MOD.GetSurfaceAngleAtTime.Auto())
            {
                if (shape == null)
                {
                    throw new NotSupportedException("Shape missing!");
                }

                if (hierarchy == null)
                {
                    throw new NotSupportedException("Hierarchy missing!");
                }

                var angle = 0.0f;

                var pos0 = GetPositionAtTime(shape.spline, time);
                var rad0 = GetRadiusAtTime(shape, hierarchy, time);

                if (time < 0.5f)
                {
                    var difPos = (GetPositionAtTime(shape.spline, time + 0.01f) - pos0).magnitude;
                    var difRad = GetRadiusAtTime(shape, hierarchy, time + 0.01f) - rad0;

                    angle = Mathf.Atan2(difRad, difPos);
                }
                else
                {
                    var disPos = (pos0 - GetPositionAtTime(shape.spline, time - 0.01f)).magnitude;
                    var difRad = rad0 - GetRadiusAtTime(shape, hierarchy, time - 0.01f);

                    angle = Mathf.Atan2(difRad, disPos);
                }

                return (angle * Mathf.Rad2Deg);
            }
        }

        public static float GetRadiusAtTime(
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            using (BUILD_TIME.SPLN_MOD.GetRadiusAtTime.Auto())
            {
                if (shape == null)
                {
                    throw new NotSupportedException("Shape missing!");
                }

                if (hierarchy == null)
                {
                    throw new NotSupportedException("Hierarchy missing!");
                }

                var geometry = hierarchy.geometry;

                // no radius when only fronds are displayed..
                if (geometry.geometryMode == BranchGeometryMode.Frond)
                {
                    return 0.0f;
                }

                var mainRadius = Mathf.Clamp01(hierarchy.geometry.radiusCurve.EvaluateClamped(time, .0001f, 1f)) *
                    shape.effectiveSize;

                // Smooth capping
                var capStart = 1.0f - shape.capRange;
                if (time > capStart)
                {
                    // within cap area
                    var angle = Mathf.Acos(Mathf.Clamp01((time - capStart) / shape.capRange));
                    var roundScale = Mathf.Sin(angle);
                    mainRadius *= roundScale;
                }
                
                return mainRadius;
            }
        }
        
        public static float GetRadiusWithCollarAtTime(
            IHierarchyRead hierarchies,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            using (BUILD_TIME.SPLN_MOD.GetRadiusAtTime.Auto())
            {
                var mainRadius = GetRadiusAtTime(shape, hierarchy, time);

                var flareCollarRad = GetCollarAtTime(hierarchies, shape, hierarchy, time);
                
                mainRadius += Mathf.Max(flareCollarRad.x, Mathf.Max(flareCollarRad.y, flareCollarRad.z) * 0.25f) *
                    0.1f;
                

                return mainRadius;
            }
        }

        public static Vector3 GetCollarAtTime(
            IHierarchyRead hierarchies,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            using (BUILD_TIME.SPLN_MOD.GetFlareCollarAtTime.Auto())
            {
                if (shape == null)
                {
                    throw new NotSupportedException("Shape missing!");
                }

                if (hierarchy == null)
                {
                    throw new NotSupportedException("Hierarchy missing!");
                }

                var worldScale = shape.effectiveScale;


                if (hierarchy.IsTrunk)
                {
                    return GetTrunkFlare(hierarchies, shape, hierarchy, time);
                }

                if (hierarchy.geometry.forked)
                {
                    return Vector3.zero;
                }

                if (hierarchy.IsBranch || hierarchy.IsRoot)
                {
                    return GetBranchCollar(shape, hierarchy, time);
                }

                return Vector3.zero;
            }
        }

        private static Vector3 GetBranchCollar(
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            var worldScale = shape.effectiveScale;

            var collared = hierarchy as CollaredBarkHierarchyData;

            if (collared.collar.collarHeight < 0.001f)
            {
                return Vector3.zero;
            }

            if (time > collared.collar.collarHeight)
            {
                return Vector3.zero;
            }

            var relativeTime = 1.0f - (time / collared.collar.collarHeight);

            if ((collared.collar.collarPower == null) || (collared.collar.collarPower == 0f))
            {
                collared.collar.collarPower = TreeProperty.New(1.5f);
            }

            var power = collared.collar.collarPower;

            var spreadFactor = collared.collar.collarSpreadMultiplier *
                Mathf.Pow(Mathf.Clamp01(relativeTime), power) *
                worldScale;

            return new Vector3(
                0f,
                spreadFactor * collared.collar.collarSpreadTop,
                spreadFactor * collared.collar.collarSpreadBottom
            );
        }

        private static Vector3 GetTrunkFlare(
            IHierarchyRead hierarchies,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            var trunk = hierarchy as TrunkHierarchyData;

            if (trunk.trunk.flareHeight < 0.001f)
            {
                return Vector3.zero;
            }

            var flareValue = GetTrunkFlareTime(hierarchies, shape, hierarchy, time);

            if ((trunk.trunk.flarePower == null) || (trunk.trunk.flarePower == 0))
            {
                trunk.trunk.flarePower = TreeProperty.New(1.5f);
            }

            var flarePower = trunk.trunk.flarePower;

            var flareFactor = Mathf.Pow(Mathf.Clamp01(flareValue), flarePower) * shape.effectiveScale;

            var flareRadius = shape.effectiveSize * trunk.trunk.flareRadius;
            
            return new Vector3(flareFactor * flareRadius, 0f, 0f);

        }
        
        public static float GetTrunkFlareTime(
            IHierarchyRead hierarchies,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            float time)
        {
            var trunk = hierarchy as TrunkHierarchyData;

            if (trunk.trunk.flareHeight < 0.001f)
            {
                return 0.0f;
            }

            var vertical = hierarchies.GetVerticalOffset();
            var length = GetApproximateLength(shape.spline);

            var flareEnd = trunk.trunk.flareHeight + vertical;

            var flareEndTime = flareEnd / length;

            var flareValue = (flareEndTime - Mathf.Clamp(time, 0f, flareEndTime)) / flareEndTime;

            return flareValue;
        }
        
        



        public static Matrix4x4 GetLocalMatrixAtTime(SplineData spline, float time)
        {
            var pos = GetPositionAtTime(spline, time);
            var rot = GetRotationAtTime(spline, time);

            return Matrix4x4.TRS(pos, rot, Vector3.one);
        }
    }
}
