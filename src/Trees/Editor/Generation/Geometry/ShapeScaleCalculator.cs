using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry
{
    public static class ShapeScaleCalculator
    {
        public static void UpdateShapeDistributionScales(IShapeReadWrite shapes, HierarchyData hierarchy)
        {
            using (BUILD_TIME.DIST_MGR.UpdateShapeDistributionScales.Auto())
            {
                var shapeDatas = shapes.GetShapesByHierarchy(hierarchy);
                var dist = hierarchy.distribution;

                foreach (var shape in shapeDatas)
                {
                    if (shape.IsBranch && hierarchy is BarkHierarchyData b && b.geometry.forked)
                    {
                        shape.distributionLikelihood = 1.0f;
                        shape.distributionScale = 1.0f;
                    }
                    else
                    {
                        shape.distributionScale = dist.distributionScale.Value.EvaluateScaledInverse01(shape.offset);

                        shape.distributionLikelihood =
                            dist.distributionLikelihood.Value.EvaluateScaledInverse01(shape.offset);
                    }
                }
            }
        }

        public static void UpdateSplineScale(
            BarkShapeData shape,
            BarkShapeData parentShape,
            BarkHierarchyData hierarchy,
            IHierarchyRead hierarchies,
            ISeed seed)
        {
            if (shape.type == TreeComponentType.Trunk)
            {
                var height = hierarchy.geometry.length.Value;

                var max = Mathf.Max(height.x, height.y);
                var min = Mathf.Min(height.x, height.y);

                float heightVariation = (max - min) / max;

                shape.scale = 1.0f - (seed.RandomValue() * heightVariation);
                shape.size = hierarchy.geometry.radius;
                shape.length = shape.scale * max;
            }
            else
            {
                var height = hierarchy.geometry.length.Value;
                var radius = hierarchy.geometry.radius.Value;

                var parentHierarchy = hierarchies.GetHierarchyData(hierarchy.parentHierarchyID) as BarkHierarchyData;

                var parentRadius = SplineModeler.GetRadiusAtTime(parentShape, parentHierarchy, shape.offset);

                if (hierarchy.geometry.relativeToParentAllowed && hierarchy.geometry.relativeToParent)
                {
                    hierarchy.geometry.relativeLength.CheckInitialization(new Vector2(.6f, .8f));

                    height = hierarchy.geometry.relativeLength.Value * parentShape.effectiveLength;

                    hierarchy.geometry.relativeRadius.CheckInitialization(.9f);

                    radius = hierarchy.geometry.relativeRadius.Value * parentRadius;
                }

                var lengthMax = Mathf.Max(height.x, height.y);
                var lengthMin = Mathf.Min(height.x, height.y);

                float heightVariation = (lengthMax - lengthMin) / lengthMax;

                shape.scale = 1.0f - (seed.RandomValue() * heightVariation);
                shape.length = shape.scale * lengthMax;

                if (shape.IsBranch && hierarchy.geometry.forked)
                {
                    shape.size = parentRadius;
                }
                else
                {
                    radius = Mathf.Min(radius, parentRadius * .9f);

                    shape.size = radius;

                    if (hierarchy.geometry.shorterBranchesAreThinner)
                    {
                        shape.size *= shape.scale;
                    }

                    shape.size = Mathf.Min(parentShape.effectiveSize, shape.size);
                }
            }
        }

        public static void UpdateLeafScale(LeafShapeData shape, LeafHierarchyData hierarchy, ISeed seed)
        {
            shape.scale = hierarchy.geometry.size.Value.x +
                ((hierarchy.geometry.size.Value.y - hierarchy.geometry.size.Value.x) * seed.RandomValue());
        }

        public static void UpdateFruitScale(FruitShapeData shape, FruitHierarchyData hierarchy, ISeed seed)
        {
            shape.scale = hierarchy.geometry.size.Value.x +
                ((hierarchy.geometry.size.Value.y - hierarchy.geometry.size.Value.x) * seed.RandomValue());

        }

        public static void UpdateFungusScale(FungusShapeData shape, FungusHierarchyData hierarchy, ISeed seed)
        {
            shape.scale = hierarchy.geometry.size.Value.x +
                ((hierarchy.geometry.size.Value.y - hierarchy.geometry.size.Value.x) * seed.RandomValue());
        }

        public static void UpdateKnotScale(KnotShapeData shape, KnotHierarchyData hierarchy, ISeed seed)
        {
            shape.scale = hierarchy.geometry.size.Value.x +
                ((hierarchy.geometry.size.Value.y - hierarchy.geometry.size.Value.x) * seed.RandomValue());
        }
    }
}
