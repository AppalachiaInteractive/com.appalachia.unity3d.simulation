using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.AmbientOcclusion;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Ambient Occlusion", TitleAlignment = TitleAlignments.Centered)]
    public class AmbientOcclusionSettings : ResponsiveSettings
    {
        public AmbientOcclusionSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [PropertyTooltip("The type of ambient occlusion to generate.")]
        [ShowIfGroup("AO", Condition = nameof(showTreeSettings))]
        [OnValueChanged(nameof(AmbientOcclusionSettingsChanged))]
        public AmbientOcclusionStyle style;

        [PropertyTooltip("Toggles ambient occlusion generation on or off.")]
        [ToggleLeft]
        [OnValueChanged(nameof(AmbientOcclusionSettingsChanged))]
        public bool generateAmbientOcclusion = true;

        [PropertyTooltip("Toggles generation of an AO mesh on or off.")]
        [ToggleLeft]
        [OnValueChanged(nameof(AmbientOcclusionSettingsChanged))]
        public bool generateBakeMesh = true;

        [PropertyTooltip("The density of generated ambient occlusion.  Higher is darker.")]
        [ShowIf(nameof(generateAmbientOcclusion))]
        [PropertyRange(0f, 2f)]
        [OnValueChanged(nameof(AmbientOcclusionSettingsChanged))]
        public float density = 1f;

        [PropertyTooltip("The distance to fire rays from when sampling ambient occlusion.")]
        [ShowIfGroup("AO", Condition = nameof(showTreeSettings))]
        [ShowIf(nameof(showRaytracing))]
        [PropertyRange(1f, 20f)]
        [OnValueChanged(nameof(AmbientOcclusionSettingsChanged))]
        public float raytracingRange = 10;

        [PropertyTooltip("The number of samples to take when sampling ambient occlusion.  Higher is slower.")]
        [ShowIfGroup("AO", Condition = nameof(showTreeSettings))]
        [ShowIf(nameof(showRaytracing))]
        [PropertyRange(4f, 64f)]
        [OnValueChanged(nameof(AmbientOcclusionSettingsChanged))]
        public float raytracingSamples = 16;

        #endregion

        private bool showRaytracing => showTreeSettings && (style == AmbientOcclusionStyle.Raytraced);

        private bool showTreeSettings => (settingsType == ResponsiveSettingsType.Tree) && generateAmbientOcclusion;

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is AmbientOcclusionSettings ao)
            {
                ao.density = density;
                ao.style = style;
                ao.raytracingRange = raytracingRange;
                ao.raytracingSamples = raytracingSamples;
                ao.generateAmbientOcclusion = generateAmbientOcclusion;
                ao.generateBakeMesh = generateBakeMesh;
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

                var settings = tree.settings.ao;

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
