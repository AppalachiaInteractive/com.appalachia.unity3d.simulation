using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Meshing
{
    public static class MeshGenerator
    {
        public static List<Material> GenerateMeshes(
            LODGenerationOutput output,
            Dictionary<int, ShapeData> shapeLookup,
            InputMaterialCache inputMaterials,
            List<OutputMaterial> materials,
            MeshSettings meshSettings,
            Mesh mesh, 
            Func<TreeVertex, Color> colorFunction,
            bool forceNoSubmesh,
            bool realGeometry,
            Bounds vertexBounds)
        {
            using (BUILD_TIME.MESH_GEN.GenerateMeshes.Auto())
            {
                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    vertex.visible = shapeLookup[vertex.shapeID].exportGeometry;
                    output.vertices[index] = vertex;
                }

                for (var index = 0; index < output.triangles.Count; index++)
                {
                    var triangle = output.triangles[index];
                    triangle.visible = shapeLookup[triangle.shapeID].exportGeometry;
                    output.triangles[index] = triangle;
                }

                var runtimeMaterials = CreateMeshFromData(
                    output,
                    shapeLookup,
                    inputMaterials,
                    materials,
                    meshSettings,
                    mesh,
                    colorFunction,
                    forceNoSubmesh,
                    realGeometry,
                    vertexBounds
                );
                
                mesh.UploadMeshData(false);

                return runtimeMaterials;
            }
        }

        private static List<Material> CreateMeshFromData(
            LODGenerationOutput output,
            Dictionary<int, ShapeData> shapeLookup,
            InputMaterialCache inputMaterials,
            IEnumerable<OutputMaterial> materials,
            MeshSettings meshSettings,
            Mesh mesh, 
            Func<TreeVertex, Color> colorFunction,
            bool forceNoSubmesh,
            bool realGeometry,
            Bounds vertexBounds)
        {
            using (BUILD_TIME.MESH_GEN.CreateMeshFromData.Auto())
            {
                
                var submeshMaterials = new List<Material>();
                var noRecomputeMeshIndices = new HashSet<int>();
                
                mesh.Clear();

                var positions = new List<Vector3>();
                var normals = new List<Vector3>();
                var tangents = new List<Vector4>();
                var colors = new List<Color>();
                var uv0 = new List<Vector4>();
                var uv2 = new List<Vector4>();
                var uv3 = new List<Vector4>();
                var uv4 = new List<Vector4>();
                var uv5 = new List<Vector4>();

                var vertexIndices = new Dictionary<int, int>();
                var actualVertexIndex = 0;

                var useBounds = (vertexBounds != null) && 
                    (vertexBounds != default) && 
                    (vertexBounds.size.magnitude > .01f);
                
                for (int i = 0; i < output.vertices.Count; i++)
                {
                    var vertex = output.vertices[i];

                    if (!vertex.visible)
                    {
                        continue;
                    }

                    if (useBounds)
                    {
                        if (!vertexBounds.Contains(vertex.position))
                        {
                            //vertex.position = vertexBounds.ClosestPoint(vertex.position);
                            continue;
                        }
                    }

                    if (vertex.weldTo > 0)
                    {
                        noRecomputeMeshIndices.Add(actualVertexIndex);
                    }
                    
                    vertexIndices.Add(i, actualVertexIndex);

                    if (shapeLookup[vertex.shapeID].geometry.Count <= output.lodLevel)
                    {
                        continue;
                    }
                    
                    if (realGeometry)
                    {                        
                        shapeLookup[vertex.shapeID].geometry[output.lodLevel].actualVertices.Add(actualVertexIndex);
                    }

                    positions.Add(vertex.position);
                    normals.Add(vertex.normal);
                    uv0.Add(vertex.UV0);
                    uv2.Add(vertex.UV2);
                    uv3.Add(vertex.UV3);
                    uv4.Add(vertex.UV4);
                    uv5.Add(vertex.UV5);
                    tangents.Add(vertex.tangent);
                    colors.Add(colorFunction(vertex));
                    
                    actualVertexIndex += 1;
                }

                mesh.SetVertices(positions);
                mesh.SetNormals(normals);
                mesh.SetTangents(tangents);
                mesh.SetColors(colors);
                mesh.SetUVs(0, uv0);
                mesh.SetUVs(1, uv2);
                mesh.SetUVs(2, uv3);
                mesh.SetUVs(3, uv4);
                mesh.SetUVs(4, uv5);

                List<int> triangleVertices = null;
                var submeshTriangles = new List<List<int>>();

                OutputMaterial[] mArray = null;

                if (materials != null)
                {
                    mArray = materials.ToArray(); 
                }                

                if (((mArray == null) || (mArray.Length == 0)) || forceNoSubmesh)
                {
                    if ((mArray == null) || (mArray.Length == 0))
                    {
                        submeshMaterials.Add(DefaultMaterialResource.instance.material);
                    }
                    else
                    {
                        var materialElement = mArray[mArray.Length - 1].GetMaterialElementForLOD(output.lodLevel);
                        submeshMaterials.Add(materialElement.asset);
                    }
                    
                    int triangleIndexOffset = 0;
                    
                    triangleVertices = new List<int>();

                    for (int j = 0; j < output.triangles.Count; j++)
                    {
                        var triangle = output.triangles[j];

                        if (!triangle.visible)
                        {
                            continue;
                        }

                        if (shapeLookup[triangle.shapeID].geometry.Count <= output.lodLevel)
                        {
                            continue;
                        }
                        
                        if (!vertexIndices.ContainsKey(triangle.v[0]) ||
                            !vertexIndices.ContainsKey(triangle.v[1]) ||
                            !vertexIndices.ContainsKey(triangle.v[2]))
                        {
                            continue;
                        }
                        
                        if (realGeometry)
                        {                            
                            shapeLookup[triangle.shapeID].geometry[output.lodLevel].actualTriangles.Add(triangleIndexOffset/3);
                        }

                        var v0 = vertexIndices[triangle.v[0]];
                        var v1 = vertexIndices[triangle.v[1]];
                        var v2 = vertexIndices[triangle.v[2]];

                        triangleVertices.Add(v0);
                        triangleVertices.Add(v1);
                        triangleVertices.Add(v2);
                        triangleIndexOffset += 3;

                        noRecomputeMeshIndices.Add(v0);
                        noRecomputeMeshIndices.Add(v1);
                        noRecomputeMeshIndices.Add(v2);
                    }

                    submeshTriangles.Add(triangleVertices);
                }
                else
                {
                    var fullTriangleIndexOffset = 0;
                    
                    foreach (var material in mArray)
                    {
                        int triangleIndexOffset = 0;
                        triangleVertices = new List<int>();

                        for (int j = 0; j < output.triangles.Count; j++)
                        {
                            var triangle = output.triangles[j];

                            if (!triangle.visible)
                            {
                                continue;
                            }
                                    
                            if (!vertexIndices.ContainsKey(triangle.v[0]) ||
                                !vertexIndices.ContainsKey(triangle.v[1]) ||
                                !vertexIndices.ContainsKey(triangle.v[2]))
                            {
                                continue;
                            }
                            
                            if (((triangle.context == TreeMaterialUsage.Bark) || (triangle.context == TreeMaterialUsage.SplineBreak)) &&
                                (material.MaterialContext == MaterialContext.TiledOutputMaterial))
                            {
                                var m = material as TiledOutputMaterial;

                                if (m.inputMaterialID == triangle.inputMaterialID)
                                {
                                    if (shapeLookup[triangle.shapeID].geometry.Count <= output.lodLevel)
                                    {
                                        continue;
                                    }
                                    
                                    if (realGeometry)
                                    {
                                        shapeLookup[triangle.shapeID].geometry[output.lodLevel].actualTriangles.Add(fullTriangleIndexOffset/3);
                                    }
                                    
                                    var v0 = vertexIndices[triangle.v[0]];
                                    var v1 = vertexIndices[triangle.v[1]];
                                    var v2 = vertexIndices[triangle.v[2]];
                                    
                                    triangleVertices.Add(v0);
                                    triangleVertices.Add(v1);
                                    triangleVertices.Add(v2);
                                    triangleIndexOffset += 3;
                                    fullTriangleIndexOffset += 3;

                                }
                            }
                            else if ((triangle.context != TreeMaterialUsage.Bark) &&
                                (triangle.context != TreeMaterialUsage.SplineBreak) &&
                                (material.MaterialContext == MaterialContext.AtlasOutputMaterial))
                            {
                                if (shapeLookup[triangle.shapeID].geometry.Count <= output.lodLevel)
                                {
                                    continue;
                                }

                                if (realGeometry)
                                {                                    
                                    shapeLookup[triangle.shapeID].geometry[output.lodLevel].actualTriangles.Add(fullTriangleIndexOffset/3);
                                }
                                
                                var v0 = vertexIndices[triangle.v[0]];
                                var v1 = vertexIndices[triangle.v[1]];
                                var v2 = vertexIndices[triangle.v[2]];

                                triangleVertices.Add(v0);
                                triangleVertices.Add(v1);
                                triangleVertices.Add(v2);
                                triangleIndexOffset += 3;
                                fullTriangleIndexOffset += 3;


                                var aim = inputMaterials.GetByMaterialID(
                                    triangle.inputMaterialID
                                ) as AtlasInputMaterial;

                                if ((triangle.context == TreeMaterialUsage.Billboard) ||
                                    ((aim != null) && aim.eligibleAsLeaf))
                                {
                                    noRecomputeMeshIndices.Add(v0);
                                    noRecomputeMeshIndices.Add(v1);
                                    noRecomputeMeshIndices.Add(v2);
                                }
                            }
                        }

                        if (triangleIndexOffset == 0)
                        {
                            continue;
                        }

                        submeshTriangles.Add(triangleVertices);
                        submeshMaterials.Add(material?.GetMaterialElementForLOD(output.lodLevel).asset);
                    }
                }

                mesh.subMeshCount = Mathf.Max(submeshMaterials.Count, 1);

                for (var i = 0; i < submeshMaterials.Count; i++)
                {
                    mesh.SetTriangles(submeshTriangles[i], i);
                }

                if (!meshSettings.recalculateNormals && !meshSettings.recalculateTangents)
                {
                    mesh.RecalculateBounds();

                    return submeshMaterials;
                }
                
                if (meshSettings.recalculateNormals)
                {
                    mesh.RecalculateNormals(
                        noRecomputeMeshIndices,
                        meshSettings.hardEdgeAngle, 
                        meshSettings.groupingScale);
                }

                if (meshSettings.recalculateTangents)
                {
                    mesh.RecalculateTangents();
                }

                for(var i = 0; i < output.vertices.Count; i++)
                {
                    if (!vertexIndices.ContainsKey(i))
                    {
                        continue;
                    }

                    var vertex = output.vertices[i];

                    if (vertex.weldTo < 0)
                    {
                        continue;
                    }
                    
                    var updatedIndex = vertexIndices[i];
                    var updatedWeldToIndex = vertexIndices[vertex.weldTo];

                    var position = mesh.vertices[updatedIndex];
                    var normal = mesh.normals[updatedIndex];
                    var tangent = mesh.tangents[updatedIndex];

                    var updatedPosition = mesh.vertices[updatedWeldToIndex];
                    var updatedNormal = mesh.normals[updatedWeldToIndex];
                    var updatedTangent = mesh.tangents[updatedWeldToIndex];

                    mesh.vertices[updatedIndex] = updatedPosition;
                    mesh.normals[updatedIndex] = updatedNormal;
                    mesh.tangents[updatedIndex] = updatedTangent;
                }

                /*
                foreach (var noRecomputeIndex in noRecomputeMeshIndices)
                {
                    mesh.normals[noRecomputeIndex] = normals[noRecomputeIndex];
                    mesh.tangents[noRecomputeIndex] = tangents[noRecomputeIndex];
                }*/

                mesh.RecalculateBounds();

                return submeshMaterials;
            }
        }
        
        public static void RecalculateNormals(
            this Mesh mesh, 
            HashSet<int> ignoredTriangles,
            float angleLimitDegrees,
            int groupingScale)
        {
            using (BUILD_TIME.MESH_GEN.RecalculateNormals.Auto())
            {
                var angleLimit = Mathf.Cos(angleLimitDegrees * Mathf.Deg2Rad);

                var vertices = mesh.vertices;
                var normals = mesh.normals;


                for (var submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
                {
                    var dictionary = new Dictionary<vkey, List<VertData>>(vertices.Length);

                    var triangles = mesh.GetTriangles(submeshIndex);

                    var triNormals = new Vector3[triangles.Length / 3];

                    for (var triI = 0; triI < triangles.Length; triI += 3)
                    {
                        int i1 = triangles[triI];
                        int i2 = triangles[triI + 1];
                        int i3 = triangles[triI + 2];
                        
                        if (ignoredTriangles.Contains(i1) || 
                            ignoredTriangles.Contains(i2) ||
                            ignoredTriangles.Contains(i3) )
                        {
                            continue;
                        }

                        Vector3 p1 = vertices[i2] - vertices[i1];
                        Vector3 p2 = vertices[i3] - vertices[i1];
                        Vector3 normal = Vector3.Cross(p1, p2).normalized;
                        int triIndex = triI / 3;
                        triNormals[triIndex] = normal;

                        List<VertData> entry;
                        vkey key;

                        if (!dictionary.TryGetValue(key = new vkey(vertices[i1], groupingScale), out entry))
                        {
                            entry = new List<VertData>(4);
                            dictionary.Add(key, entry);
                        }

                        entry.Add(new VertData(triIndex, i1));

                        if (!dictionary.TryGetValue(key = new vkey(vertices[i2], groupingScale), out entry))
                        {
                            entry = new List<VertData>();
                            dictionary.Add(key, entry);
                        }

                        entry.Add(new VertData(triIndex, i2));

                        if (!dictionary.TryGetValue(key = new vkey(vertices[i3], groupingScale), out entry))
                        {
                            entry = new List<VertData>();
                            dictionary.Add(key, entry);
                        }

                        entry.Add(new VertData(triIndex, i3));
                    }

                    foreach (var vertList in dictionary.Values)
                    {
                        for (var i = 0; i < vertList.Count; ++i)
                        {
                            var vertex = vertList[i];

                            var thisNormal = triNormals[vertex.tri];

                            var normalSum = new Vector3();

                            for (var j = 0; j < vertList.Count; ++j)
                            {
                                var otherVertex = vertList[j];
                                var otherNormal = triNormals[otherVertex.tri];

                                if (vertex.vert == otherVertex.vert)
                                {
                                    normalSum += otherNormal;
                                }
                                else
                                {
                                    var dot = Vector3.Dot(thisNormal, otherNormal);
                                    if (dot >= angleLimit)
                                    {
                                        normalSum += otherNormal;
                                    }
                                }
                            }

                            normals[vertex.vert] = normalSum.normalized;
                        }
                    }
                }
                
                mesh.normals = normals;
            }
        }

        private struct vkey : IEquatable<vkey>
        {
            [DebuggerStepThrough] public bool Equals(vkey other)
            {
                return (x == other.x) && (y == other.y) && (z == other.z);
            }

            [DebuggerStepThrough] public override bool Equals(object obj)
            {
                return obj is vkey other && Equals(other);
            }

            [DebuggerStepThrough] public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = x.GetHashCode();
                    hashCode = (hashCode * 397) ^ y.GetHashCode();
                    hashCode = (hashCode * 397) ^ z.GetHashCode();
                    return hashCode;
                }
            }

            [DebuggerStepThrough] public static bool operator ==(vkey left, vkey right)
            {
                return left.Equals(right);
            }

            [DebuggerStepThrough] public static bool operator !=(vkey left, vkey right)
            {
                return !left.Equals(right);
            }

            private readonly long x;
            private readonly long y;
            private readonly long z;

            public vkey(Vector3 position, int groupScale)
            {
                x = (long) (Mathf.Round(position.x * groupScale));
                y = (long) (Mathf.Round(position.y * groupScale));
                z = (long) (Mathf.Round(position.z * groupScale));
            }
        }

        private struct VertData
        {
            public int tri;
            public int vert;

            public VertData(int tri, int vert)
            {
                this.tri = tri;
                this.vert = vert;
            }
        }
    }


}