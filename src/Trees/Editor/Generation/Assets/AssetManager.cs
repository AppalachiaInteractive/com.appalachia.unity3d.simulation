using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Extensions;
using Appalachia.Core.Labels;
using Appalachia.Editing.Scene.Prefabs;
using Appalachia.Rendering.Prefabs.Rendering.MultiStage.Trees;
using Appalachia.Rendering.Prefabs.Rendering.Replacement;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Generation.Impostors;
using Appalachia.Simulation.Trees.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [InitializeOnLoad]
    public static class AssetManager
    {
        private static Scene ___previewScene;

        private static Scene _previewScene
        {
            get
            {
                if ((___previewScene == default) || !EditorSceneManager.IsPreviewScene(___previewScene))
                {
                    ___previewScene = EditorSceneManager.NewPreviewScene();
                }

                return ___previewScene;
            }
        }

        private static void ClosePreviewScene(Scene scene)
        {
            if (EditorSceneManager.IsPreviewScene(scene))
            {
                EditorSceneManager.ClosePreviewScene(scene);
            }
        }

        public static void OnFinalize()
        {
            ClosePreviewScene(___previewScene);
            ___previewScene = default;
        }


        private static Mesh SaveMesh(
            TreeAssetSubfolders subfolders,
            Mesh asset)
        {
            using (BUILD_TIME.ASSET_MGR.SaveMesh.Auto())
            {
                var path = subfolders.GetFilePathByType(TreeAssetSubfolderType.Meshes, $"{asset.name}.mesh");

                var existingPath = AssetDatabaseManager.GetAssetPath(asset);

                if (string.IsNullOrWhiteSpace(existingPath))
                {
                    AssetDatabaseManager.CreateAsset(asset, path);
                }
                else if (existingPath != path)
                {
                    AssetDatabaseManager.MoveAsset(existingPath, path);
                }

                asset.UploadMeshData(false);

                return asset;
            }
        }

        public static void SaveAllAssets(TreeDataContainer tree, bool buildImpostors)
        {
            using (BUILD_TIME.ASSET_MGR.SaveAllAssets.Auto())
            {
                foreach (var individual in tree.individuals)
                {
                    foreach (var age in individual.ages)
                    {
                        foreach (var stage in age.stages)
                        {
                            try
                            {
                                SaveAsset(tree, individual, stage, buildImpostors);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError(ex);
                            }
                        }
                    }
                }

                try
                {
                    tree.dataState = TSEDataContainer.DataState.Normal;
                    AssetDatabaseManager.SaveAssets();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    tree.dataState = TSEDataContainer.DataState.PendingSave;
                }
            }
        }

        public static void SaveAllAssets(LogDataContainer log)
        {
            using (BUILD_TIME.ASSET_MGR.SaveAllAssets.Auto())
            {
                var mat = log.material;
                string path;
                string existingPath;

                using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsGetPath.Auto())
                {
                    path = log.subfolders.GetFilePathByType(TreeAssetSubfolderType.Materials, $"{mat.name}.mat"                    );

                    existingPath = AssetDatabaseManager.GetAssetPath(mat);
                }

                if (string.IsNullOrWhiteSpace(existingPath))
                {
                    using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsCreate.Auto())
                    {
                        AssetDatabaseManager.CreateAsset(mat, path);
                    }
                }
                else if (existingPath != path)
                {
                    using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsMove.Auto())
                    {
                        AssetDatabaseManager.MoveAsset(existingPath, path);
                    }
                }
                
                foreach (var instance in log.logInstances)
                {
                    SaveAsset(log, instance);
                }
                
                AssetDatabaseManager.SaveAssets();
            }
        }
        
        public static void SaveAsset(LogDataContainer log, LogInstance instance)
        {            
            using (BUILD_TIME.ASSET_MGR.SaveNonPrefabAssets.Auto())
            {
                if (instance.asset.levels.Count > log.settings.lod.levels)
                {
                    for (var i = instance.asset.levels.Count - 1; i >= log.settings.lod.levels; i--)
                    {
                        instance.asset.levels.RemoveAt(i);
                    }
                }

                for (var i = 0; i < instance.asset.levels.Count; i++)
                {
                    var level = instance.asset.levels[i];
                    level.mesh.name = instance.asset.GetMeshName(i);
                    level.mesh = SaveMesh(log.subfolders, level.mesh);
                }

                instance.asset.collisionMeshes.SaveMeshes(m => SaveMesh(log.subfolders, m));

                UpdatePrefab(log, instance);
            }
        }

        public static void SaveAsset(TreeDataContainer tree, TreeIndividual individual, TreeStage stage, bool buildImpostor)
        {
            using (BUILD_TIME.ASSET_MGR.SaveAsset.Auto())
            {
                using (BUILD_TIME.ASSET_MGR.SaveNonPrefabAssets.Auto())
                {
                    if (stage.asset.levels.Count > tree.settings.lod.levels)
                    {
                        for (var i = stage.asset.levels.Count - 1; i >= tree.settings.lod.levels; i--)
                        {
                            stage.asset.levels.RemoveAt(i);
                        }
                    }

                    for (var i = 0; i < stage.asset.levels.Count; i++)
                    {
                        var level = stage.asset.levels[i];
                        level.mesh.name = stage.asset.GetMeshName(i);
                        level.mesh = SaveMesh(tree.subfolders, level.mesh);
                    }

                    if (stage.asset.shadowCasterMesh != null)
                    {
                        stage.asset.shadowCasterMesh = SaveMesh(tree.subfolders, stage.asset.shadowCasterMesh);
                    }
                    
                    stage.asset.collisionMeshes.SaveMeshes(m => SaveMesh(tree.subfolders, m));
                }

                UpdatePrefab(tree, individual, stage, buildImpostor);
            }
        }

        public static void SaveIntegrationAsset(TreeDataContainer tree, TreeIndividual individual)
        {
            using (BUILD_TIME.ASSET_MGR.SaveIntegrationAsset.Auto())
            {
                foreach (var age in individual.ages)
                {
                    try
                    {
                        var path = tree.subfolders.GetFilePathByType(
                            TreeAssetSubfolderType.Prefabs,
                            $"{age.integrationAsset.CleanName}.prefab"
                        );

                        if (age.integrationAsset.prefab == null)
                        {
                            var prefab = new GameObject(age.integrationAsset.CleanName);

                            PrefabUtility.SaveAsPrefabAsset(prefab, path);

                            age.integrationAsset.prefab = AssetDatabaseManager.LoadAssetAtPath<GameObject>(path);

                            Object.DestroyImmediate(prefab);
                        }
                        else
                        {
                            age.integrationAsset.prefab.name = age.integrationAsset.CleanName;

                            var existingPath = AssetDatabaseManager.GetAssetPath(age.integrationAsset.prefab);

                            if (string.IsNullOrWhiteSpace(existingPath))
                            {
                                PrefabUtility.SaveAsPrefabAsset(age.integrationAsset.prefab, path);
                            }
                            else if (existingPath != path)
                            {
                                AssetDatabaseManager.MoveAsset(existingPath, path);
                            }
                        }

                        using (var mutable = age.integrationAsset.ToMutable(_previewScene))
                        {
                            var go = mutable.Mutable;

                            age.integrationAsset.integrationPrefab = go.GetComponent<TreeGPUInstancerPrefab>();

                            if (age.integrationAsset.integrationPrefab == null)
                            {
                                age.integrationAsset.integrationPrefab = go.AddComponent<TreeGPUInstancerPrefab>();
                            }

                            age.integrationAsset.normal = age.normalStage.asset.prefab;
                            age.integrationAsset.stump = age[StageType.Stump]?.asset.prefab;
                            age.integrationAsset.stumpRotted = age[StageType.StumpRotted]?.asset.prefab;
                            age.integrationAsset.felled = age[StageType.Felled]?.asset.prefab;
                            age.integrationAsset.felledBare = age[StageType.FelledBare]?.asset.prefab;
                            age.integrationAsset.felledBareRotted = age[StageType.FelledBareRotted]?.asset.prefab;
                            age.integrationAsset.dead = age[StageType.Dead]?.asset.prefab;
                            age.integrationAsset.deadFelled = age[StageType.DeadFelled]?.asset.prefab;
                            age.integrationAsset.deadFelledRotted = age[StageType.DeadFelledRotted]?.asset.prefab;

                            for (var i = go.transform.childCount - 1; i >= 0; i--)
                            {
                                go.transform.GetChild(i).DestroySafely();
                            }

                            if (age.integrationAsset.normal != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.normal, go.transform);
                            if (age.integrationAsset.stump != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.stump, go.transform);
                            if (age.integrationAsset.stumpRotted != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.stumpRotted, go.transform);
                            if (age.integrationAsset.felled != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.felled, go.transform);
                            if (age.integrationAsset.felledBare != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.felledBare, go.transform);
                            if (age.integrationAsset.felledBareRotted != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.felledBareRotted, go.transform);
                            if (age.integrationAsset.dead != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.dead, go.transform);
                            if (age.integrationAsset.deadFelled != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.deadFelled, go.transform);
                            if (age.integrationAsset.deadFelledRotted != null)
                                PrefabUtility.InstantiatePrefab(age.integrationAsset.deadFelledRotted, go.transform);
                        }

                        PrefabReplacementCollection.instance.State.AddOrUpdate(
                            age.normalStage.asset.prefab,
                            age.integrationAsset.prefab
                        );
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        PrefabInspector.Inspect(age.integrationAsset.prefab);
                        throw;
                    }
                }
            }
        }

        public static void SaveAllAssets(BranchDataContainer branch)
        {
            using (BUILD_TIME.ASSET_MGR.SaveAllAssets.Auto())
            {
                SaveNonPrefabAssets(branch);
            }
        }

        
        public static void SaveNonPrefabAssets(BranchDataContainer branch)
        {
            using (BUILD_TIME.ASSET_MGR.SaveNonPrefabAssets.Auto())
            {
                branch.branchAsset.mesh = SaveMesh(branch.subfolders, branch.branchAsset.mesh);
                
                AssetDatabaseManager.SaveAssets();
            }
        }


        private static void UpdatePrefab(
            TreeDataContainer tree,
            TreeIndividual individual,
            TreeStage stage,
            bool buildImpostor)
        {
            using (BUILD_TIME.ASSET_MGR.UpdatePrefab.Auto())
            {
                try
                {
                    var path = tree.subfolders.GetFilePathByType(
                        TreeAssetSubfolderType.Prefabs,
                        $"{stage.asset.CleanName}.prefab"
                    );

                    if (stage.asset.prefab == null)
                    {
                        using (BUILD_TIME.ASSET_MGR.UpdatePrefabSave.Auto())
                        {
                            var prefab = new GameObject(stage.asset.CleanName);

                            PrefabUtility.SaveAsPrefabAsset(prefab, path);

                            stage.asset.prefab = AssetDatabaseManager.LoadAssetAtPath<GameObject>(path);

                            Object.DestroyImmediate(prefab);
                        }
                    }
                    else
                    {
                        stage.asset.prefab.name = stage.asset.CleanName;

                        var existingPath = AssetDatabaseManager.GetAssetPath(stage.asset.prefab);

                        if (string.IsNullOrWhiteSpace(existingPath))
                        {
                            PrefabUtility.SaveAsPrefabAsset(stage.asset.prefab, path);
                        }
                        else if (existingPath != path)
                        {
                            AssetDatabaseManager.MoveAsset(existingPath, path);
                        }
                    }

                    var requiresUpdate = RequiresUpdate(tree, individual, stage);

                    if (requiresUpdate)
                    {
                        using (BUILD_TIME.ASSET_MGR.UpdatePrefabUpdate.Auto())
                        {


                            using (var mutable = stage.asset.ToMutable(_previewScene))
                            {
                                UpdatePrefabLoaded(tree, individual, stage, mutable.Mutable, buildImpostor);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    PrefabInspector.Inspect(stage.asset.prefab);
                    throw;
                }
            }
        }

        private static bool RequiresUpdate(TreeDataContainer tree, TreeIndividual individual, TreeStage stage)
        {
            using (BUILD_TIME.ASSET_MGR.RequiresUpdate.Auto())
            {
                var previewMode = tree.settings.qualityMode == QualityMode.Working;
                var asset = stage.asset;
                var generateShadowCaster = tree.settings.lod.shadowCaster;
                var generateColliders = tree.settings.collision.generateColliders && !previewMode;
                var generateOcclusionMesh = tree.settings.ao.generateBakeMesh;
                var generateImpostor = tree.settings.lod.impostor.impostorAfterLastLevel && !previewMode;
                var assetCount = (previewMode ? 1 : asset.levels.Count);
                var lodCount = assetCount + (generateImpostor ? 1 : 0);
                var normalRendererLodCount = assetCount;
                //var generatePoints = stage.runtimeMetadata.pointsOfInterest.Count > 0;

                var shadowCasterIndex = 1;
                var colliderIndex = shadowCasterIndex + (generateShadowCaster ? 1 : 0);
                var occlusionIndex = colliderIndex + (generateColliders ? 1 : 0);
                var pointsIndex = occlusionIndex + (generateOcclusionMesh ? 1 : 0);
                var impostorIndex = pointsIndex + 1;

                var expectedChildCount =
                    2 +
                    (generateShadowCaster ? 1 : 0) +
                    (generateColliders ? 1 : 0) +
                    (generateOcclusionMesh ? 1 : 0) +
                    (generateImpostor ? 1 : 0);

                var gameObject = asset.prefab;

                var realChildCount = gameObject.transform.childCount;

                if (expectedChildCount != realChildCount)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong child count.");
                    return true;
                }

                if (individual.StageRequiresRuntimeUpdate(tree, stage))
                {
                    return true;
                }

                var lodGroup = asset.prefab.GetComponent<LODGroup>();

                if (lodGroup == null)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing LODGroup.");
                    return true;
                }
                
                if (!lodGroup.enabled)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: LODGroup disabled.");
                    return true;
                }
                
                if (lodGroup.fadeMode != LODFadeMode.CrossFade)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Incorrect fade settings.");
                    return true;
                }

                var lods = lodGroup.GetLODs();

                if (lods.Length != lodCount)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD count.");
                    return true;
                }

                var rendererGO = lodGroup.transform.Find("RENDERERS");

                if (rendererGO == null)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing renderer GO.");
                    return true;
                }

                var rendererChildren = rendererGO.transform.childCount;

                if (rendererChildren != lods.Length)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD count.");
                    return true;
                }

                for(var i = 0; i < normalRendererLodCount; i++)
                {
                    var lod = lods[i];

                    var lodSettings = tree.settings.lod[i];

                    if (Math.Abs(lod.fadeTransitionWidth - (previewMode ? 0.0f : lodSettings.fadeTransitionWidth)) > float.Epsilon)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD {i} transition width.");
                        return true;
                    }
                    
                    var rendererLength = (generateShadowCaster ? 2 : 1);
                    
                    if (lod.renderers.Length != rendererLength)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD renderer count.");
                        return true;
                    }

                    var mf = lod.renderers[0].GetComponent<MeshFilter>();

                    if (mf.sharedMesh != asset.levels[i].mesh)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD {i} mesh assigned.");
                        return true;
                    }
                        
                    if (rendererLength == 2)
                    {
                        mf = lod.renderers[1].GetComponent<MeshFilter>();
                        
                        if (mf.sharedMesh != asset.shadowCasterMesh)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD {i} shadowcaster assigned.");
                            return true;
                        }
                    }
                }
                
                GameObject renderers = gameObject.transform.GetChild(0).gameObject;

                if (renderers.name != "RENDERERS")
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong LOD name.");
                    return true;
                }

                if (!renderers.gameObject.activeSelf)
                {
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Incorrect game object active status.");
                    return true;
                }

                var dmr = renderers.GetComponents<MeshRenderer>();

                if (dmr.Length > 0)
                {                        
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra mesh renderers.");
                    return true;
                }

                var dmf = renderers.GetComponents<MeshFilter>();

                if (dmf.Length > 0)
                {                        
                    Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra mesh filters.");
                    return true;
                }

                for (var i = 0; i < normalRendererLodCount; i++)
                {
                    var child = renderers.transform.GetChild(i);

                    if (!child.gameObject.activeSelf)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Incorrect game object active status.");
                        return true;
                    }

                    var expectedName = $"LOD_{i}";

                    if (child.name != expectedName)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong child name.");
                        return true;
                    }

                    var c = child.gameObject.GetComponent<MeshCollider>();

                    if (c != null)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra MeshCollider.");
                        return true;
                    }

                    var mf = child.gameObject.GetComponent<MeshFilter>();

                    if (mf == null)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing MeshFilter.");
                        return true;
                    }

                    var mr = child.gameObject.GetComponent<MeshRenderer>();

                    if (mr == null)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing MeshRenderer.");
                        return true;
                    }

                    if (!mr.enabled)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Renderer disabled.");
                        return true;
                    }

                    if (lods[i].renderers.Length < 1)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong renderer length.");
                        return true;
                    }

                    if (mf.sharedMesh != stage.asset.levels[i].mesh)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong mesh.");
                        return true;
                    }

                    if (mr.sharedMaterials.Length != stage.asset.levels[i].materials.Length)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong material count.");
                        return true;
                    }

                    for (int j = 0; j < stage.asset.levels[i].materials.Length; j++)
                    {
                        if (mr.sharedMaterials[j] != stage.asset.levels[i].materials[j])
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong material.");
                            return true;
                        }
                    }
                }

                if (generateShadowCaster && (shadowCasterIndex <= gameObject.transform.childCount))
                {
                    GameObject shadow = gameObject.transform.GetChild(shadowCasterIndex).gameObject;

                    if (shadow.name != "SHADOWS")
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Incorrect shadow game object name.");
                        return true;
                    }

                    dmr = shadow.GetComponents<MeshRenderer>();

                    if (dmr.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra shadow mesh renderers.");
                        return true;
                    }

                    dmf = shadow.GetComponents<MeshFilter>();

                    if (dmf.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra shadow mesh filters.");
                        return true;
                    }


                    var shadowRendererCount = tree.materials.outputMaterialCache.shadowCasterOutputMaterial.Count;

                    for (var i = 0; i < shadowRendererCount; i++)
                    {                        
                        var child = shadow.transform.GetChild(i);

                        var material = tree.materials.outputMaterialCache.shadowCasterOutputMaterial
                            .GetMaterialElementByIndex(i);

                        if (!child.gameObject.activeSelf)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Incorrect game object active status.");
                            return true;
                        }

                        var expectedName = $"SHADOW_LOD_{i}";

                        if (child.name != expectedName)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong shadow name.");
                            return true;
                        }

                        var mf = child.gameObject.GetComponent<MeshFilter>();

                        if (mf == null)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing shadow MeshFilter.");
                            return true;
                        }

                        var mr = child.gameObject.GetComponent<MeshRenderer>();

                        if (mr == null)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing shadow MeshRenderer.");
                            return true;
                        }

                        if (!mr.enabled)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Shadow renderer disabled.");
                            return true;
                        }

                        if (mf.sharedMesh != stage.asset.shadowCasterMesh)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong shadow mesh.");
                            return true;
                        }

                        if (mr.sharedMaterials.Length != 1)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong shadow material count.");
                            return true;
                        }

                        if (mr.sharedMaterials[0] != material.asset)
                        {
                            Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong shadow material.");
                            return true;
                        }
                    }
                }

                if (generateColliders && (colliderIndex <= gameObject.transform.childCount))
                {
                    GameObject colliders = gameObject.transform.GetChild(colliderIndex).gameObject;
                    
                    dmr = colliders.GetComponents<MeshRenderer>();

                    if (dmr.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra collider mesh renderers.");
                        return true;
                    }

                    dmf = colliders.GetComponents<MeshFilter>();

                    if (dmf.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra collider mesh filters.");
                        return true;
                    }
                    
                    dmr = colliders.GetComponentsInChildren<MeshRenderer>();

                    if (dmr.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra collider mesh renderers.");
                        return true;
                    }

                    dmf = colliders.GetComponentsInChildren<MeshFilter>();

                    if (dmf.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra collider mesh filters.");
                        return true;
                    }
                    
                    if (!stage.asset.collisionMeshes.IsCorrectlyApplied(colliders, TreeGlobalSettings.instance))
                    {
                        return true;
                    }
                }
                
                if (generateOcclusionMesh && (occlusionIndex <= gameObject.transform.childCount))
                {
                    GameObject occlusionMesh = gameObject.transform.GetChild(occlusionIndex).gameObject;

                    if (!string.Equals(occlusionMesh.name, "OCCLUSION"))
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong occlusion GO name.");
                        return true;
                    }
                    
                    var omc = occlusionMesh.GetComponent<MeshCollider>();

                    if (omc == null)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Missing occlusion mesh collider.");
                        return true;
                    }

                    var osc = occlusionMesh.GetComponent<SphereCollider>();

                    if (osc != null)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extraneous occlusion collider.");
                        return true;
                    }

                    if (!omc.sharedMesh == asset.levels[0].mesh)
                    {       
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong occlusion mesh collider.");
                        return true;
                    }

                    if (!omc.CompareTag(TAGS.OcclusionBake))
                    {       
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Wrong occlusion tag.");
                        return true;
                    }

                    if (omc.enabled)
                    {
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Occlusion mesh collider shouold be disabled.");
                        return true;
                    }
                }

                if (pointsIndex <= gameObject.transform.childCount)
                {
                    GameObject points = gameObject.transform.GetChild(pointsIndex).gameObject;

                    dmr = points.GetComponents<MeshRenderer>();

                    if (dmr.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra points mesh renderers.");
                        return true;
                    }

                    dmf = points.GetComponents<MeshFilter>();

                    if (dmf.Length > 0)
                    {                        
                        Debug.LogWarning($"Prefab [{asset.prefab.name}] update required: Extra points mesh filters.");
                        return true;
                    }
                    
                    if (!previewMode && individual.RuntimePointsRequireUpdate(stage, points))
                    {
                        return true;
                    }
                }

                if (generateImpostor)
                {
                    return ImpostorGenerator.RequiresUpdate(stage);
                }
            }

            return false;
        }
        
        private static void UpdatePrefabLoaded(
            TreeDataContainer tree,
            TreeIndividual individual,
            TreeStage stage,
            GameObject prefab,
            bool buildImpostor)
        {
            using (BUILD_TIME.ASSET_MGR.UpdatePrefabLoaded.Auto())
            {
                var previewMode = tree.settings.qualityMode == QualityMode.Working;
                var asset = stage.asset;
                var generateShadowCaster = tree.settings.lod.shadowCaster;
                var generateColliders = tree.settings.collision.generateColliders && !previewMode;
                var generateOcclusionMesh = tree.settings.ao.generateBakeMesh;
                var generateImpostor = tree.settings.lod.impostor.impostorAfterLastLevel && !previewMode && buildImpostor;
                var assetCount = (previewMode ? 1 : asset.levels.Count);
                var lodCount = assetCount + (generateImpostor ? 1 : 0);
                var normalRendererLodCount = assetCount;
                
                
                    
                var shadowCasterIndex = 1;
                var colliderIndex = shadowCasterIndex + (generateShadowCaster ? 1 : 0);
                var occlusionIndex = colliderIndex + (generateColliders ? 1 : 0);
                var pointsIndex = occlusionIndex + (generateOcclusionMesh ? 1 : 0);
                var impostorIndex = pointsIndex + 1;

                var expectedChildCount =
                    2 +
                    (generateShadowCaster ? 1 : 0) +
                    (generateColliders ? 1 : 0) +
                    (generateOcclusionMesh ? 1 : 0) +
                    (generateImpostor ? 1 : 0);

                var childCount = prefab.transform.childCount;
                
                if (childCount > expectedChildCount)
                {
                    for (var i = childCount - 1; i >= expectedChildCount; i--)
                    {
                        Object.DestroyImmediate(prefab.transform.GetChild(i).gameObject);
                    }
                }
                else if (childCount < expectedChildCount)
                {
                    for (var i = childCount; i < expectedChildCount; i++)
                    {
                        var newChild = new GameObject();
                        newChild.transform.SetParent(prefab.transform, false);
                    }
                }

                childCount = expectedChildCount;
                
                var lodGroup = prefab.GetComponent<LODGroup>();

                if (lodGroup == null)
                {
                    lodGroup = prefab.AddComponent<LODGroup>();
                }

                lodGroup.enabled = true;
                lodGroup.fadeMode = LODFadeMode.CrossFade;
                
                var lods = new LOD[lodCount];

                for (var i = 0; i < normalRendererLodCount; i++)
                {
                    lods[i].renderers = new Renderer[generateShadowCaster ? 2 : 1];
                }
                
                GameObject renderers = prefab.transform.GetChild(0).gameObject;

                renderers.name = "RENDERERS";
                renderers.gameObject.SetActive(true);

                var dmr = renderers.GetComponents<MeshRenderer>();

                for (var i = dmr.Length - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(dmr[i]);
                }

                var dmf = renderers.GetComponents<MeshFilter>();

                for (var i = dmf.Length - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(dmf[i]);
                }

                var rendererChildren = renderers.transform.childCount;
                
                if (rendererChildren > normalRendererLodCount)
                {
                    for (var i = rendererChildren - 1; i > normalRendererLodCount; i--)
                    {
                        Object.DestroyImmediate(renderers.transform.GetChild(i).gameObject);
                    }
                }
                else if (rendererChildren < normalRendererLodCount)
                {
                    for (var i = rendererChildren; i < normalRendererLodCount; i++)
                    {
                        var newChild = new GameObject();
                        newChild.transform.SetParent(renderers.transform, false);
                    }
                }
                
                rendererChildren = renderers.transform.childCount;

                var globals = TreeGlobalSettings.instance;

                prefab.layer = globals.treeLayer;
                renderers.layer = globals.treeLayer;
                
                for (var i = 0; i < rendererChildren; i++)
                {
                    var rendererChild = renderers.transform.GetChild(i);
                    rendererChild.gameObject.layer = globals.treeLayer;

                    if (i >= normalRendererLodCount)
                    {
                        Object.DestroyImmediate(rendererChild.gameObject);
                        continue;
                    }
                    
                    rendererChild.gameObject.SetActive(true);

                    rendererChild.name = $"LOD_{i}";
                
                    var mc = rendererChild.gameObject.GetComponent<MeshCollider>();

                    if (mc != null)
                    {
                        Object.DestroyImmediate(mc);
                    }

                    var setting = tree.settings.lod.levelsOfDetail[i];

                    var mf = rendererChild.gameObject.GetComponent<MeshFilter>();

                    if (mf == null)
                    {
                        mf = rendererChild.gameObject.AddComponent<MeshFilter>();
                    }

                    var mr = rendererChild.gameObject.GetComponent<MeshRenderer>();

                    if (mr == null)
                    {
                        mr = rendererChild.gameObject.AddComponent<MeshRenderer>();
                    }

                    mr.enabled = true;
                    
                    if (generateShadowCaster)
                    {
                        mr.shadowCastingMode = ShadowCastingMode.Off;
                    }
                    else
                    {
                        mr.shadowCastingMode = ShadowCastingMode.On;
                    }

                    mr.receiveShadows = true;
                    lods[i].renderers[0] = mr;

                    mr.enabled = true;
                    lods[i].fadeTransitionWidth = previewMode ? 0.0f : setting.fadeTransitionWidth;
                
                    if ((stage.stageType == StageType.Stump) || (stage.stageType == StageType.StumpRotted))
                    {
                        var step = 1f / lodCount;
                        lods[i].screenRelativeTransitionHeight = previewMode ? 0.01f : (1f - ((1 + i) * step));
                    }
                    else
                    {
                        lods[i].screenRelativeTransitionHeight =
                            previewMode ? 0.01f : setting.screenRelativeTransitionHeight;
                    }

                    mf.sharedMesh = asset.levels[i].mesh;
                    mr.sharedMaterials = asset.levels[i].materials;
                }
                
                if (generateShadowCaster && (shadowCasterIndex <= prefab.transform.childCount))
                {
                    GameObject shadow = prefab.transform.GetChild(shadowCasterIndex).gameObject;
                    shadow.name = "SHADOWS";
                    shadow.layer = globals.treeLayer;

                    dmr = shadow.GetComponents<MeshRenderer>();

                    for (var i = dmr.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmr[i]);
                    }

                    dmf = shadow.GetComponents<MeshFilter>();

                    for (var i = dmf.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmf[i]);
                    }
                    
                    var shadowRendererCount = tree.materials.outputMaterialCache.shadowCasterOutputMaterial.Count;

                    var shadowChildren = shadow.transform.childCount;
                    
                    if (shadowChildren > shadowRendererCount)
                    {
                        for (var i = shadowChildren - 1; i >= shadowRendererCount; i--)
                        {
                            Object.DestroyImmediate(shadow.transform.GetChild(i).gameObject);
                        }
                    }
                    else if (shadowChildren < shadowRendererCount)
                    {
                        for (var i = shadowChildren; i < shadowRendererCount; i++)
                        {
                            var newChild = new GameObject();
                            newChild.transform.SetParent(shadow.transform, false);
                        }
                    }

                    var shadowRenderers = new MeshRenderer[shadowRendererCount];
                    
                    var shadowMaterial = tree.materials.outputMaterialCache.shadowCasterOutputMaterial;
                    
                    for (var i = 0; i < shadowRendererCount; i++)
                    {                        
                        var shadowChild = shadow.transform.GetChild(i);

                        var material = shadowMaterial.GetMaterialElementByIndex(i);

                        shadowChild.gameObject.SetActive(true);
                        shadowChild.name = $"SHADOW_LOD_{i}";
                        shadowChild.gameObject.layer = globals.treeLayer;
                        
                        var mf = shadowChild.gameObject.GetComponent<MeshFilter>();

                        if (mf == null)
                        {
                            mf = shadowChild.gameObject.AddComponent<MeshFilter>();
                        }

                        var mr = shadowChild.gameObject.GetComponent<MeshRenderer>();

                        if (mr == null)
                        {
                            mr = shadowChild.gameObject.AddComponent<MeshRenderer>();
                        }

                        mr.enabled = true;
                        mr.receiveShadows = false;
                        mr.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                        mf.sharedMesh = asset.shadowCasterMesh;
                        mr.sharedMaterials = new Material[] { material.asset};
                        shadowRenderers[i] = mr;
                    }

                    for (var i = 0; i < normalRendererLodCount; i++)
                    {
                        var index = shadowMaterial.GetMaterialIndexForLOD(i);
                        lods[i].renderers[1] = shadowRenderers[index];
                    }
                }

                if (generateColliders && (colliderIndex <= prefab.transform.childCount))
                {
                    GameObject colliders = prefab.transform.GetChild(colliderIndex).gameObject;
                    colliders.layer = globals.treeLayer;

                    dmr = colliders.GetComponents<MeshRenderer>();

                    for (var i = dmr.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmr[i]);
                    }

                    dmr = colliders.GetComponentsInChildren<MeshRenderer>();

                    for (var i = dmr.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmr[i]);
                    }

                    dmf = colliders.GetComponents<MeshFilter>();

                    for (var i = dmf.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmf[i]);
                    }

                    dmf = colliders.GetComponentsInChildren<MeshFilter>();

                    for (var i = dmf.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmf[i]);
                    }
                    
                    asset.collisionMeshes.Apply(colliders, TreeGlobalSettings.instance);
                }
                
                if (generateOcclusionMesh && (occlusionIndex <= prefab.transform.childCount))
                {
                    GameObject occlusion = prefab.transform.GetChild(occlusionIndex).gameObject;
                    occlusion.name = "OCCLUSION";
                    occlusion.layer = globals.treeLayer;

                    var dc = occlusion.GetComponents<Collider>();
                    for (var i = 0; i < dc.Length; i++)
                    {
                        Object.DestroyImmediate(dc[i]);
                    }
                    
                    dmr = occlusion.GetComponents<MeshRenderer>();

                    for (var i = dmr.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmr[i]);
                    }

                    dmf = occlusion.GetComponents<MeshFilter>();

                    for (var i = dmf.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmf[i]);
                    }
                    
                    var occlusionChildren = occlusion.transform.childCount;
                    
                    if (occlusionChildren > 0)
                    {
                        for (var i = occlusionChildren - 1; i >= 0; i--)
                        {
                            Object.DestroyImmediate(occlusion.transform.GetChild(i).gameObject);
                        }
                    }
                    
                    
                    var omc = occlusion.GetComponent<MeshCollider>();

                    if (omc == null)
                    {
                        omc = occlusion.AddComponent<MeshCollider>();
                    }

                    omc.sharedMesh = asset.levels[0].mesh;
                    omc.tag = TAGS.OcclusionBake;
                    omc.enabled = false;
                }

                if (pointsIndex <= prefab.transform.childCount)
                {
                    GameObject points = prefab.transform.GetChild(pointsIndex).gameObject;

                    dmr = points.GetComponents<MeshRenderer>();

                    for (var i = dmr.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmr[i]);
                    }

                    dmf = points.GetComponents<MeshFilter>();

                    for (var i = dmf.Length - 1; i >= 0; i--)
                    {
                        Object.DestroyImmediate(dmf[i]);
                    }
                    
                    individual.ApplyStageRuntime(tree, stage, prefab, points);
                }
                
                lodGroup.SetLODs(lods);
                
                if (generateImpostor && (impostorIndex <= (rendererChildren - 1)))
                {
                    GameObject impostor = prefab.transform.GetChild(impostorIndex).gameObject;
                    impostor.layer = globals.treeLayer;

                    ImpostorGenerator.Update(
                        tree,
                        stage,
                        prefab,
                        impostor,
                        renderers.transform.GetChild(rendererChildren - 1).GetComponent<MeshRenderer>()
                    );
                }
            }
        }
        
         private static void UpdatePrefab(
            LogDataContainer log,
            LogInstance instance)
        {
            using (BUILD_TIME.ASSET_MGR.UpdatePrefab.Auto())
            {
                try
                {
                    var path = log.subfolders.GetFilePathByType(
                        TreeAssetSubfolderType.Prefabs,
                        $"{instance.asset.CleanName}.prefab"
                    );

                    if (instance.asset.prefab == null)
                    {
                        using (BUILD_TIME.ASSET_MGR.UpdatePrefabSave.Auto())
                        {
                            var prefab = new GameObject(instance.asset.CleanName);

                            PrefabUtility.SaveAsPrefabAsset(prefab, path);

                            instance.asset.prefab = AssetDatabaseManager.LoadAssetAtPath<GameObject>(path);

                            Object.DestroyImmediate(prefab);
                        }
                    }
                    else
                    {
                        instance.asset.prefab.name = instance.asset.CleanName;

                        var existingPath = AssetDatabaseManager.GetAssetPath(instance.asset.prefab);

                        if (string.IsNullOrWhiteSpace(existingPath))
                        {
                            PrefabUtility.SaveAsPrefabAsset(instance.asset.prefab, path);
                        }
                        else if (existingPath != path)
                        {
                            AssetDatabaseManager.MoveAsset(existingPath, path);
                        }
                    }

                    var requiresUpdate = RequiresUpdate(log, instance);

                    if (requiresUpdate)
                    {
                        using (BUILD_TIME.ASSET_MGR.UpdatePrefabUpdate.Auto())
                        {
                            using (var mutable = instance.asset.ToMutable(_previewScene))
                            {
                                UpdatePrefabLoaded(log, instance, mutable.Mutable);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    PrefabInspector.Inspect(instance.asset.prefab);
                    throw;
                }
            }
        }
         
        private static bool RequiresUpdate(LogDataContainer log, LogInstance instance)
        {
            using (BUILD_TIME.ASSET_MGR.RequiresUpdate.Auto())
            {
                var runtime = instance.asset.prefab.GetComponent<LogRuntimeInstance>();

                var lodCount = instance.asset.levels.Count;
                var generateColliders = log.settings.collision.generateColliders;

                var expectedChildCount = generateColliders ? lodCount + 1 : lodCount;
                    
                    var realChildCount = instance.asset.prefab.transform.childCount;

                    if (expectedChildCount != realChildCount)
                    {
                        
                        Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong child count.");
                        return true;
                    }

                if (runtime == null)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Missing LogRuntimeInstance.");
                    return true;
                }

                if (instance.RequiresUpdate(runtime))
                {                    
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong log runtime parameters.");
                    return true;
                }

                var lodGroup = instance.asset.prefab.GetComponent<LODGroup>();

                if (lodGroup == null)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Missing LODGroup.");
                    return true;
                }
                
                if (!lodGroup.enabled)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: LODGroup disabled.");
                    return true;
                }
                
                if (lodGroup.fadeMode != LODFadeMode.CrossFade)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Incorrect fade settings.");
                    return true;
                }

                var lods = lodGroup.GetLODs();

                if (lods.Length != instance.asset.levels.Count)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong LOD count.");
                    return true;
                }
                
                var rigidbodies = instance.asset.prefab.GetComponents<Rigidbody>();

                if (rigidbodies.Length == 0)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Missing rigidbody.");
                    return true;
                }

                if (rigidbodies.Length > 1)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong rigidbody count.");
                    return true;
                }

                var rigidbody = rigidbodies[0];
                
                if (Math.Abs(rigidbody.mass - runtime.mass) > float.Epsilon) 
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong rigidbody mass.");
                    return true;
                }
                
                if (rigidbody.centerOfMass != runtime.centerOfMass) 
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong rigidbody center of mass.");
                    return true;
                }
                
                if (!rigidbody.useGravity) 
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong gravity setting.");
                    return true;
                }
                
                
                rigidbodies = instance.asset.prefab.GetComponentsInChildren<Rigidbody>();

                if (rigidbodies.Length > 0)
                {
                    Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong child rigidbody count.");
                    return true;
                }

                for (var i = 0; i < realChildCount; i++)
                {
                    var child = instance.asset.prefab.transform.GetChild(i);

                    var expectedName = $"LOD {i}";

                    var colliderChild = generateColliders && (i == (realChildCount - 1)); 

                    if (!colliderChild)
                    {
                        if (child.name != expectedName)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong child name.");
                            return true;
                        }
                    }

                    if (colliderChild)
                    {
                        if (!instance.asset.collisionMeshes.IsCorrectlyApplied(child.gameObject, TreeGlobalSettings.instance))
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Incorrect collider setup.");
                            return true;
                        }
                    }
                    else
                    {          
                        var c = child.gameObject.GetComponent<MeshCollider>();
                        
                        if (c != null)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Extra MeshCollider.");
                            return true;
                        }
                        
                        var mf = child.gameObject.GetComponent<MeshFilter>();

                        if (mf == null)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Missing MeshFilter.");
                            return true;
                        }
                    
                        var mr = child.gameObject.GetComponent<MeshRenderer>();
                        
                        if (mr == null)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Missing MeshRenderer.");
                            return true;
                        }

                        if (lods[i].renderers.Length < 1)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong renderer length.");
                            return true;
                        }

                        if (mf.sharedMesh != instance.asset.levels[i].mesh)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong mesh.");
                            return true;
                        }

                        if (mr.sharedMaterials.Length != instance.asset.levels[i].materials.Length)
                        {
                            Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong material count.");
                            return true;
                        }

                        for (int j = 0; j < instance.asset.levels[i].materials.Length; j++)
                        {
                            if (mr.sharedMaterials[j] != instance.asset.levels[i].materials[j])
                            {
                                Debug.LogWarning($"Prefab [{instance.asset.prefab.name}] update required: Wrong material.");
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
        
        private static void UpdatePrefabLoaded(
            LogDataContainer log,
            LogInstance instance,
            GameObject prefab)
        {
            using (BUILD_TIME.ASSET_MGR.UpdatePrefabLoaded.Auto())
            {
                var runtime = prefab.GetComponent<LogRuntimeInstance>();

                if (runtime == null)
                {
                    runtime = prefab.AddComponent<LogRuntimeInstance>();
                }           
                
                instance.UpdateRuntime(runtime);

                var lodGroup = prefab.GetComponent<LODGroup>();

                if (lodGroup == null)
                {
                    lodGroup = prefab.AddComponent<LODGroup>();
                }

                lodGroup.enabled = true;
                lodGroup.fadeMode = LODFadeMode.CrossFade;
                
                var childCount = prefab.transform.childCount;
                
                var lodCount = instance.asset.levels.Count;
                
                var generateColliders = log.settings.collision.generateColliders;
                
                var expectedChildCount = 
                    generateColliders ? lodCount + 1 : lodCount;
                
                var lods = new LOD[lodCount];

                if (childCount > expectedChildCount)
                {
                    for (var i = childCount - 1; i >= lodCount; i--)
                    {
                        Object.DestroyImmediate(prefab.transform.GetChild(i).gameObject);
                    }
                }
                else if (childCount < expectedChildCount)
                {
                    for (var i = childCount; i < expectedChildCount; i++)
                    {
                        var newChild = new GameObject();
                        newChild.transform.SetParent(prefab.transform, false);
                    }
                }

                childCount = expectedChildCount;
                
                var rigidbodies = prefab.GetComponents<Rigidbody>();
        
                if (rigidbodies.Length > 1)
                {
                    for (var i = rigidbodies.Length - 1; i >= 1; i--)
                    {
                        Object.DestroyImmediate(rigidbodies[i]);
                    }
                }
                else if (rigidbodies.Length < 1)
                {
                    prefab.AddComponent<Rigidbody>();                    
                }

                var rigidbody = prefab.GetComponent<Rigidbody>();
                rigidbody.mass = runtime.mass;
                rigidbody.centerOfMass = runtime.centerOfMass;
                rigidbody.useGravity = true;
                
                rigidbodies = prefab.GetComponentsInChildren<Rigidbody>();

                if (rigidbodies.Length > 0)
                {
                    for (var i = rigidbodies.Length - 1; i >= 1; i--)
                    {
                        Object.DestroyImmediate(rigidbodies[i]);
                    }
                }
               
                for (var i = 0; i < childCount; i++)
                {
                    var child = prefab.transform.GetChild(i);

                    child.name = $"LOD {i}";

                    var colliderChild = generateColliders && (i == (childCount - 1)); 

                    if (colliderChild)
                    {
                        instance.asset.collisionMeshes.Apply(child.gameObject, TreeGlobalSettings.instance);
                    }
                    else
                    {
                        var mc = child.gameObject.GetComponent<MeshCollider>();

                        if (mc != null)
                        {
                            Object.DestroyImmediate(mc);
                        }

                        var setting = log.settings.lod.levelsOfDetail[i];

                        var mf = child.gameObject.GetComponent<MeshFilter>();

                        if (mf == null)
                        {
                            mf = child.gameObject.AddComponent<MeshFilter>();
                        }

                        var mr = child.gameObject.GetComponent<MeshRenderer>();

                        if (mr == null)
                        {
                            mr = child.gameObject.AddComponent<MeshRenderer>();
                        }

                        lods[i].renderers = new Renderer[] {mr};
                        lods[i].fadeTransitionWidth = setting.fadeTransitionWidth;
                        lods[i].screenRelativeTransitionHeight = setting.screenRelativeTransitionHeight;                        

                        mf.sharedMesh = instance.asset.levels[i].mesh;
                        mr.sharedMaterials = instance.asset.levels[i].materials;
                    }                    
                }

                lodGroup.SetLODs(lods);
            }
        }
    }
}
