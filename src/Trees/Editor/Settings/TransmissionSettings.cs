using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Transmission", TitleAlignment = TitleAlignments.Centered)]
    public class TransmissionSettings : ResponsiveSettings
    {
        public TransmissionSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [PropertyTooltip("Should the leaf material transmission property color be calculated automatically?")]
        [ToggleLeft]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public bool setTransmissionColorsAutomatically = true;

        [PropertyTooltip("The leaf transmission color.")]
        [DisableIf(nameof(setTransmissionColorsAutomatically))]
        [HorizontalGroup("A", .5f)]
        [InfoBox("Transmission Color", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Color transmissionColor = new Color(.7f, .8f, .6f, 1f);

        [PropertyTooltip("The brightness of the leaf transmission color.")]
        [EnableIf(nameof(setTransmissionColorsAutomatically))]
        [HorizontalGroup("A", .5f)]
        [PropertyRange(0.1f, .95f)]
        [InfoBox("Automatic Transmission Brightness", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float automaticTransmissionColorBrightness = .8f;

        #endregion

        /// <inheritdoc />
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

                tree.MarkAsModified();
                tree.settings.MarkAsModified();

                tree.Save();
            }
        }
    }
}
