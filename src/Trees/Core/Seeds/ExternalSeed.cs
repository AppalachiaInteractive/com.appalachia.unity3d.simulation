using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Seeds
{
    [Serializable]
    public sealed class ExternalSeed : BaseSeed
    {
        [PropertyTooltip("The seed value for procedural generation.")]
        [PropertyRange(1, HIGH_ELEMENT)]
        [LabelText("Seed Value"), InlineProperty, LabelWidth(100)]
        public int value;

        public override int EffectiveSeed => (int)Mathf.Clamp(
            (value * (1f/HIGH_ELEMENT)) * _seed,
            0, 
            HIGH_ELEMENT);

        public ExternalSeed(int seed, int value) : base(seed)
        {
            this.value = Mathf.Clamp(value, 0, HIGH_ELEMENT);
            SetInternalSeed(seed);
        }
    }
}
