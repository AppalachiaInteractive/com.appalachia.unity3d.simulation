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

        public Vector3Tree Clone()
        {
            var clone = new Vector3Tree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        public override Vector3 CloneElement(Vector3 model)
        {
            return model;
        }
    }
}