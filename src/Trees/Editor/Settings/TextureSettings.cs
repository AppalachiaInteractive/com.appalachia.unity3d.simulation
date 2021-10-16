using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Texture Generation", TitleAlignment = TitleAlignments.Centered)]
    public class TextureSettings : ResponsiveSettings
    {
        [FormerlySerializedAs("textureSize")]
        [PropertyTooltip("What resolution should the produced textures be drawn at?")]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public TextureSize atlasTextureSize = TextureSize.k2048;

        [PropertyTooltip("Should tiled materials keep their original size?")]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public bool tiledMaterialsKeepOriginalSize = true;
        
        [HideIf(nameof(tiledMaterialsKeepOriginalSize))]
        [PropertyTooltip("What resolution should the tiled textures be drawn at?")]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public TextureSize tiledTextureSize = TextureSize.k2048;
        
        [HideIf(nameof(tiledMaterialsKeepOriginalSize))]
        [PropertyTooltip("What resolution should the tiled over textures be drawn at?")]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public TextureSize tiledCoverTextureSize = TextureSize.k2048;

        [PropertyTooltip("Save additional materials to inspect material combinations that aren't working?")]
        public bool debugCombinationOutputs;

        public Vector2 textureSizeV2 => new Vector2((int)atlasTextureSize, (int)atlasTextureSize);
        private bool hideTreeSettings => settingsType != ResponsiveSettingsType.Tree;

        public TextureSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
        
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is TextureSettings cast)
            {
                cast.atlasTextureSize = atlasTextureSize;
                cast.debugCombinationOutputs = debugCombinationOutputs;
                cast.tiledMaterialsKeepOriginalSize = tiledMaterialsKeepOriginalSize;
            }
        }
        
        [Button]
        public void PushToAll()
        {
            var trees = AssetDatabaseManager.FindAssets("t:TreeDataContainer");

            for (var i = 0; i < trees.Length; i++)
            {
                var treeGuid = trees[i];

                var treePath = AssetDatabaseManager.GUIDToAssetPath(treeGuid);

                var tree = AssetDatabaseManager.LoadAssetAtPath<TreeDataContainer>(treePath);

                var settings = tree.settings.texture;

                if (settings == this)
                {
                    continue;
                }

                CopySettingsTo(settings);

                EditorUtility.SetDirty(tree);
                EditorUtility.SetDirty(tree.settings);

                tree.Save();
            }
        }
    }
}
