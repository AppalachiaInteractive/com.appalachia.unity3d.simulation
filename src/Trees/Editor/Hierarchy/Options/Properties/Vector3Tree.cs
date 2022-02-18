using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class Vector3Tree : TreeProperty<Vector3>, ICloneable<Vector3Tree>
    {
        public Vector3Tree(Vector3 defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override Vector3 CloneElement(Vector3 model)
        {
            return model;
        }

        #region ICloneable<Vector3Tree> Members

        public Vector3Tree Clone()
        {
            var clone = new Vector3Tree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
