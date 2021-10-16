using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine; //using Appalachia.Simulation.Trees.Runtime.Settings;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    public static class BranchRingGenerator
    {
        private static int GetAdaptiveRadialSegments(float r, float adaptiveQuality, float height)
        {
            var segs = ((int) (r * 24 * adaptiveQuality) / 2) * 2;
            var heightVal = Mathf.Clamp(10 - height, 4, 10);
            return Mathf.Clamp(segs, (int) heightVal, 32);
        }


        public static void WeldBranchBaseToParentGeometry(
            LODGenerationOutput output,
            CollaredBarkHierarchyData hierarchy,
            BarkShapeData shape,
            BarkShapeData parentShape,
            float weldRadius)
        {
            var rings = shape.branchRings[output.lodLevel];
            
            var vertexStart = rings[0].vertexStart;
            var vertexEnd = rings[0].vertexEnd;

            var ringSize = vertexEnd - vertexStart - 1;

            var originalPositions = new Vector3[ringSize];
            var originalNormals = new Vector3[ringSize];

            for (var i = 0; i < ringSize; i++)
            {
                var vertex = output.vertices[vertexStart + i];
                originalPositions[i] = vertex.position;
                originalNormals[i] = vertex.normal;
            }

            var updatePositions = true;
            var updateNormals = false;

            ShapeWelder.WeldGeometryToParent(
                output,
                shape,
                parentShape,
                vertexStart,
                vertexEnd,
                shape.effectiveMatrix,
                weldRadius * 4,
                updatePositions,
                updateNormals
            );


            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (updatePositions || updateNormals)
            {
                var outputVertex = output.vertices[vertexEnd - 1];
                            
                if (updatePositions)
                {
                    outputVertex.position = output.vertices[vertexStart].position;
                }

                if (updateNormals)
                {
                    outputVertex.normal = output.vertices[vertexStart].normal;
                }

                output.vertices[vertexEnd - 1] = outputVertex;
            }
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            // ReSharper restore HeuristicUnreachableCode
            
            var positionShifts = new Vector3[ringSize];
            var normalShifts = new Vector3[ringSize];

            for (var i = 0; i < positionShifts.Length; i++)
            {
                positionShifts[i] = output.vertices[vertexStart + i].position - originalPositions[i];
                normalShifts[i] = output.vertices[vertexStart + i].normal - originalNormals[i];
            }

            if (hierarchy.collar.weldSize == null)
            {
                hierarchy.collar.weldSize = TreeProperty.New(4f);
            }
            else if (hierarchy.collar.weldSize.Value == 0f)
            {
                hierarchy.collar.weldSize.SetValue(4f);
            }
            
            if (hierarchy.collar.weldNormalBlendSize == null)
            {
                hierarchy.collar.weldNormalBlendSize = TreeProperty.New(4f);
            }
            else if (hierarchy.collar.weldNormalBlendSize.Value == 0.0f)
            {
                hierarchy.collar.weldNormalBlendSize.SetValue(4.0f);
            }
            
            if (hierarchy.collar.weldNormalBlend == null)
            {
                hierarchy.collar.weldNormalBlend = TreeProperty.New(.5f);
            }
            else if (hierarchy.collar.weldNormalBlend.Value == 0.0f)
            {
                hierarchy.collar.weldNormalBlend.SetValue(0.5f);
            }

            var blendDistance = parentShape.size * hierarchy.collar.weldSize;
            var origin = shape.effectiveMatrix.MultiplyPoint(Vector3.zero);

            for (var ringIndex = 1; ringIndex < rings.Count; ringIndex++)
            {
                vertexStart = rings[ringIndex].vertexStart;
                vertexEnd = rings[ringIndex].vertexEnd;

                var range = vertexEnd - (float) vertexStart - 1;

                for (var i = 0; i <= range; i++)
                {
                    var ringTime = i / range;
                    var originalIndex = (int) Mathf.Clamp(
                        ringTime * positionShifts.Length,
                        0,
                        positionShifts.Length - 1
                    );

                    var positionShift = positionShifts[originalIndex];
                    var normalShift = normalShifts[originalIndex];

                    var outputVertex = output.vertices[i + vertexStart];
                    var basePosition = outputVertex.position;
                    var distance = (basePosition - origin).magnitude;

                    var time = distance / blendDistance;

                    if (time > 1)
                    {
                        break;
                    }

                    var strength = 1 - time;

                    // ReSharper disable ConditionIsAlwaysTrueOrFalse
                    // ReSharper disable HeuristicUnreachableCode
                    if (updatePositions && (ringIndex > 0))
                    {
                        outputVertex.position += strength * positionShift;
                    }

                    if (updateNormals)
                    {
                        outputVertex.normal += strength * normalShift;
                    }

                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    // ReSharper restore HeuristicUnreachableCode
                    output.vertices[i + vertexStart] = outputVertex;
                }
            }

            for (var ringIndex = 0; ringIndex < rings.Count; ringIndex++)
            {
                var ring = rings[ringIndex];

                var average = output.vertices.AveragePosition(ring.vertexStart, ring.vertexEnd);

                var localAverage = ring.matrix.inverse.MultiplyPoint(average);

                ring.matrix *= Matrix4x4.Translate(localAverage);
            }
        }

        public static void AdjustBranchBaseNormals(
            LODGenerationOutput output,
            CollaredBarkHierarchyData hierarchy,
            BarkShapeData shape,
            BarkShapeData parentShape)
        {
            
            var normalBlendSize = hierarchy.collar.weldNormalBlendSize.Value;
            var normalBlend = hierarchy.collar.weldNormalBlend.Value;
            
            if ((normalBlendSize == 0.0f) || (normalBlend == 0.0f))
            {
                return;
            }

            var rings = shape.branchRings[output.lodLevel];
            
            var vertexStart = rings[0].vertexStart;
            var vertexEnd = rings[0].vertexEnd;
            
            var blendDistance = parentShape.size * normalBlendSize;
            var origin = shape.effectiveMatrix.MultiplyPoint(Vector3.zero);

            var up = shape.effectiveMatrix.MultiplyVector(Vector3.up);

            for (var ringIndex = 0; ringIndex < rings.Count; ringIndex++)
            {
                vertexStart = rings[ringIndex].vertexStart;
                vertexEnd = rings[ringIndex].vertexEnd;

                var range = vertexEnd - (float) vertexStart - 1;

                for (var i = 0; i < range; i++)
                {
                    var outputVertex = output.vertices[i + vertexStart];
                    var basePosition = outputVertex.position;
                    var distance = (basePosition - origin).magnitude;

                    var time = distance / blendDistance;

                    if (time > 1)
                    {
                        break;
                    }

                    var strength = 1 - time;

                    outputVertex.normal = Vector3.Lerp(
                        outputVertex.normal,
                        up,
                        normalBlend*strength
                    );

                    output.vertices[i + vertexStart] = outputVertex;
                }

                var treeVertex = output.vertices[vertexEnd - 1];
                treeVertex.normal = output.vertices[vertexStart].normal;
                output.vertices[vertexEnd - 1] = treeVertex;
            }
        }
        
        public static void GenerateBranchRings(
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            BarkShapeData shape,
            BarkShapeData parentShape,
            BarkHierarchyData hierarchy,
            List<SplineHeightSample> heightSamples,
            float lodQuality,
            float totalHeight)
        {
            shape.branchRings[output.lodLevel] = new List<BranchRing>();
            var rings = shape.branchRings[output.lodLevel];

            BranchRing modelRing = null;

            if (output.lodLevel != 0)
            {
                //modelRing = shape.branchRings[0][0];
            }

            var childHierarchies = hierarchies.GetHierarchiesByParent(hierarchy.hierarchyID);

            var finishCap = true;

            if (childHierarchies != null)
            {
                finishCap = !childHierarchies.Any(ch => ch is BarkHierarchyData bhd && bhd.geometry.forked);
            }

            var currentHeight = shape.effectiveMatrix
                .MultiplyPoint(SplineModeler.GetPositionAtTime(shape.spline, shape.offset))
                .y;

            var radialSamples = GetAdaptiveRadialSegments(shape.effectiveSize, lodQuality, currentHeight);

            var uvOffset = new Vector2(1, 0);

            var capStart = 1.0f - shape.capRange;

            var pastBreakThreshold = false;

            for (var i = 0; i < heightSamples.Count; i++)
            {
                var heightOffset = heightSamples[i].height;

                //if (shape.variantIndicator)
                {
                    if (heightOffset > shape.breakOffset)
                    {
                        pastBreakThreshold = true;
                    }
                }

                var position = SplineModeler.GetPositionAtTime(shape.spline, heightOffset);
                var rotation = SplineModeler.GetRotationAtTime(shape.spline, heightOffset);
                var rad = SplineModeler.GetRadiusAtTime(shape, hierarchy, heightOffset);

                var effectiveMatrix = shape.effectiveMatrix * Matrix4x4.TRS(position, rotation, Vector3.one);

                var flareRadius = 0.0f;
                var baseTopSpread = 0.0f;
                var baseBottomSpread = 0.0f;
                
                float flareNoiseStrength = 0.0f;
                float flareNoiseScaleU = 0.0f;
                float flareNoiseScaleV = 0.0f;
                float flareNoiseDepth = 0.0f;
                
                var trunk = hierarchy as TrunkHierarchyData;
                
                if (!hierarchy.geometry.forked)
                {
                    var collar = SplineModeler.GetCollarAtTime(hierarchies, shape, hierarchy, heightOffset);

                    flareRadius = collar.x;
                    baseTopSpread = collar.y;
                    baseBottomSpread = collar.z;

                    flareNoiseStrength = hierarchy.type == TreeComponentType.Trunk
                        ? trunk.trunk.flareNoise
                        : 0f;

                    if (trunk != null)
                    {
                        if (trunk.trunk.flareDepth == null)
                        {
                            trunk.trunk.flareDepth = TreeProperty.New(0.0f);
                        }

                        if (trunk.trunk.noiseScaleU == null)
                        {
                            trunk.trunk.noiseScaleU = TreeProperty.New(0.9f);
                        }

                        if (trunk.trunk.noiseScaleV == null)
                        {
                            trunk.trunk.noiseScaleV = TreeProperty.New(0.05f);
                        }

                        if (trunk.trunk.noiseScaleU == 0.0f)
                        {
                            trunk.trunk.noiseScaleU = trunk.curvature.noiseScaleU;
                        }

                        if (trunk.trunk.noiseScaleV == 0.0f)
                        {
                            trunk.trunk.noiseScaleV = trunk.curvature.noiseScaleV;
                        }

                        flareNoiseScaleU = trunk.trunk.noiseScaleU;
                        flareNoiseScaleV = trunk.trunk.noiseScaleV;
                    }
                    
                    flareNoiseDepth = hierarchy.type == TreeComponentType.Trunk
                        ? trunk.trunk.flareDepth * flareRadius
                        : 0f;
                }

                if (heightOffset <= capStart)
                {
                    var additiveRadius = Mathf.Max(flareRadius, Mathf.Max(baseTopSpread, baseBottomSpread) * 0.25f);
                    
                    radialSamples = GetAdaptiveRadialSegments(rad + additiveRadius, lodQuality, currentHeight);
                }
                
                if (trunk != null)
                {
                    var flareTime = SplineModeler.GetTrunkFlareTime(hierarchies, shape, hierarchy, heightOffset);
                        
                    radialSamples = (int)(Mathf.Lerp(
                        radialSamples,
                        radialSamples * trunk.trunk.flareResolution,
                        flareTime
                    ));
                }

                if (i > 0)
                {
                    var preT = heightSamples[i - 1].height;
                    var uvDelta = heightOffset - preT;
                    var uvRad = (rad + SplineModeler.GetRadiusAtTime(shape, hierarchy, preT)) * 0.5f;

                    uvRad = Mathf.Clamp(uvRad, .0001f, 1000f);

                    uvOffset.y += (uvDelta * totalHeight) / (uvRad * Mathf.PI * 2.0f);
                }

                var surfaceAngle = SplineModeler.GetSurfaceAngleAtTime(shape, hierarchy, heightOffset);

                var noiseStrength = Mathf.Clamp01(hierarchy.curvature.noise.Value.EvaluateScaled(heightOffset));

                var noiseScaleU = hierarchy.curvature.noiseScaleU * 10.0f;
                var noiseScaleV = hierarchy.curvature.noiseScaleV * 10.0f;

                var generateRing = (!heightSamples[i].ignored && (shape.breakInverted && pastBreakThreshold)) ||
                    (!shape.breakInverted && !pastBreakThreshold);


                if (!generateRing)
                {
                    continue;
                }

                if ((i == 0) && (modelRing != null))
                {
                    effectiveMatrix = modelRing.matrix;
                    rad = modelRing.radius;
                    uvOffset = modelRing.uvOffset;
                    radialSamples = modelRing.segments;
                    noiseStrength = modelRing.noiseStrength;
                    noiseScaleU = modelRing.noiseScaleU;
                    noiseScaleV = modelRing.noiseScaleV;
                    
                    flareRadius = modelRing.flareRadius;
                    baseTopSpread = modelRing.weldTop;
                    baseBottomSpread = modelRing.weldBottom;

                }
                else if (hierarchy.geometry.forked && (i == 0))
                {
                    var parentRings = parentShape.branchRings[output.lodLevel];
                    
                    var parentRing = parentRings[parentRings.Count - 1];

                    if (parentRing.radius < .01f)
                    {
                        parentRing =  parentRings[parentRings.Count - 2];
                    }
                    
                    effectiveMatrix.SetTRS(
                        parentRing.matrix.MultiplyPoint(Vector3.zero),
                        effectiveMatrix.rotation,
                        effectiveMatrix.lossyScale
                    );

                    rad = parentRing.radius;
                    uvOffset = parentRing.uvOffset;
                    radialSamples = parentRing.segments;
                    
                    flareRadius = 0.0f;
                    baseTopSpread = 0.0f;
                    baseBottomSpread = 0.0f;
                    
                    noiseStrength = parentRing.noiseStrength;
                    noiseScaleU = parentRing.noiseScaleU;
                    noiseScaleV = parentRing.noiseScaleV;
                }

                BranchRing branchRing = new BranchRing(
                    rad,
                    effectiveMatrix,
                    uvOffset,
                    radialSamples,
                    surfaceAngle,
                    baseTopSpread,
                    baseBottomSpread,
                    noiseStrength,
                    noiseScaleU,
                    noiseScaleV,
                    flareRadius,
                    flareNoiseStrength,
                    flareNoiseScaleU,
                    flareNoiseScaleV,
                    flareNoiseDepth,
                    hierarchy.hierarchyID,
                    shape.shapeID,
                    shape.type
                );

                branchRing.BuildVertices(output, shape, hierarchy, heightOffset);

                if (hierarchy.geometry.forked && (i == 0))
                {
                    var main = output.vertices[branchRing.vertexStart];

                    var parentRings = parentShape.branchRings[output.lodLevel];
                    
                    var parentRing = parentRings[parentRings.Count - 1];

                    if (parentRing.radius < .01f)
                    {
                        parentRing =  parentRings[parentRings.Count - 2];
                    }

                    var matches = new (int x, int y)[branchRing.segments];

                    for (var k = 0; k < matches.Length; k++)
                    {
                        matches[k].x = k;
                        matches[k].y = k;
                    }

                    var nearest = ShapeWelder.GetMostSimilarByNormal(
                        main.normal,
                        output.vertices,
                        parentRing.vertexStart,
                        parentRing.vertexEnd
                    );

                    nearest -= parentRing.vertexStart;

                    for (var k = 0; k < matches.Length; k++)
                    {
                        matches[k].y += nearest;
                    }

                    for (var k = 0; k < matches.Length; k++)
                    {
                        if (matches[k].y >= matches.Length)
                        {
                            matches[k].y -= matches.Length;
                        }
                    }

                    for (var k = 0; k < matches.Length; k++)
                    {
                        var local = output.vertices[branchRing.vertexStart + matches[k].x];
                        var parent = output.vertices[parentRing.vertexStart + matches[k].y];

                        if (k == 0)
                        {
                            uvOffset.y = parent.raw_uv0.y;
                            uvOffset.x = parent.raw_uv0.x;
                        }

                        /*
                        local.raw_uv0.x = local.raw_uv0.x - uvOffset.x;
                        local.raw_uv0.y = uvOffset.y;
                        */

                        local.raw_uv0 = parent.raw_uv0;

                        if (local.raw_uv0.x > uvOffset.x)
                        {
                            local.raw_uv0.x -= 1;
                        }

                        local.position = parent.position;
                        local.normal = parent.normal;
                        local.tangent = parent.tangent;
                    }
                }

                var last = output.vertices[branchRing.vertexEnd - 1];
                var first = output.vertices[branchRing.vertexStart];

                last.position = first.position;
                last.normal = first.normal;
                last.tangent = first.tangent;
                last.raw_uv0.y = first.raw_uv0.y;
                last.raw_uv0.x = first.raw_uv0.x - 1;
                rings.Add(branchRing);

                if (finishCap && (heightOffset >= 1.0f) && (branchRing.radius > 0.001f))
                {
                    var r2 = branchRing.Clone();
                    r2.radius = 0.0f;
                    r2.uvOffset.y += rad / (Mathf.PI * 2.0f);
                    r2.BuildVertices(output, shape, hierarchy, heightOffset);
                    rings.Add(r2);
                }
            }
        }

        private const float _MIN_BREAK_CAP_RADIUS = 0.005f;

        public static void HandleCappingBrokenBranch(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            BarkShapeData shape,
            BarkHierarchyData hierarchy)
        {
            var rings = shape.branchRings[output.lodLevel];
            
            if (!(rings[rings.Count - 1].radius > _MIN_BREAK_CAP_RADIUS))
            {
                return;
            }

            /*
            var capMaterial = hierarchy.geometry.barkMaterial;
            var capMaterialStyle = TreeMaterialUsage.SplineBreak;
            var capStyle = LimbBreakUVMappingStyle.FollowBranch;
            */

            if (Math.Abs(hierarchy.limb.limbBreakTextureScale - 0.8f) < float.Epsilon)
            {
                hierarchy.limb.limbBreakTextureScale.SetValue(1.0f);
            }
            
            var uvRange = hierarchy.limb.limbBreakTextureScale;
            var capNoise = hierarchy.limb.breakRandomness;
            var capNoiseScale = hierarchy.limb.breakRandomnessScale;
            var capSpherical = hierarchy.limb.breakSphericalCap;
            var beaverFactor = hierarchy.limb.breakDepth;

            //var mappingScale = shape.effectiveScale * shape.effectiveSize * Mathf.PI * 2.0f;

            /*if (hierarchy.limb.breakMaterial != null || hierarchy.settingsType == ResponsiveSettingsType.Log)
            {
                capMaterial = hierarchy.limb.breakMaterial;
                capMaterialStyle = TreeMaterialUsage.SplineBreak;
                capStyle = LimbBreakUVMappingStyle.EndCap;
            }*/

            var m = inputMaterialCache?.GetInputMaterialData(
                hierarchy.geometry.barkMaterial,
                TreeMaterialUsage.SplineBreak
            );

            var materialID = -1;

            if (m != null)
            {
                materialID = m.materialID;
            }

            var ringA = rings[rings.Count - 1];
            var ringB = rings[rings.Count - 2];

            var averageA = output.vertices.AveragePosition(ringA.vertexStart, ringA.vertexEnd);
            var averageB = output.vertices.AveragePosition(ringB.vertexStart, ringB.vertexEnd);

            var breakNormal = (averageA - averageB).normalized;
            
            rings[rings.Count - 1]
                .FinishBrokenBranch(
                    output,
                    hierarchy,
                    shape,
                    breakNormal,
                    capSpherical,
                    capNoise,
                    capNoiseScale,
                    //capStyle,
                    //capMaterialStyle,
                    //mappingScale,
                    materialID,
                    uvRange,
                    shape.breakOffset,
                    false,
                    beaverFactor
                );
        }

        public static void HandleTrunkCut(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            TreeVariantSettings variantSettings,
            BarkShapeData shape,
            BarkHierarchyData hierarchy,
            bool stumpSide)
        {
            //var mat = hierarchy.geometry.barkMaterial;
            //var style = TreeMaterialUsage.Bark;
            //var mappingStyle = LimbBreakUVMappingStyle.FollowBranch;
            var capNoise = variantSettings.trunkCutRandomness;
            var capNoiseScale = variantSettings.trunkCutRandomnessScale;
            var capSpherical = variantSettings.trunkCutSphericalCap;
            //var mappingScale = shape.effectiveScale * shape.effectiveSize * Mathf.PI * 2.0f;

            /*if (hierarchy.limb.breakMaterial != null)
            {
                mat = hierarchy.limb.breakMaterial;
                style = TreeMaterialUsage.SplineBreak;
                mappingStyle = LimbBreakUVMappingStyle.EndCap;
                uvRange = variantSettings.trunkCutTextureScale;
            }*/

            var inputMaterial = inputMaterialCache?.GetInputMaterialData(hierarchy.geometry.barkMaterial, TreeMaterialUsage.SplineBreak);

            var materialID = -1;

            if (inputMaterial != null)
            {
                materialID = inputMaterial.materialID;
            }

            var rings = shape.branchRings[output.lodLevel];
            
            if (stumpSide)
            {
                var ringA = rings[rings.Count - 1];
                var ringB = rings[rings.Count - 2];

                var averageA = output.vertices.AveragePosition(ringA.vertexStart, ringA.vertexEnd);
                var averageB = output.vertices.AveragePosition(ringB.vertexStart, ringB.vertexEnd);

                var breakNormal = (averageA - averageB).normalized;

                rings[rings.Count - 1]
                    .FinishBrokenBranch(
                        output,
                        hierarchy,
                        shape,
                        breakNormal,
                        capSpherical,
                        capNoise,
                        capNoiseScale,
                        /*mappingStyle,
                        style,
                        mappingScale,*/
                        materialID,
                        1,
                        shape.breakOffset,
                        false,
                        variantSettings.trunkCutDepth,
                        variantSettings.trunkRingResolution,
                        variantSettings.trunkSegmentResolution
                    );
            }
            else
            {
                var ringA = rings[1];
                var ringB = rings[0];

                var averageA = output.vertices.AveragePosition(ringA.vertexStart, ringA.vertexEnd);
                var averageB = output.vertices.AveragePosition(ringB.vertexStart, ringB.vertexEnd);

                var breakNormal = (averageB - averageA).normalized;
                
                rings[0]
                    .FinishBrokenBranch(
                        output,
                        hierarchy,
                        shape,
                        breakNormal,
                        capSpherical,
                        capNoise,
                        capNoiseScale,
                        /*mappingStyle,
                        style,
                        mappingScale,*/
                        materialID,
                        1.0f,
                        shape.breakOffset,
                        true,
                        variantSettings.trunkCutDepth,
                        variantSettings.trunkRingResolution,
                        variantSettings.trunkSegmentResolution
                    );
            }
        }
        

        public static void HandleCappingLog(
            LODGenerationOutput output,
            BarkShapeData shape,
            BarkHierarchyData hierarchy)
        {
            var rings = shape.branchRings[output.lodLevel];
            
            if (!(rings[rings.Count - 1].radius > 0.015f))
            {
                return;
            }

            //var mappingScale = shape.effectiveScale * shape.effectiveSize * Mathf.PI * 2.0f;

            //var capMaterialStyle = TreeMaterialUsage.SplineBreak;
            //var capStyle = LimbBreakUVMappingStyle.EndCap;
            
            if (Math.Abs(hierarchy.limb.limbBreakTextureScale - 0.8f) < float.Epsilon)
            {
                hierarchy.limb.limbBreakTextureScale.SetValue(1.0f);
            }
            
            var uvRange = hierarchy.limb.limbBreakTextureScale;
            var capNoise = hierarchy.limb.breakRandomness;
            var capNoiseScale = hierarchy.limb.breakRandomnessScale;
            var capSpherical = hierarchy.limb.breakSphericalCap;
            var beaverFactor = hierarchy.limb.breakDepth;

            var materialID = -1;

            var ringA = rings[rings.Count - 1];
            var ringB = rings[rings.Count - 2];

            var averageA = output.vertices.AveragePosition(ringA.vertexStart, ringA.vertexEnd);
            var averageB = output.vertices.AveragePosition(ringB.vertexStart, ringB.vertexEnd);

            var breakNormal = (averageA - averageB).normalized;

            rings[rings.Count - 1]
                .FinishBrokenBranch(
                    output,
                    hierarchy,
                    shape,
                    breakNormal,
                    capSpherical,
                    capNoise,
                    capNoiseScale,
                    /*capStyle,
                    capMaterialStyle,
                    mappingScale,*/
                    materialID,
                    uvRange,
                    shape.breakOffset,
                    false,
                    beaverFactor
                );

            ringA = rings[1];
            ringB = rings[0];

            averageA = output.vertices.AveragePosition(ringA.vertexStart, ringA.vertexEnd);
            averageB = output.vertices.AveragePosition(ringB.vertexStart, ringB.vertexEnd);

            breakNormal = (averageB - averageA).normalized;

            rings[0]
                .FinishBrokenBranch(
                    output,
                    hierarchy,
                    shape,
                    breakNormal,
                    capSpherical,
                    capNoise,
                    capNoiseScale,
                    /*capStyle,
                    capMaterialStyle,
                    mappingScale,*/
                    materialID,
                    uvRange,
                    shape.breakOffset,
                    true,
                    beaverFactor
                );

        }
        
        public static void CloseTrunkBase(
            LODGenerationOutput output,
            TrunkShapeData shape,
            TrunkHierarchyData hierarchy)
        {
            var rings = shape.branchRings[output.lodLevel];
           
            var uvRange = hierarchy.limb.limbBreakTextureScale;
            var capNoise = hierarchy.limb.breakRandomness;
            var capNoiseScale = hierarchy.limb.breakRandomnessScale;
            var capSpherical = hierarchy.limb.breakSphericalCap;
            var beaverFactor = hierarchy.limb.breakDepth;

            var materialID = -1;

            var ringA = rings[1];
            var ringB = rings[0];
            var averageA = output.vertices.AveragePosition(ringA.vertexStart, ringA.vertexEnd);
            var averageB = output.vertices.AveragePosition(ringB.vertexStart, ringB.vertexEnd);
            var breakNormal = (averageB - averageA).normalized;

            rings[0]
                .FinishBrokenBranch(
                    output,
                    hierarchy,
                    shape,
                    breakNormal,
                    capSpherical,
                    capNoise,
                    capNoiseScale,
                    materialID,
                    uvRange,
                    shape.breakOffset,
                    true,
                    beaverFactor
                );

        }
    }
}
