using System.Collections.Generic;
using Appalachia.Core.Math.Geometry;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Meshing;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry
{
    public static class ShapeWelder
    {
        public static Matrix4x4 WeldShapeOriginToParent(
            LODGenerationOutput output,
            ShapeData shape,
            ShapeData parentShape,
            Matrix4x4 matrix,
            float maxDistance)
        {
            using (BUILD_TIME.SHAPE_WELDR.WeldShapeOriginToParent.Auto())
            {
                if (parentShape == null)
                {
                    return matrix;
                }

                if ((parentShape.type != TreeComponentType.Trunk) &&
                    (parentShape.type != TreeComponentType.Branch) &&
                    (parentShape.type != TreeComponentType.Root)
                    )
                {
                    return matrix;
                }

                var vertices = output.vertices;
                var triangles = output.triangles;
                var pgeo = parentShape.geometry[output.lodLevel];
                var origin = matrix.MultiplyPoint(Vector3.zero);

                if (pgeo.modelVertexStart == pgeo.modelVertexEnd)
                {
                    return matrix;
                }

                var originalMatrix = matrix;
                
                var parentMatrix = parentShape.matrix;
                var parentEffectiveMatrix = parentShape.effectiveMatrix;

                var difference = parentEffectiveMatrix.MultiplyPoint(Vector3.zero) -
                    parentMatrix.MultiplyPoint(Vector3.zero);

                matrix *= Matrix4x4.Translate(difference);

                var inwardsDirection = matrix.MultiplyVector(-Vector3.up);
       
                if ((shape.type == TreeComponentType.Fungus) ||
                    (shape.type == TreeComponentType.Knot) ||
                    (shape.type == TreeComponentType.Fruit))
                {
                    inwardsDirection = matrix.MultiplyVector(-Vector3.forward);
                }

                if (GetNearestPointIntersectingRay(origin, inwardsDirection, vertices, triangles, pgeo, maxDistance, WeldFaceSide.Front, out var bestHit))
                {
                    matrix.SetTRS(bestHit.hitPoint, matrix.rotation, matrix.lossyScale);
                    return matrix;
                }

                if (GetNearestPointIntersectingRay(origin, -inwardsDirection, vertices, triangles, pgeo, maxDistance, WeldFaceSide.Back, out bestHit))
                {
                    matrix.SetTRS(bestHit.hitPoint, matrix.rotation, matrix.lossyScale);
                    return matrix;
                }
                
                if (GetNearestPointOnMesh(origin, vertices, triangles, pgeo, maxDistance, out var bestTri))
                {
                    matrix.SetTRS(bestTri.hitPoint, matrix.rotation, matrix.lossyScale);
                    return matrix;
                }

                if (GetNearestPointOnMesh(origin, vertices, triangles, pgeo, maxDistance*100, out var bestTri2))
                {
                    var direction = (bestTri2.hitPoint - origin).normalized;

                    var similarity = Vector3.Dot(direction, inwardsDirection);

                    if (similarity > 0)
                    {
                        if (GetNearestPointIntersectingRay(origin, direction, vertices, triangles, pgeo, maxDistance*maxDistance, WeldFaceSide.Front, out var goodHit))
                        {
                            matrix.SetTRS(goodHit.hitPoint, matrix.rotation, matrix.lossyScale);
                            return matrix;
                        }
                    }
                    else
                    {
                        if (GetNearestPointIntersectingRay(origin, -direction, vertices, triangles, pgeo, maxDistance*maxDistance, WeldFaceSide.Back, out var goodHit))
                        {
                            matrix.SetTRS(goodHit.hitPoint, matrix.rotation, matrix.lossyScale);
                            return matrix;
                        }
                    }

                    matrix.SetTRS(bestTri2.hitPoint, matrix.rotation, matrix.lossyScale);
                    return matrix;
                }
                
                return matrix;
            }
        }
        
        public static Matrix4x4 WeldLeafShapeOriginToParent(
            LODGenerationOutput output,
            ShapeData parentShape,
            Matrix4x4 matrix)
        {
            using (BUILD_TIME.SHAPE_WELDR.WeldShapeOriginToParent.Auto())
            {
                if (parentShape == null)
                {
                    return matrix;
                }

                if ((parentShape.type != TreeComponentType.Trunk) &&
                    (parentShape.type != TreeComponentType.Branch) &&
                    (parentShape.type != TreeComponentType.Root)
                    )
                {
                    return matrix;
                }

                var pgeo = parentShape.geometry[output.lodLevel];

                if (pgeo.modelVertexStart == pgeo.modelVertexEnd)
                {
                    return matrix;
                }
                
                var parent_position_original = parentShape.matrix.MultiplyPoint(Vector3.zero);
                var parent_position_updated = parentShape.effectiveMatrix.MultiplyPoint(Vector3.zero);

                var local_ppo = matrix.inverse.MultiplyPoint(parent_position_original);
                var local_ppu = matrix.inverse.MultiplyPoint(parent_position_updated);

                var difference = local_ppu - local_ppo;

                return matrix * Matrix4x4.Translate(difference);
            }
        }

        
         public static void WeldGeometryToParent(
            LODGenerationOutput output,
            ShapeData shape,
            ShapeData parentShape,
            int vertexStart,
            int vertexEnd,            
            Matrix4x4 matrix,
            float maxDistance,
            bool updatePosition,
            bool updateNormal
            )
        {
            var pgeo = parentShape.geometry[output.lodLevel];
            var verts = output.vertices;
            var tris = output.triangles;


            if (shape != null)
            {
                var ray = new Ray();

                for (var v = vertexStart; v < vertexEnd; v++)
                {
                    var vertex = verts[v];

                    var anyHit = false;

                    ray.origin = vertex.position;
                    ray.direction = matrix.MultiplyVector(Vector3.up);

                    var minDist = -10000.0f;
                    var maxDist = 100000.0f;

                    // project vertices onto parent
                    for (var tri = pgeo.modelTriangleStart; tri < pgeo.modelTriangleEnd; tri++)
                    {
                        var pv0 = verts[tris[tri].v[0]];
                        var pv1 = verts[tris[tri].v[1]];
                        var pv2 = verts[tris[tri].v[2]];

                        var hit = MathUtils.IntersectRayTriangle(ray, pv2.position, pv1.position, pv0.position, false);

                        if (hit != null)
                        {
                            var rayHit = (RaycastHit) hit;
                            if ((Mathf.Abs(rayHit.distance) < maxDist) && (rayHit.distance > minDist))
                            {
                                anyHit = true;
                                maxDist = Mathf.Abs(rayHit.distance);

                                if (updateNormal)
                                {
                                    vertex.normal = rayHit.normal;
                                }

                                if (updatePosition)
                                {
                                    vertex.position = rayHit.point;
                                }
                            }
                        }
                    }

                    if (anyHit)
                    {
                        continue;
                    }

                    // project vertices onto parent

                    ray.direction = matrix.MultiplyVector(-Vector3.up);

                    for (var tri = pgeo.modelTriangleStart; tri < pgeo.modelTriangleEnd; tri++)
                    {
                        var pv0 = verts[tris[tri].v[0]];
                        var pv1 = verts[tris[tri].v[1]];
                        var pv2 = verts[tris[tri].v[2]];

                        var hit = MathUtils.IntersectRayTriangle(ray, pv0.position, pv1.position, pv2.position, false);

                        if (hit != null)
                        {
                            var rayHit = (RaycastHit) hit;
                            if ((Mathf.Abs(rayHit.distance) < maxDist) && (rayHit.distance > minDist))
                            {
                                anyHit = true;
                                maxDist = Mathf.Abs(rayHit.distance);

                                if (updateNormal)
                                {
                                    vertex.normal = rayHit.normal;
                                }

                                if (updatePosition)
                                {
                                    vertex.position = rayHit.point;
                                }
                            }
                        }
                    }

                    if (anyHit)
                    {
                        continue;
                    }

                    if (GetNearestPointOnMesh(vertex.position, verts, tris, pgeo, maxDistance, out var bestTri))
                    {
                        if (updateNormal)
                        {
                            vertex.normal = bestTri.hitNormal;
                        }

                        if (updatePosition)
                        {
                            vertex.position = bestTri.hitPoint;
                        }

                        continue;
                    }

                    if (GetNearestPointOnMesh(vertex.position, verts, tris, pgeo, maxDistance * 100, out var bestTri2))
                    {
                        var direction = (bestTri2.hitPoint - vertex.position).normalized;
                        var inwardsDirection = matrix.MultiplyVector(-Vector3.up);

                        var similarity = Vector3.Dot(direction, inwardsDirection);

                        if (similarity > 0)
                        {
                            if (GetNearestPointIntersectingRay(
                                vertex.position,
                                direction,
                                verts,
                                tris,
                                pgeo,
                                maxDistance * maxDistance,
                                WeldFaceSide.Front,
                                out var goodHit
                            ))
                            {
                                if (updateNormal)
                                {
                                    vertex.normal = goodHit.normal;
                                }

                                if (updatePosition)
                                {
                                    vertex.position = goodHit.hitPoint;
                                }
                            }
                        }
                        else
                        {
                            if (GetNearestPointIntersectingRay(
                                vertex.position,
                                -direction,
                                verts,
                                tris,
                                pgeo,
                                maxDistance * maxDistance,
                                WeldFaceSide.Back,
                                out var goodHit
                            ))
                            {
                                if (updateNormal)
                                {
                                    vertex.normal = goodHit.normal;
                                }

                                if (updatePosition)
                                {
                                    vertex.position = goodHit.hitPoint;
                                }
                            }
                        }
                    }
                }
            }
        }
         
        private class TreeTriangleHit
        {
            public Vector3 hitPoint;
            public Vector3 hitNormal;
            public TreeVertex[] vertices;
            public TreeTriangle triangle;
            public Vector3 barycentric;
        }
        
        private static bool GetNearestPointOnMesh(
            Vector3 origin,
            IList<TreeVertex> vertices,
            IList<TreeTriangle> triangles,
            ShapeGeometryData pgeo,
            float maxDistance,
            out TreeTriangleHit bestHit)
        {
            var pv = new TreeVertex[3];

            bestHit = default;
            var found = false;

            var maxDist = maxDistance * maxDistance;

            for (var tri = pgeo.modelTriangleStart; tri < pgeo.modelTriangleEnd; tri++)
            {
                for (var i = 0; i < 3; i++)
                {
                    pv[i] = vertices[triangles[tri].v[i]];
                }

                var center = (pv[0].position + pv[1].position + pv[2].position) / 3f;
                var edge_ab = pv[1].position - pv[0].position;
                var edge_ac = pv[2].position - pv[0].position;
                var normal = Vector3.Cross(edge_ab, edge_ac);

                /*var plane = new Plane(normal, center);*/
                var plane = new Plane(pv[0].position, pv[1].position, pv[2].position);

                Vector3 nearest = plane.ClosestPointOnPlane(origin);
                Vector3 nearestNormal = plane.normal;

                var bary = new Barycentric(pv[0].position, pv[1].position, pv[2].position, nearest);


                if (bary.IsInside)
                {
                    //nearest = nearest;
                }
                else if (bary.IsInsideUV && (bary.w > -0.5f))
                {
                   // nearest = nearest;
                }
                else //if (!bary.IsInside)
                {
                    var v0D = (pv[0].position - origin).sqrMagnitude;
                    var v1D = (pv[1].position - origin).sqrMagnitude;
                    var v2D = (pv[2].position - origin).sqrMagnitude;

                    if ((v0D < v1D) && (v0D < v2D))
                    {
                        nearest = pv[0].position;
                        nearestNormal = pv[0].normal;
                    }
                    else if ((v1D < v0D) && (v1D < v2D))
                    {
                        nearest = pv[1].position;
                        nearestNormal = pv[1].normal;
                    }
                    else
                    {
                        nearest = pv[2].position;
                        nearestNormal = pv[2].normal;
                    }
                }
    
                var dist = (nearest - origin).sqrMagnitude;
                if (dist < maxDist)
                {
                    maxDist = dist;
                    bestHit = new TreeTriangleHit()
                    {
                        barycentric = new Vector3(bary.u, bary.v, bary.w),
                        hitPoint = nearest,
                        hitNormal = nearestNormal,
                        triangle = triangles[tri],
                        vertices = new [] { pv[0],pv[1],pv[2]}
                    };
                    found = true;
                }
                else
                {
                    //maxDist = maxDist;
                }
            }

            return found;
        }
        
        public static TreeVertex GetNearestVertex(
            Vector3 origin,
            IReadOnlyList<TreeVertex> vertices,
            int start, int end)
        {
            TreeVertex nearest = default;
            var nearestDistance = 0f;            
            
            for (var i = start; i < end; i++)
            {
                var vertex = vertices[i];
                var distance = (vertex.position - origin).sqrMagnitude;
                
                if ((nearest == default) || (distance < nearestDistance))
                {                    
                    nearest = vertex;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }
        
        public static int GetMostSimilarByNormal(
            Vector3 origin,
            IList<TreeVertex> vertices,
            int start, int end)
        {
            int nearest = -1;
            var nearestDistance = float.MaxValue;            
            
            for (var i = start; i < end; i++)
            {
                var vertex = vertices[i];
                var distance = (vertex.normal - origin).sqrMagnitude;
                
                if (distance < nearestDistance)
                {                    
                    nearest = i;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }
        
        private class TreeRayHit
        {
            public Vector3 hitPoint;
            public Vector3 normal;
            public Vector3 barycentric;
        }

        private static bool GetNearestPointIntersectingRay(
            Vector3 origin,
            Vector3 direction,
            IList<TreeVertex> vertices,
            IList<TreeTriangle> triangles,
            ShapeGeometryData pgeo,
            float maxDistance,
            WeldFaceSide faceSide,
            out TreeRayHit bestHit)
        {
            var pv = new TreeVertex[3];

            bestHit = new TreeRayHit();
            var found = false;
            var bestHitFrontFace = false;
            var bestHitBackFace = false;

            var maxDist = maxDistance * maxDistance;

            for (var tri = pgeo.modelTriangleStart; tri < pgeo.modelTriangleEnd; tri++)
            {
                for (var i = 0; i < 3; i++)
                {
                    pv[i] = vertices[triangles[tri].v[i]];
                }

                var edge_ab = pv[1].position - pv[0].position;
                var edge_ac = pv[2].position - pv[0].position;
                var normal = Vector3.Cross(edge_ab, edge_ac);

                var dot = Vector3.Dot(direction, normal);

                var frontFaceHit = dot < 0.0f;
                var backfaceHit = dot > 0.0f;

                var plane = new Plane(pv[0].position, pv[1].position, pv[2].position);

                if (plane.Raycast(new Ray(origin, direction), out var distance))
                {
                    var hit = origin + (direction * distance);
                    var bary = new Barycentric(pv[0].position, pv[1].position, pv[2].position, hit);

                    if (bary.IsInside)
                    {
                        if (distance < maxDist)
                        {
                            maxDist = distance;
                            bestHit.hitPoint = hit;
                            bestHit.barycentric = new Vector3(bary.u, bary.v, bary.w);
                            bestHit.normal = plane.normal;
                            found = true;
                            bestHitFrontFace = frontFaceHit;
                            bestHitBackFace = backfaceHit;
                        }
                    }
                }
            }

            if (!found)
            {
                return false;
            }

            if ((faceSide == WeldFaceSide.Front) && !bestHitFrontFace)
            {
                return false;
            }
            if ((faceSide == WeldFaceSide.Back) && !bestHitBackFace)
            {
                return false;            
            }

            return true;
        }
    }
}
