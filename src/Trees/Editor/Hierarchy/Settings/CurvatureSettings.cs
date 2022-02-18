using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class CurvatureSettings : AgeOverrideResponsiveSettings, ICloneable<CurvatureSettings>
    {
        public CurvatureSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [PropertyTooltip("Adjusts how crooked the branches are, use the curve to fine-tune.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree crookedness = TreeProperty.fCurve(0.1f, 0f, .5f);

        [PropertyTooltip("Adjusts how likely it is for a crook to occur.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree crookLikelihood = TreeProperty.fCurve(1.0f, 0f, .33f, 0.0f);

        [FormerlySerializedAs("relativeDirectionality")]
        [PropertyTooltip("Use the curve to adjust how the branches are curl back towards their parent.")]
        [PropertyRange(0f, 1f), TreeProperty]
        [TreeCurve(SettingsUpdateTarget.Distribution, -1f)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree curliness = TreeProperty.fCurve(0f, 0f, 0f, 0f);

        [FormerlySerializedAs("thigmotropism")]
        [PropertyTooltip("Use the curve to adjust how the branches seek towards or away from their parent.")]
        [PropertyRange(0f, 1f), TreeProperty]
        [TreeCurve(SettingsUpdateTarget.Distribution, -1f)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree exotropism = TreeProperty.fCurve(0f, 0f, .25f, 0f);

        /*
        [SerializeField, HideInInspector]
        public bool showThigmotropism;*/

        [PropertyTooltip("Overall noise factor, use the curve to fine-tune.")]
        [PropertyRange(0f, 1f), TreeProperty]
        [TreeCurve(SettingsUpdateTarget.Geometry)]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatCurveTree noise = TreeProperty.fCurve(0.1f, .5f, 0f);

        [PropertyTooltip(
            "Use the curve to adjust how the branches are bent towards/away from the sun and the slider to change the scale."
        )]
        [PropertyRange(0f, 1f), TreeProperty]
        [TreeCurve(SettingsUpdateTarget.Distribution, -1f)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree phototropism = TreeProperty.fCurve(0f, 0f, .25f, 0f);

        [PropertyTooltip("How quickly a crook causes branch direction to change.")]
        [PropertyRange(0f, 1f), TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree crookAbruptness = TreeProperty.New(0.33f);

        [PropertyTooltip("How long a crook influences branch direction.")]
        [PropertyRange(0f, 1f), TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree crookMemory = TreeProperty.New(0.9f);

        [PropertyTooltip(
            "Scale of the noise around the branch, lower values will give a more wobbly look, while higher values gives a more stochastic look."
        )]
        [PropertyRange(0f, 8.0f), TreeProperty, ShowIf(nameof(showNoiseSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree noiseScaleU = TreeProperty.New(0.9f);

        [PropertyTooltip(
            "Scale of the noise along the branch, lower values will give a more wobbly look, while higher values gives a more stochastic look."
        )]
        [PropertyRange(0f, 8.0f), TreeProperty, ShowIf(nameof(showNoiseSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree noiseScaleV = TreeProperty.New(0.05f);

        #endregion

        private bool showNoiseSettings => noise.accessor.value > 0;

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is CurvatureSettings cast)
            {
                cast.crookedness = crookedness.Clone();
                cast.curliness = curliness.Clone();
                cast.phototropism = phototropism.Clone();
                cast.exotropism = exotropism.Clone();
                cast.noise = noise.Clone();
                cast.noiseScaleU = noiseScaleU.Clone();
                cast.noiseScaleV = noiseScaleV.Clone();
                cast.crookAbruptness = crookAbruptness.Clone();
                cast.crookLikelihood = crookLikelihood.Clone();
                cast.crookMemory = crookMemory.Clone();
            }
        }

        #region ICloneable<CurvatureSettings> Members

        public CurvatureSettings Clone()
        {
            var clone = new CurvatureSettings(settingsType);
            clone.crookedness = crookedness.Clone();
            clone.curliness = curliness.Clone();
            clone.phototropism = phototropism.Clone();
            clone.exotropism = exotropism.Clone();
            clone.noise = noise.Clone();
            clone.noiseScaleU = noiseScaleU.Clone();
            clone.noiseScaleV = noiseScaleV.Clone();
            clone.crookAbruptness = crookAbruptness.Clone();
            clone.crookLikelihood = crookLikelihood.Clone();
            clone.crookMemory = crookMemory.Clone();
            return clone;
        }

        #endregion
    }
}
