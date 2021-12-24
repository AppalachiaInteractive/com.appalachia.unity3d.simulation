using System;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Distribution
{
    public static class DistributionManager
    {
        [NonSerialized] private static AppaContext _context;

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(DistributionManager));
                }

                return _context;
            }
        }

        public static float ComputeOffset(
            int index,
            int count,
            float distributionSum,
            float distributionStep,
            DistributionSettings distribution)
        {
            var current = 0.0f;

            var target = ((index + 1.0f) / (count + 1.0f)) * distributionSum;

            for (var j = 0.0f; j <= 1.0f; j += distributionStep)
            {
                current += distribution.distributionCurve.EvaluateClamped01(j);
                if (current >= target)
                {
                    return j;
                }
            }

            return target / distributionSum;
        }

        public static void PopulateShapes(
            IHierarchyRead hierarchies,
            IShapeReadWrite shapes,
            HierarchyData hierarchy,
            AgeType ageType)
        {
            using (BUILD_TIME.DIST_MGR.PopulateShapes.Auto())
            {
                var parentHierarchy = hierarchies.GetHierarchyData(hierarchy.parentHierarchyID);

                if (!hierarchy.IsEligibleForAge(ageType))
                {
                    shapes.UpdateIndividualHierarchyShapeCounts(hierarchy, parentHierarchy, 0);

                    return;
                }

                if (parentHierarchy == null)
                {
                    var definedFrequency = hierarchy.distribution.distributionFrequency;

                    shapes.UpdateIndividualHierarchyShapeCounts(hierarchy, null, definedFrequency);
                }
                else
                {
                    var definedFrequency = hierarchy.distribution.distributionFrequency.Value;

                    var parentShapes = shapes.GetShapesByHierarchy(parentHierarchy);

                    var parentScale = parentShapes.FirstOrDefault()?.effectiveScale ?? 0f;

                    if (parentScale == 0f)
                    {
                        Context.Log.Warn(
                            ZString.Format(
                                "Not building shapes for hierarchy {0} [{1}] because of parent scale.",
                                hierarchy.hierarchyID,
                                hierarchy.type
                            )
                        );
                    }

                    var actualTargetFrequency = Mathf.RoundToInt(definedFrequency * parentScale);

                    if ((definedFrequency > .5f) && (actualTargetFrequency == 0))
                    {
                        actualTargetFrequency = 1;
                    }

                    shapes.UpdateIndividualHierarchyShapeCounts(
                        hierarchy,
                        parentHierarchy,
                        actualTargetFrequency
                    );
                }
            }
        }

        public static void UpdateBreakage(BarkShapeData shape, BarkHierarchyData hierarchy, ISeed seed)
        {
            var breakSpot = seed.RandomValue();
            var breakDeterminant = seed.RandomValue();

            shape.breakOffset = 1.0f;

            if ((breakDeterminant <= hierarchy.limb.breakingChance) &&
                (hierarchy.limb.breakingChance > 0.001f))
            {
                shape.breakOffset = hierarchy.limb.breakingSpot.Value.x +
                                    ((hierarchy.limb.breakingSpot.Value.y -
                                      hierarchy.limb.breakingSpot.Value.x) *
                                     breakSpot);

                if (shape.breakOffset < 0.01f)
                {
                    shape.breakOffset = 0.01f;
                }
            }
        }

        public static void UpdateShapeAngles(IShapeReadWrite shapes, HierarchyData hierarchy)
        {
            using (BUILD_TIME.DIST_MGR.UpdateShapeAngles.Auto())
            {
                var shapeDatas = shapes.GetShapesByHierarchy(hierarchy);
                var dist = hierarchy.distribution;

                foreach (var shape in shapeDatas)
                {
                    //shape.angle = shape.baseAngle;

                    shape.verticalAngle = dist.growthAngle.Value.EvaluateScaledClamped(
                                              shape.offset,
                                              -1.0f,
                                              1.0f
                                          ) *
                                          -75.0f /* * dist.growthAngle.Value.value*/;
                }
            }
        }

        public static void UpdateShapeOffsets(
            IShapeReadWrite shapes,
            IHierarchyRead hierarchies,
            HierarchyData hierarchy,
            ISeed seed,
            Vector2 fungiRange)
        {
            using (BUILD_TIME.DIST_MGR.UpdateShapeOffsets.Auto())
            {
                var distribution = hierarchy.distribution;

                var distributionSum = 0.0f;

                var distributionSamples = new float[100];

                var distributionStep = 1.0f / distributionSamples.Length;

                var distributionDivisor = (float)distributionSamples.Length - 1;

                for (var i = 0; i < distributionSamples.Length; i++)
                {
                    var j = i / distributionDivisor;
                    distributionSamples[i] = distribution.distributionCurve.EvaluateClamped01(j);
                    distributionSum += distributionSamples[i];
                }

                var shapeDatas = shapes.GetShapesByHierarchy(hierarchy);

                var randomDistributionOffset = seed.RandomValue(-360f, 360f);

                for (var i = 0; i < shapeDatas.Count; i++)
                {
                    var shape = shapeDatas[i];

                    //shape.seed.Reset();

                    if ((i == 0) && (shapeDatas.Count == 1) && (shape.type == TreeComponentType.Trunk))
                    {
                        // first child of the root is always centered..
                        shape.offset = 0.0f;
                        shape.radialAngle = 0.0f;
                        shape.verticalAngle = 0.0f;
                        shape.scale =
                            distribution.distributionScale.accessor.EvaluateScaledInverse01(shape.offset);
                    }
                    else
                    {
                        var nodeLocalIndex = 0;
                        var nodeLocalCount = 0;

                        for (var j = 0; j < shapeDatas.Count; j++)
                        {
                            if (shapeDatas[j].parentShapeID == shape.parentShapeID)
                            {
                                if (i == j)
                                {
                                    nodeLocalIndex = nodeLocalCount;
                                }

                                nodeLocalCount++;
                            }
                        }

                        if ((distribution.vertical.Value == DistributionVerticalMode.Random) ||
                            (distribution.vertical.Value == DistributionVerticalMode.Equal))
                        {
                            distribution.clusterCount.SetValue(1);
                        }

                        switch (distribution.vertical.Value)
                        {
                            case DistributionVerticalMode.Random:
                            {
                                var offset = 0.0f;
                                var weight = seed.RandomValue() * distributionSum;

                                for (var j = 0; j < distributionSamples.Length; j++)
                                {
                                    offset = j / distributionDivisor;
                                    weight -= distributionSamples[j];
                                    if (weight <= 0.0f)
                                    {
                                        break;
                                    }
                                }

                                shape.offset = offset;
                            }
                                break;
                            case DistributionVerticalMode.Equal:
                            {
                                shape.offset = ComputeOffset(
                                    nodeLocalIndex,
                                    nodeLocalCount,
                                    distributionSum,
                                    distributionStep,
                                    distribution
                                );
                            }

                                break;
                            case DistributionVerticalMode.Clusters:
                            {
                                shape.offset = ComputeOffset(
                                    nodeLocalIndex / distribution.clusterCount,
                                    nodeLocalCount / distribution.clusterCount,
                                    distributionSum,
                                    distributionStep,
                                    distribution
                                );
                            }

                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        switch (distribution.radial.Value)
                        {
                            case DistributionRadialMode.Random:
                            {
                                shape.radialAngle = seed.RandomValue() * 360.0f;
                            }
                                break;
                            case DistributionRadialMode.Equal:
                            {
                                var step = nodeLocalIndex / distribution.clusterCount;

                                var element = (nodeLocalIndex % distribution.clusterCount) /
                                              (float)distribution.clusterCount;

                                var elementOffset = 360f * element;

                                shape.radialAngle = step * distribution.radialStepOffset;

                                if (distribution.radialStepJitter == null)
                                {
                                    distribution.radialStepJitter = TreeProperty.New(0.1f);
                                }

                                if (distribution.radialStepJitter > 0.0f)
                                {
                                    var jitter = seed.RandomValue(-0.5f, 0.5f) *
                                                 distribution.radialStepJitter *
                                                 distribution.radialStepOffset;

                                    shape.radialAngle += jitter;
                                }

                                shape.radialAngle += elementOffset;
                            }
                                break;

                            case DistributionRadialMode.Range:
                            {
                                var range = distribution.growthRange.Value;

                                var angle = range.x + (seed.RandomValue() * (range.y - range.x));
                                shape.radialAngle = angle;
                            }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (distribution.randomDistributionAngleOffset)
                        {
                            shape.radialAngle += randomDistributionOffset;
                        }
                        else
                        {
                            if (distribution.distributionAngleOffsetJitter == null)
                            {
                                distribution.distributionAngleOffsetJitter = TreeProperty.New(0.1f);
                            }

                            if (distribution.distributionAngleOffsetJitter > 0.0f)
                            {
                                var jitter = seed.RandomValue(-360.0f, 360.0f) *
                                             distribution.distributionAngleOffsetJitter;

                                shape.radialAngle += jitter;
                            }

                            shape.radialAngle += distribution.distributionAngleOffset.Value;
                        }

                        shape.radialAngle +=
                            distribution.distributionSpin.Value.EvaluateScaled(shape.offset) * 360.0f;
                    }

                    if (shape.type == TreeComponentType.Root)
                    {
                        var groundOffset = hierarchies.GetVerticalOffset();
                        var heightRange = new Vector2(-groundOffset, 1f);

                        var parent = shapes.GetShapeData(shape.parentShapeID) as BarkShapeData;
                        var parentLength = SplineModeler.GetApproximateLength(parent.spline);

                        var newRange = heightRange.y - heightRange.x;

                        shape.offset *= newRange / parentLength;
                    }
                    else if (shape.type == TreeComponentType.Fungus)
                    {
                        var groundOffset = hierarchies.GetVerticalOffset();

                        var parent = shapes.GetShapeData(shape.parentShapeID) as BarkShapeData;
                        var parentLength = SplineModeler.GetApproximateLength(parent.spline);

                        var low = (fungiRange.x + groundOffset) / parentLength;
                        var high = (fungiRange.y + groundOffset) / parentLength;

                        shape.offset = low + (shape.offset * (high - low));
                    }
                    else if ((hierarchy.type == TreeComponentType.Branch) &&
                             hierarchy is BranchHierarchyData b &&
                             b.geometry.forked)
                    {
                        shape.offset = 1.0f;
                    }
                }
            }
        }

        public static void UpdateShapeVisibility(IShapeReadWrite shapes, HierarchyData hierarchy, ISeed seed)
        {
            using (BUILD_TIME.DIST_MGR.UpdateShapeVisibility.Auto())
            {
                var shapeDatas = shapes.GetShapesByHierarchy(hierarchy);

                foreach (var shape in shapeDatas)
                {
                    var parentShape = shapes.GetShapeData(shape.parentShapeID);

                    shape.visible = true;

                    if (hierarchy.hidden)
                    {
                        shape.hidden = true;
                    }
                    else if ((parentShape != null) && !parentShape.visible)
                    {
                        shape.visible = false;
                    }

                    if (shape.visible)
                    {
                        if ((parentShape != null) && parentShape is BarkShapeData barkShape)
                        {
                            if (shape.offset > barkShape.breakOffset)
                            {
                                shape.visible = false;
                            }
                        }
                    }

                    if (shape.visible)
                    {
                        shape.visible = seed.RandomValue() < shape.distributionLikelihood;
                    }
                }
            }
        }
    }
}
