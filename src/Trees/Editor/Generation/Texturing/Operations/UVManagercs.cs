#region

using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    [CallStaticConstructorInEditor]
    public static class UVManager
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static UVManager()
        {
            LeafUVRectCollection.InstanceAvailable += i => _leafUVRectCollection = i;
        }

        #region Static Fields and Autoproperties

        private static LeafUVRectCollection _leafUVRectCollection;

        #endregion

        public static void ApplyLeafRects(
            IHierarchyRead readStructure,
            IShapeRead shapes,
            InputMaterialCache inputMaterials,
            LODGenerationOutput output,
            BaseSeed seed)
        {
            using (BUILD_TIME.UV_MGR.ApplyLeafRects.Auto())
            {
                ApplyLeafRectsInternal(
                    readStructure,
                    shapes.GetShapes(TreeComponentType.Leaf).Cast<LeafShapeData>(),
                    inputMaterials,
                    output,
                    seed
                );
            }
        }

        public static void RemapUVCoordinates(
            LODGenerationOutput output,
            TreeMaterialCollection materials,
            TreeVariantSettings variantSettings)
        {
            using (BUILD_TIME.UV_MGR.RemapUVCoordinates.Auto())
            {
                RemapUVCoordinates(
                    output,
                    i => (materials.GetOutputMaterialByInputID(i) as TiledOutputMaterial).uvScale,
                    i => (materials.inputMaterialCache.GetByMaterialID(i) as AtlasInputMaterial).atlasUVRect,
                    variantSettings
                );
            }
        }

        public static void RemapUVCoordinates(LODGenerationOutput output, BranchMaterialCollection materials)
        {
            using (BUILD_TIME.UV_MGR.RemapUVCoordinates.Auto())
            {
                RemapUVCoordinates(
                    output,
                    i => new UVScale(),
                    i => (materials.inputMaterialCache.GetByMaterialID(i) as AtlasInputMaterial).atlasUVRect,
                    null
                );
            }
        }

        public static void RemapUVCoordinates(
            LODGenerationOutput output,
            Func<int, UVScale> outputMaterialScaleByInputID,
            Func<int, Rect> getRectByMaterialID,
            TreeVariantSettings variantSettings)
        {
            using (BUILD_TIME.UV_MGR.RemapUVCoordinates.Auto())
            {
                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    vertex.uvScaleUpdated = false;
                    output.vertices[index] = vertex;
                }

                for (var index = 0; index < output.triangles.Count; index++)
                {
                    var triangle = output.triangles[index];

                    if (triangle.inputMaterialID == -1)
                    {
                        continue;
                    }

                    if (triangle.context == TreeMaterialUsage.Bark)
                    {
                        var scale = outputMaterialScaleByInputID(triangle.inputMaterialID);

                        var uvScale = scale * triangle.uvScale;

                        //uvScale.x = Mathf.RoundToInt(uvScale.x);
                        //uvScale.y = Mathf.RoundToInt(uvScale.y);

                        for (var vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                        {
                            var vertex = output.vertices[triangle.v[vertexIndex]];

                            if (!vertex.uvScaleUpdated)
                            {
                                vertex.uvScale = uvScale;
                                vertex.uvScaleUpdated = true;
                            }
                        }
                    }
                    else if (triangle.context == TreeMaterialUsage.SplineBreak)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            var vertex = output.vertices[triangle.v[i]];

                            vertex.splineBreak = true;

                            if (!vertex.uvScaleUpdated)
                            {
                                vertex.uvScale = variantSettings?.trunkTextureUVScale ?? Vector2.one;
                                vertex.uvOffset = variantSettings?.trunkTextureUVOffset ?? Vector2.zero;
                                vertex.uvScaleUpdated = true;
                            }
                        }
                    }
                    else
                    {
                        var uvRect = getRectByMaterialID(triangle.inputMaterialID);

                        uvRect.x += uvRect.width * .02f;
                        uvRect.width -= uvRect.width * .04f;
                        uvRect.y += uvRect.height * .02f;
                        uvRect.height -= uvRect.height * .04f;

                        for (var vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                        {
                            var vertex = output.vertices[triangle.v[vertexIndex]];

                            if (!vertex.uvScaleUpdated)
                            {
                                vertex.uvRect = uvRect;
                                vertex.uvScaleUpdated = true;
                            }
                        }
                    }
                }
            }
        }

        private static void ApplyLeafRectsInternal(
            IHierarchyRead readStructure,
            IEnumerable<LeafShapeData> leaves,
            InputMaterialCache inputMaterials,
            LODGenerationOutput output,
            BaseSeed baseSeed)
        {
            using (BUILD_TIME.UV_MGR.ApplyLeafRects.Auto())
            {
                var materialRects = new Dictionary<int, AtlasInputMaterial>();

                foreach (var leafHierarchy in readStructure.GetHierarchies(TreeComponentType.Leaf)
                                                           .Cast<LeafHierarchyData>()
                                                           .Where(
                                                                l => l.geometry.geometryMode !=
                                                                     LeafGeometryMode.Mesh
                                                            ))
                {
                    var mat = inputMaterials.GetInputMaterialData(
                        leafHierarchy.geometry.leafMaterial,
                        TreeMaterialUsage.LeafPlane
                    ) as AtlasInputMaterial;

                    if (mat == null)
                    {
                        return;
                    }

                    var rects = _leafUVRectCollection.Get(mat.material);

                    if (rects.Count == 0)
                    {
                        rects.Add(new LeafUVRect());
                    }

                    if (!materialRects.ContainsKey(leafHierarchy.hierarchyID))
                    {
                        materialRects.Add(leafHierarchy.hierarchyID, mat);
                    }

                    var probabilitySum = rects.Sum(r => r.probability);

                    foreach (var rect in rects)
                    {
                        rect.probability /= probabilitySum;
                    }
                }

                var shapeIDRects = new Dictionary<int, UVRect>();

                foreach (var leafShape in leaves)
                {
                    var leafHierarchy =
                        readStructure.GetHierarchyData(leafShape.hierarchyID) as LeafHierarchyData;

                    var seed = new VirtualSeed(baseSeed, leafHierarchy.seed);

                    var rects = _leafUVRectCollection.Get(materialRects[leafShape.hierarchyID].material);

                    LeafUVRect rect = null;

                    if (rects.Count == 0)
                    {
                        rect = new LeafUVRect();
                        rects.Add(rect);
                    }
                    else
                    {
                        var value = seed.RandomValue();

                        var sum = 0f;
                        for (var i = 0; i < rects.Count; i++)
                        {
                            sum += rects[i].probability;

                            if (value < sum)
                            {
                                rect = rects[i];
                                break;
                            }
                        }
                    }

                    if (rect == null)
                    {
                        rect = rects[0];
                    }

                    shapeIDRects.Add(leafShape.shapeID, rect.rect);
                }

                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];

                    if (shapeIDRects.ContainsKey(vertex.shapeID))
                    {
                        var rect = shapeIDRects[vertex.shapeID];
                        var raw = vertex.raw_uv0;

                        vertex.rect_uv0 = rect.ScaleNormalizedPointWithin(raw);
                    }
                    else
                    {
                        vertex.rect_uv0 = vertex.raw_uv0;
                    }

                    output.vertices[index] = vertex;
                }
            }
        }
    }
}
