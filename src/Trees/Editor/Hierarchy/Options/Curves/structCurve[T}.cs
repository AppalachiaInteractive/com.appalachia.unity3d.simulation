using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Curves
{
    [Serializable]
    public abstract class structCurve<T> : structCurve
    {

        [HideLabel, InlineProperty, PropertyOrder(0), HorizontalGroup()]
        public T value;

        public abstract T Evaluate(float t);

        public abstract T EvaluateClamped(float t, T min, T max);

        public abstract T EvaluateScaled(float t);

        protected structCurve(T initial) : this(initial, 1f, 1f)
        {
            
        }

        protected structCurve(T initial, float curveStart, float curveEnd) :
            this(initial, new Keyframe(0.0f, curveStart), new Keyframe(1.0f, curveEnd))
        {
        }

        protected structCurve(T initial, Keyframe start, Keyframe end) : this(
            initial, new AnimationCurve(start, end)
            {
                preWrapMode = WrapMode.ClampForever, postWrapMode = WrapMode.ClampForever
            })
        {
        }

        protected structCurve(T initial, AnimationCurve curve)
        {
            value = initial;
            this.curve = curve;
        }
    }
}
