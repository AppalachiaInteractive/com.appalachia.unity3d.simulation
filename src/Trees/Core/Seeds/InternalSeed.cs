using System;

namespace Appalachia.Simulation.Trees.Core.Seeds
{
    [Serializable]
    public sealed class InternalSeed : BaseSeed
    {
        public InternalSeed(int seed) : base(seed)
        {
        }

        public InternalSeed Clone()
        {
            var newSeed = new InternalSeed(_seed);
            return newSeed;
        }
    }
}
