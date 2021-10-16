using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Curves
{
    [Serializable]
    public abstract class structCurve : ResponsiveSettings
    {
        
        [HideLabel, InlineProperty, HideReferenceObjectPicker, PropertyOrder(100), HorizontalGroup(.2f)]
        //[OnValueChanged(nameof(MeshSettingsChanged))]
        public AnimationCurve curve;

        protected structCurve() : base(ResponsiveSettingsType.Tree)
        {
        }
    }
}
