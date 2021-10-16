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
    public class KnotSettings : AgeOverrideResponsiveSettings, ICloneable<KnotSettings>
    {
        [InlineProperty, HideLabel]
        public PrefabSetup prefab;

        [PropertyTooltip(
            "Adjusts the size of the knots, use the range to adjust the minimum and the maximum size.")]
        [MinMaxSlider(.05f, 3.0f, true),TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree size = TreeProperty.v2(1f, 1f);
        
        [PropertyTooltip("How deep into the tree the knot is pushed.")]
        [PropertyRange(0f, 1f),TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree depth = TreeProperty.New(0.0f);
        
        public KnotSettings Clone()
        {
            var clone = new KnotSettings(settingsType);
            clone.prefab = prefab.Clone();
            clone.size = size.Clone();
            clone.depth = depth.Clone();
            return clone;
        }
        
        /*
        public override void ToggleCheckboxes(bool enabled)
        {
            size.overridesVisible = enabled;
            depth.overridesVisible = enabled;
        }*/

        public KnotSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            prefab = new PrefabSetup(settingsType);
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is KnotSettings cast)
            {
                cast.prefab = prefab.Clone();
                cast.size = size.Clone();
                cast.depth = depth.Clone();
            }
        }
    }
}
