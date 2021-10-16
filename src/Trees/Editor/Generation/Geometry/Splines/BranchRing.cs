using System;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Hierarchy;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    [Serializable]
    public class BranchRing
    {
        public Vector2 uvOffset;
        public int hierarchyID;

        public Matrix4x4 matrix;
        [FormerlySerializedAs("noiseScale")] public float noiseStrength;
        public float noiseScaleU = 1.0f;
        public float noiseScaleV = 1.0f;
        public float maxNoise;
        public float radius;
        public float offset;
        public int segments;
        public int shapeID;
        public float surfaceAngle;
        public float surfaceAngleCosine;
        public float surfaceAngleSine;
        public TreeComponentType type;
        public int vertexStart;
        public int vertexEnd;
        public int capVertexCenter;
        public int capVertexStart;
        public int capVertexEnd;
        public float weldBottom;
        public float weldTop;
        
        [FormerlySerializedAs("flareNoise")] public float flareNoiseStrength;
        public float flareNoiseScaleU = 1.0f;
        public float flareNoiseScaleV = 1.0f;
        public float flareNoiseDepth;
        public float flareRadius;

        public BranchRing(
            float radius,
            Matrix4x4 matrix,
            Vector2 uvOffset,
            int segments,
            float surfaceAngle,
            float weldTop,
            float weldBottom,
            float noiseStrength,
            float noiseScaleU,
            float noiseScaleV,
            float flareRadius,
            float flareNoiseStrength,
            float flareNoiseScaleU,
            float flareNoiseScaleV,
            float flareNoiseDepth,
            int hierarchyID,
            int shapeID,
            TreeComponentType type)
        {
            this.radius = radius;
            this.matrix = matrix;
            this.segments = segments;
            this.uvOffset = uvOffset;
            vertexStart = 0;
            vertexEnd = 0;
            this.surfaceAngle = surfaceAngle;
            surfaceAngleCosine = Mathf.Cos(surfaceAngle * Mathf.Deg2Rad);
            surfaceAngleSine = -Mathf.Sin(surfaceAngle * Mathf.Deg2Rad);
            this.weldTop = weldTop;
            this.weldBottom = weldBottom;
            this.noiseStrength = noiseStrength;
            this.noiseScaleU = noiseScaleU;
            this.noiseScaleV = noiseScaleV;
            this.flareRadius = flareRadius;
            this.flareNoiseStrength = flareNoiseStrength;
            this.flareNoiseScaleU = flareNoiseScaleU;
            this.flareNoiseScaleV = flareNoiseScaleV;
            this.flareNoiseDepth = flareNoiseDepth;
            this.hierarchyID = hierarchyID;
            this.shapeID = shapeID;
            this.type = type;
        }

        public BranchRing Clone()
        {
            using (BUILD_TIME.BRANCH_RING.Clone.Auto())
            {
                var r2 = new BranchRing(
                    radius,
                    matrix,
                    uvOffset,
                    segments,
                    surfaceAngle,
                    weldTop,
                    weldBottom,
                    noiseStrength,
                    noiseScaleU,
                    noiseScaleV,
                    flareRadius,
                    flareNoiseStrength,
                    flareNoiseScaleU,
                    flareNoiseScaleV,
                    flareNoiseDepth,
                    hierarchyID,
                    shapeID,
                    type
                );

                r2.maxNoise = maxNoise;

                return r2;
            }
        }

        public void BuildVertices(
            LODGenerationOutput output,
            ShapeData shape,
            HierarchyData hierarchy,
            float heightOffset)
        {
            using (BUILD_TIME.BRANCH_RING.BuildVertices.Auto())
            {
                offset = heightOffset;
                vertexStart = output.vertices.Count;

                maxNoise = 0f;

                for (var i = 0; i <= segments; i++)
                {
                    var a = (i * Mathf.PI * 2.0f) / segments;

                    var v = new TreeVertex();
                    v.Set(shape, heightOffset);
                    
                    var baseRadius = radius;
                    
                    var spreadFactor = Mathf.Cos(a);
                    var spreadRadius = 0.0f;
                    
                    if (spreadFactor > 0.0f)
                    {
                        spreadRadius = Mathf.Pow(spreadFactor, 3.0f) * radius * weldBottom;
                    }
                    else
                    {
                        spreadRadius = Mathf.Pow(Mathf.Abs(spreadFactor), 3.0f) * radius * weldTop;
                    }

                    var uvCoordX = uvOffset.x - ((float) i / segments);
                    var uvCoordY = uvOffset.y;
                    
                    if (i == segments)
                    {
                        uvCoordX = 1.0f;
                    }
                    
                    var noise = hierarchy.seed.Noise2(uvCoordX, uvCoordY, noiseScaleU, noiseScaleV);
                    
                    var noiseAdditive = baseRadius * noise * noiseStrength;

                    var flareNoise = hierarchy.seed.Noise2(uvCoordX, uvCoordY, flareNoiseScaleU, flareNoiseScaleV);
                    flareNoise -= flareNoiseDepth;

                    var trunkAdditive = flareRadius + (flareRadius * flareNoise * flareNoiseStrength);
                    
                    v.noise = Mathf.Min(maxNoise, noiseAdditive+trunkAdditive);
                    
                    v.position = matrix.MultiplyPoint(
                        new Vector3(
                            Mathf.Sin(a) * (baseRadius + noiseAdditive + trunkAdditive + (spreadRadius * 0.25f)),
                            0.0f,
                            Mathf.Cos(a) * (baseRadius + noiseAdditive + trunkAdditive + spreadRadius)
                        )
                    );

                    v.raw_uv0 = new Vector2(uvCoordX, uvCoordY);
                    v.log.bark = 1f;

                    output.AddVertex(v);
                }

                if (radius == 0.0f)
                {
                    for (var i = 0; i <= segments; i++)
                    {
                        var v = output.vertices[i + vertexStart];
                        var a = (i * Mathf.PI * 2.0f) / segments;
                        var b = a - (Mathf.PI * 0.5f);

                        var normal = Vector3.zero;
                        normal.x = Mathf.Sin(a) * surfaceAngleCosine;
                        normal.y = surfaceAngleSine;
                        normal.z = Mathf.Cos(a) * surfaceAngleCosine;

                        v.normal = Vector3.Normalize(matrix.MultiplyVector(normal));

                        var tangent = Vector3.zero;
                        tangent.x = Mathf.Sin(b);
                        tangent.y = 0.0f;
                        tangent.z = Mathf.Cos(b);

                        tangent = Vector3.Normalize(matrix.MultiplyVector(tangent));
                        v.tangent = new Vector4(tangent.x, tangent.y, tangent.z, -1.0f);
                    }

                    vertexEnd = output.vertices.Count;
                    return;
                }

                var matrixInv = matrix.inverse;
                for (var vertexIndex = 0; vertexIndex <= segments; vertexIndex++)
                {
                    var previousVertexIndex = vertexIndex - 1;
                    if (previousVertexIndex < 0)
                    {
                        previousVertexIndex = segments - 1;
                    }

                    var nextVertexIndex = vertexIndex + 1;
                    if (nextVertexIndex > segments)
                    {
                        nextVertexIndex = 1;
                    }

                    var previousVertex = output.vertices[previousVertexIndex + vertexStart];
                    var nextVertex = output.vertices[nextVertexIndex + vertexStart];
                    var thisVertex = output.vertices[vertexIndex + vertexStart];

                    var tangent = Vector3.Normalize(previousVertex.position - nextVertex.position);
                    var normal = matrixInv.MultiplyVector(previousVertex.position - nextVertex.position);

                    normal.y = normal.x;
                    normal.x = normal.z;
                    normal.z = -normal.y;
                    normal.y = 0.0f;
                    normal.Normalize();

                    normal.x = surfaceAngleCosine * normal.x;
                    normal.y = surfaceAngleSine;
                    normal.z = surfaceAngleCosine * normal.z;

                    thisVertex.normal = Vector3.Normalize(matrix.MultiplyVector(normal));

                    thisVertex.tangent.x = tangent.x;
                    thisVertex.tangent.y = tangent.y;
                    thisVertex.tangent.z = tangent.z;
                    thisVertex.tangent.w = -1.0f;
                }

                vertexEnd = output.vertices.Count;
            }
        }


        public void FinishBrokenBranch(
            LODGenerationOutput output,
            HierarchyData hierarchy,
            ShapeData shape,
            Vector3 breakNormal,
            float sphereFactor,
            float noise,
            float ns,
            //LimbBreakUVMappingStyle style,
            //TreeMaterialUsage materialUsage,
            //float mappingScale,
            int materialID,
            float uvRange,
            float heightOffset,
            bool flip,
            float beaverFactor,
            float ringResolution = 1.0f,
            float segmentResolution = 1.0f)
        {
            using (BUILD_TIME.BRANCH_RING.FinishBrokenBranch.Auto())
            {
                var capSegments = (int) (segments * segmentResolution);
                
                var rings = Mathf.Max(1, (int) ((capSegments / 2f) * Mathf.Clamp01(sphereFactor)));
                var edgeRings = (int) (rings * ringResolution);
                var uvAdjustment = .5f * (1 - uvRange);

                /*if (style == LimbBreakUVMappingStyle.FollowBranch)
                {
                    capSegments += 1;
                    edgeRings += 1;
                    mappingScale /= Mathf.Max(1.0f, sphereFactor);
                }*/


                var rootPosition = matrix.MultiplyPoint(Vector3.zero);

                var centralVertex = new TreeVertex() /*TreeVertex.Get()*/;
                centralVertex.Set(shape, heightOffset);
                centralVertex.normal = breakNormal;
                centralVertex.tangent = matrix.MultiplyVector(Vector3.right);

                centralVertex.position = rootPosition + (centralVertex.normal * (beaverFactor * radius));

                //if (style == LimbBreakUVMappingStyle.EndCap)
                {
                    centralVertex.raw_uv0 = new Vector2(0.5f, 0.5f);
                }
                /*else
                {
                    centralVertex.raw_uv0 = new Vector2(0.5f, uvOffset.y + (sphereFactor * mappingScale));
                }*/

                capVertexCenter = output.vertices.Count;
                output.AddVertex(centralVertex);

                capVertexStart = capVertexCenter+1;

                var inverseMatrix = matrix.inverse;

                for (var loopIndex = 0; loopIndex < edgeRings; loopIndex++)
                {
                    var stepAngle = (1.0f - ((float) loopIndex / rings)) * Mathf.PI * 0.5f;

                    var uvScale = Mathf.Sin(stepAngle);
                    var positionBlending = uvScale;
                    var normalBlending = (uvScale * Mathf.Clamp01(sphereFactor)) +
                        (uvScale * 0.5f * Mathf.Clamp01(1.0f - sphereFactor));

                    var stepCosine = Mathf.Cos(stepAngle);

                    for (var i = 0; i < capSegments; i++)
                    {
                        var vertex = output.vertices[vertexStart + i];

                        var uv = inverseMatrix.MultiplyPoint(vertex.position).normalized * (0.5f * uvScale);

                        uv.x += .5f;
                        uv.z += .5f;

                        var newVertex = new TreeVertex() /*TreeVertex.Get()*/;
                        newVertex.Set(shape, heightOffset);

                        newVertex.log.woodDarkening = Mathf.Clamp01(1f - (loopIndex * .5f));
                
                        newVertex.position = (vertex.position * positionBlending) +
                            (rootPosition * (1.0f - positionBlending)) +
                            (breakNormal * (stepCosine * sphereFactor * radius));

                        newVertex.normal = ((vertex.normal * normalBlending) + (breakNormal * (1.0f - normalBlending)))
                            .normalized;

                        newVertex.tangent = vertex.tangent;

                        //if (style == LimbBreakUVMappingStyle.EndCap)
                        {
                            var uvXAdjustment = (uv.x - .5f) * 2f * uvAdjustment;
                            var uvYAdjustment = (uv.z - .5f) * 2f * uvAdjustment;

                            //newVertex.tangent = centralVertex.tangent;
                            newVertex.raw_uv0 = new Vector2(uv.x - uvXAdjustment, uv.z - uvYAdjustment);
                        }
                        /*else
                        {
                            //newVertex.tangent = vertex.tangent;
                            newVertex.raw_uv0 = new Vector2(
                                (float) i / segments,
                                uvOffset.y + (sphereFactor * stepCosine * mappingScale)
                            );
                        }*/

                        output.AddVertex(newVertex);
                    }
                }

                capVertexEnd = output.vertices.Count;

                for (var index = capVertexCenter; index < capVertexEnd; index++)
                {
                    var outputVertex = output.vertices[index];
                    var perlinX = outputVertex.position.x;
                    var perlinY = outputVertex.position.z;
                    
                    var push = radius * hierarchy.seed.Noise2(perlinX, perlinY, ns) * noise;

                    outputVertex.position += centralVertex.normal * push;
                    output.vertices[index] = outputVertex;
                }

                for (var index = vertexStart; index < vertexEnd; index++)
                {
                    var capVertex = capVertexStart + (index - vertexStart);

                    if (index == (vertexEnd - 1))
                    {
                        capVertex = capVertexStart;
                    }

                    var outputVertex = output.vertices[index];
                    outputVertex.position = output.vertices[capVertex].position;
                    output.vertices[index] = outputVertex;
                }

                for (var i = 0; i < capSegments; i++)
                {
                    var capOffset = capVertexStart + (capSegments * (edgeRings - 1));
                    
                    var v1 = i + capOffset;
                    var v2 = i == (capSegments - 1) ? capOffset : v1 + 1;
                    
                    var t = new TreeTriangle();
                    t.Set(shape, materialID, capVertexCenter, v1, v2, TreeMaterialUsage.SplineBreak, flip);

                    output.AddTriangle(t);
                }

                for (var loopIndex = 0; loopIndex < (edgeRings - 1); loopIndex++)
                {
                    for (var i = 0; i < capSegments; i++)
                    {
                        var outerRingOffset = (capSegments * loopIndex);
                        var innerRingOffset = (capSegments * (loopIndex + 1));
                        
                        var outerRingSegmentOffset = capVertexStart + outerRingOffset;
                        var innerRingSegmentOffset = capVertexStart + innerRingOffset;

                        var isLastSegment = i == (capSegments - 1);
                        
                        var v0 = i + outerRingSegmentOffset;
                        var v1 = isLastSegment ? outerRingSegmentOffset : v0 + 1;
                        var v2 = i + innerRingSegmentOffset;
                        var v3 = isLastSegment ? innerRingSegmentOffset : v2 + 1;

                        var t1 = new TreeTriangle();
                        var t2 = new TreeTriangle();

                        t1.Set(shape, materialID, v0, v1, v3, TreeMaterialUsage.SplineBreak, flip);
                        t2.Set(shape, materialID, v0, v3, v2, TreeMaterialUsage.SplineBreak, flip);
                    
                        output.AddTriangle(t1);
                        output.AddTriangle(t2);
                        
                    }
                }

                /*var range = vertexEnd - vertexStart;
                for(var i = 0; i <= range; i++)
                {
                    var vertexIndex = i + vertexStart;
                    var capVertexIndex = i + capVertexStart;

                    output.vertices[capVertexIndex].position = output.vertices[vertexIndex].position;
                    output.vertices[capVertexIndex].normal = output.vertices[vertexIndex].normal;
                    output.vertices[capVertexIndex].tangent = output.vertices[vertexIndex].tangent;
                }*/
            }
        }

        public void ConnectLoops(
            BranchRing other,
            LODGenerationOutput output,
            ShapeData shape,
            int materialID,
            bool reverseTriangleOrder)
        {
            using (BUILD_TIME.BRANCH_RING.ConnectLoops.Auto())
            {
                if (other.segments > segments)
                {
                    other.ConnectLoops(this, output, shape, materialID, true);
                    return;
                }

                for (var i = 0; i < segments; i++)
                {
                    var connect0 = Mathf.Min(
                        (int) Mathf.Round((i / (float) segments) * other.segments),
                        other.segments
                    );
                    var connect1 = Mathf.Min(
                        (int) Mathf.Round(((i + 1) / (float) segments) * other.segments),
                        other.segments
                    );

                    var k = Mathf.Min(i + 1, segments);

                    var v0 = connect0 + other.vertexStart;
                    var v1 = i + vertexStart;
                    var v2 = connect1 + other.vertexStart;

                    var v3 = i + vertexStart;
                    var v4 = k + vertexStart;
                    var v5 = connect1 + other.vertexStart;

                    if (reverseTriangleOrder)
                    {
                        var temp = v1;
                        v1 = v0;
                        v0 = temp;

                        temp = v4;
                        v4 = v3;
                        v3 = temp;
                    }

                    var t1 = new TreeTriangle();
                    var t2 = new TreeTriangle();

                    t1.Set(shape, materialID, v0, v1, v2, TreeMaterialUsage.Bark, false);
                    t2.Set(shape, materialID, v3, v4, v5, TreeMaterialUsage.Bark, false);

                    output.AddTriangle(t1);
                    output.AddTriangle(t2);
                }
            }
        }
        
        public void CloseRing(
            LODGenerationOutput output)
        {
            using (BUILD_TIME.BRANCH_RING.CloseRings.Auto())
            {
                var vertexStartVertex = output.vertices[vertexStart];
                var vertexEndVertex = output.vertices[vertexEnd - 1];
                
                vertexStartVertex.normal = vertexEndVertex.normal;
                vertexStartVertex.position = vertexEndVertex.position;
                vertexStartVertex.tangent = vertexEndVertex.tangent;
                vertexStartVertex.billboardData = vertexEndVertex.billboardData;
                vertexStartVertex.billboard = vertexEndVertex.billboard;
                vertexStartVertex.variation = vertexEndVertex.variation;
                vertexStartVertex.rawWind = vertexEndVertex.rawWind;
                vertexStartVertex.wind = vertexEndVertex.wind;
                vertexStartVertex.ambientOcclusion = vertexEndVertex.ambientOcclusion;
                vertexStartVertex.matrix = vertexEndVertex.matrix;
                vertexStartVertex.weldTo = vertexEnd-1;
                vertexEndVertex.weldTo = vertexStart;
                
                output.vertices[vertexStart] = vertexStartVertex;
                output.vertices[vertexEnd - 1] = vertexEndVertex;
            }
        }
    }
}
