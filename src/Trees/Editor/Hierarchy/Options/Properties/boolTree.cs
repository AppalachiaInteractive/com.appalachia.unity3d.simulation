using System;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class boolTree : TreeProperty<bool>, ICloneable<boolTree>
    {
        public boolTree(bool defaultValue) : base(defaultValue)
        {
        }

      
        public boolTree Clone()
        {
            var clone = new boolTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        public override bool CloneElement(bool model)
        {
            return model;
        }
    }
}
