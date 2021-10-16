using System;
using Appalachia.Simulation.Trees.Hierarchy.Options.Curves;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class floatCurveTree : TreeProperty<floatCurve>, ICloneable<floatCurveTree>
    {
        public floatCurveTree(floatCurve defaultValue) : base(defaultValue)
        {
        }

        public floatCurveTree Clone()
        {
            var clone = new floatCurveTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        public override floatCurve CloneElement(floatCurve model)
        {
            return model.Clone();
        }
    }
}