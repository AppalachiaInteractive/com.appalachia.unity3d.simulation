using System;
using Appalachia.Simulation.Trees.ResponsiveUI;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TreeCurveAttribute : Attribute
    {
        public SettingsUpdateTarget target;
        public float MinTime;
        public float MaxTime = 1f;
        public float MinValue;
        public float MaxValue = 1f;

        public TreeCurveAttribute(
            SettingsUpdateTarget target = SettingsUpdateTarget.Distribution,
            float minValue = 0f,
            float maxValue = 1f,
            float minTime = 0f,
            float maxTime = 1f)
        {
            this.target = target;
            MinTime = minTime;
            MaxTime = maxTime;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
