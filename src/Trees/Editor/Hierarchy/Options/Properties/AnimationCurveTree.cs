using System;
using System.Linq;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class AnimationCurveTree : TreeProperty<AnimationCurve>, ICloneable<AnimationCurveTree>
    {
        public AnimationCurveTree(AnimationCurve defaultValue) : base(defaultValue)
        {
        }

        /// <inheritdoc />
        public override AnimationCurve CloneElement(AnimationCurve model)
        {
            var curve = new AnimationCurve
            {
                keys = model.keys.ToArray(),
                preWrapMode = model.preWrapMode,
                postWrapMode = model.postWrapMode
            };

            return curve;
        }

        public float Evaluate(float t)
        {
            return accessor.Evaluate(Mathf.Clamp01(t));
        }

        public float EvaluateClamped(float t, float min, float max)
        {
            return Mathf.Clamp(Evaluate(t), min, max);
        }

        public float EvaluateClamped01(float t)
        {
            return EvaluateClamped(t, 0, 1);
        }

        #region ICloneable<AnimationCurveTree> Members

        public AnimationCurveTree Clone()
        {
            var clone = new AnimationCurveTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

        #endregion
    }
}
