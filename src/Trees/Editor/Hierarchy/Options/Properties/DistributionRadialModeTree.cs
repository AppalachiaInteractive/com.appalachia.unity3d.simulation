using System;
using Appalachia.Simulation.Trees.Generation.Distribution;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class DistributionRadialModeTree : TreeProperty<DistributionRadialMode>,
                                              ICloneable<DistributionRadialModeTree>
    {
        public DistributionRadialModeTree(DistributionRadialMode defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override DistributionRadialMode CloneElement(DistributionRadialMode model)
        {
            return model;
        }

        #region ICloneable<DistributionRadialModeTree> Members

        public DistributionRadialModeTree Clone()
        {
            var clone = new DistributionRadialModeTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
