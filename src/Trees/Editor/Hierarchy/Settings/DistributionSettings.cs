using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Distribution;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class DistributionSettings : AgeOverrideResponsiveSettings, ICloneable<DistributionSettings>
    {
        [PropertyTooltip("Adjusts the count and placement of shapes in the group.")]
        [HorizontalGroup("CurveA")]
        [PropertyRange(1, 100)]
        [TreeProperty, LabelText("Frequency")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public intTree distributionFrequency = TreeProperty.New(3);
        
        [PropertyTooltip("Use the curve to fine tune position, rotation and scale. " +
        "The curve is relative to the parent.")]
        [TreeCurve()]
        [TreeProperty, HideLabel]
        [HorizontalGroup("CurveA", .3f)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public AnimationCurveTree distributionCurve = 
            TreeProperty.Curve(0f, .6f, .9f, 1f, .6f, 0f);

        [PropertyTooltip(
            "Defines the chance of generation of shapes along the parent. Use the curve to adjust and the slider to fade the effect in and out.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve()]
        [TreeProperty, LabelText("Likelihood")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree distributionLikelihood = TreeProperty.fCurve(1f);

        [PropertyTooltip(
            "Defines the scale of shapes along the parent. Use the curve to adjust and the slider to fade the effect in and out.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve()]
        [TreeProperty, LabelText("Scale")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree distributionScale = TreeProperty.fCurve(1f, 1f, .6f);

        [PropertyTooltip("The way that the shapes are distributed along their parent.")]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public DistributionVerticalModeTree vertical =
            TreeProperty.New(DistributionVerticalMode.Equal);

        [FormerlySerializedAs("distributionNodes")]
        [PropertyTooltip(
            "Defines how many nodes are in each cluster when using Clusters distribution. " +
            "For real plants this is normally a Fibonacci number.")]
        [ShowIf(nameof(showClusterCount))]
        [PropertyRange(1, 21)]
        [TreeProperty, LabelText("Cluster Count")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public intTree clusterCount = TreeProperty.New(3);

        [PropertyTooltip("The way that the shapes are rotated around their parent.")]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public DistributionRadialModeTree radial =
            TreeProperty.New(DistributionRadialMode.Equal);
        
        [PropertyTooltip("Adjusts rotation per cluster in the cluster.")]
        [ShowIf(nameof(showRadialStepOffset))]
        [PropertyRange(-359, 359)]
        [TreeProperty, LabelText("Step Offset")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public intTree radialStepOffset = TreeProperty.New(60);
        
        
        [PropertyTooltip("Adjusts rotation per cluster in the cluster.")]
        [ShowIf(nameof(showRadialStepOffset))]
        [PropertyRange(0.0f, 1.0f)]
        [TreeProperty, LabelText("Step Jitter")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree radialStepJitter = TreeProperty.New(0.1f);

        [PropertyTooltip(
            "Defines the angle range that the shape can grow on.  For something like fungus on a northern face."
        )]
        [ShowIf(nameof(showDistributionRange))]
        [MinMaxSlider(-360f, 360f)]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree growthRange = TreeProperty.v2(-90f, 90f);

        [PropertyTooltip("Should the angle offset be random??")]
        
        [TreeProperty, LabelText("Random?")]
        [HorizontalGroup("Offset")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public boolTree randomDistributionAngleOffset = TreeProperty.New(true);

        private bool showDistributionAngle => !randomDistributionAngleOffset;
        
        [PropertyTooltip("Spin offset around the parent.")]
        [PropertyRange(-360f, 360f)]
        [TreeProperty, LabelText("Angle Offset"), EnableIf(nameof(showDistributionAngle))]
        [HorizontalGroup("Offset")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree distributionAngleOffset = TreeProperty.New(0f);
        
        
        [PropertyTooltip("Adjusts rotation of the offset per element.")]
        [ShowIf(nameof(showDistributionAngle))]
        [PropertyRange(0.0f, 1.0f)]
        [TreeProperty, LabelText("Offset Jitter")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree distributionAngleOffsetJitter = TreeProperty.New(0.1f);

        [PropertyTooltip("Spin around the parent.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve()]
        [TreeProperty, LabelText("Spin")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree distributionSpin = TreeProperty.fCurve(0.5f, .1f, 1f);

        [PropertyTooltip(
            "Defines the initial angle of growth relative to the parent. Use the curve to adjust and the slider to fade the effect in and out.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve(SettingsUpdateTarget.Distribution,-1f)]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree growthAngle = TreeProperty.fCurve(0f, 0f, .5f);

        private bool showClusterCount => vertical.UIValue == DistributionVerticalMode.Clusters;
        private bool showRadialStepOffset => radial.UIValue == DistributionRadialMode.Equal;
        private bool showDistributionRange => radial.UIValue == DistributionRadialMode.Range;
        
        public DistributionSettings Clone()
        {
            var clone = new DistributionSettings(settingsType);
            clone.distributionFrequency = distributionFrequency.Clone();
            clone.distributionCurve = distributionCurve.Clone();
            clone.distributionLikelihood = distributionLikelihood.Clone();
            clone.radial = radial.Clone();
            clone.vertical = vertical.Clone();
            clone.clusterCount = clusterCount.Clone();
            clone.growthRange = growthRange.Clone();
            clone.radialStepOffset = radialStepOffset.Clone();
            clone.radialStepJitter = radialStepJitter.Clone();
            clone.distributionScale = distributionScale.Clone();
            clone.randomDistributionAngleOffset = randomDistributionAngleOffset.Clone();
            clone.distributionAngleOffset = distributionAngleOffset.Clone();
            clone.distributionAngleOffsetJitter = distributionAngleOffsetJitter.Clone();
            clone.distributionSpin = distributionSpin.Clone();
            clone.growthAngle = growthAngle.Clone();
            return clone;
        }
      
        public DistributionSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is DistributionSettings cast)
            {
                cast.distributionFrequency = distributionFrequency.Clone();
                cast.distributionCurve = distributionCurve.Clone();
                cast.distributionLikelihood = distributionLikelihood.Clone();
                cast.radial = radial.Clone();
                cast.vertical = vertical.Clone();
                cast.clusterCount = clusterCount.Clone();
                cast.growthRange = growthRange.Clone();
                cast.radialStepOffset = radialStepOffset.Clone();
                cast.radialStepJitter = radialStepJitter.Clone();
                cast.distributionScale = distributionScale.Clone();
                cast.randomDistributionAngleOffset = randomDistributionAngleOffset.Clone();
                cast.distributionAngleOffsetJitter = distributionAngleOffsetJitter.Clone();
                cast.distributionAngleOffset = distributionAngleOffset.Clone();
                cast.distributionSpin = distributionSpin.Clone();
                cast.growthAngle = growthAngle.Clone();
            }
        }
    }
}
