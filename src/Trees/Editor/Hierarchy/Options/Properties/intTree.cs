using System;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class intTree : TreeProperty<int>, ICloneable<intTree>
    {
        public intTree(int defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override int CloneElement(int model)
        {
            return model;
        }

        #region ICloneable<intTree> Members

        public intTree Clone()
        {
            var clone = new intTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
