using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Prefabs
{
    [Serializable]
    public class TreePrefabLODRendererCloningData : AppalachiaSimpleBase
    {
        [SerializeField] private bool? _canMergeIntoTree;
        [SerializeField] private Material[] materials;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Matrix4x4 meshMatrix;
        [SerializeField] private Vector3[] normals;
        [SerializeField] private Vector4[] tangents;

        [SerializeField] private TreeComponentType type;
        [SerializeField] private Vector2[] uvs;
        [SerializeField] private Vector3[] verts;

        public bool IsValid =>
            (mesh != null) &&
            (materials != null) &&
            (materials.Length > 0) &&
            (materials.Length >= mesh.subMeshCount) &&
            (verts != null) &&
            (verts.Length > 0) &&
            (normals != null) &&
            (normals.Length == verts.Length) &&
            (uvs != null) &&
            (uvs.Length == verts.Length) &&
            (tangents != null) &&
            (tangents.Length == verts.Length);

        public void Update(Renderer renderer)
        {
            var mf = renderer.GetComponent<MeshFilter>();

            if (mf == null) return;

            mesh = mf.sharedMesh;
            materials = renderer.sharedMaterials;
            meshMatrix = renderer.transform.localToWorldMatrix;

            verts = mesh.vertices;
            normals = mesh.normals;
            uvs = mesh.uv;
            tangents = mesh.tangents;

            // normalize size

            var meshSize = mesh.bounds.extents;
            var meshScale = Mathf.Max(meshSize.x, meshSize.z) * 0.5f;
            for (var i = 0; i < verts.Length; i++)
            {
                verts[i].x /= meshScale;
                verts[i].y /= meshScale;
                verts[i].z /= meshScale;
            }
        }

        public IEnumerable<Object> GetExternalObjects()
        {
            return new Object[] {mesh}.Concat(materials);
        }

        public Material[] GetMaterials()
        {
            return materials;
        }

        public Mesh GetMesh()
        {
            return mesh;
        }

        public bool CanMergeIntoTree()
        {
            if (_canMergeIntoTree.HasValue)
            {
                return _canMergeIntoTree.Value;
            }

            foreach (var material in materials)
            {
                var shader = TextureExtractor.GetShader(material);

                if (shader != null)
                {
                    continue;
                }

                _canMergeIntoTree = false;
                break;
            }

            if (!_canMergeIntoTree.HasValue)
            {
                _canMergeIntoTree = true;
            }

            return _canMergeIntoTree.Value;
        }

        public void MergeVertices(
            LODGenerationOutput output,
            ShapeData shape,
            int hierarchyID,
            int shapeID,
            float heightOffset)
        {
            var matrix = shape.effectiveMatrix * meshMatrix;

            for (var i = 0; i < verts.Length; i++)
            {
                var vertex = new TreeVertex()/*TreeVertex.Get()*/;
                vertex.Set(shape, heightOffset);

                vertex.position = matrix.MultiplyPoint(verts[i]);
                vertex.normal = matrix.MultiplyVector(normals[i]).normalized;
                vertex.raw_uv0 = new Vector2(uvs[i].x, uvs[i].y);

                var tangent = matrix.MultiplyVector(
                        new Vector3(tangents[i].x, tangents[i].y, tangents[i].z)
                    )
                    .normalized;

                vertex.tangent = new Vector4(tangent.x, tangent.y, tangent.z, tangents[i].w);

                output.AddVertex(vertex);
            }
        }

        public void MergeTriangles(
            LODGenerationOutput output,
            ShapeData shape,
            InputMaterialCache inputMaterialCache,
            int vertexOffset)
        {
            for (var submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
            {
                var material = inputMaterialCache.GetInputMaterialData(materials[submeshIndex], TreeMaterialUsage.Prefab);

                var instanceTris = mesh.GetTriangles(submeshIndex);

                for (var i = 0; i < instanceTris.Length; i += 3)
                {
                    var triangle = new TreeTriangle();
                    triangle.Set(
                        shape,
                        material.materialID,
                        instanceTris[i] + vertexOffset,
                        instanceTris[i + 1] + vertexOffset,
                        instanceTris[i + 2] + vertexOffset,
                        TreeMaterialUsage.Prefab,
                        false
                    );

                    output.AddTriangle(triangle);
                }
            }
        }

        public TreePrefabLODRendererCloningData(Renderer renderer, TreeComponentType type)
        {
            this.type = type;
            Update(renderer);
        }
    }
}