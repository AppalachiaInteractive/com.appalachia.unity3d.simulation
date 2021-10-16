using System;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class floatTree : TreeProperty<float>, ICloneable<floatTree>
    {
        public floatTree(float defaultValue) : base(defaultValue)
        {
        }

        public floatTree Clone()
        {
            var clone = new floatTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        public override float CloneElement(float model)
        {
            return model;
        }
    }
}