using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Vertex", TitleAlignment = TitleAlignments.Centered)]
    public class VertexSettings : ResponsiveSettings
    {
        [PropertyTooltip(
            "Whether or not log data should be baked into the vertex colors. (R - bark, G - wood, B - noise 1, A - noise 2")]
        [ToggleLeft]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public bool generate = true;
        
        [ShowIfGroup("Log", Condition = nameof(generate))]

        [BoxGroup("Log/Log Settings", CenterLabel = true)]
        [PropertyTooltip("Adjusts how big the first noise scale is.")]
        [PropertyRange(0.1f, 100f)] [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float noise1Scale = .1f;

        [BoxGroup("Log/Log Settings", CenterLabel = true)]
        [PropertyTooltip("Adjusts how far the first noise offset is.")]
        [PropertyRange(0f, 10f)] [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float noise1Offset;

        [BoxGroup("Log/Log Settings", CenterLabel = true)]
        [PropertyTooltip("Adjusts the first noise contrast.")]
        [PropertyRange(0f, 1f)] [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float noise1Contrast = 0.5f;
        
        [BoxGroup("Log/Log Settings", CenterLabel = true)]
        [PropertyTooltip("Adjusts how big the second noise scale is.")]
        [PropertyRange(0.1f, 100f)] [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float noise2Scale = .5f;
        
        [BoxGroup("Log/Log Settings", CenterLabel = true)]
        [PropertyTooltip("Adjusts how far the second noise offset is.")]
        [PropertyRange(0f, 10f)] [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float noise2Offset;
        
        [BoxGroup("Log/Log Settings", CenterLabel = true)]
        [PropertyTooltip("Adjusts the second noise contrast.")]
        [PropertyRange(0f, 1f)] [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float noise2Contrast = 0.5f;
      
        public VertexSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
        
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is VertexSettings cast)
            {
                cast.generate = generate;
                cast.noise1Contrast = noise1Contrast;
                cast.noise1Offset = noise1Offset;
                cast.noise1Scale = noise1Scale;
                cast.noise2Contrast = noise2Contrast;
                cast.noise2Offset = noise2Offset;
                cast.noise2Scale = noise2Scale;
            }
        }
        
        [Button]
        public void PushToAll()
        {
            var trees = AssetDatabaseManager.FindAssets("t:LogDataContainer");

            for (var i = 0; i < trees.Length; i++)
            {
                var treeGuid = trees[i];

                var treePath = AssetDatabaseManager.GUIDToAssetPath(treeGuid);

                var tree = AssetDatabaseManager.LoadAssetAtPath<LogDataContainer>(treePath);

                var settings = tree.settings.vertex;

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
