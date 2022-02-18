using System;
using Appalachia.Simulation.Trees.Hierarchy.Options.Curves;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class intCurveTree : TreeProperty<intCurve>, ICloneable<intCurveTree>
    {
        public intCurveTree(intCurve defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override intCurve CloneElement(intCurve model)
        {
            return model.Clone();
        }

        #region ICloneable<intCurveTree> Members

        public intCurveTree Clone()
        {
            var clone = new intCurveTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
