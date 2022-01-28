#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Extensions;
using Appalachia.Core.Objects.Availability;
using Appalachia.Core.Shading;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Generation;
using Appalachia.Simulation.Trees.Generation.AmbientOcclusion;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Collisions;
using Appalachia.Simulation.Trees.Generation.Geometry;
using Appalachia.Simulation.Trees.Generation.Impostors;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.ShadowCasters;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Transmission;
using Appalachia.Simulation.Trees.Generation.VertexData;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Spatial.Octree;
using Appalachia.Utility.Execution;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Build.Execution
{
    [CallStaticConstructorInEditor]
    public static class TreeBuildManager
    {
        static TreeBuildManager()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeSpeciesEditorSelection>()
                                     .IsAvailableThen(i => _treeSpeciesEditorSelection = i);
            RegisterInstanceCallbacks.WithoutSorting().When.Object<GSR>().IsAvailableThen(i => _GSR = i);
        }

        #region Static Fields and Autoproperties

        public static bool _autobuilding;
        public static bool _enabled;
        public static bool _executing;
        public static EditorCoroutine _coroutine;
        [NonSerialized] private static AppaContext _context;

        private static GSR _GSR;

        private static TreeSpeciesEditorSelection _selectionInstance;

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(TreeBuildManager));
                }

                return _context;
            }
        }

        private static TreeDataContainer CTX => selectionInstance.tree.selection.selected;

        private static TreeSpeciesEditorSelection selectionInstance
        {
            get
            {
                if (_selectionInstance == null)
                {
                    _selectionInstance = _treeSpeciesEditorSelection;
                }

                return _selectionInstance;
            }
        }

        public static IEnumerator ExecuteAllBuildsEnumerator(
            QualityMode quality,
            Action buildAction,
            string searchString)
        {
            var trees = AssetDatabaseManager.FindAssets<TreeDataContainer>(searchString);

            return ExecuteAllBuildsEnumerator(quality, buildAction, trees);
        }

        public static IEnumerator ExecuteAllBuildsEnumerator(
            QualityMode quality,
            Action buildAction,
            IEnumerable<TreeDataContainer> trees)
        {
            try
            {
                if (EditorApplication.isCompiling || AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    yield break; // ========================================================== BREAK
                }

                foreach (var tree in trees)
                {
                    tree.settings.qualityMode = quality;

                    _treeSpeciesEditorSelection.tree.selection.Set(tree);

                    buildAction();

                    var build = ExecutePendingBuild(true);

                    while (build.MoveNext())
                    {
                        yield return build.Current;
                    }

                    tree.Save();
                }
            }
            finally
            {
                TreeMaterialUsageTracker.Refresh();
                _autobuilding = false;
                _executing = false;
                EditorCoroutineUtility.StopCoroutine(_coroutine);
            }
        }

        internal static void Update()
        {
            using (_PRF_Update.Auto())
            {
                try
                {
                    if ((CTX != null) &&
                        (CTX.progressTracker != null) &&
                        (CTX.progressTracker.timeSinceBuildComplete > 600) &&
                        !CTX.progressTracker.buildActive)
                    {
                        _autobuilding = false;
                        _executing = false;
                    }

                    if (!_enabled)
                    {
                        EditorApplication.update -= Update;
                        return;
                    }

                    if (_executing ||
                        _autobuilding ||
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

        private static IEnumerator ExecutePendingBuild(bool automatic = false)
        {
            //using (BUILD_TIME.TREE_BUILD_MGR.ExecutePendingBuild.Auto())
            //{
            var completed = false;

            var tree = CTX;
            tree.dataState = TSEDataContainer.DataState.Normal;

            try
            {
                if (EditorApplication.isCompiling || AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    completed = true;
                    yield break; // ========================================================== BREAK
                }

                if ((tree.buildState == BuildState.Cancelled) ||
                    (tree.buildState == BuildState.Disabled) ||
                    (tree.requestLevel == BuildRequestLevel.None))
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    completed = true;
                    yield break; // ========================================================== BREAK
                }

                if (tree.buildState == BuildState.ForceFull)
                {
                    if (tree.dataState == TSEDataContainer.DataState.PendingSave)
                    {
                        tree.Save();
                    }

                    tree.hierarchyPrefabs.ResetPrefabs();
                    tree.materials.hash = null;
                }

                tree.settings.Check();

                tree.RebuildStructures();

                tree.progressTracker.InitializeBuildBatch();

                if (tree.subfolders == null)
                {
                    tree.subfolders = TreeAssetSubfolders.CreateNested(tree);
                }

                tree.subfolders.CreateFolders();

                if (tree.settings.lod.impostor.impostorAfterLastLevel)
                {
                    tree.subfolders.CreateImpostorFolder();
                }

                SeedManager.UpdateSeeds(tree.species, tree.individuals);

                tree.hierarchyPrefabs.UpdatePrefabs(tree.species);
                tree.hierarchyPrefabs.UpdatePrefabAvailability(tree.species);

                tree.materials.UpdateMaterials(
                    tree.species,
                    tree.hierarchyPrefabs,
                    tree.settings.lod.levels,
                    tree.settings.lod.shadowCaster
                );
                tree.materials.inputMaterialCache.SetDefaultMaterials(
                    tree.settings.material.defaultMaterials
                );

                tree.materials.inputMaterialCache.UpdateDefaultMaterials();

                var buildAll = tree.buildState >= BuildState.Full;

                while (tree.requestLevel > BuildRequestLevel.None)
                {
                    var requestLevel = tree.requestLevel;

                    tree.progressTracker.InitializeBuild(requestLevel, tree.GetBuildCosts(requestLevel));

                    foreach (var individual in tree.individuals)
                    {
                        if (!buildAll && !individual.active)
                        {
                            continue;
                        }

                        if (individual.GetRequestLevel(tree.buildState) < requestLevel)
                        {
                            continue;
                        }

                        for (var i = 0; i < individual.ages.Count; i++)
                        {
                            var age = individual.ages[i];

                            if (age.stages.All(s => s.shapes.TotalShapeCount() > 0))
                            {
                                if (!buildAll && !age.active)
                                {
                                    continue;
                                }

                                if (age.GetRequestLevel(tree.buildState) < requestLevel)
                                {
                                    continue;
                                }
                            }

                            TreeProperty.SetActiveGenerationAge(age.ageType);

                            if (age.ShouldRebuildDistribution(requestLevel))
                            {
                                GenerateAgeDistribution(tree, individual.seed, requestLevel, age);

                                yield return null;
                            }

                            var builtNormal = false;

                            foreach (var stage in age.stages)
                            {
                                #region BREAK / CONTINUE

                                if (tree.requestLevel != requestLevel)
                                {
                                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                                    completed = true;
                                    yield break; // ========================================== BREAK
                                }

                                #endregion

                                if (stage.shapes.TotalShapeCount() > 0)
                                {
                                    if (!buildAll && !stage.active)
                                    {
                                        continue;
                                    }

                                    if (stage.GetRequestLevel(tree.buildState) < requestLevel)
                                    {
                                        continue;
                                    }
                                }

                                individual.seed.Reset();

                                if (stage.ShouldRebuildGeometry(requestLevel))
                                {
                                    if (!builtNormal && (stage.stageType != StageType.Normal))
                                    {
                                        GenerateIndividualStageGeometry(
                                            requestLevel,
                                            tree,
                                            individual,
                                            age.normalStage,
                                            age.samplePoints,
                                            null,
                                            true
                                        );

                                        individual.seed.Reset();
                                        builtNormal = true;
                                    }

                                    GenerateIndividualStageGeometry(
                                        requestLevel,
                                        tree,
                                        individual,
                                        stage,
                                        age.samplePoints,
                                        age.normalStage,
                                        false
                                    );

                                    if (requestLevel <= BuildRequestLevel.FinalPass)
                                    {
                                        yield return null;
                                    }
                                }
                            }
                        }
                    }

                    #region BREAK

                    if (tree.requestLevel != requestLevel)
                    {
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        completed = true;
                        yield break; // ====================================================== BREAK
                    }

                    #endregion

                    yield return null;

                    GenerateSpeciesMaterials(requestLevel, tree, ref completed);

                    yield return null;

                    #region BREAK

                    if (tree.requestLevel != requestLevel)
                    {
                        completed = true;
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        yield break; // ====================================================== BREAK
                    }

                    #endregion

                    if (tree.buildRequest.ShouldBuild(BuildCategory.MaterialProperties, requestLevel))
                    {
                        foreach (var individual in tree.individuals)
                        {
                            foreach (var age in individual.ages)
                            {
                                foreach (var stage in age.stages)
                                {
                                    UpdateStageSpecificMaterials(
                                        stage,
                                        tree.materials.transmission,
                                        tree.settings.lod.impostor
                                    );

                                    yield return null;
                                }
                            }
                        }
                    }

                    #region BREAK

                    if (tree.requestLevel != requestLevel)
                    {
                        completed = true;
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        yield break; // ====================================================== BREAK
                    }

                    #endregion

                    foreach (var individual in tree.individuals)
                    {
                        if (!buildAll && !individual.active)
                        {
                            continue;
                        }

                        if (individual.GetRequestLevel(tree.buildState) < requestLevel)
                        {
                            continue;
                        }

                        for (var i = 0; i < individual.ages.Count; i++)
                        {
                            var age = individual.ages[i];
                            /*}
                            foreach (var age in individual.ages)
                            {*/

                            if (age.stages.All(
                                    s => s.asset.levels.All(l => (l.mesh != null) && (l.mesh.vertexCount > 0))
                                ))
                            {
                                if (!buildAll && !age.active)
                                {
                                    continue;
                                }

                                if (age.GetRequestLevel(tree.buildState) < requestLevel)
                                {
                                    continue;
                                }
                            }

                            TreeProperty.SetActiveGenerationAge(age.ageType);

                            foreach (var stage in age.stages)
                            {
                                if (stage.asset.levels.All(l => (l.mesh != null) && (l.mesh.vertexCount > 0)))
                                {
                                    if (!buildAll && !stage.active)
                                    {
                                        continue;
                                    }

                                    if (stage.GetRequestLevel(tree.buildState) < requestLevel)
                                    {
                                        continue;
                                    }
                                }

                                #region BREAK / CONTINUE / YIELD

                                if (tree.requestLevel != requestLevel)
                                {
                                    completed = true;
                                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                                    yield break; // ===========================================BREAK
                                }

                                #endregion

                                individual.seed.Reset();

                                GenerateIndividualStageEnhancements(requestLevel, tree, individual, stage);

                                yield return null;
                            }
                        }
                    }

                    /*if (tree.buildRequest.ShouldBuild(BuildCategory.PrefabCreation, requestLevel))
                    {
                        AssetManager.SaveAllAssets(tree);
                    }*/

                    foreach (var individual in tree.individuals)
                    {
                        foreach (var age in individual.ages)
                        {
                            if (!buildAll && !age.active)
                            {
                                age.buildRequest.CollapseDelayedBuildLevel();
                            }
                            else
                            {
                                age.buildRequest.DecreaseBuildLevel(requestLevel);
                            }

                            TreeProperty.SetActiveGenerationAge(age.ageType);

                            foreach (var stage in age.stages)
                            {
                                if (!buildAll && !stage.active)
                                {
                                    stage.buildRequest.CollapseDelayedBuildLevel();
                                }
                                else
                                {
                                    var buildCollisions = stage.buildRequest.ShouldBuild(
                                        BuildCategory.Collision,
                                        requestLevel
                                    );
                                    var buildImpostor = stage.buildRequest.ShouldBuild(
                                        BuildCategory.Impostor,
                                        requestLevel
                                    );

                                    if (buildImpostor)
                                    {
                                        tree.progressTracker.Update(BuildCategory.Impostor);
                                    }

                                    if (buildImpostor || buildCollisions)
                                    {
                                        AssetManager.SaveAsset(tree, individual, stage, buildImpostor);
                                    }

                                    stage.buildRequest.DecreaseBuildLevel(requestLevel);
                                }
                            }
                        }
                    }

                    tree.buildRequest.DecreaseBuildLevel(requestLevel);

                    tree.progressTracker.CompleteBuild();
                }

                completed = true;
            }
            finally
            {
                using (BUILD_TIME.TREE_BUILD_MGR.FinalizeBuild.Auto())
                {
                    if (!completed)
                    {
                        using (BUILD_TIME.TREE_BUILD_MGR.HandleBuildError.Auto())
                        {
                            Context.Log.Error("Tree build failed.");

                            if (tree != null)
                            {
                                tree.PushBuildRequestLevelAll(BuildRequestLevel.None);
                            }
                        }
                    }

                    if (tree.progressTracker.buildActive)
                    {
                        /*if (saveAssets)
                        {
                            using (BUILD_TIME.TREE_BUILD_MGR.SaveBuildAssets.Auto())
                            {
                                AssetDatabaseManager.SaveAssets();
                            }
                        }*/

                        tree.dataState = TSEDataContainer.DataState.PendingSave;

                        tree.progressTracker.CompleteBuildBatch(completed);
                    }

                    if (tree.buildState == BuildState.Cancelled)
                    {
                        tree.buildState = BuildState.Default;
                    }

                    if (!automatic)
                    {
                        _executing = false;
                        TreeMaterialUsageTracker.Refresh();
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                    }
                }
            }

            //}
        }

        private static void GenerateAgeDistribution(
            TreeDataContainer tree,
            BaseSeed individualSeed,
            BuildRequestLevel requestLevel,
            TreeAge age)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateIndividualStageShapes.Auto())
            {
                if (age.GetRequestLevel(tree.buildState) < requestLevel)
                {
                    return;
                }

                TreeProperty.SetActiveGenerationAge(age.ageType);

                var regenerateAOSampling = tree.settings.ao.generateAmbientOcclusion &&
                                           (tree.settings.qualityMode >= QualityMode.Preview) &&
                                           ((age.samplePoints == null) ||
                                            (age.samplePoints.NodeCount == 0) ||
                                            age.CheckAnyStageSetting(
                                                s => (s.highQualityGeometry == requestLevel) ||
                                                     (s.ambientOcclusion == requestLevel)
                                            ));

                if (age.buildRequest.distribution == requestLevel)
                {
                    tree.progressTracker.Update(BuildCategory.Distribution);

                    age.normalStage.Rebuild();

                    ShapeGenerationManager.GenerateShapes(
                        individualSeed,
                        age.normalStage.shapes,
                        tree.species.hierarchies,
                        tree.settings.variants,
                        age.ageType
                    );

                    age.normalStage.Rebuild();
                }

                if (age.normalStage.shapes.TotalShapeCount() == 0)
                {
                    throw new NotSupportedException("Bad shape count!");
                }

                if (regenerateAOSampling)
                {
                    tree.progressTracker.Update(BuildCategory.AmbientOcclusion);

                    age.samplePoints = AmbientOcclusionGenerator.GenerateAOSamplePoints(
                        age.normalStage,
                        tree.species.hierarchies
                    );
                }
            }
        }

        private static void GenerateIndividualStageEnhancements(
            BuildRequestLevel requestLevel,
            TreeDataContainer tree,
            TreeIndividual individual,
            TreeStage stage)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateIndividualStageEnhancements.Auto())
            {
                var buildVertexData = stage.buildRequest.ShouldBuild(BuildCategory.VertexData, requestLevel);
                var buildMesh = stage.buildRequest.ShouldBuildMesh(requestLevel);
                var buildUV = stage.buildRequest.ShouldBuild(BuildCategory.UV, requestLevel);
                var buildCollision = tree.settings.collision.generateColliders &&
                                     (tree.settings.qualityMode > QualityMode.Preview) &&
                                     stage.buildRequest.ShouldBuild(BuildCategory.Collision, requestLevel);

                var buildShadowCaster = tree.settings.lod.shadowCaster;

                //var lowQuality = !stage.buildRequest.ShouldBuild(BuildCategory.HighQualityGeometry, requestLevel);
                //var buildLOD = stage.buildRequest.ShouldBuild(BuildCategory.LevelsOfDetail, requestLevel);

                /*if (lowQuality)
                {
                    buildLOD = false;
                }*/

                var geometryIndex = -1;

                foreach (var output in stage.lods)
                {
                    geometryIndex += 1;

                    /*if (!buildLOD && (output.lodLevel > 0))
                    {
                        break;
                    }*/

                    if (buildVertexData)
                    {
                        if (tree.settings.wind.generateWind)
                        {
                            if (output.lodLevel == 0)
                            {
                                tree.progressTracker.Update(BuildCategory.VertexData);
                            }

                            WindGenerator.ApplyMeshWindData(
                                stage.shapes,
                                tree.species.hierarchies,
                                output,
                                geometryIndex,
                                tree.settings.wind,
                                stage.stageType,
                                individual.seed
                            );
                        }
                    }
                }

                if (buildVertexData)
                {
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
                }

                geometryIndex = -1;

                foreach (var output in stage.lods)
                {
                    geometryIndex += 1;

                    /*if (!buildLOD && (output.lodLevel > 0))
                    {
                        break;
                    }*/

                    if (buildUV)
                    {
                        if (output.lodLevel == 0)
                        {
                            tree.progressTracker.Update(BuildCategory.UV);
                        }

                        UVManager.RemapUVCoordinates(output, tree.materials, tree.settings.variants);

                        UVManager.ApplyLeafRects(
                            tree.species.hierarchies,
                            stage.shapes,
                            tree.materials.inputMaterialCache,
                            output,
                            individual.seed
                        );
                    }
                }

                if (buildMesh)
                {
                    for (var i = 0; i < stage.lods.Count; i++)
                    {
                        var vertices = stage.lods[i].vertices;
                        foreach (var shape in stage.shapes)
                        {
                            if (shape is BarkShapeData bsd)
                            {
                                var rings = bsd.branchRings[i];

                                for (var ri = 0; ri < rings.Count; ri++)
                                {
                                    var ring = rings[ri];

                                    var vertexOne = vertices[ring.vertexStart];
                                    var vertexTwo = vertices[ring.vertexEnd - 1];

                                    vertexTwo.position = vertexOne.position;
                                    vertexTwo.normal = vertexOne.normal;
                                    vertexTwo.tangent = vertexOne.tangent;

                                    vertexTwo.weldTo = ring.vertexStart;
                                }
                            }
                        }
                    }
                }

                geometryIndex = -1;

                foreach (var output in stage.lods)
                {
                    geometryIndex += 1;

                    /*if (!buildLOD && (output.lodLevel > 0))
                    {
                        break;
                    }*/

                    if (buildMesh)
                    {
                        var asset = stage.asset.levels[output.lodLevel];

                        if (output.lodLevel == 0)
                        {
                            tree.progressTracker.Update(BuildCategory.Mesh);
                        }

                        if (asset.mesh == null)
                        {
                            stage.RecreateMesh(output.lodLevel);
                        }

                        var origMaterialIDs = output.materialIDs;

                        var materials = new HashSet<OutputMaterial>();

                        foreach (var materialID in origMaterialIDs)
                        {
                            if (materialID == -1)
                            {
                                continue;
                            }

                            var m = tree.materials.GetOutputMaterialByInputID(materialID);

                            materials.Add(m);
                        }

                        var mats = materials.ToList();

                        mats.Sort(
                            (a, b) =>
                            {
                                if ((a.MaterialContext == MaterialContext.AtlasOutputMaterial) &&
                                    (b.MaterialContext != MaterialContext.AtlasOutputMaterial))
                                {
                                    return -1;
                                }

                                if ((a.MaterialContext != MaterialContext.AtlasOutputMaterial) &&
                                    (b.MaterialContext == MaterialContext.AtlasOutputMaterial))
                                {
                                    return 1;
                                }

                                return a.materialID.CompareTo(b.materialID);
                            }
                        );

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

                        foreach (var shape in stage.shapes.trunkShapes)
                        {
                            var baseRing = shape.branchRings[output.lodLevel][0];

                            for (var i = baseRing.vertexStart; i < baseRing.vertexEnd; i++)
                            {
                                var outputVertex = output.vertices[i];
                                var position = outputVertex.position;

                                if (position.y >= 0)
                                {
                                    break;
                                }

                                position.y *= 2;

                                outputVertex.position = position;
                                output.vertices[i] = outputVertex;
                            }
                        }

                        var runtimeMaterials = MeshGenerator.GenerateMeshes(
                            output,
                            stage.shapes.byID,
                            tree.materials.inputMaterialCache,
                            mats,
                            tree.settings.mesh,
                            asset.mesh,
                            colorFunction,
                            false,
                            true,
                            default
                        );

                        stage.SetMaterials(output.lodLevel, runtimeMaterials.ToArray());
                    }
                }

                if (buildShadowCaster)
                {
                    var cache = tree.materials.outputMaterialCache;

                    cache.shadowCasterOutputMaterial.EnsureCreated(tree.settings.lod.levels);
                    cache.shadowCasterOutputMaterial.textureSet.Set(
                        cache.atlasOutputMaterial.textureSet.outputTextures
                    );

                    cache.shadowCasterOutputMaterial.FinalizeMaterial();

                    ShadowCasterGenerator.GenerateShadowCaster(
                        tree,
                        individual,
                        stage,
                        tree.species.hierarchies,
                        stage.shapes,
                        tree.materials.inputMaterialCache,
                        tree.hierarchyPrefabs,
                        tree.settings,
                        true,
                        individual.seed,
                        stage.shapes.byID
                    );
                }

                if (buildCollision)
                {
                    tree.progressTracker.Update(BuildCategory.Collision);

                    stage.asset.collisionMeshes.CreateCollisionMeshes(stage.asset.CleanName);

                    ColliderGenerator.GenerateCollider(
                        stage.stageType,
                        tree.species.hierarchies,
                        stage.shapes,
                        tree.materials.inputMaterialCache,
                        tree.hierarchyPrefabs,
                        tree.settings,
                        stage.shapes.byID,
                        stage.asset.collisionMeshes
                    );
                }
            }
        }

        private static void GenerateIndividualStageGeometry(
            BuildRequestLevel requestLevel,
            TreeDataContainer tree,
            TreeIndividual individual,
            TreeStage stage,
            BoundsOctree<AmbientOcclusionSamplePoint> samplePoints,
            TreeStage model,
            bool forceBuildGeometry)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateIndividualStageGeometry.Auto())
            {
                var buildHQGeometry = (stage.buildRequest.highQualityGeometry == requestLevel) ||
                                      forceBuildGeometry;
                var buildLQGeometry = stage.buildRequest.lowQualityGeometry == requestLevel;
                var buildAO = stage.buildRequest.ambientOcclusion == requestLevel;

                if (!buildHQGeometry && !buildLQGeometry && !buildAO)
                {
                    return;
                }

                if (buildHQGeometry || buildLQGeometry)
                {
                    tree.progressTracker.Update(
                        buildHQGeometry ? BuildCategory.HighQualityGeometry : BuildCategory.LowQualityGeometry
                    );

                    stage.Rebuild();

                    var baseSeed = individual.seed;
                    baseSeed.Reset();

                    foreach (var hierarchy in tree.species)
                    {
                        hierarchy.seed.Reset();
                    }

                    stage.FinalizeStageShapes(
                        stage.stageType == StageType.Normal ? null : model,
                        tree.species.hierarchies,
                        tree.settings.variants,
                        tree.materials.inputMaterialCache,
                        baseSeed
                    );

                    stage.Rebuild();

                    stage.ClearGeometry(tree.settings.lod);

                    foreach (var levelOfDetail in stage.lods)
                    {
                        GeometryGenerator.GenerateGeometry(
                            tree.species.hierarchies,
                            stage.shapes,
                            levelOfDetail,
                            tree.materials.inputMaterialCache,
                            tree.hierarchyPrefabs,
                            tree.settings,
                            levelOfDetail.lodLevel == 0,
                            individual.seed
                        );

                        if (levelOfDetail.vertices.Count == 0)
                        {
                            throw new NotSupportedException("Bad vertex count!");
                        }
                    }
                }

                if (buildAO)
                {
                    foreach (var levelOfDetail in stage.lods)
                    {
                        if (tree.settings.ao.generateAmbientOcclusion &&
                            (tree.settings.qualityMode >= QualityMode.Preview) &&
                            buildAO)
                        {
                            if (levelOfDetail.lodLevel == 0)
                            {
                                tree.progressTracker.Update(BuildCategory.AmbientOcclusion);
                            }

                            AmbientOcclusionGenerator.ApplyAmbientOcclusion(
                                levelOfDetail,
                                samplePoints,
                                tree.settings.ao.style,
                                tree.settings.ao.density,
                                tree.settings.ao.raytracingSamples,
                                tree.settings.ao.raytracingRange
                            );
                        }
                        else
                        {
                            AmbientOcclusionGenerator.ApplyDefaultAmbientOcclusion(levelOfDetail);
                        }
                    }
                }

                if (buildHQGeometry || buildLQGeometry)
                {
                    foreach (var shape in stage.shapes)
                    {
                        if (shape is BarkShapeData bsd)
                        {
                            for (var i = 0; i < stage.lods.Count; i++)
                            {
                                var output = stage.lods[i];

                                var level = bsd.branchRings[i];

                                foreach (var shapeRing in level)
                                {
                                    for (var j = shapeRing.vertexStart; j < shapeRing.vertexEnd; j++)
                                    {
                                        var vertex = output.vertices[j];

                                        vertex.noise /= shapeRing.maxNoise;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void GenerateSpeciesMaterials(
            BuildRequestLevel requestLevel,
            TreeDataContainer tree,
            ref bool completed)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateSpeciesMaterials.Auto())
            {
                var doMaterialGeneration = tree.buildRequest.ShouldBuild(
                    BuildCategory.MaterialGeneration,
                    requestLevel
                );

                var doMaterialProperties = tree.buildRequest.ShouldBuild(
                    BuildCategory.MaterialProperties,
                    requestLevel
                );

                if (!doMaterialGeneration && !doMaterialProperties)
                {
                    return;
                }

                if (doMaterialGeneration)
                {
                    tree.progressTracker.Update(BuildCategory.MaterialGeneration);

                    tree.dataState = TSEDataContainer.DataState.PendingSave;

                    if (tree.materials.RequiresUpdate(tree.settings.material, out var hash))
                    {
                        /*if (!tree.settings.material.doNotUnsetForceRegenerateMaterials)
                        {
                            tree.settings.material.forceRegenerateMaterials = false;
                        }*/

                        if (tree.settings.material.imageAtlasIsProportionalToArea)
                        {
                            var materialModel = tree.individuals
                                                    .SelectMany(
                                                         i => i.ages.SelectMany(
                                                             a => a.stages.Select(s => s.LOD0)
                                                         )
                                                     )
                                                    .FirstOrDefault();

                            MaterialPropertyManager.SetMaterialProportionalAreas(
                                tree.materials.inputMaterialCache,
                                materialModel
                            );
                        }
                        else
                        {
                            MaterialPropertyManager.SetMaterialNonProportionalAreas(
                                tree.materials.inputMaterialCache
                            );
                        }

                        if (tree.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            return;
                        }

                        tree.progressTracker.Update(BuildCategory.MaterialGeneration);

                        var combinedInputMaterials = tree.materials.inputMaterialCache.atlasInputMaterials
                                                         .Concat<InputMaterial>(
                                                              tree.materials.inputMaterialCache
                                                                  .tiledInputMaterials
                                                          )
                                                         .ToArray();

                        tree.materials.UpdateMaterialNames(tree.species.nameBasis);

                        var textureOutputCollection = new Dictionary<OutputMaterial, List<OutputTexture>>();

                        combinedInputMaterials.ActivateProcessingTextureImporterSettings(
                            tree.settings.texture.atlasTextureSize
                        );

                        var adjustedTextuerSize = tree.settings.texture.textureSizeV2;
                        if (tree.settings.qualityMode == QualityMode.Preview)
                        {
                            adjustedTextuerSize *= .25f;
                        }
                        else if (tree.settings.qualityMode == QualityMode.Working)
                        {
                            adjustedTextuerSize *= .125f;
                        }

                        var imc = tree.materials.inputMaterialCache;
                        var omc = tree.materials.outputMaterialCache;

                        var aim = imc.atlasInputMaterials;
                        var aom = omc.atlasOutputMaterial;

                        var textureSettings = tree.settings.texture;

                        aom.EnsureCreated(tree.settings.lod.levels);

                        var outputTextures = TextureCombiner.DrawAtlasTextures(
                            aim,
                            aom,
                            adjustedTextuerSize,
                            textureSettings.debugCombinationOutputs
                        );

                        textureOutputCollection.Add(aom, outputTextures);

                        tree.progressTracker.Update(BuildCategory.MaterialGeneration);

                        foreach (var tiledMaterial in omc.tiledOutputMaterials)
                        {
                            var match = imc.tiledInputMaterials.FirstOrDefault(
                                tim => tiledMaterial.inputMaterialID == tim.materialID
                            );

                            tiledMaterial.EnsureCreated(tree.settings.lod.levels);

                            var ot = TextureCombiner.DrawTiledTextures(
                                match,
                                tiledMaterial,
                                adjustedTextuerSize,
                                textureSettings.tiledMaterialsKeepOriginalSize,
                                textureSettings.debugCombinationOutputs
                            );

                            textureOutputCollection.Add(tiledMaterial, ot);
                        }

                        if (tree.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            return;
                        }

                        tree.progressTracker.Update(BuildCategory.SavingTextures);

                        if (tree.settings.lod.shadowCaster)
                        {
                            textureOutputCollection.Add(
                                omc.shadowCasterOutputMaterial,
                                new List<OutputTexture>()
                            );
                        }

                        var filePaths = textureOutputCollection.SaveTextures(tree.subfolders, 1);

                        textureOutputCollection.ReimportTextures(
                            tree.subfolders,
                            textureSettings.atlasTextureSize,
                            tree.settings.qualityMode,
                            filePaths,
                            1
                        );

                        foreach (var tiledOutputMaterial in tree.materials.outputMaterialCache
                                                                .tiledOutputMaterials)
                        {
                            tiledOutputMaterial.FinalizeMaterial();
                        }

                        tree.materials.outputMaterialCache.atlasOutputMaterial.FinalizeMaterial();

                        if (tree.settings.qualityMode == QualityMode.Finalized)
                        {
                            combinedInputMaterials.RestoreOriginalTextureImporterSettings();
                        }

                        if (tree.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            return;
                        }
                    }
                }

                if (doMaterialProperties)
                {
                    tree.progressTracker.Update(BuildCategory.MaterialProperties);

                    MaterialPropertyManager.AssignTransmissionMaterialProperties(
                        tree.settings.transmission,
                        tree.materials.transmission,
                        tree.materials.inputMaterialCache,
                        tree.materials.outputMaterialCache,
                        tree.settings.material
                    );

                    MaterialPropertyManager.CopyLODMaterialSettings(tree.materials.outputMaterialCache);
                }

                tree.materials.hash = tree.materials.CalculateHash();
            }
        }

        private static void UpdateStageSpecificMaterials(
            TreeStage stage,
            MaterialTransmissionValues transmission,
            ImpostorSettings impostor)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.UpdateStageSpecificMaterials.Auto())
            {
                if (stage.asset.impostor == null)
                {
                    return;
                }

                if (stage.asset.impostor.Material == null)
                {
                    return;
                }

                ImpostorGenerator.UpdateImpostorMaterialProperties(
                    stage.asset.impostor,
                    impostor,
                    transmission
                );
            }
        }

        #region Menu Items

        [MenuItem(PKG.Menu.Appalachia.Tools.Base + "Trees/Mip Maps/Preserve Coverage")]
        private static void MipMaps_PreserveCoverage()
        {
            var treeGuids = AssetDatabaseManager.FindAssets("t: TreeDataContainer");

            for (var i = 0; i < treeGuids.Length; i++)
            {
                var treeGuid = treeGuids[i];

                var treePath = AssetDatabaseManager.GUIDToAssetPath(treeGuid);

                var tree = AssetDatabaseManager.LoadAssetAtPath<TreeDataContainer>(treePath);

                foreach (var material in tree.materials.outputMaterialCache.atlasOutputMaterial)
                {
                    var asset = material.asset;

                    if (asset.shader == _GSR.leafShaders[0])
                    {
                        var mainTexture = asset.GetTexture(GSC.GENERAL._MainTex) as Texture2D;

                        var importer = mainTexture.GetTextureImporter();

                        var cutoffLowNear = asset.GetFloat(GSC.GENERAL._CutoffLowNear);
                        var cutoffHighNear = asset.GetFloat(GSC.GENERAL._CutoffHighNear);

                        var cutoff = (cutoffLowNear + cutoffHighNear) / 2.0f;

                        asset.SetFloat(GSC.GENERAL._Cutoff, cutoff);

                        importer.mipMapsPreserveCoverage = true;
                        importer.alphaTestReferenceValue = cutoff;

                        importer.SaveAndReimport();
                    }
                }
            }
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(TreeBuildManager) + ".";

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));

        #endregion
    }
}
