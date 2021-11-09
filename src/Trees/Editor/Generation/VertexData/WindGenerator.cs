using System;
using System.Collections.Generic;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Utility.Logging;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Appalachia.Simulation.Trees.Generation.VertexData
{
    public static class WindGenerator
    {
        public static void ApplyMeshWindData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            int geometryIndex,
            WindSettings wind,
            StageType stageType,
            BaseSeed seed)
        {
            using (BUILD_TIME.WIND_GEN.ApplyMeshWindData.Auto())
            {
                seed.Reset();

                var maxHierarchyDepth = 8;

                hierarchies.RecurseHierarchies(
                    data => { maxHierarchyDepth = Mathf.Max(maxHierarchyDepth, data.hierarchyDepth); }
                );

                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    vertex.wind = vertex.rawWind;
                    output.vertices[index] = vertex;
                }

                /*var octree = GenerateOctree(output);*/

                GeneratePrimaryWindData(shapes, hierarchies, output, geometryIndex, wind);

                GenerateSecondaryWindData(shapes, hierarchies, output, geometryIndex, wind);

                GenerateTertiaryWindData(shapes, hierarchies, output, wind);

                GeneratePhaseWindData(shapes, hierarchies, output, seed);

                ApplyStageType(output, stageType);
            }
        }

        private static void GeneratePrimaryWindData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            int geometryIndex,
            WindSettings wind)
        {
            using (BUILD_TIME.WIND_GEN.GeneratePrimaryWindData.Auto())
            {
                float windRangeY = 0f;

                foreach (var vertex in output.vertices)
                {
                    if (vertex.type == TreeComponentType.Trunk)
                    {
                        windRangeY = Mathf.Max(windRangeY, vertex.position.y);
                    }
                }

                var maxTrunk = 0f;

                shapes.RecurseSplines(
                    hierarchies,
                    data =>
                    {
                        if ((data.hierarchy.geometry.windStrength == null) ||
                            ((data.hierarchy.geometry.windStrength == 0.0f) && !data.hierarchy.geometry.disableWind))
                        {
                            data.hierarchy.geometry.windStrength = TreeProperty.New(1.0f);
                        }

                        if (!data.shape.IsTrunk)
                        {
                            return;
                        }

                        var length = SplineModeler.GetApproximateLength(data.shape.spline);

                        if (length > maxTrunk)
                        {
                            maxTrunk = length;
                        }
                    }
                );

                maxTrunk -= hierarchies.GetVerticalOffset();

                var cutoffHeight = maxTrunk * wind.trunkWindDeadZonePerMeter;

                var trunkDeadZone = Mathf.Clamp(
                    cutoffHeight,
                    wind.trunkWindDeadZoneVertical.x,
                    wind.trunkWindDeadZoneVertical.y
                );

                var windStartY = trunkDeadZone;
                windRangeY -= windStartY;

                float WindTime(float height)
                {
                    return Mathf.Clamp01((height - windStartY) / windRangeY);
                }

                var maxRollY = wind.trunkWindPerMeterVertical * windRangeY;

                float CalculateRoll(float height)
                {
                    if (height < windStartY)
                    {
                        return 0f;
                    }

                    return maxRollY * WindTime(height);
                }

                var trunkStrengthLookup = new Dictionary<int, float>();

                hierarchies.RecurseHierarchiesWithData(
                    data =>
                    {
                        //float windStrength = 0.0f;

                        if (data.hierarchy is TrunkHierarchyData thd)
                        {
                            trunkStrengthLookup.Add(thd.hierarchyID, thd.geometry.windStrength);
                        }
                        else
                        {
                            var parentStrength = trunkStrengthLookup[data.parentHierarchy.hierarchyID];
                            float thisStrength;

                            if (data.hierarchy is RootHierarchyData rhd)
                            {
                                thisStrength = 0.0f;
                            }
                            /*else if (data.hierarchy is BranchHierarchyData bhd)
                            {
                                thisStrength = bhd.geometry.windStrength;
                            }*/
                            else
                            {
                                thisStrength = 1.0f;
                            }

                            trunkStrengthLookup.Add(data.hierarchy.hierarchyID, parentStrength * thisStrength);
                        }
                    }
                );

                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    var roll = CalculateRoll(vertex.position.y);

                    roll *= trunkStrengthLookup[vertex.hierarchyID];

                    vertex.wind.primaryRoll = roll;
                    vertex.wind.primaryPivot = new Vector3(0, windStartY, 0);
                    output.vertices[index] = vertex;
                }

                foreach (var shape in shapes)
                {
                    if (shape is BarkShapeData bsd)
                    {
                        EnsureRingParity(bsd, output, v => v.wind.primaryRoll, (v, r) => v.wind.primaryRoll = r);
                    }
                }

                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.type == TreeComponentType.Root) return;

                        if (data.shape.geometry.Count <= geometryIndex)
                        {
                            return;
                        }

                        var geo = data.shape.geometry[geometryIndex];

                        if (geo.modelVertexStart == geo.modelVertexEnd)
                        {
                            return;
                        }

                        var p = data.parentShape as BarkShapeData;

                        TreeVertex testVert = default;

                        for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            if (testVert == default)
                            {
                                testVert = vertex;
                                continue;
                            }

                            if (vertex.heightOffset < testVert.heightOffset)
                            {
                                testVert = vertex;
                            }
                        }

                        var minimumPrimaryBend = 0f;

                        if (p != null)
                        {

                            var nearest = NearestTo(
                                output,
                                p.geometry[geometryIndex].modelVertexStart,
                                p.geometry[geometryIndex].modelVertexEnd,
                                testVert.position
                            );

                            if (nearest == default)
                            {
                                return;
                            }

                            minimumPrimaryBend = nearest.wind.primaryBend;
                        }

                        if (data.shape.IsRoot)
                        {
                            for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                            {
                                var vertex = output.vertices[i];

                                vertex.wind.primaryBend = 0f;
                            }

                            return;
                        }

                        if (!data.shape.IsSpline)
                        {
                            for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                            {
                                var vertex = output.vertices[i];

                                vertex.wind.primaryBend = minimumPrimaryBend;
                            }

                            return;
                        }

                        var barkShape = data.shape as BarkShapeData;
                        var barkHierarchy = data.hierarchy as BarkHierarchyData;

                        if (!barkShape.IsTrunk && !(barkShape.IsBranch && barkHierarchy.geometry.forked))
                        {
                            for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                            {
                                var vertex = output.vertices[i];

                                vertex.wind.primaryBend = minimumPrimaryBend;
                            }

                            return;
                        }

                        for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            if (vertex.position.y < windStartY)
                            {
                                vertex.wind.primaryBend = 0f;
                                continue;
                            }

                            var distanceToPivot = (vertex.position - vertex.wind.primaryPivot).magnitude;

                            if (wind.trunkWindPerMeterPower < 1.0f)
                            {
                                wind.trunkWindPerMeterPower = 1.015f;
                            }

                            var bend = Mathf.Pow(wind.trunkWindPerMeterPower, distanceToPivot) - 1;

                            bend *= trunkStrengthLookup[vertex.hierarchyID];

                            vertex.wind.primaryBend = bend;
                        }
                    }
                );

                foreach (var shape in shapes)
                {
                    if (shape is BarkShapeData bsd)
                    {
                        EnsureRingParity(bsd, output, v => v.wind.primaryBend, (v, r) => v.wind.primaryBend = r);
                    }
                }
            }
        }


        private static void GenerateSecondaryWindData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            int geometryIndex,
            WindSettings wind)
        {
            using (BUILD_TIME.WIND_GEN.GenerateSecondaryWindData.Auto())
            {
                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    
                    if ((vertex.type == TreeComponentType.Trunk) || (vertex.type == TreeComponentType.Root))
                    {
                        vertex.wind.secondaryRoll = 0f;
                    }

                    output.vertices[index] = vertex;
                }

                SetSecondaryRoll(shapes, hierarchies, output, geometryIndex, wind);
                SetSecondaryPivotData(shapes, hierarchies, output);
                SetSecondaryBend(shapes, hierarchies, output, geometryIndex, wind);
            }
        }

        private static void SetSecondaryRoll(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            int geometryIndex,
            WindSettings wind)
        {
            shapes.RecurseShapes(
                hierarchies,
                data =>
                {
                    if (data.type == TreeComponentType.Trunk) return;
                    if (data.type == TreeComponentType.Root) return;

                    if (data.shape.geometry.Count <= geometryIndex)
                    {
                        return;
                    }

                    var geo = data.shape.geometry[geometryIndex];

                    if (geo.modelVertexStart == geo.modelVertexEnd)
                    {
                        return;
                    }

                    var p = data.parentShape as BarkShapeData;

                    var parentRings = p.branchRings[output.lodLevel];

                    BranchRing closestParentRing = null;
                    var nearestDistance = Mathf.Infinity;

                    for (var i = 0; i < parentRings.Count; i++)
                    {
                        var ring = parentRings[i];
                        var distance = Mathf.Abs(ring.offset - data.shape.offset);

                        if (distance <= nearestDistance)
                        {
                            nearestDistance = distance;
                            closestParentRing = parentRings[i];
                        }
                    }

                    var sampleVertex = output.vertices[closestParentRing.vertexStart];

                    var minimumWindStrength = sampleVertex.wind.secondaryRoll;
                    var maximumWindStrength = minimumWindStrength;

                    for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                    {
                        var vertex = output.vertices[i];

                        var currentWindStrength = GetMaximiumWindAtPoint(
                            wind.branchWindPerMeterHorizontal,
                            wind.branchWindPerMeterVertical,
                            vertex.position,
                            wind.normalizeWind
                        );

                        if (currentWindStrength > maximumWindStrength)
                        {
                            maximumWindStrength = currentWindStrength;
                        }
                    }

                    for (var i = 0; i < data.hierarchy.hierarchyDepth; i++)
                    {
                        maximumWindStrength *= wind.branchLevelMultiplier;
                    }

                    if (data.hierarchy is BarkHierarchyData bh)
                    {
                        if ((bh.geometry.windStrength == null) ||
                            ((bh.geometry.windStrength == 0.0f) && !bh.geometry.disableWind))
                        {
                            bh.geometry.windStrength = TreeProperty.New(1.0f);
                        }

                        if (bh.geometry.disableWind)
                        {
                            maximumWindStrength = minimumWindStrength;
                        }
                        else
                        {
                            maximumWindStrength *= bh.geometry.windStrength;
                        }
                    }

                    for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                    {
                        var vertex = output.vertices[i];

                        var time = (1.1f * vertex.heightOffset) - .1f;

                        if (time <= 0f)
                        {
                            vertex.wind.secondaryRoll = minimumWindStrength;
                        }
                        else
                        {
                            vertex.wind.secondaryRoll = minimumWindStrength +
                                ((maximumWindStrength - minimumWindStrength) * time);
                        }
                    }

                    if (data.shape is BarkShapeData bsd)
                    {
                        EnsureRingParity(bsd, output, v => v.wind.secondaryRoll, (v, r) => v.wind.secondaryRoll = r);
                    }
                }
            );
        }

        private static void SetSecondaryPivotData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output)
        {

            var branchVectorsByShapeID = new Dictionary<int, (Vector3 vector, float verticality, Vector3 pivot)>();

            shapes.RecurseShapes(
                hierarchies,
                data =>
                {
                    if (branchVectorsByShapeID.ContainsKey(data.shape.shapeID))
                    {
                        return;
                    }

                    if (data.shape.type == TreeComponentType.Trunk)
                    {
                        return;
                    }

                    if (data.parentShape.type == TreeComponentType.Trunk)
                    {
                        var vector = data.shape.effectiveMatrix.MultiplyVector(Vector3.up);

                        var verticality = Vector3.Dot(vector, Vector3.up);

                        var pivot = data.shape.effectiveMatrix.MultiplyPoint(Vector3.zero);

                        branchVectorsByShapeID.Add(data.shape.shapeID, (vector, verticality, pivot));
                    }
                    else
                    {
                        branchVectorsByShapeID.Add(
                            data.shape.shapeID,
                            branchVectorsByShapeID[data.shape.parentShapeID]
                        );
                    }
                }
            );


            for (var i = 0; i < output.vertices.Count; i++)
            {
                var vertex = output.vertices[i];
                if (vertex.type == TreeComponentType.Trunk)
                {
                    vertex.wind.secondaryRoll = 0;
                    vertex.wind.secondaryForward = Vector3.zero;
                    vertex.wind.secondaryBend = 0;
                    vertex.wind.secondaryPivot = Vector3.zero;
                    vertex.wind.secondaryVerticality = 0;
                    continue;
                }

                if (!branchVectorsByShapeID.ContainsKey(vertex.shapeID))
                {
                    var message = "Could not find vertex's shape in wind shape lookup!";
                    AppaLog.Error(message);
                    throw new NotSupportedException(message);
                }

                var match = branchVectorsByShapeID[vertex.shapeID];

                vertex.wind.secondaryForward = new Vector3(match.vector.x, match.vector.y, match.vector.z);
                vertex.wind.secondaryVerticality = match.verticality;

                vertex.wind.secondaryPivot = new Vector3(match.pivot.x, match.pivot.y, match.pivot.z);
            }

            foreach (var shape in shapes)
            {
                if (shape is BarkShapeData bsd)
                {
                    EnsureRingParity(bsd, output, v => v.wind.secondaryForward, (v, r) => v.wind.secondaryForward = r);
                    EnsureRingParity(
                        bsd,
                        output,
                        v => v.wind.secondaryVerticality,
                        (v, r) => v.wind.secondaryVerticality = r
                    );
                    EnsureRingParity(bsd, output, v => v.wind.secondaryPivot, (v, r) => v.wind.secondaryPivot = r);
                }
            }
        }


        private static void SetSecondaryBend(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            int geometryIndex,
            WindSettings wind)
        {

            shapes.RecurseShapes(
                hierarchies,
                data =>
                {
                    if (data.type == TreeComponentType.Trunk) return;
                    if (data.type == TreeComponentType.Root) return;

                    if (data.shape.geometry.Count <= geometryIndex)
                    {
                        return;
                    }

                    var geo = data.shape.geometry[geometryIndex];

                    if (geo.modelVertexStart == geo.modelVertexEnd)
                    {
                        return;
                    }

                    var p = data.parentShape as BarkShapeData;

                    TreeVertex testVert = default;

                    for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                    {
                        var vertex = output.vertices[i];

                        if (testVert == default)
                        {
                            testVert = vertex;
                            continue;
                        }

                        if (vertex.heightOffset < testVert.heightOffset)
                        {
                            testVert = vertex;
                        }
                    }

                    var nearest = NearestTo(
                        output,
                        p.geometry[geometryIndex].modelVertexStart,
                        p.geometry[geometryIndex].modelVertexEnd,
                        testVert.position
                    );

                    if (nearest == null)
                    {
                        return;
                    }

                    if (!data.shape.IsBranch)
                    {
                        for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                        {
                            var vertex = output.vertices[i];

                            vertex.wind.secondaryBend = nearest.wind.secondaryBend;
                        }

                        return;
                    }

                    var barkShape = data.shape as BarkShapeData;
                    var barkHierarchy = data.hierarchy as BarkHierarchyData;

                    var minimumBend = nearest.wind.secondaryBend;

                    for (var i = geo.modelVertexStart; i < geo.modelVertexEnd; i++)
                    {
                        var vertex = output.vertices[i];
                        var pivot = new Vector3(
                            vertex.wind.secondaryPivot.x,
                            vertex.wind.secondaryPivot.y,
                            vertex.wind.secondaryPivot.z
                        );

                        var distanceToPivot = (vertex.position - pivot).magnitude;

                        var radius = SplineModeler.GetRadiusWithCollarAtTime(
                            hierarchies,
                            barkShape,
                            barkHierarchy,
                            vertex.heightOffset
                        );

                        var lengthMultiplier = Mathf.Pow(1f + wind.branchBendPerLengthM, distanceToPivot);

                        var radiusMultiplier = Mathf.Pow(1f - wind.branchBendDropPerRadiusM, radius);

                        var windStrength = 1.0f;

                        if (data.hierarchy is BarkHierarchyData bh)
                        {
                            if ((bh.geometry.windStrength == null) ||
                                ((bh.geometry.windStrength == 0.0f) && !bh.geometry.disableWind))
                            {
                                bh.geometry.windStrength = TreeProperty.New(1.0f);
                            }

                            if (bh.geometry.disableWind)
                            {
                                windStrength = 0.0f;
                            }
                            else
                            {
                                windStrength = bh.geometry.windStrength;
                            }
                        }

                        var bend = Mathf.Lerp(
                            minimumBend,
                            minimumBend + (wind.branchBendRange * lengthMultiplier * radiusMultiplier * windStrength),
                            vertex.heightOffset * 4f
                        );

                        vertex.wind.secondaryBend = bend;
                    }

                    EnsureRingParity(barkShape, output, v => v.wind.secondaryBend, (v, r) => v.wind.secondaryBend = r);
                }
            );

        }

        private static void GenerateTertiaryWindData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            WindSettings wind)
        {
            using (BUILD_TIME.WIND_GEN.GenerateTertiaryWindData.Auto())
            {
                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    
                    if (vertex.type != TreeComponentType.Leaf)
                    {
                        continue;
                    }

                    var windPosition = vertex.position.Abs();

                    windPosition.y = vertex.position.y;

                    var maxLeaf = GetMaximiumWindAtPoint(
                        wind.leafWindPerMeterHorizontal,
                        wind.leafWindPerMeterVertical,
                        windPosition,
                        wind.normalizeWind
                    );

                    vertex.wind.tertiaryRoll *= maxLeaf;
                    vertex.wind.tertiaryRoll *= vertex.billboard ? .5f : 1f;
                    output.vertices[index] = vertex;
                }
            }
        }

        private static void GeneratePhaseWindData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            BaseSeed seed)
        {
            using (BUILD_TIME.WIND_GEN.GeneratePhaseWindData.Auto())
            {
                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    if (vertex.type == TreeComponentType.Trunk)
                    {
                        vertex.wind.phase = 0f;
                        output.vertices[index] = vertex;
                    }
                }

                var phaseValuesByShapeID = new Dictionary<int, float>();

                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (phaseValuesByShapeID.ContainsKey(data.shape.shapeID))
                        {
                            return;
                        }

                        if (data.shape.type == TreeComponentType.Trunk)
                        {
                            return;
                        }

                        if (data.parentShape.type == TreeComponentType.Trunk)
                        {
                            data.hierarchy.seed.Reset();
                            var virtualSeed = new VirtualSeed(seed, data.hierarchy.seed);
                            phaseValuesByShapeID.Add(data.shape.shapeID, virtualSeed.RandomValue());
                        }
                        else
                        {
                            phaseValuesByShapeID.Add(
                                data.shape.shapeID,
                                phaseValuesByShapeID[data.shape.parentShapeID]
                            );
                        }
                    }
                );


                for (var i = 0; i < output.vertices.Count; i++)
                {
                    var vertex = output.vertices[i];

                    if (vertex.type == TreeComponentType.Trunk)
                    {
                        vertex.wind.phase = 0f;
                        continue;
                    }

                    if (!phaseValuesByShapeID.ContainsKey(vertex.shapeID))
                    {
                        var message = "Could not find vertex's shape in wind shape lookup!";
                        AppaLog.Error(message);
                        throw new NotSupportedException(message);
                    }

                    vertex.wind.phase = phaseValuesByShapeID[vertex.shapeID];
                }

                /*
                var shape = structure.GetShapeData(context.stage.individualID,
                    context.stage.ageType,
                    context.stage.stageType,
                    vertex.shapeID);

                var hierarchy = structure.GetHierarchyData(vertex.hierarchyID);

                vertex.wind.phase = hierarchy.animationPhase * shape.seed.phase;
                */

                /*
                if (vertex.type == TreeComponentType.Leaf)
                {
                    vertex.wind.phase *= shape.seed.RandomValue();
                }*/ //
            }
        }

        private static TreeVertex NearestTo(
            LODGenerationOutput output,
            int vertexStart,
            int vertexEnd,
            Vector3 position)
        {
            return NearestTo(output, vertexStart, vertexEnd, position, null);
        }

        private static TreeVertex NearestAbove(
            LODGenerationOutput output,
            int vertexStart,
            int vertexEnd,
            Vector3 position)
        {
            return NearestTo(output, vertexStart, vertexEnd, position, v => v.position.y >= position.y);
        }

        private static TreeVertex NearestBelow(
            LODGenerationOutput output,
            int vertexStart,
            int vertexEnd,
            Vector3 position)
        {
            return NearestTo(output, vertexStart, vertexEnd, position, v => v.position.y <= position.y);
        }

        private static TreeVertex NearestTo(
            LODGenerationOutput output,
            int vertexStart,
            int vertexEnd,
            Vector3 position,
            Predicate<TreeVertex> predicate)
        {
            TreeVertex nearest = default;
            var currentDistance = float.MaxValue;

            for (var i = vertexStart; i < vertexEnd; i++)
            {
                var newVert = output.vertices[i];

                if ((predicate != null) && !predicate(newVert))
                {
                    continue;
                }

                var distance = (newVert.position - position).sqrMagnitude;

                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    nearest = newVert;
                }
            }

            return nearest;
        }

        private static void ApplyStageType(LODGenerationOutput output, StageType stageType)
        {
            var multiplier = Vector3.one;

            switch (stageType)
            {
                case StageType.Felled:
                    multiplier.x = 0.0f;
                    multiplier.y = 0.0f;
                    multiplier.z = 0.5f;
                    break;

                case StageType.Stump:
                case StageType.StumpRotted:
                case StageType.FelledBare:
                case StageType.FelledBareRotted:
                case StageType.DeadFelled:
                case StageType.DeadFelledRotted:
                    multiplier.x = 0.0f;
                    multiplier.y = 0.0f;
                    multiplier.z = 0.1f;
                    break;

                case StageType.Dead:
                    multiplier.x = 1.0f;
                    multiplier.y = 1.0f;
                    multiplier.z = 0.5f;
                    break;

                case StageType.Normal:
                default:
                    multiplier.x = 1.0f;
                    multiplier.y = 1.0f;
                    multiplier.z = 1.0f;
                    break;
            }

            for (var index = 0; index < output.vertices.Count; index++)
            {
                var vertex = output.vertices[index];
                vertex.wind.primaryRoll *= multiplier.x;
                vertex.wind.primaryBend *= multiplier.x;
                vertex.wind.secondaryRoll *= multiplier.y;
                vertex.wind.secondaryBend *= multiplier.y;
                vertex.wind.tertiaryRoll *= multiplier.z;
                output.vertices[index] = vertex;
            }
        }

        private static void EnsureRingParity(
            BarkShapeData barkShapeData,
            LODGenerationOutput output,
            Func<TreeVertex, float> getter,
            Action<TreeVertex, float> setter)
        {
            var rings = barkShapeData.branchRings[output.lodLevel];
            foreach (var ring in rings)
            {
                var average = 0f;

                var count = 0;
                for (var i = ring.vertexStart; i < ring.vertexEnd; i++)
                {
                    count += 1;

                    var vertex = output.vertices[i];

                    average += getter(vertex);
                }

                average /= count;

                for (var i = ring.vertexStart; i < ring.vertexEnd; i++)
                {
                    var vertex = output.vertices[i];

                    setter(vertex, average);
                }

                if (ring.capVertexCenter != 0)
                {
                    for (var i = ring.capVertexCenter; i < ring.capVertexEnd; i++)
                    {
                        var vertex = output.vertices[i];

                        setter(vertex, average);
                    }
                }
            }
        }

        private static void EnsureRingParity(
            BarkShapeData barkShapeData,
            LODGenerationOutput output,
            Func<TreeVertex, Vector3> getter,
            Action<TreeVertex, Vector3> setter)
        {
            var rings = barkShapeData.branchRings[output.lodLevel];
            foreach (var ring in rings)
            {
                var average = Vector3.zero;

                var count = 0;
                for (var i = ring.vertexStart; i < ring.vertexEnd; i++)
                {
                    count += 1;

                    var vertex = output.vertices[i];

                    average += getter(vertex);
                }

                average /= count;

                for (var i = ring.vertexStart; i < ring.vertexEnd; i++)
                {
                    var vertex = output.vertices[i];

                    setter(vertex, average);
                }

                if (ring.capVertexCenter != 0)
                {
                    for (var i = ring.capVertexCenter; i < ring.capVertexEnd; i++)
                    {
                        var vertex = output.vertices[i];

                        setter(vertex, average);
                    }
                }
            }
        }

        private static float GetMaximiumWindAtPoint(float windFactorXZ, float windFactorY, Vector3 position, bool clamp)
        {
            var testPosition = position.Abs();
            testPosition.y = position.y;

            float xFactor;
            float yFactor;

            if (clamp && (windFactorXZ > .9999f))
            {
                xFactor = 1f;
            }
            else
            {
                var maxXZ = Mathf.Max(testPosition.x, testPosition.z);
                xFactor = windFactorXZ * maxXZ;
            }

            if (clamp && (windFactorY > .9999f))
            {
                yFactor = 1f;
            }
            else
            {
                yFactor = windFactorY * testPosition.y;
            }

            var value = Mathf.Min(xFactor, yFactor);

            if (clamp)
            {
                return Mathf.Clamp01(value);
            }

            return value;
        }

        /*
        private static Octree<TreeVertex> GenerateOctree(LODGenerationOutput output)
        {
            using (BUILD_TIME.WIND_GEN.GenerateOctree.Auto())
            {
                var bounds = new Bounds();

                using (BUILD_TIME.WIND_GEN.GenerateOctreeEncapsulate.Auto())
                {
                    foreach (var vert in output.vertices)
                    {
                        bounds.Encapsulate(vert.position);
                    }
                }

                bounds.size += Vector3.one * 5f;

                var octree = Octree<TreeVertex>.Create(bounds);

                foreach (var vert in output.vertices)
                {
                    var data = new Octree<TreeVertex>.Data
                    {
                        bounds = new Bounds() {center = vert.position, size = Vector3.one * .25f}, value = vert
                    };

                    if (!octree.Add(data))
                    {
                        throw new NotSupportedException("Bad vertex data for octree");
                    }
                }

                return octree;
            }
        }
        */

        private struct vkey : IEquatable<vkey>
        {
            public bool Equals(vkey other)
            {
                return (x == other.x) && (y == other.y) && (z == other.z);
            }

            public override bool Equals(object obj)
            {
                return obj is vkey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = x.GetHashCode();
                    hashCode = (hashCode * 397) ^ y.GetHashCode();
                    hashCode = (hashCode * 397) ^ z.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(vkey left, vkey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(vkey left, vkey right)
            {
                return !left.Equals(right);
            }

            private readonly long x;
            private readonly long y;
            private readonly long z;

            public vkey(Vector3 position)
            {
                x = (long) (Mathf.Round(position.x));
                y = (long) (Mathf.Round(position.y));
                z = (long) (Mathf.Round(position.z));
            }
        }

    }
}