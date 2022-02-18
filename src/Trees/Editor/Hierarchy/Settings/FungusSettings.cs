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
    public class FungusSettings : AgeOverrideResponsiveSettings, ICloneable<FungusSettings>
    {
        /*
        /// <inheritdoc />
public override void ToggleCheckboxes(bool enabled)
        {
            size.overridesVisible = enabled;
            //horizontalAlign.overridesVisible = enabled;
            perpendicularAlign.overridesVisible = enabled;
        }*/

        public FungusSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            prefab = new PrefabSetup(settingsType);
        }

        #region Fields and Autoproperties

        /*[PropertyTooltip("Adjusts whether the fungi are aligned horizontally.")]
        [UnityEngine.Range(0f, 1f),TreeProperty, PropertyOrder(20)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree  horizontalAlign = TreeProperty.New(0.0f);*/

        [PropertyTooltip("Adjusts whether the fungi are aligned perpendicular to the parent branch.")]
        [UnityEngine.Range(0f, 1f), TreeProperty, PropertyOrder(30)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree perpendicularAlign = TreeProperty.New(1.0f);

        [InlineProperty, HideLabel]
        public PrefabSetup prefab;

        [PropertyTooltip(
            "Adjusts the size of the fungi, use the range to adjust the minimum and the maximum size."
        )]
        [MinMaxSlider(.05f, 3.0f, true), TreeProperty, PropertyOrder(10)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree size = TreeProperty.v2(1f, 1f);

        #endregion

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is FungusSettings cast)
            {
                cast.prefab = prefab.Clone();
                cast.size = size.Clone();
                cast.perpendicularAlign = perpendicularAlign.Clone();
            }
        }

        #region ICloneable<FungusSettings> Members

        public FungusSettings Clone()
        {
            var clone = new FungusSettings(settingsType);
            clone.prefab = prefab.Clone();
            clone.size = size.Clone();

            //clone.horizontalAlign = horizontalAlign.Clone();
            clone.perpendicularAlign = perpendicularAlign.Clone();
            return clone;
        }

        #endregion
    }
}
