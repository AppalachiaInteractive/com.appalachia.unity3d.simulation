using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    public class DefaultMaterialSettingsPublic : ResponsiveSettings
    {
        [HorizontalGroup("A")]
        [LabelWidth(60)]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public Material branches;

        [HorizontalGroup("B")]
        [LabelWidth(60)]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public Material breaks;

        [HorizontalGroup("B")]
        [LabelWidth(60)]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public Material fronds;

        [HorizontalGroup("A")]
        [LabelWidth(60)]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public Material leaves;

        public DefaultMaterialSettingsPublic(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
    }
}
