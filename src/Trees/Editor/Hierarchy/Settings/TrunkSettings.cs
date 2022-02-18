using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class TrunkSettings : AgeOverrideResponsiveSettings, ICloneable<TrunkSettings>
    {
        public TrunkSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [PropertyTooltip("The density of rings of the flare, relative to the rest of the trunk.")]
        [PropertyRange(1f, 15f), TreeProperty, ShowIf(nameof(showTrunkSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flareDensity = TreeProperty.New(2.0f);

        [PropertyTooltip("Pushes the flare noise inward.")]
        [PropertyRange(0.0f, 1.0f), TreeProperty, ShowIf(nameof(showTrunkSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flareDepth = TreeProperty.New(0.0f);

        [PropertyTooltip("Defines how far up the trunk the flares end.")]
        [PropertyRange(0f, 20f), TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flareHeight = TreeProperty.New(1.0f);

        [PropertyTooltip(
            "Defines the noise of the flares, lower values will give a more wobbly look, while higher values gives a more stochastic look."
        )]
        [PropertyRange(0f, 3f), TreeProperty, ShowIf(nameof(showTrunkSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flareNoise = TreeProperty.New(0.3f);

        [PropertyTooltip("Defines how sharply the trunk flares fades.")]
        [PropertyRange(0.5f, 10f), TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flarePower = TreeProperty.New(1.5f);

        [PropertyTooltip("The radius of the flare, relative to the non-flared trunk.")]
        [PropertyRange(0f, 10f), TreeProperty, ShowIf(nameof(showTrunkSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flareRadius = TreeProperty.New(0.1f);

        [PropertyTooltip("The density of vertices of the flare, relative to the rest of the trunk.")]
        [PropertyRange(1f, 15f), TreeProperty, ShowIf(nameof(showTrunkSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree flareResolution = TreeProperty.New(2.0f);

        [PropertyTooltip(
            "Scale of the noise around the trunk, lower values will give a more wobbly look, while higher values gives a more stochastic look."
        )]
        [PropertyRange(0f, 24.0f), TreeProperty, ShowIf(nameof(showNoiseSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree noiseScaleU = TreeProperty.New(0.9f);

        [PropertyTooltip(
            "Scale of the noise along the trunk, lower values will give a more wobbly look, while higher values gives a more stochastic look."
        )]
        [PropertyRange(0f, 24.0f), TreeProperty, ShowIf(nameof(showNoiseSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree noiseScaleV = TreeProperty.New(0.05f);

        [PropertyTooltip("Pushes the trunk further underground.")]
        [PropertyRange(1.0f, 5.0f), TreeProperty, ShowIf(nameof(showTrunkSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree sinkDepth = TreeProperty.New(2.0f);

        [PropertyTooltip("How far apart trunks can be spread.")]
        [PropertyRange(0f, 15f), TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree trunkSpread = TreeProperty.New(3.0f);

        #endregion

        private bool showNoiseSettings => showTrunkSettings && (flareNoise.accessor > 0);

        private bool showTrunkSettings => flareHeight > 0;

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is TrunkSettings cast)
            {
                cast.flareHeight = flareHeight;
                cast.flarePower = flarePower;
                cast.flareNoise = flareNoise;
                cast.noiseScaleU = noiseScaleU.Clone();
                cast.noiseScaleV = noiseScaleV.Clone();
                cast.flareRadius = flareRadius;
                cast.trunkSpread = trunkSpread;
                cast.flareDensity = flareDensity.Clone();
                cast.flareResolution = flareResolution.Clone();
                cast.flareDepth = flareDepth.Clone();
            }
        }

        #region ICloneable<TrunkSettings> Members

        public TrunkSettings Clone()
        {
            var clone = new TrunkSettings(settingsType);
            clone.trunkSpread = trunkSpread.Clone();
            clone.flareHeight = flareHeight.Clone();
            clone.flarePower = flarePower.Clone();
            clone.flareNoise = flareNoise.Clone();
            clone.noiseScaleU = noiseScaleU.Clone();
            clone.noiseScaleV = noiseScaleV.Clone();
            clone.flareRadius = flareRadius.Clone();
            clone.flareDensity = flareDensity.Clone();
            clone.flareResolution = flareResolution.Clone();
            clone.flareDepth = flareDepth.Clone();
            return clone;
        }

        #endregion
    }
}
