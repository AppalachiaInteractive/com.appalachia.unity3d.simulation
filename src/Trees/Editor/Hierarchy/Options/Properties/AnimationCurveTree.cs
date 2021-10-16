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

        public float Evaluate(float t) => accessor.Evaluate(Mathf.Clamp01(t));

        public float EvaluateClamped(float t, float min, float max) => Mathf.Clamp(Evaluate(t), min, max);

        public float EvaluateClamped01(float t) => EvaluateClamped(t, 0, 1);

        public AnimationCurveTree Clone()
        {
            var clone = new AnimationCurveTree(CloneElement(defaultValue));
            base.Clone(clone);

            return clone;
        }

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
    }
}