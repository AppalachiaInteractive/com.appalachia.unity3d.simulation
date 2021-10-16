using System;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class UVScaleTree : TreeProperty<UVScale>, ICloneable<UVScaleTree>
    {
        public UVScaleTree(UVScale defaultValue) : base(defaultValue)
        {
        }

        public UVScaleTree Clone()
        {
            var clone = new UVScaleTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        public override UVScale CloneElement(UVScale model)
        {
            return model.Clone();
        }
    }
}
