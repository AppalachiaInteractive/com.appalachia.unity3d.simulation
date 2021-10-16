using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Transmission", TitleAlignment = TitleAlignments.Centered)]
    public class TransmissionSettings : ResponsiveSettings
    {
        [PropertyTooltip(
            "Should the leaf material transmission property color be calculated automatically?")]
        [ToggleLeft]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public bool setTransmissionColorsAutomatically = true;
        
        [PropertyTooltip("The brightness of the leaf transmission color.")]
        [EnableIf(nameof(setTransmissionColorsAutomatically))]
        [HorizontalGroup("A", .5f)]
        [PropertyRange(0.1f, .95f)]
        [InfoBox("Automatic Transmission Brightness", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float automaticTransmissionColorBrightness = .8f;


        [PropertyTooltip("The leaf transmission color.")]
        [DisableIf(nameof(setTransmissionColorsAutomatically))]
        [HorizontalGroup("A", .5f)]
        [InfoBox("Transmission Color", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Color transmissionColor = new Color(.7f, .8f, .6f, 1f);

        public TransmissionSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
        
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is TransmissionSettings cast)
            {
                cast.transmissionColor = transmissionColor;
                cast.automaticTransmissionColorBrightness = automaticTransmissionColorBrightness;
                cast.setTransmissionColorsAutomatically = setTransmissionColorsAutomatically;
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

                var settings = tree.settings.transmission;

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