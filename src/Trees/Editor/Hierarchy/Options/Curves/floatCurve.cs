using System;
using System.Linq;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Curves
{
    [Serializable]
    public class floatCurve : structCurve<float>, ICloneable<floatCurve>
    {
        public override float Evaluate(float t) => curve.Evaluate(Mathf.Clamp01(t));

        public override float EvaluateClamped(float t, float min, float max) =>
            Mathf.Clamp(Evaluate(t), min, max);

        public override float EvaluateScaled(float t) => curve.Evaluate(Mathf.Clamp01(t)) * value;

        public float EvaluateScaledInverse(float t) => (curve.Evaluate(Mathf.Clamp01(t)) * value) + (1.0f - value);


        public float EvaluateScaled01(float t) => EvaluateScaledClamped(t, 0f, 1f);

        public float EvaluateScaledInverse01(float t) => Mathf.Clamp01(EvaluateScaledInverse(t));

        public float EvaluateScaledClamped(float t, float min, float max) =>
            Mathf.Clamp(EvaluateScaled(t), min, max);

        public floatCurve(float initial) : base(initial)
        {
        }

        public floatCurve(float initial, float curveStart, float curveEnd) : base(initial, curveStart, curveEnd)
        {
        }

        public floatCurve(float initial, Keyframe start, Keyframe end) : base(initial, start, end)
        {
        }
        
        public floatCurve(float initial, params Keyframe[] args) : base(initial, 
            new AnimationCurve(args))
        {
        }

        public floatCurve(float initial, AnimationCurve curve) : base(initial, curve)
        {
        }

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
    }
}
