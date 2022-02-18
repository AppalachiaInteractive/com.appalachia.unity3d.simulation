using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class LimbSettings : AgeOverrideResponsiveSettings, ICloneable<LimbSettings>
    {
        #region Constants and Static Readonly

        private static readonly string[] SearchStrings = { "break", "wood", "ring" };

        #endregion

        public LimbSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [PropertyTooltip("Defines whether or not the branch is really a log.")]
        [TreeProperty]
        [ShowIf(nameof(showLogTop))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public bool log;

        [HideInInspector] public boolTree disableWelding = TreeProperty.New(false);

        [PropertyRange(0.1f, 5f)]
        [TreeProperty, LabelText("Beaver Factor"), ShowIf(nameof(showBreakShape))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree breakDepth = TreeProperty.New(1.5f);

        [PropertyTooltip(
            "Chance of a branch breaking, i.e. 0 = no branches are broken, 0.5 = half of the branches are broken, 1.0 = all the branches are broken."
        )]
        [PropertyRange(0f, 1f)]
        [TreeProperty, HideIf(nameof(showLog))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree breakingChance = TreeProperty.New(0.0f);

        [PropertyTooltip("Adds random noise of the broken limb.")]
        [PropertyRange(0f, 4f)]
        [TreeProperty, LabelText("Break Noise"), ShowIf(nameof(showBreakShape))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree breakRandomness = TreeProperty.New(0.0f);

        [PropertyTooltip("Scales the random noise for the broken limb.")]
        [PropertyRange(0.1f, 10f)]
        [TreeProperty, LabelText("Break Noise Scale"), ShowIf(nameof(showBreakShape))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree breakRandomnessScale = TreeProperty.New(3.0f);

        [PropertyTooltip("Controls the shape of the broken limb.")]
        [PropertyRange(0f, 2f)]
        [TreeProperty, LabelText("Break Size"), ShowIf(nameof(showBreakShape))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree breakSphericalCap = TreeProperty.New(1f);

        /*[FormerlySerializedAs("materialBreak")]
        [PropertyTooltip("Material for capping broken branches.")]
        [HideIf(nameof(showLog))]
        [TreePropertySimple]
        [OnValueChanged(nameof(MaterialGenerationChanged), true)]
        [DelayedProperty]
        [ValueDropdown(nameof(GetBreakMaterials))]
        public Material breakMaterial;*/

        [PropertyTooltip("Defines the roundness of the tip of the spline.")]
        [PropertyRange(0f, 1f), TreeProperty, HideIf(nameof(showLog))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree capSmoothing = TreeProperty.New(0.0f);

        [PropertyTooltip("The scale of the limb break texture.")]
        [PropertyRange(0.5f, 1.5f)]
        [TreeProperty, LabelText("Scale"), ShowIf(nameof(showBreakShape))]
        [OnValueChanged(nameof(UVSettingsChanged), true)]
        public floatTree limbBreakTextureScale = TreeProperty.New(1.0f);

        [HideInInspector] public floatTree weldNormalBlend = TreeProperty.New(.25f);

        [HideInInspector] public floatTree weldNormalBlendSize = TreeProperty.New(4f);

        [HideInInspector] public floatTree weldSize = TreeProperty.New(4f);

        [PropertyTooltip(
            "This range defines where the branches will be broken. Relative to the length of the branch."
        )]
        [MinMaxSlider(0.05f, 0.95f, ShowFields = false)]
        [TreeProperty, ShowIf(nameof(showBreakPosition))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree breakingSpot = TreeProperty.v2(0.4f, 0.6f);

        #endregion

        private bool showBreakPosition => !showLog && (breakingChance > 0);

        private bool showBreakShape => showLog || (breakingChance > 0);

        private bool showLog => log;
        private bool showLogTop => settingsType == ResponsiveSettingsType.Log;

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is LimbSettings cast)
            {
                cast.capSmoothing = capSmoothing.Clone();
                cast.breakingChance = breakingChance.Clone();

                //cast.breakMaterial = breakMaterial;
                cast.breakingSpot = breakingSpot.Clone();
                cast.breakSphericalCap = breakSphericalCap.Clone();
                cast.breakRandomness = breakRandomness.Clone();
                cast.breakRandomnessScale = breakRandomnessScale.Clone();
                cast.limbBreakTextureScale = limbBreakTextureScale.Clone();
            }
        }

        private List<Material> GetBreakMaterials()
        {
            var assets = AssetDatabaseManager.FindAssets("t: Material");
            var results = new List<Material>();

            foreach (var assetGuid in assets)
            {
                var material =
                    AssetDatabaseManager.LoadAssetAtPath<Material>(
                        AssetDatabaseManager.GUIDToAssetPath(assetGuid)
                    );

                foreach (var searchString in SearchStrings)
                {
                    var s = searchString.ToLowerInvariant();

                    if (material.name.ToLowerInvariant().Contains(s))
                    {
                        results.Add(material);
                        break;
                    }

                    if (material.shader.name.ToLowerInvariant().Contains(s))
                    {
                        results.Add(material);
                        break;
                    }
                }
            }

            return results;
        }

        #region ICloneable<LimbSettings> Members

        public LimbSettings Clone()
        {
            var clone = new LimbSettings(settingsType);
            clone.capSmoothing = capSmoothing.Clone();
            clone.breakingChance = breakingChance.Clone();

            //clone.breakMaterial = breakMaterial;
            clone.breakingSpot = breakingSpot.Clone();
            clone.breakSphericalCap = breakSphericalCap.Clone();
            clone.breakRandomness = breakRandomness.Clone();
            clone.breakRandomnessScale = breakRandomnessScale.Clone();
            clone.limbBreakTextureScale = limbBreakTextureScale.Clone();
            return clone;
        }

        #endregion
    }
}
