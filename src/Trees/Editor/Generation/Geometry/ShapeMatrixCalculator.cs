using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Utility.Logging;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry
{
    public static class ShapeMatrixCalculator
    {
        public static void UpdateSplineMatrix(
            float rootDepth,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            BarkShapeData parentShape,
            BarkHierarchyData parentHierarchy
            )
        {
            using (BUILD_TIME.SHAPE_ARR.UpdateSplineOrigin.Auto())
            {

                if (shape.type == TreeComponentType.Trunk)
                {
                    var trunkHierarchy = hierarchy as TrunkHierarchyData;
                    var distance = shape.offset * trunkHierarchy.trunk.trunkSpread;
                    var angle = shape.radialAngle * Mathf.Deg2Rad;

                    var position = new Vector3(
                        Mathf.Cos(angle) * distance,
                        -rootDepth,
                        Mathf.Sin(angle) * distance
                    );

                    var rotation =
                        Quaternion.Euler(shape.verticalAngle * -Mathf.Sin(angle), 0, shape.verticalAngle * Mathf.Cos(angle)) *
                        Quaternion.Euler(0, shape.radialAngle, 0);

                    shape.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                }
                else
                {
                    var position = SplineModeler.GetPositionAtTime(parentShape.spline, shape.offset);
                    var rotation = SplineModeler.GetRotationAtTime(parentShape.spline, shape.offset);

                    var angle = Quaternion.Euler(90.0f, shape.radialAngle, 0);

                    var anglematrix = Matrix4x4.Rotate(angle);

                    var pitchMatrix = Matrix4x4.Rotate(Quaternion.Euler(shape.verticalAngle, 0.0f, 0.0f));

                    shape.matrix = parentShape.matrix *
                        Matrix4x4.TRS(position, rotation, Vector3.one) *
                        anglematrix *
                        pitchMatrix;

                    
                    if (!hierarchy.geometry.forked)
                    {
                        var radius = SplineModeler.GetRadiusAtTime(parentShape, parentHierarchy, shape.offset);
                    
                        var radiusOffset = shape.matrix.MultiplyPoint(new Vector3(0, radius, 0));

                        shape.matrix.m03 = radiusOffset.x;
                        shape.matrix.m13 = radiusOffset.y;
                        shape.matrix.m23 = radiusOffset.z;
                    }

                    if (shape.matrix.isIdentity)
                    {
                       AppaLog.Warn(
                            $"Default matrix present for shape {shape.shapeID} in hierarchy {shape.hierarchyID}"
                        );
                    }
                }
            }
        }

        public static void UpdateLeafMatrix(
            LeafShapeData shape,
            LeafHierarchyData hierarchy,
            BarkShapeData parentShape,
            BarkHierarchyData parentHierarchy
            
            )
        {
            using (BUILD_TIME.SHAPE_ARR.UpdateLeafOrigin.Auto())
            {
                var position = SplineModeler.GetPositionAtTime(parentShape.spline, shape.offset);
                var rotation = SplineModeler.GetRotationAtTime(parentShape.spline, shape.offset);
                var radius = SplineModeler.GetRadiusAtTime(parentShape, parentHierarchy, shape.offset);
                var surfaceAngle = SplineModeler.GetSurfaceAngleAtTime(parentShape, parentHierarchy, shape.offset);
                
                
                var angle = Quaternion.Euler(90.0f, shape.radialAngle, 0.0f);

                /*
                var perpendicular = GetPerpendicularRotation(hierarchy.geometry.perpendicularAlign, shape.offset);
                */
                
                
                var parentMatrix = parentShape.matrix;
                var baseMatrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                
                var radialAngleMatrix = Matrix4x4.TRS(Vector3.zero, angle, Vector3.one);

                var verticalAngleMatrix = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(shape.verticalAngle + surfaceAngle, 0.0f, 0.0f),
                    Vector3.one
                );

                /*var perpendicularMatrix = Matrix4x4.TRS(Vector3.zero, perpendicular, Vector3.one);*/

                shape.matrix = parentMatrix;
                shape.matrix *= baseMatrix;
                shape.matrix *= radialAngleMatrix;
                shape.matrix *= verticalAngleMatrix;

                var originalPosition = shape.matrix.GetColumn(3);
                
                var horizontalStrength = hierarchy.geometry.horizontalAlign.Value.EvaluateScaled(shape.offset);
                var rotationalStrength = 360.0f * hierarchy.geometry.rotationalAlign.Value.EvaluateScaled(shape.offset);

                var outwardDirection = 
                    (shape.matrix.MultiplyPoint(Vector3.zero) - 
                    parentShape.matrix.MultiplyPoint(Vector3.zero)).normalized;
                    
                var horizontalTarget = Quaternion.LookRotation(Vector3.down, outwardDirection);
                
                var rotationalAdjustment = Matrix4x4.Rotate(Quaternion.Euler(0.0f, rotationalStrength, 0.0f));

                var horizontalRotation = Quaternion.Slerp(
                    MathUtils.QuaternionFromMatrix(shape.matrix),
                    horizontalTarget,
                    horizontalStrength
                );
                
                var horizontalMatrix = Matrix4x4.TRS(Vector3.zero, horizontalRotation, Vector3.one);
                horizontalMatrix.SetColumn(3, originalPosition);
                
                shape.matrix = horizontalMatrix * rotationalAdjustment;
                
                var radiusOffset = shape.matrix.MultiplyPoint(new Vector3(0, radius + shape.effectiveScale, 0));

                var prefabScaleMatrix = Matrix4x4.Scale(Vector3.one);

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.Mesh)
                {
                    radiusOffset = shape.matrix.MultiplyPoint(new Vector3(0, radius, 0));
                    prefabScaleMatrix = Matrix4x4.Scale(Vector3.one * shape.effectiveScale);
                }

                shape.matrix *= prefabScaleMatrix;
                shape.matrix.m03 = radiusOffset.x;
                shape.matrix.m13 = radiusOffset.y;
                shape.matrix.m23 = radiusOffset.z;
            }
        }

        public static void UpdateAddOnShapeMatrix(
            ShapeData shape,
            HierarchyData hierarchy,
            ShapeData parentShape,
            HierarchyData parentHierarchy
        )
        {
            using (BUILD_TIME.SHAPE_ARR.UpdateAddOnShapeOrigin.Auto())
            {
                if (parentHierarchy is BarkHierarchyData barkParentHierarchy)
                {
                    var _t = Vector3.zero;
                    var _r = Quaternion.identity;
                    var _s = Vector3.one;
                    
                    var barkParentShape = parentShape as BarkShapeData;
                    var parentSpline = barkParentShape.spline;

                    var basePosition = SplineModeler.GetPositionAtTime(parentSpline, shape.offset);
                    var baseRotation = SplineModeler.GetRotationAtTime(parentSpline, shape.offset);
                    
                    var baseMatrix = Matrix4x4.TRS(basePosition, baseRotation, _s);

                    var radialAngleMatrix = Matrix4x4.TRS(_t, Quaternion.Euler(0.0f, shape.radialAngle, 0.0f), _s);

                    var surfaceAngle = SplineModeler.GetSurfaceAngleAtTime(
                        barkParentShape,
                        barkParentHierarchy,
                        shape.offset
                    );

                    var verticalAngleMatrix =
                        Matrix4x4.TRS(_t, Quaternion.Euler(shape.verticalAngle + surfaceAngle, 0.0f, 0.0f), _s);
                    
                    var radius = SplineModeler.GetRadiusAtTime(
                        barkParentShape,
                        barkParentHierarchy,
                        shape.offset
                    );
                    
                    var radiusOffset = Matrix4x4.TRS(new Vector3(0, 0, radius), _r, _s);
                    var effectiveScaleMatrix =  Matrix4x4.TRS(_t, _r,
                        new Vector3(shape.effectiveScale, shape.effectiveScale, shape.effectiveScale)
                    );

                    var positionOffset = Vector3.zero;
                    var rotationOffset = Quaternion.identity;
                    var perpendicular = Quaternion.identity;


                    if (hierarchy is FruitHierarchyData frh)
                    {
                        positionOffset = frh.geometry.prefab.positionOffset;
                        rotationOffset = frh.geometry.prefab.rotationOffset;

                        perpendicular = GetPerpendicularRotation(frh.geometry.perpendicularAlign);
                    }

                    if (hierarchy is FungusHierarchyData fgh)
                    {
                        positionOffset = fgh.geometry.prefab.positionOffset;
                        rotationOffset = fgh.geometry.prefab.rotationOffset;
                        
                        perpendicular = GetPerpendicularRotation(fgh.geometry.perpendicularAlign);
                    }

                    if (hierarchy is KnotHierarchyData khd)
                    {
                        positionOffset = khd.geometry.prefab.positionOffset;
                        rotationOffset = khd.geometry.prefab.rotationOffset;
                    }
                    
                    var offsetPositionMatrix =  Matrix4x4.TRS(positionOffset,_r, _s);
                    var offsetRotationMatrix =  Matrix4x4.TRS(_t, rotationOffset, _s);
                    var perpendicularnMatrix =  Matrix4x4.TRS(_t, perpendicular, _s);
                  
                    shape.matrix = parentShape.matrix;
                    shape.matrix *= baseMatrix;
                    shape.matrix *= radialAngleMatrix;
                    shape.matrix *= radiusOffset;
                    shape.matrix *= verticalAngleMatrix;
                    shape.matrix *= offsetPositionMatrix;
                    shape.matrix *= offsetRotationMatrix;
                    shape.matrix *= perpendicularnMatrix;
                    shape.matrix *= effectiveScaleMatrix;
                }
                else
                {
                    shape.matrix = parentShape.matrix *
                        Matrix4x4.Scale(new Vector3(shape.effectiveScale, shape.effectiveScale, shape.effectiveScale));
                }
            }
        }

        
        private static Quaternion GetPerpendicularRotation(floatCurveTree fct, /*ISeed seed,*/ float offset)
        {
            var perpendicular = fct.Value.EvaluateScaled(offset);
                
            var rx = /*(seed.RandomValue() - 0.5f) * */180.0f * (1.0f - perpendicular);
            var ry = /*(seed.RandomValue() - 0.5f) * */180.0f * (1.0f - perpendicular);
            var rz = 0.0f;
            
            return Quaternion.Euler(rx, ry, rz);
        }
        
        private static Quaternion GetPerpendicularRotation(floatTree ft/*, ISeed seed*/)
        {
            var rx = /*(seed.RandomValue() - 0.5f) **/ 180.0f * (1.0f - ft);
            var ry = /*(seed.RandomValue() - 0.5f) **/ 180.0f * (1.0f - ft);
            var rz = 0.0f;
            
            return Quaternion.Euler(rx, ry, rz);
        }
    }
}
