using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class Vector2Tree : TreeProperty<Vector2>, ICloneable<Vector2Tree>
    {
        public Vector2Tree(Vector2 defaultValue) : base(defaultValue)
        {
        }

        public Vector2Tree Clone()
        {
            var clone = new Vector2Tree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        public override Vector2 CloneElement(Vector2 model)
        {
            return model;
        }
    }
}