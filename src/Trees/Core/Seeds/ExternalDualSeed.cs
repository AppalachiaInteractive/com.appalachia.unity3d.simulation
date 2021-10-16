using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Seeds
{
    [Serializable]
    public sealed class ExternalDualSeed
    {
        [PropertyTooltip("The seed value for procedural generation.")]
        [PropertyRange(1, BaseSeed.HIGH_ELEMENT)]
        [LabelText("Seed Value"), InlineProperty, LabelWidth(100)]
        [OnValueChanged(nameof(UpdateChildren))]
        public int value;

        [HideInInspector]
        public ExternalSeed primary;
        
        [HideInInspector]
        public ExternalSeed secondary;

        public ExternalDualSeed(int seed1, int seed2, int value)
        {
            this.value = Mathf.Clamp(value, 0, BaseSeed.HIGH_ELEMENT);
            primary = new ExternalSeed(seed1, value);
            secondary = new ExternalSeed(seed2, value);
        }

        private void UpdateChildren()
        {
            primary.value = value;
            secondary.value = value;
        }

        public void Reset()
        {
            primary.Reset();
            secondary.Reset();
        }
        
        public void SetInternalSeed(double seed1, double seed2)
        {
            SetInternalSeed((int) seed1, (int) seed2);
        }
        
        public void SetInternalSeed(int seed1, int seed2)
        {
            primary.SetInternalSeed(seed1);
            secondary.SetInternalSeed(seed2);

            Reset();
        }
    }
}
