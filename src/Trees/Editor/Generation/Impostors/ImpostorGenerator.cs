using System.Linq;
using AmplifyImpostors;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Transmission;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Impostors
{
    public static class ImpostorGenerator
    {
        public static void RefreshImpostorAsset(TreeDataContainer tree, TreeAsset asset, GameObject prefab)
        {
            if (!tree.settings.lod.impostor.impostorAfterLastLevel)
            {
                asset.impostor = null;
                return;
            }

            if (asset.impostor == null)
            {

                var path = tree.subfolders.GetFilePathByType(
                    TreeAssetSubfolderType.Impostors,
                    $"{prefab.name}_impostor.asset"
                );

                var loaded = AssetDatabaseManager.LoadAssetAtPath<AmplifyImpostorAsset>(path);

                if (loaded != null)
                {
                    asset.impostor = loaded;
                }
                else
                {
                    var instance = ScriptableObject.CreateInstance<AmplifyImpostorAsset>();

                    AssetDatabaseManager.CreateAsset(instance, path);

                    asset.impostor = AssetDatabaseManager.LoadAssetAtPath<AmplifyImpostorAsset>(path);
                }
            }

            asset.impostor.Preset = DefaultShaderResource.instance.impostorPreset;
            asset.impostor.SelectedSize = (int) tree.settings.texture.atlasTextureSize;
            asset.impostor.TexSize = new Vector2(asset.impostor.SelectedSize, asset.impostor.SelectedSize);

        }

        public static bool RequiresUpdate(TreeStage stage)
        {
            var prefab = stage.asset.prefab;

            var impostor = prefab.GetComponent<AmplifyImpostor>();

            if (impostor == null)
            {
                Debug.LogWarning("Prefab update required: Missing impostor.");
                return true;
            }

            if (stage.asset.impostor == null)
            {
                Debug.LogWarning("Prefab update required: Missing impostor asset.");
                return true;
            }

            if (impostor.Data == null)
            {
                Debug.LogWarning("Prefab update required: Impostor asset not assigned.");
                return true;
            }

            if ((impostor.Renderers == null) || (impostor.Renderers.Length != 1))
            {
                Debug.LogWarning("Prefab update required: Impostor renderers not set up.");
                return true;
            }

            if (impostor.LodGroup == null)
            {
                Debug.LogWarning("Prefab update required: Impostor LOD group not assigned.");
                return true;
            }

            if (impostor.RootTransform == null)
            {
                Debug.LogWarning("Prefab update required: Impostor root transform not assigned.");
                return true;
            }

            var lods = impostor.LodGroup.GetLODs();

            var impostorLOD = lods[lods.Length - 1];

            if (impostorLOD.renderers.Length != 1)
            {
                Debug.LogWarning("Prefab update required: Incorrect impostor renderer count.");
                return true;
            }

            if (impostorLOD.renderers[0].sharedMaterial != impostor.Data.Material)
            {
                Debug.LogWarning("Prefab update required: Incorrect impostor material.");
                return true;
            }

            var mf = impostorLOD.renderers[0].GetComponent<MeshFilter>();
            if (mf.sharedMesh != impostor.Data.Mesh)
            {
                Debug.LogWarning("Prefab update required: Incorrect impostor mesh.");
                return true;
            }

            return false;
        }

        public static void Update(
            TreeDataContainer tree,
            TreeStage stage,
            GameObject prefab,
            GameObject level,
            MeshRenderer reference)
        {
            var impostorSettings = tree.settings.lod.impostor;

            var impostor = prefab.GetComponent<AmplifyImpostor>();

            if (impostor == null)
            {
                impostor = prefab.AddComponent<AmplifyImpostor>();
            }

            if (stage.asset.impostor == null)
            {
                RefreshImpostorAsset(tree, stage.asset, prefab);
            }

            if (stage.asset.impostor.name != (prefab.name + "_impostor"))
            {
                stage.asset.impostor = null;
            }

            impostor.Data = stage.asset.impostor;

            impostor.Renderers = new Renderer[] {reference};

            impostor.LodGroup = prefab.GetComponent<LODGroup>();

            level.name = "IMPOSTOR";

            var colliders = level.GetComponents<Collider>();

            for (var i = colliders.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(colliders[i]);
            }

            impostor.RootTransform = prefab.transform;
            impostor.m_lastImpostor = level;
            impostor.m_lodReplacement = LODReplacement.ReplaceLast;

            impostor.Data.NormalScale = impostorSettings.normalScale;
            impostor.Data.Tolerance = impostorSettings.outlineTolerance;
            impostor.Data.PixelPadding = impostorSettings.impostorAxisPadding;
            impostor.Data.HorizontalFrames = impostorSettings.impostorAxisFrames;
            impostor.Data.VerticalFrames = impostorSettings.impostorAxisFrames;

            var mf = level.GetComponent<MeshFilter>();
            var mr = level.GetComponent<MeshRenderer>();

            if (mf == null)
            {
                level.AddComponent<MeshFilter>();
            }

            if (mr == null)
            {
                level.AddComponent<MeshRenderer>();
            }

            impostor.RenderAllDeferredGroups(impostor.Data, true, stage.lods[0].vertices.Max(v => v.wind.primaryRoll));

            var lods = impostor.LodGroup.GetLODs();
            lods[lods.Length - 1].renderers = new[] {mr};

            impostor.LodGroup.SetLODs(lods);

            UpdateImpostorMaterialProperties(impostor.Data, impostorSettings, tree.materials.transmission);
        }


        public static void UpdateImpostorMaterialProperties(
            AmplifyImpostorAsset data,
            ImpostorSettings impostorSettings,
            MaterialTransmissionValues transmission)
        {
            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_Clip,
                impostorSettings.impostorClip
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_UseHueVariation,
                impostorSettings.hueAdjustment
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_HueAdjustment,
                impostorSettings.hueAdjustment
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_SaturationAdjustment,
                impostorSettings.saturationAdjustment
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_BrightnessAdjustment,
                impostorSettings.brightnessAdjustment
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_ContrastAdjustment,
                impostorSettings.contrastAdjustment
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_PrimaryRoll,
                impostorSettings.primaryRollStrength
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_PrimaryBend,
                impostorSettings.primaryBendStrength
            );

            data.Material.SetColorIfNecessary(
                TreeMaterialProperties.Impostor_FadeVariation,
                impostorSettings.fadeVariation
            );

            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_UseHueVariation,
                impostorSettings.useHueVariation ? 1.0f : 0.0f
            );


            if (impostorSettings.hueVariation == Color.clear)
            {
                impostorSettings.hueVariation = transmission.automaticTransmissionColor;
            }

            data.Material.SetColorIfNecessary(
                TreeMaterialProperties.HueVariation,
                impostorSettings.hueVariation
            );

            data.Material.SetColorIfNecessary(
                TreeMaterialProperties.Impostor_HueVariationStrength,
                new Color(0, 0, 0, impostorSettings.hueVariationStrength)
            );
            
            
            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_CorrectContrast,
                impostorSettings.correctContrast ? 1.0f : 0.0f
            );
            
            data.Material.SetFloatIfNecessary(
                TreeMaterialProperties.Impostor_ContrastCorrection,
                impostorSettings.contrastCorrection
            );

        }
    }
}

