using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class Vector4Tree : TreeProperty<Vector4>, ICloneable<Vector4Tree>
    {
        public Vector4Tree(Vector4 defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override Vector4 CloneElement(Vector4 model)
        {
            return model;
        }

        #region ICloneable<Vector4Tree> Members

        public Vector4Tree Clone()
        {
            var clone = new Vector4Tree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
