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
    public class CollarSettings : AgeOverrideResponsiveSettings, ICloneable<CollarSettings>
    {
        /*public override void ToggleCheckboxes(bool enabled)
        {
            collarHeight.overridesVisible = enabled;
            collarSpreadBottom.overridesVisible = enabled;
            collarSpreadMultiplier.overridesVisible = enabled;
            collarSpreadTop.overridesVisible = enabled;
        }*/

        public CollarSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [TabGroup("Base Weld")]
        [PropertyTooltip("Defines whether or not branches are welded to their parents.")]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public boolTree disableWelding = TreeProperty.New(false);

        [TabGroup("Collar")]
        [PropertyTooltip("Defines how far up the branch the collar spread starts.")]
        [PropertyRange(0.00f, .2f), TreeProperty, PropertyOrder(100)]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree collarHeight = TreeProperty.New(0.00f);

        [TabGroup("Collar")]
        [PropertyTooltip("How quickly the collar fades.")]
        [PropertyRange(.1f, 3f), TreeProperty, PropertyOrder(103), ShowIf(nameof(showCollarSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree collarPower = TreeProperty.New(1.5f);

        [TabGroup("Collar")]
        [PropertyTooltip(
            "Collar spread factor on the bottom-side of the branch, relative to its parent branch. Zero means no spread."
        )]
        [PropertyRange(0f, 1f), TreeProperty, PropertyOrder(101), ShowIf(nameof(showCollarSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree collarSpreadBottom = TreeProperty.New(0.0f);

        [TabGroup("Collar")]
        [PropertyTooltip("Adjusts the collar size.")]
        [PropertyRange(0f, 10f), TreeProperty, PropertyOrder(107), ShowIf(nameof(showCollarSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree collarSpreadMultiplier = TreeProperty.New(5.0f);

        [TabGroup("Collar")]
        [PropertyTooltip(
            "Collar spread factor on the top-side of the branch, relative to its parent branch. Zero means no spread."
        )]
        [PropertyRange(0f, 1f), TreeProperty, PropertyOrder(102), ShowIf(nameof(showCollarSettings))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree collarSpreadTop = TreeProperty.New(0.0f);

        [TabGroup("Base Weld")]
        [PropertyTooltip("The base normal blend strength.")]
        [PropertyRange(0.0f, 1.0f)]
        [TreeProperty, LabelText("Base Normal Blend")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree weldNormalBlend = TreeProperty.New(.50f);

        [TabGroup("Base Weld")]
        [PropertyTooltip("The normal blend distance relative to the parent radius at the weld point.")]
        [PropertyRange(0.0f, 10.0f)]
        [TreeProperty, LabelText("Normal Blend Size")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree weldNormalBlendSize = TreeProperty.New(5f);

        [TabGroup("Base Weld")]
        [PropertyTooltip("The weld distance relative to the parent radius at the weld point.")]
        [PropertyRange(1.0f, 10.0f)]
        [TreeProperty, LabelText("Weld Size"), HideIf(nameof(hideWeldingDistance))]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree weldSize = TreeProperty.New(4f);

        #endregion

        private bool hideWeldingDistance => disableWelding;

        private bool showCollarSettings => collarHeight > 0;

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is CollarSettings cast)
            {
                cast.collarHeight = collarHeight.Clone();
                cast.collarSpreadBottom = collarSpreadBottom.Clone();
                cast.collarSpreadTop = collarSpreadTop.Clone();
                cast.collarPower = collarPower.Clone();
                cast.collarSpreadMultiplier = collarSpreadMultiplier.Clone();
                cast.disableWelding = disableWelding.Clone();
                cast.weldSize = weldSize.Clone();
                cast.weldNormalBlendSize = weldNormalBlendSize.Clone();
                cast.weldNormalBlend = weldNormalBlend.Clone();
            }
        }

        #region ICloneable<CollarSettings> Members

        public CollarSettings Clone()
        {
            var clone = new CollarSettings(settingsType);
            clone.collarHeight = collarHeight.Clone();
            clone.collarSpreadBottom = collarSpreadBottom.Clone();
            clone.collarSpreadTop = collarSpreadTop.Clone();
            clone.collarPower = collarPower.Clone();
            clone.collarSpreadMultiplier = collarSpreadMultiplier.Clone();
            clone.disableWelding = disableWelding.Clone();
            clone.weldSize = weldSize.Clone();
            clone.weldNormalBlendSize = weldNormalBlendSize.Clone();
            clone.weldNormalBlend = weldNormalBlend.Clone();
            return clone;
        }

        #endregion
    }
}
