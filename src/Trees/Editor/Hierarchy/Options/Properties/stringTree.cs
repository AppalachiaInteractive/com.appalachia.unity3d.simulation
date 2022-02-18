using System;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class stringTree : TreeProperty<string>, ICloneable<stringTree>
    {
        public stringTree(string defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override string CloneElement(string model)
        {
            return model;
        }

        #region ICloneable<stringTree> Members

        public stringTree Clone()
        {
            var clone = new stringTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
