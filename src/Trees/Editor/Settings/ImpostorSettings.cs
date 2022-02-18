using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    public class ImpostorSettings : ResponsiveSettings
    {
        public ImpostorSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [BoxGroup("A/Material")]
        [PropertyTooltip("Should the brightness be darkened to account for contrast change?")]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public bool correctContrast = true;

        [PropertyTooltip("Should an impostor be created after the last LOD?")]
        public bool impostorAfterLastLevel = true;

        [BoxGroup("A/Material")]
        [PropertyTooltip("Should the hue of the trees be varied?")]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public bool useHueVariation = true;

        [BoxGroup("A/Material")]
        [PropertyTooltip("What color should the impostor fade to near the top?")]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public Color fadeVariation = Color.white;

        [BoxGroup("A/Material")]
        [PropertyTooltip("The hue variation color.")]
        [EnableIf(nameof(useHueVariation))]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public Color hueVariation = Color.white;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much should the brightness be adjusted?")]
        [PropertyRange(-1.0f, 1.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float brightnessAdjustment;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much should the contrast be adjusted?")]
        [PropertyRange(0.0f, 10.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float contrastAdjustment = 1.0f;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much should the brightness be darkened to account for contrast change?")]
        [PropertyRange(0.0f, 10.0f)]
        [EnableIf(nameof(correctContrast))]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float contrastCorrection = 1.0f;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much should the hue be adjusted?")]
        [PropertyRange(-1.0f, 1.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float hueAdjustment;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much should the hue vary?")]
        [PropertyRange(0.0f, 1.0f)]
        [EnableIf(nameof(useHueVariation))]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float hueVariationStrength = 1.0f;

        [BoxGroup("A/Material")]
        [PropertyTooltip("What should the opacity cutoff threshold be?")]
        [PropertyRange(0.0f, 1.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float impostorClip = .1f;

        [BoxGroup("A/Mesh")]
        [PropertyTooltip("Pushes the outline outward.")]
        [PropertyRange(0.0f, 1.0f)]
        public float normalScale = .1f;

        [BoxGroup("A/Mesh")]
        [PropertyTooltip("The outline tolerance.")]
        [PropertyRange(0.0f, 1.0f)]
        public float outlineTolerance = .25f;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much the impostor should bend in the wind?")]
        [PropertyRange(0.0f, 2.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float primaryBendStrength = 1.0f;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much the impostor should roll in the wind?")]
        [PropertyRange(0.0f, 4.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float primaryRollStrength = 1.0f;

        [BoxGroup("A/Material")]
        [PropertyTooltip("How much should the saturation be adjusted?")]
        [PropertyRange(-1.0f, 1.0f)]

        //[OnValueChanged(nameof(ImpostorSettingsChanged))]
        public float saturationAdjustment;

        [ShowIfGroup("A", Condition = nameof(_showImpostors))]
        [BoxGroup("A/Mesh")]
        [PropertyTooltip("How many snapshots per axis.")]
        [PropertyRange(8, 24)]
        public int impostorAxisFrames = 16;

        [BoxGroup("A/Mesh")]
        [PropertyTooltip("How much padding should the impostor have?")]
        [PropertyRange(8, 24)]
        public int impostorAxisPadding = 8;

        #endregion

        private bool _showImpostors => impostorAfterLastLevel;

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is ImpostorSettings cast)
            {
                cast.brightnessAdjustment = brightnessAdjustment;
                cast.contrastAdjustment = contrastAdjustment;
                cast.contrastCorrection = contrastCorrection;
                cast.correctContrast = correctContrast;
                cast.fadeVariation = fadeVariation;
                cast.hueAdjustment = hueAdjustment;
                cast.hueVariation = hueVariation;
                cast.impostorClip = impostorClip;
                cast.normalScale = normalScale;
                cast.outlineTolerance = outlineTolerance;
                cast.saturationAdjustment = saturationAdjustment;
                cast.hueVariationStrength = hueVariationStrength;
                cast.impostorAxisFrames = impostorAxisFrames;
                cast.impostorAxisPadding = impostorAxisPadding;
                cast.primaryBendStrength = primaryBendStrength;
                cast.primaryRollStrength = primaryRollStrength;
                cast.useHueVariation = useHueVariation;
                cast.impostorAfterLastLevel = impostorAfterLastLevel;
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

                var settings = tree.settings.lod.impostor;

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
