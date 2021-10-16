using System;
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
    public class FrondSettings : AgeOverrideResponsiveSettings, ICloneable<FrondSettings>
    {
        
        [PropertyTooltip("Material for fronds.")]
        [TreePropertySimple]
        [OnValueChanged(nameof(MaterialGenerationChanged), true)]
        [DelayedProperty]
        public Material frondMaterial;
        
        [PropertyTooltip(
            "Defines the number of fronds per branch. Fronds are always evenly spaced around the branch.")]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public intTree frondCount = TreeProperty.New(1);

        [PropertyTooltip("Adjust to crease / fold the fronds.")]
        [PropertyRange(0f, 1f)]
        [TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree frondCrease = TreeProperty.New(0.0f);

        [PropertyTooltip("Defines the starting and ending point of the fronds.")]
        [MinMaxSlider(0f, 1f)]
        [TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public Vector2Tree frondRange = TreeProperty.v2(0.1f, 1.0f);

        [PropertyTooltip("Defines rotation around the parent branch.")]
        [PropertyRange(0f, 1f)]
        [TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree frondRotation = TreeProperty.New(0.0f);

        [PropertyTooltip(
            "The width of the fronds, use the curve to adjust the specific shape along the length of the branch.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve(SettingsUpdateTarget.Geometry)]
        [TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatCurveTree frondWidth = TreeProperty.fCurve(1.0f, 1f, 0f);
        
        
        public FrondSettings Clone()
        {
            var clone = new FrondSettings(settingsType);
            clone.frondMaterial = frondMaterial;
            clone.frondCount = frondCount.Clone();
            clone.frondCrease = frondCrease.Clone();
            clone.frondRange = frondRange.Clone();
            clone.frondRotation = frondRotation.Clone();
            clone.frondWidth = frondWidth.Clone();
            return clone;
        }
        
        /*
        public override void ToggleCheckboxes(bool enabled)
        {
            frondCount.overridesVisible = enabled;
            frondCrease.overridesVisible = enabled;
            frondRange.overridesVisible = enabled;
            frondRotation.overridesVisible = enabled;
            frondWidth.overridesVisible = enabled;
        }*/

        public FrondSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is FrondSettings cast)
            {
                cast.frondMaterial = frondMaterial;
                cast.frondCount = frondCount.Clone();
                cast.frondCrease = frondCrease.Clone();
                cast.frondRange = frondRange.Clone();
                cast.frondRotation = frondRotation.Clone();
                cast.frondWidth = frondWidth.Clone();
            }
        }
    }
}