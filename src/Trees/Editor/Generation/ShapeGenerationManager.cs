using System;
using System.Linq;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Distribution;
using Appalachia.Simulation.Trees.Generation.Geometry;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation
{
    public static class ShapeGenerationManager
    {
        public static void GenerateShapes(
            BaseSeed individualSeed,
            IShapeReadWrite shapes,
            IHierarchyRead hierarchies,
            TreeVariantSettings variantSettings,
            AgeType age
        )
        {
            using (BUILD_TIME.SHAPE_GEN_MGR.GenerateShapes.Auto())
            {
                var flags = new[]
                {
                    TreeAgeTypeFlags.Sapling,
                    TreeAgeTypeFlags.Young,
                    TreeAgeTypeFlags.Adult,
                    TreeAgeTypeFlags.Mature,
                    TreeAgeTypeFlags.Spirit
                };
                
                foreach (var hierarchy in hierarchies.GetHierarchies())
                {
                    if (hierarchy is BarkHierarchyData bhd)
                    {
                        if (hierarchy.IsTrunk)
                        {
                            bhd.geometry.forked = false;
                        }
                        else if (bhd.geometry.forked)
                        {
                            var parent = hierarchies.GetHierarchyData(hierarchy.parentHierarchyID) as BarkHierarchyData;

                            bhd.geometry.barkScale.Value.x = parent.geometry.barkScale.Value.x;
                        }
                    }
                    
                    var childHierarchies = hierarchies.GetHierarchiesByParent(hierarchy.hierarchyID);

                    if (childHierarchies == null)
                    {
                        continue;
                    }

                    foreach (var childHierarchy in childHierarchies)
                    {
                        foreach (var flag in flags)
                        {
                            
                            if (!hierarchy.ageEligibility.HasFlag(flag) && childHierarchy.ageEligibility.HasFlag(flag))
                            {                            
                                childHierarchy.ageEligibility &= ~flag;
                            }
                        }
                    }
                }
                
                foreach (var shape in shapes)
                {
                    var hierarchy = hierarchies.GetHierarchyData(shape.hierarchyID);
                    shape.SetUp(hierarchy.seed);
                }

                hierarchies.RecurseHierarchies(
                    hierarchy =>
                    {
                        hierarchy.seed.Reset();
                        individualSeed.Reset();
                        var seed = new VirtualSeed(individualSeed, hierarchy.seed);

                        DistributionManager.PopulateShapes(hierarchies, shapes, hierarchy, age);
                        
                        var minFungiHeight =
                            Mathf.Min(
                                variantSettings.liveHeightRange.Value.x,
                                variantSettings.deadHeightRange.Value.x,
                                variantSettings.rottedHeightRange.Value.x
                            );
                        
                        var maxFungiHeight =
                            Mathf.Max(
                                variantSettings.liveHeightRange.Value.y,
                                variantSettings.deadHeightRange.Value.y,
                                variantSettings.rottedHeightRange.Value.y
                            );
                        
                        GenerateShapes(
                            hierarchy,
                            shapes,
                            hierarchies, 
                            seed, 
                            new Vector2(minFungiHeight, maxFungiHeight));
                    }
                );
            }
        }

        public static void GenerateShapes(
            BaseSeed instanceSeed,
            IShapeReadWrite shapes,
            IHierarchyRead hierarchies)
        {
            using (BUILD_TIME.SHAPE_GEN_MGR.GenerateShapes.Auto())
            {
                foreach (var shape in shapes)
                {
                    shape.SetUp(instanceSeed);
                }

                hierarchies.RecurseHierarchies(
                    hierarchy =>
                    {
                        hierarchy.seed.Reset();
                        instanceSeed.Reset();
                        var seed = new VirtualSeed(instanceSeed, hierarchy.seed);
                        
                        DistributionManager.PopulateShapes(
                            hierarchies, 
                            shapes,
                            hierarchy,
                            AgeType.None
                        );

                        /*
                        foreach (var shape in context.branch.shapes.Where(s => s.hierarchyID == hierarchy.hierarchyID))
                        {
                            hierarchy.seed.UpdateShapeSeed(hierarchy.seed, context.branch.seed);
                        }
                        */

                        GenerateShapes(
                            hierarchy,
                            shapes,
                            hierarchies,
                            seed,
                            Vector2.one
                        );
                    }
                );
            }
        }
        
        public static void GenerateShapes(
            IShapeReadWrite shapes,
            IHierarchyRead hierarchies)
        {
            using (BUILD_TIME.SHAPE_GEN_MGR.GenerateShapes.Auto())
            {
                var shapeit = shapes.ToArray();

                for (var index = shapeit.Length - 1; index >= 0; index--)
                {
                    var shape = shapeit[index];

                    if (!hierarchies.DoesHierarchyExist(shape.hierarchyID))
                    {
                        shapes.DeleteShapeChain(shape.shapeID);
                        continue;
                    }
                    
                    var hierarchy = hierarchies.GetHierarchyData(shape.hierarchyID);
                    shape.SetUp(hierarchy.seed);
                }

                hierarchies.RecurseHierarchies(
                    hierarchy =>
                    {
                        hierarchy.seed.Reset();

                        DistributionManager.PopulateShapes(
                            hierarchies, shapes,
                            hierarchy,
                            AgeType.None
                        );

                        /*
                        foreach (var shape in context.branch.shapes.Where(s => s.hierarchyID == hierarchy.hierarchyID))
                        {
                            hierarchy.seed.UpdateShapeSeed(hierarchy.seed, context.branch.seed);
                        }
                        */

                        GenerateShapes(
                            hierarchy,
                            shapes,
                            hierarchies,
                            hierarchy.seed,
                            Vector2.one
                        );
                    }
                );
            }
        }

        private static void GenerateShapes(
            HierarchyData hierarchy,
            IShapeReadWrite shapeStructure,
            IHierarchyRead readStructure,
            ISeed seed,
            Vector2 fungiRange)
        {
            DistributionManager.UpdateShapeOffsets(shapeStructure, readStructure, hierarchy, seed, fungiRange);

            ShapeScaleCalculator.UpdateShapeDistributionScales(shapeStructure, hierarchy);

            var shapes = shapeStructure.GetShapesByHierarchy(hierarchy.hierarchyID);

            for (var i = shapes.Count - 1; i >= 0; i--)
            {
                if (shapes[i].type != hierarchy.type)
                {
                    shapes.RemoveAt(i);
                    continue;
                }
                
                var parentShape = shapeStructure.GetParentShapeData(shapes[i]);
                if ((hierarchy.type != TreeComponentType.Trunk) && (parentShape == null))
                {
                    shapes.RemoveAt(i);
                }
            }

            foreach (var shape in shapes)
            {
                shape.scale = 1f;
                shape.size = 1f;
                shape.effectiveScale = 0f;
                shape.effectiveSize = 0f;

                var parentShape = shapeStructure.GetParentShapeData(shape);

                switch (hierarchy.type)
                {
                    case TreeComponentType.Root:
                    case TreeComponentType.Trunk:
                    case TreeComponentType.Branch:
                    {
                        var barkHierarchy = hierarchy as BarkHierarchyData;
                        var barkShape = shape as BarkShapeData;

                        BarkShapeData barkParentShape = null;

                        if (barkShape.parentShapeID != -1)
                        {
                            barkParentShape = parentShape as BarkShapeData;

                            if (barkParentShape == null)
                            {
                                throw new NotSupportedException("Missing parent shape!");
                            }
                        }

                        DistributionManager.UpdateBreakage(barkShape, barkHierarchy, seed);
                        
                        ShapeScaleCalculator.UpdateSplineScale(
                            barkShape,
                            barkParentShape,
                            barkHierarchy,
                            readStructure,
                            seed
                        );
                    }

                        break;
                    case TreeComponentType.Leaf:
                        ShapeScaleCalculator.UpdateLeafScale(
                            shape as LeafShapeData,
                            hierarchy as LeafHierarchyData,
                            seed
                        );
                        break;
                    case TreeComponentType.Fruit:
                        ShapeScaleCalculator.UpdateFruitScale(
                            shape as FruitShapeData,
                            hierarchy as FruitHierarchyData,
                            seed
                        );
                        break;
                    case TreeComponentType.Knot:
                        ShapeScaleCalculator.UpdateKnotScale(
                            shape as KnotShapeData,
                            hierarchy as KnotHierarchyData,
                            seed
                        );
                        break;
                    case TreeComponentType.Fungus:
                        ShapeScaleCalculator.UpdateFungusScale(
                            shape as FungusShapeData,
                            hierarchy as FungusHierarchyData,
                            seed
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                shape.effectiveScale = shape.scale;
                shape.effectiveSize = shape.size;
                shape.effectiveLength = shape.length;

                shape.effectiveScale *= shape.distributionScale;
                shape.effectiveSize *= shape.distributionScale;
                shape.effectiveLength *= shape.distributionScale;

                if (parentShape != null)
                {
                    shape.effectiveScale *= parentShape.effectiveScale;
                    shape.effectiveLength *= parentShape.effectiveScale;

                    //shape.effectiveSize *= parentShape.effectiveSize;
                }
            }

            DistributionManager.UpdateShapeVisibility(shapeStructure, hierarchy, seed);

            DistributionManager.UpdateShapeAngles(shapeStructure, hierarchy);

            for (var index = 0; index < shapes.Count; index++)
            {
                var shape = shapes[index];
                var parentShape = shapeStructure.GetParentShapeData(shape);
                var parentHierarchy = readStructure.GetHierarchyData(hierarchy.parentHierarchyID);

                if ((shape == null) || ((parentShape == null) && (shape.type != TreeComponentType.Trunk)))
                {
                    shapes.RemoveAt(index);
                    index -= 1;
                    continue;
                }

                switch (hierarchy.type)
                {
                    case TreeComponentType.Root:
                    case TreeComponentType.Trunk:
                    case TreeComponentType.Branch:

                        var barkShape = shape as BarkShapeData;

                        ShapeMatrixCalculator.UpdateSplineMatrix(
                            readStructure.GetVerticalOffset(),
                            barkShape,
                            hierarchy as BarkHierarchyData,
                            parentShape as BarkShapeData,
                            parentHierarchy as BarkHierarchyData
                        );

                        SplineModeler.UpdateSpline(
                            hierarchy as BarkHierarchyData,
                            barkShape,
                            parentShape as BarkShapeData,
                            seed
                        );

                        break;
                    case TreeComponentType.Leaf:

                        ShapeMatrixCalculator.UpdateLeafMatrix(
                            shape as LeafShapeData,
                            hierarchy as LeafHierarchyData,
                            parentShape as BarkShapeData,
                            parentHierarchy as BarkHierarchyData
                        );

                        break;
                    case TreeComponentType.Fruit:
                    case TreeComponentType.Knot:
                    case TreeComponentType.Fungus:
                        ShapeMatrixCalculator.UpdateAddOnShapeMatrix(shape, hierarchy, parentShape, parentHierarchy);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                shape.effectiveMatrix = shape.matrix;

                if (shape.matrix.isIdentity && (shape.parentShapeID != -1))
                {
                    throw new NotSupportedException(
                        ZString.Format("Bad matrix for shape {0}!", shape.shapeID)
                    );
                }
            }
        }
    }
}
