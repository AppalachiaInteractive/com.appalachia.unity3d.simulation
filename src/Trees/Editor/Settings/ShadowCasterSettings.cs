using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Shadow", TitleAlignment = TitleAlignments.Centered)]
    public class ShadowCasterSettings : ResponsiveSettings
    {
        public ShadowCasterSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            quality = new LevelOfDetailSettings(100, settingsType)
            {
                leafFullness = .5f,
                showFruit = true,
                showFungus = true,
                showKnots = true,
                branchesGeometryQuality = 0,
                rootsGeometryQuality = 0,
                leafGeometryQuality = .5f,
                trunkGeometryQuality = 0,
                resampleLeaves = false,
                resampleOnTrunkOnly = false,
                resampleSplineAtChildren = false,
                resampleAddOns = false
            };

            properties = new MeshSettings(settingsType)
            {
                recalculateNormals = false, recalculateTangents = false
            };
        }

        #region Fields and Autoproperties

        [PropertyTooltip("Should shadow caster geometry be double sided?")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true), InlineProperty, HideLabel]
        public bool doubleSided = true;

        [PropertyTooltip("Adjust shadow caster mesh quality.")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true), InlineProperty, HideLabel]
        public LevelOfDetailSettings quality;

        [PropertyTooltip("Adjust shadow caster mesh properties.")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true), InlineProperty, HideLabel]
        public MeshSettings properties;

        #endregion

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is ShadowCasterSettings cast)
            {
                properties.CopySettingsTo(cast.properties);
                quality.CopySettingsTo(cast.quality);
                cast.doubleSided = doubleSided;
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

                var settings = tree.settings.shadow;

                if (settings == this)
                {
                    continue;
                }

                CopySettingsTo(settings);

                tree.MarkAsModified();
                tree.settings.MarkAsModified();

                tree.Save();
            }
        }
    }
}
