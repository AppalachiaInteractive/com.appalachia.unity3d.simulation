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
    public class FruitSettings : AgeOverrideResponsiveSettings, ICloneable<FruitSettings>
    {
        /*
        /// <inheritdoc />
public override void ToggleCheckboxes(bool enabled)
        {
            size.overridesVisible = enabled;
            perpendicularAlign.overridesVisible = enabled;
            //horizontalAlign.overridesVisible = enabled;
        }
        */

        public FruitSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            prefab = new PrefabSetup(settingsType);
        }

        #region Fields and Autoproperties

        [PropertyTooltip("Adjusts whether the fruits are aligned perpendicular to the parent branch.")]
        [UnityEngine.Range(0f, 1f)]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree perpendicularAlign = TreeProperty.New(0.0f);

        [InlineProperty, HideLabel]
        public PrefabSetup prefab;

        [PropertyTooltip(
            "Adjusts the size of the fruits, use the range to adjust the minimum and the maximum size."
        )]
        [MinMaxSlider(.05f, 3.0f, true)]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree size = TreeProperty.v2(1f, 1f);

        #endregion

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is FruitSettings cast)
            {
                cast.prefab = prefab.Clone();
                cast.size = size.Clone();
                cast.perpendicularAlign = perpendicularAlign.Clone();
            }
        }

        #region ICloneable<FruitSettings> Members

        /*
        [PropertyTooltip("Adjusts whether the leaves are aligned horizontally.")]
        [UnityEngine.Range(0f, 1f)]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree horizontalAlign = TreeProperty.New(0.0f);
        */

        public FruitSettings Clone()
        {
            var clone = new FruitSettings(settingsType);
            clone.prefab = prefab.Clone();
            clone.size = size.Clone();
            clone.perpendicularAlign = perpendicularAlign.Clone();

            //clone.horizontalAlign = horizontalAlign.Clone();
            return clone;
        }

        #endregion
    }
}
