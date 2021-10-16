using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable, HideLabel, InlineProperty]
    public class InitializationSettings : ResponsiveSettings
    {
        [FormerlySerializedAs("speciesName")]
        [DelayedProperty]
        public string name;

        public bool convertTreeData;

        [EnableIf(nameof(convertTreeData))]
        public TreeEditor.TreeData original;

        public InitializationSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
    }
}
