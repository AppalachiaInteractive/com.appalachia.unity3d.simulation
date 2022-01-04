#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Generation;
using Appalachia.Simulation.Trees.Generation.AmbientOcclusion;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Geometry;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Snapshot;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Strings;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedParameter.Local

#endregion

namespace Appalachia.Simulation.Trees.Build.Execution
{
    [CallStaticConstructorInEditor]
    public static class BranchBuildManager
    {
        static BranchBuildManager()
        {
            TreeSpeciesEditorSelection.InstanceAvailable += i => _treeSpeciesEditorSelection = i;
        }

        #region Static Fields and Autoproperties

        public static bool _autobuilding;
        public static bool _enabled;
        public static bool _executing;
        public static EditorCoroutine _coroutine;

        [NonSerialized] private static AppaContext _context;

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(BranchBuildManager));
                }

                return _context;
            }
        }

        private static BranchDataContainer CTX =>
            (_treeSpeciesEditorSelection == null ? null : _treeSpeciesEditorSelection)?.branch?.selection
          ?.selected;

        public static IEnumerator ExecuteAllBuildsEnumerator(
            QualityMode quality,
            Action buildAction,
            string searchString)
        {
            var branches = AssetDatabaseManager.FindAssets<BranchDataContainer>(searchString);

            return ExecuteAllBuildsEnumerator(quality, buildAction, branches);
        }

        public static IEnumerator ExecuteAllBuildsEnumerator(
            QualityMode quality,
            Action buildAction,
            IEnumerable<BranchDataContainer> branches)
        {
            try
            {
                if (EditorApplication.isCompiling || AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    yield break; // ========================================================== BREAK
                }

                foreach (var branch in branches)
                {
                    branch.settings.qualityMode = quality;
                    foreach (var snapshot in branch.snapshots)
                    {
                        snapshot.locked = false;
                        snapshot.active = true;
                    }

                    _treeSpeciesEditorSelection.branch.selection.Set(branch);

                    buildAction();

                    var build = ExecutePendingBuild(true);

                    while (build.MoveNext())
                    {
                        yield return build.Current;
                    }

                    branch.Save();
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

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (AppalachiaApplication.IsPlayingOrWillPlay)
            {
                return;
            }

            EditorApplication.update += Update;
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
                        CTX.progressTracker.buildActive ||
                        (CTX.requestLevel == BuildRequestLevel.None))
                    {
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

        private static void DilateTexture(Texture2D texture, int iterations)
        {
            var avg = Vector3.zero;
            var avgCount = 0;

            var cols = texture.GetPixels();
            var copyCols = texture.GetPixels();
            var borderIndices = new HashSet<int>();
            var indexBuffer = new HashSet<int>();
            var w = texture.width;

            //int h = texture.height;
            for (var i = 0; i < cols.Length; i++)
            {
                if (cols[i].a < 0.5f)
                {
                    for (var x = -1; x < 2; x++)
                    {
                        for (var y = -1; y < 2; y++)
                        {
                            var index = i + (y * w) + x;
                            if ((index >= 0) &&
                                (index < cols.Length) &&
                                (cols[index].a >
                                 0.5f)) // if a non transparent pixel is near the transparent one, add the transparent pixel index to border indices
                            {
                                borderIndices.Add(i);
                                goto End;
                            }
                        }
                    }

                    End: ;
                }
                else
                {
                    var col = cols[i];
                    avg += new Vector3(col.r, col.g, col.b);
                    avgCount += 1;
                }
            }

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                foreach (var i in borderIndices)
                {
                    var meanCol = Color.black;
                    var opaqueNeighbours = 0;
                    for (var x = -1; x < 2; x++)
                    {
                        for (var y = -1; y < 2; y++)
                        {
                            var index = i + (y * w) + x;
                            if ((index >= 0) && (index < cols.Length) && (index != i))
                            {
                                if (cols[index].a > 0.5f)
                                {
                                    cols[index].a = 1;
                                    meanCol += cols[index];
                                    opaqueNeighbours++;
                                }
                                else
                                {
                                    indexBuffer.Add(index);
                                }
                            }
                        }
                    }

                    cols[i] = meanCol / opaqueNeighbours;
                }

                indexBuffer.ExceptWith(borderIndices);

                borderIndices = indexBuffer;
                indexBuffer = new HashSet<int>();
            }

            var empty = Color.black;
            empty.a = 0f;
            var bv = avg / avgCount;
            var background = new Color(bv.x, bv.y, bv.z);
            background.a = 0f;
            for (var i = 0; i < cols.Length; i++)
            {
                cols[i].a = copyCols[i].a;

                if (cols[i] == empty)
                {
                    cols[i] = background;
                    cols[i].a = 0f;
                }
            }

            texture.SetPixels(cols);
        }

        private static void EqualizeSurface(Texture2D texture, bool r, bool g, bool b, bool a)
        {
            var minimums = Vector4.one;
            var maximums = Vector4.zero;

            var pixels = texture.GetPixels32();
            var w = texture.width;
            var h = texture.height;

            for (var i = 0; i < pixels.Length; i++)
            {
                var pixel = pixels[i];

                if (pixel.r < minimums.x)
                {
                    minimums.x = pixel.r;
                }

                if (pixel.r > maximums.x)
                {
                    maximums.x = pixel.r;
                }

                if (pixel.g < minimums.y)
                {
                    minimums.y = pixel.g;
                }

                if (pixel.g > maximums.y)
                {
                    maximums.y = pixel.g;
                }

                if (pixel.b < minimums.z)
                {
                    minimums.z = pixel.b;
                }

                if (pixel.b > maximums.z)
                {
                    maximums.z = pixel.b;
                }

                if (pixel.a < minimums.w)
                {
                    minimums.w = pixel.a;
                }

                if (pixel.a > maximums.w)
                {
                    maximums.w = pixel.a;
                }
            }

            var rRange = maximums.x - minimums.x;
            var gRange = maximums.y - minimums.y;
            var bRange = maximums.z - minimums.z;
            var aRange = maximums.w - minimums.w;

            for (var i = 0; i < pixels.Length; i++)
            {
                var pixel = pixels[i];

                if (r)
                {
                    pixel.r = (byte)(255 * (rRange == 0.0f ? 0.0f : (pixel.r - minimums.x) / rRange));
                }

                if (g)
                {
                    pixel.g = (byte)(255 * (gRange == 0.0f ? 0.0f : (pixel.g - minimums.y) / gRange));
                }

                if (b)
                {
                    pixel.b = (byte)(255 * (bRange == 0.0f ? 0.0f : (pixel.b - minimums.z) / bRange));
                }

                if (a)
                {
                    pixel.a = (byte)(255 * (aRange == 0.0f ? 0.0f : (pixel.a - minimums.w) / aRange));
                }

                pixels[i] = pixel;
            }

            texture.SetPixels32(pixels);
        }

        private static IEnumerator ExecutePendingBuild(bool automatic = false)
        {
            //using (BUILD_TIME.TREE_BUILD_MGR.ExecutePendingBuild.Auto())
            //{
            var completed = false;

            var branchData = CTX;
            branchData.dataState = TSEDataContainer.DataState.Normal;

            try
            {
                if (EditorApplication.isCompiling || AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    completed = true;
                    yield break; // ========================================================== BREAK
                }

                if ((branchData.buildState == BuildState.Disabled) ||
                    (branchData.requestLevel == BuildRequestLevel.None))
                {
                    EditorCoroutineUtility.StopCoroutine(_coroutine);
                    completed = true;
                    yield break; // ========================================================== BREAK
                }

                if (branchData.buildState == BuildState.ForceFull)
                {
                    if (branchData.dataState == TSEDataContainer.DataState.PendingSave)
                    {
                        branchData.Save();
                    }

                    branchData.hierarchyPrefabs.ResetPrefabs();
                    branchData.materials.hash = null;
                }

                branchData.RebuildStructures();

                branchData.progressTracker.InitializeBuildBatch();

                if (branchData.subfolders == null)
                {
                    branchData.subfolders = TreeAssetSubfolders.CreateNested(branchData);
                }

                branchData.subfolders.CreateFolders();
                branchData.subfolders.CreateSnapshotFolder();

                SeedManager.UpdateSeeds(branchData.branch);

                branchData.hierarchyPrefabs.UpdatePrefabs(branchData.branch);
                branchData.hierarchyPrefabs.UpdatePrefabAvailability(branchData.branch);

                branchData.materials.UpdateMaterials(branchData.branch, branchData.hierarchyPrefabs);
                branchData.materials.inputMaterialCache.SetDefaultMaterials(
                    branchData.settings.material.defaultMaterials
                );

                branchData.materials.inputMaterialCache.UpdateDefaultMaterials();

                if (branchData.requestLevel > BuildRequestLevel.None)
                {
                    var requestLevel = branchData.requestLevel;

                    branchData.progressTracker.InitializeBuild(
                        requestLevel,
                        branchData.buildRequest.GetBuildCosts(requestLevel)
                    );

                    TreeProperty.SetActiveGenerationAge(AgeType.Mature);

                    if (branchData.buildRequest.ShouldBuild(BuildCategory.Distribution, requestLevel))
                    {
                        GenerateDistribution(requestLevel, branchData);

                        yield return null;
                    }

                    #region BREAK / CONTINUE

                    if (branchData.requestLevel != requestLevel)
                    {
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        completed = true;
                        yield break; // ========================================== BREAK
                    }

                    #endregion

                    branchData.branch.seed.Reset();

                    if (branchData.buildRequest.ShouldBuildGeometry(requestLevel))
                    {
                        GenerateGeometry(requestLevel, branchData);

                        if (requestLevel <= BuildRequestLevel.FinalPass)
                        {
                            yield return null;
                        }
                    }

                    #region BREAK

                    if (branchData.requestLevel != requestLevel)
                    {
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        completed = true;
                        yield break; // ====================================================== BREAK
                    }

                    #endregion

                    yield return null;

                    GenerateMaterials(requestLevel, branchData, ref completed);

                    yield return null;

                    #region BREAK

                    if (branchData.requestLevel != requestLevel)
                    {
                        completed = true;
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                        yield break; // ===========================================BREAK
                    }

                    #endregion

                    branchData.branch.seed.Reset();

                    GenerateEnhancements(requestLevel, branchData);

                    yield return null;

                    if (branchData.settings.qualityMode > QualityMode.Working)
                    {
                        GenerateBranchMaterial(requestLevel, branchData, ref completed);
                    }

                    //if (branchData.buildRequest.ShouldBuild(BuildCategory.PrefabCreation, requestLevel))
                    /*{
                        AssetManager.SaveAllAssets(branchData);
                    }*/

                    branchData.PushBuildRequestLevelAll(false);

                    branchData.progressTracker.CompleteBuild();
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

                            if (branchData != null)
                            {
                                branchData.PushBuildRequestLevelAll(false);
                            }
                        }
                    }

                    if (branchData.progressTracker.buildActive)
                    {
                        branchData.dataState = TSEDataContainer.DataState.PendingSave;

                        branchData.progressTracker.CompleteBuildBatch(completed);
                    }

                    if (!automatic)
                    {
                        TreeMaterialUsageTracker.Refresh();
                        _executing = false;
                        EditorCoroutineUtility.StopCoroutine(_coroutine);
                    }
                }
            }

            //}
        }

        private static void GenerateBranchMaterial(
            BuildRequestLevel requestLevel,
            BranchDataContainer branchData,
            ref bool completed)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateBranchMaterial.Auto())
            {
                branchData.dataState = TSEDataContainer.DataState.PendingSave;

                foreach (var snapshot in branchData.snapshots)
                {
                    if (snapshot.locked)
                    {
                        continue;
                    }

                    if (!snapshot.active)
                    {
                        if (branchData.buildState == BuildState.Default)
                        {
                            continue;
                        }
                    }

                    if (snapshot.textureSize == 0)
                    {
                        snapshot.textureSize = TextureSize.k1024;
                    }

                    snapshot.branchOutputMaterial.EnsureCreated(1);

                    var material = snapshot.branchOutputMaterial.GetMaterialElementByIndex(0);
                    material.asset.name = snapshot.name + "_snapshot";

                    snapshot.branchOutputMaterial.textureSet.Clear();

                    var outputTextures = new List<OutputTexture>();

                    foreach (var outputProfile in
                             snapshot.branchOutputMaterial.GetOutputTextureProfiles(true))
                    {
                        var newTextureName = ZString.Format(
                            "{0}_{1}",
                            material.asset.name,
                            outputProfile.fileNameSuffix
                        );

                        var adjustedTextuerSize = snapshot.textureSizeV2;

                        Texture renderTexture;

                        if (outputProfile.map == TextureMap.Albedo)
                        {
                            renderTexture = SnapshotRenderer.RenderMeshPreview(
                                branchData,
                                snapshot,
                                adjustedTextuerSize,
                                SnapshotRenderer.SnapshotMode.Albedo
                            );
                        }
                        else if (outputProfile.map == TextureMap.Normal_TS)
                        {
                            renderTexture = SnapshotRenderer.RenderMeshPreview(
                                branchData,
                                snapshot,
                                adjustedTextuerSize,
                                SnapshotRenderer.SnapshotMode.Normal
                            );
                        }
                        else
                        {
                            renderTexture = SnapshotRenderer.RenderMeshPreview(
                                branchData,
                                snapshot,
                                adjustedTextuerSize,
                                SnapshotRenderer.SnapshotMode.Surface
                            );
                        }

                        var newTexture = outputProfile.settings.Create(
                            new Vector2(renderTexture.width, renderTexture.height),
                            newTextureName
                        );

                        var rt = RenderTexture.active;
                        RenderTexture.active = (RenderTexture)renderTexture;

                        try
                        {
                            using (BUILD_TIME.TEX_COMBR.ReadRenderTexture.Auto())
                            {
                                var rect = new Rect(0, 0, renderTexture.width, renderTexture.height);

                                newTexture.ReadPixels(rect, 0, 0);
                                newTexture.Apply(true);
                            }
                        }
                        finally
                        {
                            RenderTexture.active = rt;
                        }

                        if (outputProfile.map == TextureMap.Albedo)
                        {
                            DilateTexture(newTexture, 48);
                        }
                        else if ((outputProfile.map == TextureMap.MAOTS) &&
                                 (snapshot.equalizeAmbientOcclusion ||
                                  snapshot.equalizeTranslucency ||
                                  snapshot.equalizeSmoothness))
                        {
                            EqualizeSurface(
                                newTexture,
                                false,
                                snapshot.equalizeAmbientOcclusion,
                                snapshot.equalizeTranslucency,
                                snapshot.equalizeSmoothness
                            );
                        }

                        var outputTexture = new OutputTexture(outputProfile, newTexture);

                        outputTextures.Add(outputTexture);
                        newTexture.name = newTextureName;
                    }

                    /*
                    GL.sRGBWrite = srgb;
                    */

                    snapshot.branchOutputMaterial.textureSet.Set(outputTextures);

                    var filePaths = outputTextures.SaveTextures(
                        snapshot.branchOutputMaterial,
                        branchData.subfolders,
                        1,
                        TreeAssetSubfolderType.Snapshots
                    );

                    outputTextures.ReimportTextures(
                        snapshot.branchOutputMaterial,
                        branchData.subfolders,
                        snapshot.textureSize,
                        branchData.settings.qualityMode,
                        filePaths,
                        TreeAssetSubfolderType.Snapshots
                    );

                    snapshot.branchOutputMaterial.FinalizeMaterial();
                }
            }
        }

        private static void GenerateDistribution(
            BuildRequestLevel requestLevel,
            BranchDataContainer branchData)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateIndividualStageShapes.Auto())
            {
                TreeProperty.SetActiveGenerationAge(AgeType.Mature);

                var regenerateAOSampling =
                    branchData.buildRequest.ShouldBuild(BuildCategory.AmbientOcclusion, requestLevel) ||
                    (branchData.samplePoints == null);

                if (branchData.buildRequest.distribution == requestLevel)
                {
                    branchData.progressTracker.Update(BuildCategory.Distribution);

                    branchData.branch.hierarchies.Rebuild();
                    branchData.branch.shapes.Rebuild();

                    ShapeGenerationManager.GenerateShapes(
                        branchData.branch.shapes,
                        branchData.branch.hierarchies
                    );

                    branchData.branch.shapes.Rebuild();
                }

                if (regenerateAOSampling)
                {
                    branchData.progressTracker.Update(BuildCategory.AmbientOcclusion);

                    branchData.samplePoints =
                        AmbientOcclusionGenerator.GenerateAOSamplePoints(branchData.branch);
                }
            }
        }

        private static void GenerateEnhancements(
            BuildRequestLevel requestLevel,
            BranchDataContainer branchData)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateIndividualStageEnhancements.Auto())
            {
                var buildMesh = branchData.buildRequest.ShouldBuildMesh(requestLevel);
                var buildUV = branchData.buildRequest.ShouldBuild(BuildCategory.UV, requestLevel);

                //var lowQuality = !branchData.buildRequest.ShouldBuild(BuildCategory.HighQualityGeometry, requestLevel);

                if (buildUV)
                {
                    branchData.progressTracker.Update(BuildCategory.UV);

                    UVManager.RemapUVCoordinates(branchData.branch.output, branchData.materials);
                    UVManager.ApplyLeafRects(
                        branchData.branch.hierarchies,
                        branchData.branch.shapes,
                        branchData.materials.inputMaterialCache,
                        branchData.branch.output,
                        branchData.branch.seed
                    );
                }

                if (buildMesh)
                {
                    branchData.progressTracker.Update(BuildCategory.Mesh);

                    if (branchData.branchAsset.mesh == null)
                    {
                        branchData.branchAsset.Refresh();
                    }

                    var origMaterialIDs = branchData.branch.output.materialIDs;

                    var materials = new HashSet<OutputMaterial>();

                    foreach (var materialID in origMaterialIDs)
                    {
                        if (materialID == -1)
                        {
                            continue;
                        }

                        var m = branchData.materials.GetOutputMaterialByInputID(materialID);

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

                    var runtimeMaterials = MeshGenerator.GenerateMeshes(
                        branchData.branch.output,
                        branchData.branch.shapes.byID,
                        branchData.materials.inputMaterialCache,
                        mats,
                        branchData.settings.mesh,
                        branchData.branchAsset.mesh,
                        vd => Color.black,
                        false,
                        true,
                        default
                    );

                    branchData.branchAsset.mesh.uv2 = null;
                    branchData.branchAsset.mesh.uv3 = null;
                    branchData.branchAsset.mesh.uv4 = null;
                    branchData.branchAsset.mesh.uv5 = null;

                    branchData.branchAsset.SetMaterials(runtimeMaterials.ToArray());
                }
            }
        }

        private static void GenerateGeometry(BuildRequestLevel requestLevel, BranchDataContainer branchData)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateIndividualStageGeometry.Auto())
            {
                var buildHQGeometry = branchData.buildRequest.geometry == requestLevel;

                branchData.branch.seed.Reset();

                foreach (var hierarchy in branchData.branch.hierarchies)
                {
                    hierarchy.seed.Reset();
                }

                branchData.branch.shapes.Rebuild();

                branchData.branch.output.Clear();

                branchData.progressTracker.Update(
                    buildHQGeometry ? BuildCategory.HighQualityGeometry : BuildCategory.LowQualityGeometry
                );

                GeometryGenerator.GenerateGeometry(
                    branchData.branch.hierarchies,
                    branchData.branch.shapes,
                    branchData.branch.output,
                    branchData.materials.inputMaterialCache,
                    branchData.hierarchyPrefabs,
                    branchData.settings,
                    buildHQGeometry,
                    branchData.branch.seed
                );

                if (branchData.branch.output.vertices.Count == 0)
                {
                    throw new NotSupportedException("Bad vertex count!");
                }

                branchData.progressTracker.Update(BuildCategory.AmbientOcclusion);

                if (branchData.buildRequest.ShouldBuild(BuildCategory.AmbientOcclusion, requestLevel))
                {
                    AmbientOcclusionGenerator.ApplyAmbientOcclusion(
                        branchData.branch.output,
                        branchData.samplePoints,
                        branchData.settings.ao.style,
                        branchData.settings.ao.density,
                        branchData.settings.ao.raytracingSamples,
                        branchData.settings.ao.raytracingRange
                    );
                }
                else
                {
                    AmbientOcclusionGenerator.ApplyDefaultAmbientOcclusion(branchData.branch.output);
                }
            }
        }

        private static void GenerateMaterials(
            BuildRequestLevel requestLevel,
            BranchDataContainer branchData,
            ref bool completed)
        {
            using (BUILD_TIME.TREE_BUILD_MGR.GenerateSpeciesMaterials.Auto())
            {
                if (branchData.buildRequest.ShouldBuild(BuildCategory.MaterialGeneration, requestLevel))
                {
                    branchData.progressTracker.Update(BuildCategory.MaterialGeneration);

                    branchData.dataState = TSEDataContainer.DataState.PendingSave;

                    if (branchData.materials.RequiresUpdate(branchData.settings.material, out _))
                    {
                        /*if (!branchData.settings.material.doNotUnsetForceRegenerateMaterials)
                        {
                            branchData.settings.material.forceRegenerateMaterials = false;
                        }
                        */

                        if (branchData.settings.material.imageAtlasIsProportionalToArea)
                        {
                            MaterialPropertyManager.SetMaterialProportionalAreas(
                                branchData.materials.inputMaterialCache,
                                branchData.branch.output
                            );
                        }
                        else
                        {
                            MaterialPropertyManager.SetMaterialNonProportionalAreas(
                                branchData.materials.inputMaterialCache
                            );
                        }

                        if (branchData.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            return;
                        }

                        branchData.progressTracker.Update(BuildCategory.MaterialGeneration);

                        var combinedInputMaterials = branchData.materials.inputMaterialCache
                                                               .atlasInputMaterials.Concat<InputMaterial>(
                                                                    branchData.materials.inputMaterialCache
                                                                       .tiledInputMaterials
                                                                )
                                                               .ToArray();

                        branchData.materials.UpdateMaterialNames(branchData.branch.nameBasis);

                        var textureOutputCollection = new Dictionary<OutputMaterial, List<OutputTexture>>();

                        combinedInputMaterials.ActivateProcessingTextureImporterSettings(
                            branchData.settings.texture.atlasTextureSize
                        );

                        var adjustedTextuerSize = branchData.settings.texture.textureSizeV2;
                        if (branchData.settings.qualityMode == QualityMode.Preview)
                        {
                            adjustedTextuerSize *= .25f;
                        }
                        else if (branchData.settings.qualityMode == QualityMode.Working)
                        {
                            adjustedTextuerSize *= .125f;
                        }

                        branchData.materials.outputMaterialCache.atlasOutputMaterial.EnsureCreated(1);

                        var outputTextures = TextureCombiner.DrawAtlasTextures(
                            branchData.materials.inputMaterialCache.atlasInputMaterials,
                            branchData.materials.outputMaterialCache.atlasOutputMaterial,
                            adjustedTextuerSize,
                            branchData.settings.texture.debugCombinationOutputs
                        );

                        textureOutputCollection.Add(
                            branchData.materials.outputMaterialCache.atlasOutputMaterial,
                            outputTextures
                        );

                        branchData.progressTracker.Update(BuildCategory.MaterialGeneration);

                        foreach (var tiledMaterial in branchData.materials.outputMaterialCache
                                                                .tiledOutputMaterials)
                        {
                            var match =
                                branchData.materials.inputMaterialCache.tiledInputMaterials.FirstOrDefault(
                                    tim => tiledMaterial.inputMaterialID == tim.materialID
                                );

                            tiledMaterial.EnsureCreated(1);

                            var ot = TextureCombiner.DrawTiledTextures(
                                match,
                                tiledMaterial,
                                adjustedTextuerSize,
                                branchData.settings.texture.tiledMaterialsKeepOriginalSize,
                                branchData.settings.texture.debugCombinationOutputs
                            );

                            textureOutputCollection.Add(tiledMaterial, ot);
                        }

                        if (branchData.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            return;
                        }

                        branchData.progressTracker.Update(BuildCategory.SavingTextures);

                        var filePaths = textureOutputCollection.SaveTextures(branchData.subfolders, 1);

                        textureOutputCollection.ReimportTextures(
                            branchData.subfolders,
                            branchData.settings.texture.atlasTextureSize,
                            branchData.settings.qualityMode,
                            filePaths,
                            1
                        );

                        branchData.materials.outputMaterialCache.atlasOutputMaterial.FinalizeMaterial();

                        foreach (var tiledMaterial in branchData.materials.outputMaterialCache
                                                                .tiledOutputMaterials)
                        {
                            tiledMaterial.FinalizeMaterial();
                        }

                        if (branchData.settings.qualityMode == QualityMode.Finalized)
                        {
                            combinedInputMaterials.RestoreOriginalTextureImporterSettings();
                        }

                        if (branchData.requestLevel != requestLevel)
                        {
                            EditorCoroutineUtility.StopCoroutine(_coroutine);
                            completed = true;
                            return;
                        }
                    }
                }

                if (branchData.buildRequest.ShouldBuild(BuildCategory.MaterialProperties, requestLevel))
                {
                    branchData.progressTracker.Update(BuildCategory.MaterialProperties);

                    /*MaterialPropertyManager.PrepareMaterialProperties(
                        branchData.settings.material.enableDynamicMaterialPrototypes,
                        branchData.materials.inputMaterialCache,
                        branchData.materials.outputMaterialCache
                    );

                    MaterialPropertyManager.AssignDefaultMaterialProperties(
                        branchData.materials.outputMaterialCache,
                        branchData.settings.material
                    );*/
                }

                branchData.materials.hash = branchData.materials.CalculateHash();
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(BranchBuildManager) + ".";

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));

        #endregion
    }
}
