using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Geometry.Addons;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using Appalachia.Simulation.Trees.Generation.VertexData;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.ShadowCasters
{
    public static class ShadowCasterGenerator
    {
        public static void GenerateShadowCaster(
            TreeDataContainer tree,
            TreeIndividual individual,
            TreeStage stage,
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            InputMaterialCache inputMaterialCache,
            TreePrefabCollection prefabs,
            TreeSettings settings,            
            bool weld,
            BaseSeed individualSeed,
            Dictionary<int, ShapeData> shapeLookup
            )
        {
            using (BUILD_TIME.SHDW_GEN.GenerateShadowCaster.Auto())
            {
                var output = new LODGenerationOutput(settings.lod.levelsOfDetail.Count);

                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        while (data.shape.geometry.Count <= output.lodLevel)
                        {
                            data.shape.geometry.Add(new ShapeGeometryData());
                        }

                        if (data.shape is BarkShapeData bsd)
                        {
                            if (bsd.branchRings == null)
                            {
                                bsd.branchRings = new List<List<BranchRing>>();
                            }
                            
                            while (bsd.branchRings.Count <= output.lodLevel)
                            {
                                bsd.branchRings.Add(new List<BranchRing>());
                            }
                        }

                        var geometry = data.shape.geometry[output.lodLevel];

                        geometry.modelTriangleStart = output.triangles.Count;
                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexStart = output.vertices.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        data.hierarchy.seed.Reset();
                        
                        var seed = new VirtualSeed(individualSeed, data.hierarchy.seed);

                        using (BUILD_TIME.GEO_GEN.GenerateMeshShape.Auto())
                        {
                            
                            var size = data.shape.size;
                            var eSize = data.shape.effectiveSize;

                            data.shape.size *= .95f;
                            data.shape.effectiveSize *= .95f;
                            
                            switch (data.type)
                            {
                                case TreeComponentType.Root:
                                case TreeComponentType.Trunk:
                                case TreeComponentType.Branch:
                                {
                                    var barkHierarchyData = data.hierarchy as BarkHierarchyData;
                                    var barkShapeData = data.shape as BarkShapeData;
                                    var parentShapeData = data.parentShape as BarkShapeData;

                                    var weldRadius = 1f;

                                    if (data.type != TreeComponentType.Trunk)
                                    {
                                        var parentHierarchyData = data.parentHierarchy as BarkHierarchyData;
                                        weldRadius = SplineModeler.GetRadiusAtTime(
                                            parentShapeData,
                                            parentHierarchyData,
                                            data.shape.offset
                                        );

                                        weldRadius = Mathf.Max(1, weldRadius);
                                    }

                                    SplineGeometryGenerator.GenerateSplineGeometry(
                                        hierarchies,
                                        shapes,
                                        output,
                                        inputMaterialCache,
                                        settings.shadow.quality,
                                        settings.variants,
                                        barkShapeData,
                                        parentShapeData,
                                        barkHierarchyData,
                                        weld,
                                        weldRadius
                                    );

                                }

                                    break;

                                case TreeComponentType.Leaf:
                                {
                                    var leafHierarchyData = data.hierarchy as LeafHierarchyData;
                                    var leafShapeData = data.shape as LeafShapeData;
                                    var parentShapeData = data.parentShape as BarkShapeData;

                                    LeafGeometryGenerator.GenerateLeafGeometry(
                                        output,
                                        inputMaterialCache,
                                        settings.shadow.quality,
                                        settings.shadow.properties,
                                        prefabs,
                                        leafShapeData,
                                        parentShapeData,
                                        leafHierarchyData,
                                        seed,
                                        !leafHierarchyData.geometry.disableWelding
                                    );
                                }

                                    break;

                                case TreeComponentType.Fruit:
                                {
                                    var fruitHierarchyData = data.hierarchy as FruitHierarchyData;
                                    var fruitShapeData = data.shape as FruitShapeData;
                                    var parentShapeData = data.parentShape as BarkShapeData;
                                    var parentHierarchyData = data.parentHierarchy as BarkHierarchyData;
                                    var weldRadius = SplineModeler.GetRadiusAtTime(
                                        parentShapeData,
                                        parentHierarchyData,
                                        data.shape.offset
                                    );

                                    weldRadius = Mathf.Max(1, weldRadius);

                                    AddonGeometryGenerator.GenerateFruitGeometry(
                                        output,
                                        inputMaterialCache,
                                        settings.shadow.quality,
                                        prefabs,
                                        fruitShapeData,
                                        data.parentShape,
                                        fruitHierarchyData,
                                        weld,
                                        weldRadius
                                    );
                                }
                                    break;

                                case TreeComponentType.Knot:
                                {
                                    var knotHierarchyData = data.hierarchy as KnotHierarchyData;
                                    var knotShapeData = data.shape as KnotShapeData;
                                    var parentShapeData = data.parentShape as BarkShapeData;
                                    var parentHierarchyData = data.parentHierarchy as BarkHierarchyData;
                                    var weldRadius = SplineModeler.GetRadiusAtTime(
                                        parentShapeData,
                                        parentHierarchyData,
                                        data.shape.offset
                                    );

                                    weldRadius = Mathf.Max(1, weldRadius);

                                    AddonGeometryGenerator.GenerateKnotGeometry(
                                        output,
                                        inputMaterialCache,
                                        settings.shadow.quality,
                                        prefabs,
                                        knotShapeData,
                                        data.parentShape,
                                        knotHierarchyData,
                                        weld,
                                        weldRadius
                                    );

                                    break;
                                }

                                case TreeComponentType.Fungus:
                                {
                                    var fungusHierarchyData = data.hierarchy as FungusHierarchyData;
                                    var fungusShapeData = data.shape as FungusShapeData;
                                    var parentShapeData = data.parentShape as BarkShapeData;
                                    var parentHierarchyData = data.parentHierarchy as BarkHierarchyData;
                                    var weldRadius = SplineModeler.GetRadiusAtTime(
                                        parentShapeData,
                                        parentHierarchyData,
                                        data.shape.offset
                                    );

                                    weldRadius = Mathf.Max(1, weldRadius);

                                    AddonGeometryGenerator.GenerateFungusGeometry(
                                        output,
                                        inputMaterialCache,
                                        settings.shadow.quality,
                                        prefabs,
                                        fungusShapeData,
                                        data.parentShape,
                                        fungusHierarchyData,
                                        weld,
                                        weldRadius
                                    );

                                    break;
                                }

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            data.shape.size = size;
                            data.shape.effectiveSize = eSize;
                        }
                        
                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        for (var i = geometry.modelVertexStart; i < geometry.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            vertex.SetDynamicVariationValue(data.shape.variationSeed);
                        }
                    }
                );

                UVManager.RemapUVCoordinates(output, tree.materials, tree.settings.variants);
                        
                UVManager.ApplyLeafRects(
                    tree.species.hierarchies,
                    stage.shapes,
                    tree.materials.inputMaterialCache,
                    output,
                    individual.seed
                );

                var vertices = output.vertices;
                foreach (var shape in stage.shapes)
                {
                    if (shape is BarkShapeData bsd)
                    {
                        var rings = bsd.branchRings[output.lodLevel];

                        for (var ri = 0; ri < rings.Count; ri++)
                        {
                            var ring = rings[ri];

                            var vertexOne = vertices[ring.vertexStart];
                            var vertexTwo = vertices[ring.vertexEnd-1];

                            vertexTwo.position = vertexOne.position;
                            vertexTwo.normal = vertexOne.normal;
                            vertexTwo.tangent = vertexOne.tangent;

                            vertexTwo.weldTo = ring.vertexStart;
                        }
                    }
                }
                
                if (tree.settings.wind.generateWind)
                {
                    WindGenerator.ApplyMeshWindData(
                        stage.shapes,
                        tree.species.hierarchies,
                        output,
                        output.lodLevel,
                        tree.settings.wind,
                        stage.stageType,
                        individual.seed
                    );
                }

                foreach (var shape in stage.shapes)
                {
                    for (var i = 1; i < stage.lods.Count; i++)
                    {
                        var model = shape.geometry[0];
                        var target = shape.geometry[i];
                        var modelVertex = stage.lods[0].vertices[model.modelVertexStart];

                        for (var v = target.modelVertexStart; v < target.modelVertexEnd; v++)
                        {
                            var vertex = stage.lods[i].vertices[v];

                            vertex.wind.phase = modelVertex.wind.phase;
                            vertex.wind.variation = modelVertex.wind.variation;
                            vertex.wind.primaryPivot = modelVertex.wind.primaryPivot;
                            vertex.wind.secondaryPivot = modelVertex.wind.secondaryPivot;
                        }
                    }
                }

                Func<TreeVertex, Color> colorFunction;

                if (tree.settings.wind.generateWind)
                {
                    if (tree.settings.wind.normalizeWind)
                    {
                        colorFunction = v => v.TreeVertexColor;
                    }
                    else
                    {
                        colorFunction = v => v.TreeVertexColorUnclamped;
                    }
                }
                else
                {
                    colorFunction = v => Color.black;
                }

                if (settings.shadow.doubleSided)
                {
                    var triangleCount = output.triangles.Count;
                
                    for(var i = 0; i < triangleCount; i++)
                    {
                        var triangle = output.triangles[i];
                    
                        if (triangle.type != TreeComponentType.Leaf)
                        {
                            var newTriangle = triangle.Clone();
                        
                            newTriangle.FlipFace();

                            output.AddTriangle(newTriangle);
                        }
                    }
                }

                MeshGenerator.GenerateMeshes(
                    output,
                    shapeLookup,
                    null,
                    new List<OutputMaterial>(),
                    settings.collision.properties,
                    stage.asset.shadowCasterMesh,
                    colorFunction,
                    true,
                    true,
                    default);
            }
        }
    }
}
