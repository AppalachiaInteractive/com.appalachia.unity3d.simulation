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
using Appalachia.Simulation.Trees.Generation.Texturing.Materials;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.Recursion;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Settings.Log;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry
{
    public static class GeometryGenerator
    {
        private static readonly TreeVertex[] _planeVerts = new TreeVertex[8];
        private static readonly TreeVertex[] _planeVerts2 = new TreeVertex[8];

        public static void GenerateGeometry(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            TreePrefabCollection prefabs,
            TreeSettings settings,
            bool weld,
            BaseSeed individualSeed)
        {
            using (BUILD_TIME.GEO_GEN.GenerateGeometry.Auto())
            {
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

                        GenerateMeshShape(
                            hierarchies,
                            shapes,
                            output,
                            inputMaterialCache,
                            settings.lod[output.lodLevel],
                            settings.mesh,
                            settings.variants,
                            prefabs,
                            data,
                            weld,
                            seed
                        );

                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        for (var i = geometry.modelVertexStart; i < geometry.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            vertex.SetDynamicVariationValue(data.shape.variationSeed);
                        }

                        if (data.hierarchy is BarkHierarchyData b)
                        {
                            if (b.geometry.barkScale == null)
                            {
                                b.geometry.barkScale = TreeProperty.uv(1, 1);
                            }

                            b.geometry.barkScale.CheckInitialization(new UVScale());
                            
                            for (var i = geometry.modelTriangleStart; i < geometry.modelTriangleEnd; i++)
                            {
                                var triangle = output.triangles[i];

                                triangle.uvScale = b.geometry.barkScale.Value;
                            }
                        }
                    }
                );
            }
        }
        
        public static void GenerateGeometry(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LODGenerationOutput output,
            LogSettings logSettings,
            ISeed seed)
        {
            using (BUILD_TIME.GEO_GEN.GenerateGeometry.Auto())
            {
                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.shape.geometry.Count <= output.lodLevel)
                        {
                            data.shape.geometry.Add(new ShapeGeometryData());
                        }

                        var geometry = data.shape.geometry[output.lodLevel];

                        geometry.modelTriangleStart = output.triangles.Count;
                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexStart = output.vertices.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        /*data.shape.seed.Reset();*/

                        GenerateMeshShape(
                            hierarchies,
                            shapes,
                            output,
                            null,
                            logSettings.lod[output.lodLevel],
                            logSettings.mesh,
                            null,
                            null,
                            data,
                            true,
                            seed
                        );

                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        for (var i = geometry.modelVertexStart; i < geometry.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            vertex.SetDynamicVariationValue(data.shape.variationSeed);
                        }

                        if (data.hierarchy is BarkHierarchyData b)
                        {
                            if (b.geometry.barkScale == null)
                            {
                                b.geometry.barkScale = TreeProperty.uv(1, 1);
                            }

                            b.geometry.barkScale.CheckInitialization(new UVScale());
                            
                            for (var i = geometry.modelTriangleStart; i < geometry.modelTriangleEnd; i++)
                            {
                                var triangle = output.triangles[i];

                                triangle.uvScale = b.geometry.barkScale.Value;
                            }
                        }
                    }
                );
            }
        }

        public static void GenerateGeometry(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            TreePrefabCollection hierarchyPrefabs,
            BranchSettings branchSettings,
            bool buildFast,
            ISeed seed)
        {
            using (BUILD_TIME.GEO_GEN.GenerateGeometry.Auto())
            {
                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.shape.geometry.Count == 0)
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
                        
                        var geometry = data.shape.geometry[0];

                        geometry.modelTriangleStart = output.triangles.Count;
                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexStart = output.vertices.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        /*data.shape.seed.Reset();*/

                        GenerateMeshShape(
                            hierarchies,
                            shapes,
                            output,
                            inputMaterialCache,
                            branchSettings.lod,
                            branchSettings.mesh,
                            null,
                            hierarchyPrefabs,
                            data,
                            buildFast,
                            seed
                        );

                        geometry.modelTriangleEnd = output.triangles.Count;
                        geometry.modelVertexEnd = output.vertices.Count;

                        for (var i = geometry.modelVertexStart; i < geometry.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            vertex.SetDynamicVariationValue(data.shape.variationSeed);
                        }

                        if (data.hierarchy is BarkHierarchyData b)
                        {
                            if (b.geometry.barkScale == null)
                            {
                                b.geometry.barkScale = TreeProperty.uv(1, 1);
                            }

                            b.geometry.barkScale.CheckInitialization(new UVScale());
                            
                            for (var i = geometry.modelTriangleStart; i < geometry.modelTriangleEnd; i++)
                            {
                                var triangle = output.triangles[i];

                                triangle.uvScale = b.geometry.barkScale.Value;
                            }
                        }
                    }
                );
            }
        }

        private static void GenerateMeshShape(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings lodSettings,
            MeshSettings meshSettings,
            TreeVariantSettings variantSettings,
            TreePrefabCollection prefabs,
            GenericRecursionData data,
            bool weld,
            ISeed seed)
        {
            using (BUILD_TIME.GEO_GEN.GenerateMeshShape.Auto())
            {
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

                        var disableWelding = barkHierarchyData is CollaredBarkHierarchyData collared &&
                            collared.collar.disableWelding;

                        SplineGeometryGenerator.GenerateSplineGeometry(
                            hierarchies,
                            shapes,
                            output,
                            inputMaterialCache,
                            lodSettings,
                            variantSettings,
                            barkShapeData,
                            parentShapeData,
                            barkHierarchyData,
                            weld && !disableWelding,
                            weldRadius
                        );
                    }

                        break;

                    case TreeComponentType.Leaf:
                    {
                        var leafHierarchyData = data.hierarchy as LeafHierarchyData;
                        var leafShapeData = data.shape as LeafShapeData;
                        var parentShapeData = data.parentShape as BarkShapeData;

                        if (leafHierarchyData.geometry.disableWelding == null)
                        {
                            leafHierarchyData.geometry.disableWelding = TreeProperty.New(false);
                        }
                        if (leafHierarchyData.geometry.xRatio == null)
                        {
                            leafHierarchyData.geometry.xRatio = TreeProperty.New(1.0f);
                        }      
                        if (leafHierarchyData.geometry.xOffset == null)
                        {
                            leafHierarchyData.geometry.xOffset = TreeProperty.New(0.0f);
                        }                        
                        if (leafHierarchyData.geometry.zOffset == null)
                        {
                            leafHierarchyData.geometry.zOffset = TreeProperty.New(0.0f);
                        }                    
                        if (leafHierarchyData.geometry.correctOffset == null)
                        {
                            leafHierarchyData.geometry.correctOffset = TreeProperty.New(true);
                        }


                        LeafGeometryGenerator.GenerateLeafGeometry(
                            output,
                            inputMaterialCache,
                            lodSettings,
                            meshSettings,
                            prefabs,
                            leafShapeData,
                            parentShapeData,
                            leafHierarchyData,
                            seed,
                            weld && !leafHierarchyData.geometry.disableWelding
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
                            lodSettings,
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
                            lodSettings,
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
                            lodSettings,
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
            }
        }
    }
}
