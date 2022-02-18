using System;
using System.Linq;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Curves
{
    [Serializable]
    public class floatCurve : structCurve<float>, ICloneable<floatCurve>
    {
        public floatCurve(float initial) : base(initial)
        {
        }

        public floatCurve(float initial, float curveStart, float curveEnd) : base(
            initial,
            curveStart,
            curveEnd
        )
        {
        }

        public floatCurve(float initial, Keyframe start, Keyframe end) : base(initial, start, end)
        {
        }

        public floatCurve(float initial, params Keyframe[] args) : base(initial, new AnimationCurve(args))
        {
        }

        public floatCurve(float initial, AnimationCurve curve) : base(initial, curve)
        {
        }

        /// <inheritdoc />
        public override float Evaluate(float t)
        {
            return curve.Evaluate(Mathf.Clamp01(t));
        }

        /// <inheritdoc />
        public override float EvaluateClamped(float t, float min, float max)
        {
            return Mathf.Clamp(Evaluate(t), min, max);
        }

        /// <inheritdoc />
        public override float EvaluateScaled(float t)
        {
            return curve.Evaluate(Mathf.Clamp01(t)) * value;
        }

        public float EvaluateScaled01(float t)
        {
            return EvaluateScaledClamped(t, 0f, 1f);
        }

        public float EvaluateScaledClamped(float t, float min, float max)
        {
            return Mathf.Clamp(EvaluateScaled(t), min, max);
        }

        public float EvaluateScaledInverse(float t)
        {
            return (curve.Evaluate(Mathf.Clamp01(t)) * value) + (1.0f - value);
        }

        public float EvaluateScaledInverse01(float t)
        {
            return Mathf.Clamp01(EvaluateScaledInverse(t));
        }

        #region ICloneable<floatCurve> Members

        public floatCurve Clone()
        {
            var c = new AnimationCurve
            {
                keys = curve.keys.ToArray(),
                preWrapMode = curve.preWrapMode,
                postWrapMode = curve.postWrapMode
            };

            var clone = new floatCurve(value, c);

            return clone;
        }

        #endregion
    }
}
