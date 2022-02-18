#region

using System;
using System.Linq;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Curves
{
    [Serializable]
    public class intCurve : structCurve<int>, ICloneable<intCurve>
    {
        public intCurve(int initial) : base(initial)
        {
        }

        public intCurve(int initial, float curveStart, float curveEnd) : base(initial, curveStart, curveEnd)
        {
        }

        public intCurve(int initial, Keyframe start, Keyframe end) : base(initial, start, end)
        {
        }

        public intCurve(int initial, params Keyframe[] args) : base(initial, new AnimationCurve(args))
        {
        }

        public intCurve(int initial, AnimationCurve curve) : base(initial, curve)
        {
        }

        /// <inheritdoc />
        public override int Evaluate(float t)
        {
            return (int)curve.Evaluate(Mathf.Clamp01(t));
        }

        /// <inheritdoc />
        public override int EvaluateClamped(float t, int min, int max)
        {
            return Mathf.Clamp(Evaluate(t), min, max);
        }

        /// <inheritdoc />
        public override int EvaluateScaled(float t)
        {
            return (int)(curve.Evaluate(Mathf.Clamp01(t)) * value);
        }

        #region ICloneable<intCurve> Members

        public intCurve Clone()
        {
            {
                var c = new AnimationCurve
                {
                    keys = curve.keys.ToArray(),
                    preWrapMode = curve.preWrapMode,
                    postWrapMode = curve.postWrapMode
                };

                var clone = new intCurve(value, c);

                return clone;
            }
        }

        #endregion
    }
}
