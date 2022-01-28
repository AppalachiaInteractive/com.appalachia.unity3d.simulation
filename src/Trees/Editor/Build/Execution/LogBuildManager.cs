#region

using System;
using System.Collections;
using Appalachia.CI.Constants;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Generation;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Collisions;
using Appalachia.Simulation.Trees.Generation.Geometry;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using Appalachia.Simulation.Trees.Generation.VertexData;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Utility.Execution;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Build.Execution
{
    [CallStaticConstructorInEditor]
    public static class LogBuildManager
    {
        static LogBuildManager()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeSpeciesEditorSelection>()
                                     .IsAvailableThen(i => _treeSpeciesEditorSelection = i);
        }

        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        private static bool _executing;

        private static EditorCoroutine _coroutine;

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(LogBuildManager));
                }

                return _context;
            }
        }

        private static LogDataContainer CTX => _treeSpeciesEditorSelection?.log?.selection?.selected;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (AppalachiaApplication.IsPlayingOrWillPlay)
            {
                return;
            }

            EditorApplication.update += Update;
        }

        private static void AssignMaterialProperties(
            BuildRequestLevel requestLevel,
            LogDataContainer log,
            ref bool completed)
        {
            using (BUILD_TIME.LOG_BUILD_MGR.AssignMaterialProperties.Auto())
            {
                if (log.buildRequest.ShouldBuild(BuildCategory.MaterialProperties, requestLevel))
                {
                    log.progressTracker.Update(BuildCategory.MaterialProperties);

                    LogMaterialPropertyManager.AssignMaterialProperties(log);
                }
            }
        }

        private static IEnumerator ExecutePendingBuild()
        {
            //using (BUILD_TIME.TREE_BUILD_MGR.ExecutePendingBuild.Auto())
            //{
            var completed = false;

            var log = CTX;
            log.dataState = TSEDataContainer.DataState.Normal;

            try
            {
                if (EditorApplication.isCompiling || AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    completed = true;
                    yield break; // ========================================================== BREAK
                }

                if ((log.buildState == BuildState.Cancelled) ||
                    (log.buildState == BuildState.Disabled) ||
                    (log.requestLevel == BuildRequestLevel.None))
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    completed = true;
                    yield break; // ========================================================== BREAK
                }

                if (log.buildState == BuildState.ForceFull)
                {
                    if (log.dataState == TSEDataContainer.DataState.PendingSave)
                    {
                        log.Save();
                    }
                }

                log.settings.Check();

                log.RebuildStructures();

                log.progressTracker.InitializeBuildBatch();

                if (log.subfolders == null)
                {
                    log.subfolders = TreeAssetSubfolders.CreateNested(log);
                }

                log.subfolders.CreateFolders();

                SeedManager.UpdateSeeds(log.log, log.logInstances);

                var buildAll = log.buildState >= BuildState.Full;

                while (log.requestLevel > BuildRequestLevel.None)
                {
                    var requestLevel = log.requestLevel;

                    log.progressTracker.InitializeBuild(requestLevel, log.GetBuildCosts(requestLevel));

                    foreach (var instance in log.logInstances)
                    {
                        if (!buildAll && !instance.active)
                        {
                            continue;
                        }

                        if (instance.GetRequestLevel(log.buildState) < requestLevel)
                        {
                            continue;
                        }

                        instance.seed.Reset();

                        TreeProperty.SetActiveGenerationAge(AgeType.Mature);

                        if (instance.ShouldRebuildDistribution(requestLevel))
                        {
                            GenerateDistribution(log, requestLevel, instance);

                            yield return null;
                        }

                        #region BREAK / CONTINUE

                        if (log.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            yield break; // ========================================== BREAK
                        }

                        #endregion

                        if (instance.ShouldRebuildGeometry(requestLevel))
                        {
                            GenerateLogGeometry(requestLevel, log, instance);

                            yield return null;
                        }
                    }

                    #region BREAK

                    if (log.requestLevel != requestLevel)
                    {
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        completed = true;
                        yield break; // ====================================================== BREAK
                    }

                    #endregion

                    yield return null;

                    AssignMaterialProperties(requestLevel, log, ref completed);

                    yield return null;

                    #region BREAK

                    if (log.requestLevel != requestLevel)
                    {
                        completed = true;
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        yield break; // ====================================================== BREAK
                    }

                    #endregion

                    foreach (var instance in log.logInstances)
                    {
                        if (!buildAll && !instance.active)
                        {
                            continue;
                        }

                        if (instance.GetRequestLevel(log.buildState) < requestLevel)
                        {
                            continue;
                        }

                        #region BREAK / CONTINUE / YIELD

                        if (log.requestLevel != requestLevel)
                        {
                            completed = true;
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            yield break; // ===========================================BREAK
                        }

                        #endregion

                        instance.seed.Reset();

                        GenerateEnhancements(requestLevel, log, instance);

                        yield return null;
                    }

                    /*if (tree.buildRequest.ShouldBuild(BuildCategory.PrefabCreation, requestLevel))
                    {
                        AssetManager.SaveAllAssets(tree);
                    }*/

                    foreach (var instance in log.logInstances)
                    {
                        if (!buildAll && !instance.active)
                        {
                            instance.buildRequest.CollapseDelayedBuildLevel();
                        }
                        else
                        {
                            if (instance.buildRequest.ShouldBuild(BuildCategory.Collision, requestLevel))
                            {
                                AssetManager.SaveAsset(log, instance);
                            }

                            instance.buildRequest.DecreaseBuildLevel(requestLevel);
                        }

                        yield return null;
                    }

                    log.buildRequest.DecreaseBuildLevel(requestLevel);

                    log.progressTracker.CompleteBuild();
                }

                completed = true;
            }
            finally
            {
                using (BUILD_TIME.LOG_BUILD_MGR.FinalizeBuild.Auto())
                {
                    if (!completed)
                    {
                        using (BUILD_TIME.LOG_BUILD_MGR.HandleBuildError.Auto())
                        {
                            Context.Log.Error("Tree build failed.");

                            if (log != null)
                            {
                                log.PushBuildRequestLevelAll(BuildRequestLevel.None);
                            }
                        }
                    }

                    if (log.progressTracker.buildActive)
                    {
                        if (log.buildState >= BuildState.Full)
                        {
                            log.Save();
                        }

                        log.dataState = TSEDataContainer.DataState.PendingSave;

                        log.progressTracker.CompleteBuildBatch(completed);
                    }

                    if (log.buildState == BuildState.Cancelled)
                    {
                        log.buildState = BuildState.Default;
                    }

                    _executing = false;
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                }
            }

            //}
        }

        private static void GenerateDistribution(
            LogDataContainer log,
            BuildRequestLevel requestLevel,
            LogInstance instance)
        {
            using (BUILD_TIME.LOG_BUILD_MGR.GenerateDistribution.Auto())
            {
                if (instance.GetRequestLevel(log.buildState) < requestLevel)
                {
                    return;
                }

                foreach (var trunk in log.log.Trunks)
                {
                    trunk.limb.log = true;
                    trunk.trunk.flareHeight.SetValue(0.0f);
                    trunk.trunk.flareNoise.SetValue(0.0f);
                    trunk.trunk.flareRadius.SetValue(0.0f);
                    trunk.trunk.flareDensity.SetValue(1.0f);
                    trunk.trunk.flareResolution.SetValue(1.0f);
                    trunk.trunk.trunkSpread.SetValue(1.0f);
                }

                if (instance.buildRequest.distribution == requestLevel)
                {
                    log.progressTracker.Update(BuildCategory.Distribution);

                    instance.Rebuild();

                    ShapeGenerationManager.GenerateShapes(
                        instance.seed,
                        instance.shapes,
                        log.log.hierarchies
                    );

                    instance.Rebuild();
                }

                if (instance.shapes.TotalShapeCount() == 0)
                {
                    throw new NotSupportedException("Bad shape count!");
                }
            }
        }

        private static void GenerateEnhancements(
            BuildRequestLevel requestLevel,
            LogDataContainer log,
            LogInstance instance)
        {
            using (BUILD_TIME.LOG_BUILD_MGR.GenerateEnhancements.Auto())
            {
                var buildVertexData = instance.buildRequest.ShouldBuild(
                    BuildCategory.VertexData,
                    requestLevel
                );
                var buildMesh = instance.buildRequest.ShouldBuildMesh(requestLevel);

                //var buildUV = instance.buildRequest.ShouldBuild(BuildCategory.UV, requestLevel);
                var buildCollision = instance.buildRequest.ShouldBuild(BuildCategory.Collision, requestLevel);
                var buildLOD = instance.buildRequest.ShouldBuild(BuildCategory.LevelsOfDetail,  requestLevel);

                foreach (var levelOfDetail in instance.lods)
                {
                    if (!buildLOD && (levelOfDetail.lodLevel > 0))
                    {
                        break;
                    }

                    if (buildVertexData)
                    {
                        if (log.settings.vertex.generate)
                        {
                            LogGenerator.ApplyMeshLogData(
                                instance.shapes,
                                log.log.hierarchies,
                                levelOfDetail,
                                log.settings.vertex,
                                instance.seed
                            );
                        }
                    }

                    if (buildMesh)
                    {
                        var asset = instance.asset.levels[levelOfDetail.lodLevel];

                        if (levelOfDetail.lodLevel == 0)
                        {
                            log.progressTracker.Update(BuildCategory.Mesh);
                        }

                        if (asset.mesh == null)
                        {
                            instance.RecreateMesh(levelOfDetail.lodLevel);
                        }

                        Func<TreeVertex, Color> colorFunction;

                        if (log.settings.vertex.generate)
                        {
                            colorFunction = v => v.LogColor;
                        }
                        else
                        {
                            colorFunction = v => Color.black;
                        }

                        MeshGenerator.GenerateMeshes(
                            levelOfDetail,
                            instance.shapes.byID,
                            null,
                            null,
                            log.settings.mesh,
                            asset.mesh,
                            colorFunction,
                            true,
                            true,
                            default
                        );

                        asset.mesh.uv2 = null;
                        asset.mesh.uv3 = null;
                        asset.mesh.uv4 = null;
                        asset.mesh.uv5 = null;

                        instance.SetMaterials(levelOfDetail.lodLevel, new[] { log.material });
                    }
                }

                if (buildCollision)
                {
                    log.progressTracker.Update(BuildCategory.Collision);

                    instance.asset.collisionMeshes.CreateCollisionMeshes(instance.asset.CleanName);

                    ColliderGenerator.GenerateLogColliders(
                        log.log.hierarchies,
                        instance.shapes,
                        instance,
                        log.settings,
                        instance.shapes.byID,
                        instance.asset.collisionMeshes
                    );
                }
            }
        }

        private static void GenerateLogGeometry(
            BuildRequestLevel requestLevel,
            LogDataContainer log,
            LogInstance instance)
        {
            using (BUILD_TIME.LOG_BUILD_MGR.GenerateLogGeometry.Auto())
            {
                var buildLOD = instance.buildRequest.ShouldBuild(BuildCategory.LevelsOfDetail, requestLevel);

                instance.Rebuild();

                var baseSeed = instance.seed;
                baseSeed.Reset();

                foreach (var hierarchy in log.log)
                {
                    hierarchy.seed.Reset();
                }

                instance.Rebuild();

                instance.ClearGeometry(log.settings.lod);

                foreach (var output in instance.lods)
                {
                    if (output.lodLevel == 0)
                    {
                        log.progressTracker.Update(BuildCategory.HighQualityGeometry);

                        foreach (var hierarchy in log.log.hierarchies)
                        {
                            var bh = hierarchy as BarkHierarchyData;

                            /*if (bh.geometry.relativeToParent)
                            {
                                continue;
                            }*/

                            foreach (var shape in instance.shapes.GetShapesByHierarchy(bh.hierarchyID))
                            {
                                shape.effectiveSize *= instance.thickness;
                            }
                        }

                        foreach (var shape in instance.shapes.GetShapes(TreeComponentType.Trunk))
                        {
                            var bs = shape as BarkShapeData;
                            bs.breakOffset = .95f;
                        }
                    }

                    if (!buildLOD && (output.lodLevel > 0))
                    {
                        break;
                    }

                    GeometryGenerator.GenerateGeometry(
                        log.log.hierarchies,
                        instance.shapes,
                        output,
                        log.settings,
                        instance.seed
                    );

                    if (output.vertices.Count == 0)
                    {
                        throw new NotSupportedException("Bad vertex count!");
                    }

                    var bounds = output.GetBounds();

                    if (output.lodLevel == 0)
                    {
                        instance.effectiveScale = instance.length / (bounds.size.y == 0f ? 1 : bounds.size.y);
                    }

                    for (var index = 0; index < output.vertices.Count; index++)
                    {
                        var vertex = output.vertices[index];
                        vertex.rect_uv0 = vertex.raw_uv0;

                        if (instance.constrainLength)
                        {
                            vertex.position -= bounds.center;

                            vertex.position *= instance.effectiveScale;

                            if (Math.Abs(vertex.log.bark - 1.0f) < float.Epsilon)
                            {
                                vertex.rect_uv0.y *= instance.effectiveScale;
                            }
                        }

                        if ((vertex.type == TreeComponentType.Branch) &&
                            (Math.Abs(vertex.log.bark - 1.0f) < float.Epsilon))
                        {
                            vertex.rect_uv0.y *= 2;
                        }
                    }

                    bounds = output.GetBounds();

                    for (var index = 0; index < output.vertices.Count; index++)
                    {
                        var vertex = output.vertices[index];
                        vertex.position.x -= bounds.center.x;
                        vertex.position.y -= bounds.min.y;
                        vertex.position.z -= bounds.center.z;
                    }

                    if (output.lodLevel == 0)
                    {
                        bounds = output.GetBounds();

                        instance.center = bounds.center;
                        instance.centerOfMass = Vector3.zero;
                        instance.volume = 0f;

                        float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
                        {
                            return Vector3.Dot(p1, Vector3.Cross(p2, p3)) / 6.0f;
                        }

                        foreach (var triangle in output.triangles)
                        {
                            var v0 = output.vertices[triangle.v[0]];
                            var v1 = output.vertices[triangle.v[1]];
                            var v2 = output.vertices[triangle.v[2]];

                            var volume = SignedVolumeOfTriangle(v0.position, v1.position, v2.position);

                            instance.centerOfMass +=
                                (volume * (v0.position + v1.position + v2.position)) / 4f;
                            instance.volume += volume;
                        }

                        instance.centerOfMass /= instance.volume > 0 ? instance.volume : 1f;

                        instance.actualLength = bounds.size.y;

                        var rings = instance.shapes.trunkShapes[0].branchRings[output.lodLevel];
                        var ringIndex = rings.Count / 2;
                        var ring = rings[ringIndex];
                        var midSegment = ring.segments / 2;

                        var start = output.vertices[ring.vertexStart];
                        var end = output.vertices[ring.vertexStart + midSegment];

                        instance.actualDiameter = (end.position - start.position).magnitude;

                        bounds = output.GetBounds();

                        instance.center = bounds.center;
                    }
                }
            }
        }

        private static void Update()
        {
            try
            {
                if (!TreeBuildManager._enabled ||
                    _executing ||
                    EditorApplication.isCompiling ||
                    AppalachiaApplication.IsPlayingOrWillPlay ||
                    (CTX == null) ||
                    !CTX.initialized ||
                    (CTX.dataState == TSEDataContainer.DataState.Normal) ||
                    (CTX.buildState == BuildState.Disabled) ||
                    (CTX.buildState == BuildState.Cancelled) ||
                    CTX.progressTracker.buildActive ||
                    (CTX.requestLevel == BuildRequestLevel.None))
                {
                    if ((CTX != null) && (CTX.progressTracker != null))
                    {
                        CTX.progressTracker.Validate();
                    }

                    return;
                }

                _executing = true;
                _coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(ExecutePendingBuild());
            }
            catch (Exception ex)
            {
                Context.Log.Error(ex);
            }
        }
    }
}
