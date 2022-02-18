using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class QuaternionTree : TreeProperty<Quaternion>, ICloneable<QuaternionTree>
    {
        public QuaternionTree(Quaternion defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override Quaternion CloneElement(Quaternion model)
        {
            return model;
        }

        #region ICloneable<QuaternionTree> Members

        public QuaternionTree Clone()
        {
            var clone = new QuaternionTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
