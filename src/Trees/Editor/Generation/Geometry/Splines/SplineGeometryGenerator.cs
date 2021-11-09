using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Utility.Extensions;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    public static class SplineGeometryGenerator
    {
        public static void GenerateSplineGeometry(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings lodSettings,
            TreeVariantSettings variantSettings,
            BarkShapeData shape,
            BarkShapeData parentShape,
            BarkHierarchyData hierarchy,
            bool weld,
            float weldRadius)
        {
            using (BUILD_TIME.GEO_GEN.GenerateSplineGeometry.Auto())
            {
                var shapeQuality = 
                    shape.type switch
                    {
                        TreeComponentType.Branch => lodSettings.branchesGeometryQuality,
                        TreeComponentType.Trunk  => lodSettings.trunkGeometryQuality,
                        _                        => lodSettings.rootsGeometryQuality
                    };

                var lodQuality = Mathf.Clamp01(hierarchy.geometry.lodQualityMultiplier * shapeQuality);

                if (shape.type == TreeComponentType.Branch)
                {
                    var heightDropoff = lodSettings.heightQualityDropoff;

                    var height = shape.effectiveMatrix.MultiplyPoint(Vector3.zero).y;

                    if (height < heightDropoff.x)
                    {
                    }
                    else if ((height >= heightDropoff.x) && (height < heightDropoff.y))
                    {
                        var time = (height - heightDropoff.x) / (heightDropoff.y - heightDropoff.x);

                        lodQuality -= time;
                    }
                    else if (height >= heightDropoff.y)
                    {
                        lodQuality -= 1f;
                    }
                }

                lodQuality = Mathf.Clamp01(lodQuality);

                var heightSamples = SplineSampleManager.GetSamplePoints(hierarchies, shapes, shape, hierarchy, lodQuality);

                heightSamples = SplineSampleManager.CollapseHeightSamples(heightSamples);

                SplineSampleManager.ResampleSplineChildren(shapes, lodSettings, shape, heightSamples);

                if ((hierarchy.geometry.geometryMode == BranchGeometryMode.Branch) ||
                    (hierarchy.geometry.geometryMode == BranchGeometryMode.BranchFrond))
                {
                    GenerateBranchGeometry(
                        hierarchies,
                        output,
                        inputMaterialCache,
                        variantSettings,
                        shape,
                        parentShape,
                        hierarchy,
                        heightSamples,
                        lodQuality,
                        weld,
                        weldRadius
                    );
                }

                if ((hierarchy.geometry.geometryMode == BranchGeometryMode.Frond) ||
                    (hierarchy.geometry.geometryMode == BranchGeometryMode.BranchFrond))
                {
                    FrondGeometryGenerator.GenerateFrondGeometry(output, lodSettings, inputMaterialCache, shape, hierarchy, heightSamples);
                }
            }
        }
        
        public static void GenerateSplineGeometry(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LODGenerationOutput output,
            LevelOfDetailSettings lodSettings,
            BarkShapeData shape,
            BarkShapeData parentShape,
            BarkHierarchyData hierarchy,
            bool weld,
            float weldRadius)
        {
            using (BUILD_TIME.GEO_GEN.GenerateSplineGeometry.Auto())
            {
                var shapeQuality = 
                    shape.type switch
                    {
                        TreeComponentType.Branch => lodSettings.branchesGeometryQuality,
                        TreeComponentType.Trunk  => lodSettings.trunkGeometryQuality,
                        _                        => lodSettings.rootsGeometryQuality
                    };

                var lodQuality = Mathf.Clamp01(hierarchy.geometry.lodQualityMultiplier * shapeQuality);

                if (shape.type == TreeComponentType.Branch)
                {
                    var heightDropoff = lodSettings.heightQualityDropoff;

                    var height = shape.effectiveMatrix.MultiplyPoint(Vector3.zero).y;

                    if (height < heightDropoff.x)
                    {
                    }
                    else if ((height >= heightDropoff.x) && (height < heightDropoff.y))
                    {
                        var time = (height - heightDropoff.x) / (heightDropoff.y - heightDropoff.x);

                        lodQuality -= time;
                    }
                    else if (height >= heightDropoff.y)
                    {
                        lodQuality -= 1f;
                    }
                }

                lodQuality = Mathf.Clamp01(lodQuality);

                var heightSamples = SplineSampleManager.GetSamplePoints(hierarchies, shapes, shape, hierarchy, lodQuality);

                SplineSampleManager.ResampleSplineChildren(shapes, lodSettings, shape, heightSamples);

                heightSamples = SplineSampleManager.CollapseHeightSamples(heightSamples);

                if ((hierarchy.geometry.geometryMode == BranchGeometryMode.Branch) ||
                    (hierarchy.geometry.geometryMode == BranchGeometryMode.BranchFrond))
                {
                    GenerateBranchGeometry(
                        hierarchies,
                        output,
                        null,
                        null,
                        shape,
                        parentShape,
                        hierarchy,
                        heightSamples,
                        lodQuality,
                        weld,
                        weldRadius
                    );
                }

                if ((hierarchy.geometry.geometryMode == BranchGeometryMode.Frond) ||
                    (hierarchy.geometry.geometryMode == BranchGeometryMode.BranchFrond))
                {
                    FrondGeometryGenerator.GenerateFrondGeometry(output, lodSettings, null, shape, hierarchy, heightSamples);
                }
            }
        }

        private static void GenerateBranchGeometry(
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            TreeVariantSettings variantSettings,
            BarkShapeData shape,
            BarkShapeData parentShape,
            BarkHierarchyData hierarchy,
            List<SplineHeightSample> heightSamples,
            float lodQuality,
            bool weld,
            float weldRadius)
        {
            using (BUILD_TIME.GEO_GEN.GenerateBranchGeometry.Auto())
            {
                var totalHeight = SplineModeler.GetApproximateLength(shape.spline);

                /*if (!hierarchy.geometry.forked &&
                    !hierarchy.limb.disableWelding &&
                    output.lodLevel == 0)*/
                
                /*if (weld && !hierarchy.geometry.forked && output.lodLevel == 0)
                {
                    shape.effectiveMatrix = ShapeWelder.WeldShapeOriginToParent(
                        output,
                        shape,
                        parentShape,
                        shape.effectiveMatrix,
                        weldRadius * 2
                    );
                }*/

                var collared = hierarchy as CollaredBarkHierarchyData;

                if ((collared != null) && (collared.collar.disableWelding == null))
                {
                    if (collared.limb.disableWelding != null)
                    {
                        collared.collar.disableWelding = collared.limb.disableWelding;
                    }
                    else
                    {
                        collared.collar.disableWelding = TreeProperty.New(false);
                    }
                }
                
                if ((collared != null) && (collared.collar.weldSize == null))
                {
                    if (collared.limb.weldSize != null)
                    {
                        collared.collar.weldSize = collared.limb.weldSize;
                    }
                    else
                    {
                        collared.collar.weldSize = TreeProperty.New(4.0f);
                    }
                }
                
                if ((collared != null) && (collared.collar.weldNormalBlendSize == null))
                {
                    if (collared.limb.weldNormalBlendSize != null)
                    {
                        collared.collar.weldNormalBlendSize = collared.limb.weldNormalBlendSize;
                    }
                    else
                    {
                        collared.collar.weldNormalBlendSize = TreeProperty.New(4.0f);
                    }
                }
                
                if ((collared != null) && (collared.collar.weldNormalBlend == null))
                {
                    if (collared.limb.weldNormalBlend != null)
                    {
                        collared.collar.weldNormalBlend = collared.limb.weldNormalBlend;
                    }
                    else
                    {
                        collared.collar.weldNormalBlend = TreeProperty.New(.25f);
                    }
                }
                
                if (weld && 
                    (shape.type != TreeComponentType.Trunk) &&
                    (collared != null) &&
                    !collared.collar.disableWelding &&
                    !hierarchy.geometry.forked && 
                    (output.lodLevel == 0))
                {
                    var parentPoint = parentShape.effectiveMatrix.MultiplyPoint(
                        SplineModeler.GetPositionAtTime(parentShape.spline, shape.offset)
                    );

                    shape.effectiveMatrix.SetTRS(
                        parentPoint,
                        shape.effectiveMatrix.rotation,
                        shape.effectiveMatrix.lossyScale
                    );
                }

                if (shape.type == TreeComponentType.Trunk)
                {
                    var trunk = hierarchy as TrunkHierarchyData;

                    var vertical = hierarchies.GetVerticalOffset();
                    var length = SplineModeler.GetApproximateLength(shape.spline);

                    var flareEnd = trunk.trunk.flareHeight + vertical;

                    var flareEndTime = flareEnd / length;

                    heightSamples.Add(SplineHeightSample.At(flareEndTime));

                    if ((trunk.trunk.flareDensity == null) || (trunk.trunk.flareDensity.accessor < 1))
                    {
                        trunk.trunk.flareDensity = TreeProperty.New(1.5f); 
                    } 
                    
                    if ((trunk.trunk.flareResolution == null) || (trunk.trunk.flareResolution.accessor < 1))
                    {
                        trunk.trunk.flareResolution = TreeProperty.New(1.5f); 
                    }
                    
                    heightSamples = heightSamples.Distinct().ToList();

                    var density = trunk.trunk.flareDensity.accessor;

                    if (density > 1.0f)
                    {

                        var trunkIndices = heightSamples.ToReverseIndexLookup();
                        
                        var relevantSamples = heightSamples.Where(h => h.height < flareEndTime).ToArray();

                        var minSample = relevantSamples.Min();
                        var maxSample = relevantSamples.Max();
                        var sampleCount = relevantSamples.Length;

                        var removalIndices = new List<int>();

                        for (var i = 0; i < relevantSamples.Length; i++)
                        {
                            removalIndices.Add(trunkIndices[relevantSamples[i]]);
                        }

                        removalIndices.Sort();

                        for (var i = removalIndices.Count - 1; i >= 0; i--)
                        {
                            var index = removalIndices[i];

                            heightSamples.RemoveAt(index);
                        }
                        
                        var range = maxSample.height - minSample.height;
                        var newCount = Mathf.RoundToInt(sampleCount * density);
                        
                        var step = range / newCount;

                        for (var i = 0; i < newCount; i++)
                        {
                            var sample = step * i;

                            heightSamples.Add(SplineHeightSample.At(sample));
                        }
                    }
                }

                if (shape.breakOffset < 1.0f)
                {
                    if (shape.stageType.IsStumpOrFelled() && (shape.type == TreeComponentType.Trunk))
                    {
                        var offset = .5f * (variantSettings.trunkCutThickness / totalHeight);

                        heightSamples.Add(SplineHeightSample.TrunkCutStump(shape.breakOffset - offset));
                        heightSamples.Add(SplineHeightSample.TrunkCutTimber(shape.breakOffset + offset));
                    }
                    else
                    {
                        heightSamples.Add(SplineHeightSample.At(shape.breakOffset));
                    }
                }

                if ((collared != null) && (collared.collar.collarHeight > 0.001f))
                {
                    heightSamples.Add(SplineHeightSample.At(collared.collar.collarHeight));
                }

                using (BUILD_TIME.GEO_GEN.HeightSampleSort.Auto())
                {
                    heightSamples.Sort();
                    heightSamples = heightSamples.Distinct().ToList();
                }

                if (shape.stageType.IsStumpOrFelled() && (shape.type == TreeComponentType.Trunk))
                {
                    var engaged = false;

                    for (var i = 0; i < heightSamples.Count; i++)
                    {
                        var sample = heightSamples[i];

                        if (sample.trunkCutStump)
                        {
                            engaged = true;
                        }
                        else if (sample.trunkCutTimber)
                        {
                            break;
                        }
                        else if (engaged)
                        {
                            sample.ignored = true;
                            heightSamples[i] = sample;
                        }
                    }
                }
                
                using (BUILD_TIME.GEO_GEN.GenerateRingsFromSamples.Auto())
                {
                    BranchRingGenerator.GenerateBranchRings(
                        hierarchies,
                        output,
                        shape,
                        parentShape,
                        hierarchy,
                        heightSamples,
                        lodQuality,
                        totalHeight
                    );
                }

                var rings = shape.branchRings[output.lodLevel];

                if (rings.Count < 2)
                {
                    throw new NotSupportedException("Bad ring count!");
                }

                if ((shape.type != TreeComponentType.Trunk) && 
                    weld && 
                    (collared != null) &&
                    !collared.collar.disableWelding &&
                    !hierarchy.geometry.forked)
                {
                    BranchRingGenerator.WeldBranchBaseToParentGeometry(output, collared, shape, parentShape, weldRadius);
                }
                
                if ((shape.type != TreeComponentType.Trunk) && 
                    (collared != null) &&
                    !hierarchy.geometry.forked)
                {
                    BranchRingGenerator.AdjustBranchBaseNormals(output, collared, shape, parentShape);
                }

                if (hierarchy.limb.log)
                {
                    BranchRingGenerator.HandleCappingLog(output, shape, hierarchy);
                }
                else if (shape.breakOffset < 1.0f)
                {
                    if (shape.stageType.IsStumpOrFelled())
                    {
                        BranchRingGenerator.HandleTrunkCut(
                            output,
                            inputMaterialCache,
                            variantSettings,
                            shape,
                            hierarchy,
                            !shape.breakInverted
                        );
                    }
                    else
                    {
                        BranchRingGenerator.HandleCappingBrokenBranch(output, inputMaterialCache, shape, hierarchy);
                    }
                }

                var material = inputMaterialCache?.GetInputMaterialData(
                    hierarchy.geometry.barkMaterial,
                    TreeMaterialUsage.Bark
                );

                var mID = material?.materialID ?? -1;
                
                for (var i = 0; i < (rings.Count - 1); i++)
                {
                    var outputVertex = output.vertices[rings[i].vertexEnd - 1];
                    outputVertex.weldTo = rings[i].vertexStart;
                    output.vertices[rings[i].vertexEnd - 1] = outputVertex;

                    rings[i].ConnectLoops(rings[i + 1], output, shape, mID, false);
                }
                
                for (var i = 0; i < rings.Count; i++)
                {
                    rings[i].CloseRing(output);
                }

                if (hierarchy is TrunkHierarchyData thd &&
                    shape is TrunkShapeData tsd &&
                    !hierarchy.limb.log) 
                {
                    if ((thd.trunk.sinkDepth == null) || (thd.trunk.sinkDepth == 0.0f))
                    {
                        thd.trunk.sinkDepth = TreeProperty.New(1.5f);
                    }

                    if ((tsd.stageType == StageType.Normal) ||
                        (tsd.stageType == StageType.Dead) ||
                        (tsd.stageType == StageType.Stump) ||
                        (tsd.stageType == StageType.StumpRotted))
                    { 
                        
                        var lowestRing = rings[0];
                        
                        for (var i = lowestRing.vertexStart; i < lowestRing.vertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            /*vertex.position.x *= 1.2f;
                            vertex.position.z *= 1.2f;*/
                            vertex.position.x = 0.0f;
                            vertex.position.z = 0.0f;
                            vertex.position.y = -thd.trunk.sinkDepth;
                            vertex.raw_uv0.y -= thd.trunk.sinkDepth;
                        }
                    }
                  
                }
            }
        }
    }
}
