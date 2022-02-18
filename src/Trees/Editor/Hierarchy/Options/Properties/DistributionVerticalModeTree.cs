using System;
using Appalachia.Simulation.Trees.Generation.Distribution;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class DistributionVerticalModeTree : TreeProperty<DistributionVerticalMode>,
                                                ICloneable<DistributionVerticalModeTree>
    {
        public DistributionVerticalModeTree(DistributionVerticalMode defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override DistributionVerticalMode CloneElement(DistributionVerticalMode model)
        {
            return model;
        }

        #region ICloneable<DistributionVerticalModeTree> Members

        public DistributionVerticalModeTree Clone()
        {
            var clone = new DistributionVerticalModeTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