/*
private enum AssetMode
{
    Create,
    Unassign,
    Delete,
    UnassignAndCreate,
    DeleteAndCreate
}


static void Impostors_Assets(bool all, AssetMode mode) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var absoluteFolderPath = instance.GetFolderPathForImpostor();
        var folderPath = FileUtil.GetProjectRelativePath(absoluteFolderPath);
        
        switch (mode)
        {
            case AssetMode.Create:
                if (instance.Data == null)
                {
                    instance.CreateAssetFile(folderPath);
                }
                break;
            case AssetMode.Delete:
                if (instance.Data != null)
                {
                    var existing = AssetDatabaseManager.GetAssetPath(instance.Data);
                    if (!string.IsNullOrWhiteSpace(existing))
                    {
                        AssetDatabaseManager.DeleteAsset(existing);
                        instance.Data = null;
                    }
                }
                break;
            case AssetMode.Unassign:
                instance.Data = null;
                break;
            case AssetMode.UnassignAndCreate:
                instance.Data = null;
                instance.CreateAssetFile(folderPath);
                break;
            case AssetMode.DeleteAndCreate:
                if (instance.Data != null)
                {
                    var existing = AssetDatabaseManager.GetAssetPath(instance.Data);
                    if (!string.IsNullOrWhiteSpace(existing))
                    {
                        AssetDatabaseManager.DeleteAsset(existing);
                        instance.Data = null;
                    }
                }
                instance.CreateAssetFile(folderPath);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}

static void Impostors_Renderers_UniformScale(bool all) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var scale = instance.transform.localScale;

        if (!scale.IsUniform())
        {
            scale = new Vector3(scale.x, scale.x, scale.x);
            
            instance.transform.localScale = scale;
        }
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}


static void Impostors_Settings_ImpostorType(bool all, ImpostorType type) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var data = instance.Data;

        if (data != null)
        {
            instance.Data.ImpostorType = type;
        }
        
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}


static void Impostors_Settings_TextureSize(bool all, int textureSize) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var data = instance.Data;

        if (data != null)
        {
            instance.Data.LockedSizes = true;
            instance.Data.TexSize = new Vector2(textureSize, textureSize);
            instance.Data.SelectedSize = textureSize;
        }
        
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}



static void Impostors_Settings_AxisFrames(bool all, int frames) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var data = instance.Data;

        if (data != null)
        {
            instance.Data.DecoupleAxisFrames = false;
            instance.Data.HorizontalFrames = frames;
            instance.Data.VerticalFrames = frames;
        }
        
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}



static void Impostors_Settings_PixelPadding(bool all, int padding) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var data = instance.Data;

        if (data != null)
        {
            instance.Data.PixelPadding = padding;
        }
        
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}

 
[MenuItem(     "Tools/Impostors/All/Settings/Bake Preset/Individual Plant")]       static void Impostors_All_Settings_BakePreset_IndividualPlant() { Impostors_Settings_BakePreset( true, "internal_plant-individual_billboard-bake"); }
[MenuItem("Tools/Impostors/Selected/Settings/Bake Preset/Individual Plant")] static void  Impostors_Selected_Settings_BakePreset_IndividualPlant() { Impostors_Settings_BakePreset(false, "internal_plant-individual_billboard-bake"); }
[MenuItem(     "Tools/Impostors/All/Settings/Bake Preset/Individual Plant Emissive")]       static void Impostors_All_BakePreset_IndividualPlantEmissive() { Impostors_Settings_BakePreset( true, "internal_plant-individual-emissive_billboard-bake"); }
[MenuItem("Tools/Impostors/Selected/Settings/Bake Preset/Individual Plant Emissive")] static void  Impostors_Selected_BakePreset_IndividualPlantEmissive() { Impostors_Settings_BakePreset(false,  "internal_plant-individual-emissive_billboard-bake"); }
[MenuItem(     "Tools/Impostors/All/Settings/Bake Preset/Standard Deferred")]       static void Impostors_All_Settings_BakePreset_StandardShader() { Impostors_Settings_BakePreset( true, "StandardDeferred"); }
[MenuItem("Tools/Impostors/Selected/Settings/Bake Preset/Standard Deferred")] static void  Impostors_Selected_Settings_BakePreset_StandardShader() { Impostors_Settings_BakePreset(false, "StandardDeferred"); }

static void Impostors_Settings_BakePreset(bool all, string presetName) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {
        var data = instance.Data;

        if (data != null)
        {
            instance.Data.Preset = AssetDatabaseManager.FindAssets($"t:AmplifyImpostorBakePreset {presetName}")
                .FilterCast2<AmplifyImpostorBakePreset>().FirstOrDefault();
        }
        
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}


[MenuItem(     "Tools/Impostors/All/Bake Impostors")]       static void Impostors_All_Bake() { Impostors_Bake( true); }
[MenuItem("Tools/Impostors/Selected/Bake Impostors")] static void  Impostors_Selected_Bake() { Impostors_Bake(false); }

static void Impostors_Bake(bool all) 
{
    var action = new Action<AmplifyImpostor>(instance =>
    {

        var meshWind = instance.GetComponent<MeshWindComponent>();

        var windStrength = 0f;
        if (meshWind != null)
        {
            windStrength = meshWind.componentData.windStrengthModifier;
        }

        instance.RenderAllDeferredGroups(windStrength: windStrength);
    });
    if (all) { ApplyToAllImpostors(action); } else { ApplyToSelectedImpostors(action); }
}










static void ApplyToSelectedImpostors(Action<AmplifyImpostor> action)
{
    var impostors = Selection.GetFiltered<AmplifyImpostor>(SelectionMode.OnlyUserModifiable);
    ApplyToImpostors(action, impostors);
}

static void ApplyToAllImpostors(Action<AmplifyImpostor> action)
{
    var impostors = GameObject.FindObjectsOfType<AmplifyImpostor>();
    ApplyToImpostors(action, impostors);
}

static void ApplyToImpostors(Action<AmplifyImpostor> action, AmplifyImpostor[] impostors)
{
    if (!ToolMenuManager.ToolsEnabled) return;
    
    var count = 0;
    var cancel = false;
    
    foreach (var impostor in impostors)
    {
        if (!ToolMenuManager.ToolsEnabled) break;
        
        try
        {
            count += 1;
            cancel = cancel | EditorUtility.DisplayCancelableProgressBar($"Applying to impostors...",
                $"{count} of {impostors.Length}: {(impostor.Data == null ? impostor.name : impostor.Data.name)}.",
                ((float) count) / ((float) impostors.Length));

            if (cancel)
            {
                break;
            }
            
            impostor.EnsureInitialized();
            action(impostor);

        }
        catch (Exception ex)
        {
            DebugHelper.LogError($"Failed to apply operation to impostor: {ex.Message}", ex, impostor);
            EditorUtility.ClearProgressBar();
        }
    }
    
    EditorUtility.ClearProgressBar();
}
*/

