using System;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Distribution;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials;
using Appalachia.Simulation.Trees.Hierarchy.Options.Curves;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public abstract class TreeProperty : ResponsiveSettings
    {
        [ShowInInspector]
        [HideLabel]
        [GUIColor(nameof(overrideColor))]
        [ShowIf(nameof(overridesVisible))]
        [HorizontalGroup(MaxWidth = 15, PaddingLeft = 5, PaddingRight = 5)]
        //[OnValueChanged(nameof(DistributionSettingsChanged))]
        public bool toggler
        {
            get => GetActiveUIToggle();
            set => SetActiveUIValue(value);
        }

        [HideInInspector] public bool overrideAdult;

        [HideInInspector] public bool overrideMature;

        [HideInInspector] public bool overrideSapling;

        [HideInInspector] public bool overrideSpirit;

        [HideInInspector] public bool overrideYoung;

        [HideInInspector] public bool initialized;

        public bool overridesVisible => settingsType == ResponsiveSettingsType.Tree;

        protected Color overrideColor =>
            currentOverrideEnabled ? new Color(.2f, .6f, .3f, .8f) :
            anyOverridesEnabled ? new Color(.5f, .4f, .1f, 1f) : new Color(.1f, .2f, .7f, 1f);

        protected static AgeType _activeGenerationAge = AgeType.Mature;

        protected static AgeType _activeUIAge = AgeType.Mature;

        protected bool anyOverridesEnabled =>
            overrideAdult || overrideMature || overrideSapling || overrideSpirit || overrideYoung;

        protected bool currentOverrideEnabled
        {
            get
            {
                var activeUIAge = settingsType == ResponsiveSettingsType.Branch ? AgeType.Mature : _activeUIAge;
                
                switch (activeUIAge)
                {
                    case AgeType.None:
                        return false;
                    case AgeType.Sapling:
                        return overrideSapling;
                    case AgeType.Young:
                        return overrideYoung;
                    case AgeType.Adult:
                        return overrideAdult;
                    case AgeType.Mature:
                        return overrideMature;
                    case AgeType.Spirit:
                        return overrideSpirit;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void SetActiveUIAge(AgeType age)
        {
            _activeUIAge = age;
        }

        public static void SetActiveGenerationAge(AgeType age)
        {
            _activeGenerationAge = age;
        }

        private bool GetActiveUIToggle()
        {
            var activeUIAge = settingsType == ResponsiveSettingsType.Branch ? AgeType.Mature : _activeUIAge;
            return GetToggle(activeUIAge);
        }

        private bool GetToggle(AgeType age)
        {
            switch (age)
            {
                case AgeType.None:
                    return false;
                case AgeType.Sapling:
                    return overrideSapling;
                case AgeType.Young:
                    return overrideYoung;
                case AgeType.Adult:
                    return overrideAdult;
                case AgeType.Mature:
                    return overrideMature;
                case AgeType.Spirit:
                    return overrideSpirit;
                default:
                    throw new ArgumentOutOfRangeException(nameof(age), age, null);
            }
        }

        public void SetActiveUIValue(bool val)
        {
            var activeUIAge = settingsType == ResponsiveSettingsType.Branch ? AgeType.Mature : _activeUIAge;
            SetToggle(val, activeUIAge);
        }

        public void SetToggle(bool val, AgeType age)
        {
            switch (age)
            {
                case AgeType.Sapling:
                    overrideSapling = val;
                    break;
                case AgeType.Young:
                    overrideYoung = val;
                    break;
                case AgeType.Adult:
                    overrideAdult = val;
                    break;
                case AgeType.Mature:
                    overrideMature = val;
                    break;
                case AgeType.Spirit:
                    overrideSpirit = val;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(age), age, null);
            }
        }

        public static boolTree New(bool defaultValue)
        {
            return new boolTree(defaultValue);
        }

        public static floatTree New(float defaultValue)
        {
            return new floatTree(defaultValue);
        }

        public static intTree New(int defaultValue)
        {
            return new intTree(defaultValue);
        }

        public static DistributionRadialModeTree New(DistributionRadialMode defaultValue)
        {
            return new DistributionRadialModeTree(defaultValue);
        }
        
        public static DistributionVerticalModeTree New(DistributionVerticalMode defaultValue)
        {
            return new DistributionVerticalModeTree(defaultValue);
        }

        /*public static LimbBreakUVMappingStyleTree New(LimbBreakUVMappingStyle defaultValue)
        {
            return new LimbBreakUVMappingStyleTree(defaultValue);
        }*/

        public static Vector2Tree v2(float x, float y)
        {
            return new Vector2Tree(new Vector2(x, y));
        }

        public static Vector3Tree v3(float x, float y, float z)
        {
            return new Vector3Tree(new Vector3(x, y, z));
        }

        public static Vector4Tree v4(float x, float y, float z, float w)
        {
            return new Vector4Tree(new Vector4(x, y, z, w));
        }

        public static UVScaleTree uv(int x, int y)
        {
            return new UVScaleTree(new UVScale(x, y));
        }

        public static floatCurveTree fCurve(float initial)
        {
            return new floatCurveTree(new floatCurve(initial));
        }

        public static floatCurveTree fCurve(float initial, float curveStart, float curveEnd)
        {
            return new floatCurveTree(new floatCurve(initial, curveStart, curveEnd));
        }

        public static floatCurveTree fCurve(
            float initial,
            float curveStart,
            float curve50,
            float curveEnd)
        {
            return new floatCurveTree(
                new floatCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.50f, curve50),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static floatCurveTree fCurve(
            float initial,
            float curveStart,
            float curve33,
            float curve66,
            float curveEnd)
        {
            return new floatCurveTree(
                new floatCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.33f, curve33),
                    new Keyframe(0.66f, curve66),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static floatCurveTree fCurve(
            float initial,
            float curveStart,
            float curve25,
            float curve50,
            float curve75,
            float curveEnd)
        {
            return new floatCurveTree(
                new floatCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.25f, curve25),
                    new Keyframe(0.50f, curve50),
                    new Keyframe(0.75f, curve75),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static floatCurveTree fCurve(
            float initial,
            float curveStart,
            float curve20,
            float curve40,
            float curve60,
            float curve80,
            float curveEnd)
        {
            return new floatCurveTree(
                new floatCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.20f, curve20),
                    new Keyframe(0.40f, curve40),
                    new Keyframe(0.60f, curve60),
                    new Keyframe(0.80f, curve80),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static floatCurveTree fCurve(float initial, Keyframe start, Keyframe end)
        {
            return new floatCurveTree(new floatCurve(initial, start, end));
        }

        public static floatCurveTree fCurve(float initial, AnimationCurve curve)
        {
            return new floatCurveTree(new floatCurve(initial, curve));
        }

        public static intCurveTree intCurve(int initial)
        {
            return new intCurveTree(new intCurve(initial));
        }

        public static intCurveTree intCurve(int initial, float curveStart, float curveEnd)
        {
            return new intCurveTree(new intCurve(initial, curveStart, curveEnd));
        }

        public static intCurveTree intCurve(
            int initial,
            float curveStart,
            float curve50,
            float curveEnd)
        {
            return new intCurveTree(
                new intCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.50f, curve50),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static intCurveTree intCurve(
            int initial,
            float curveStart,
            float curve33,
            float curve66,
            float curveEnd)
        {
            return new intCurveTree(
                new intCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.33f, curve33),
                    new Keyframe(0.66f, curve66),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static intCurveTree intCurve(
            int initial,
            float curveStart,
            float curve25,
            float curve50,
            float curve75,
            float curveEnd)
        {
            return new intCurveTree(
                new intCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.25f, curve25),
                    new Keyframe(0.50f, curve50),
                    new Keyframe(0.75f, curve75),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static intCurveTree intCurve(
            int initial,
            float curveStart,
            float curve20,
            float curve40,
            float curve60,
            float curve80,
            float curveEnd)
        {
            return new intCurveTree(
                new intCurve(
                    initial,
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.20f, curve20),
                    new Keyframe(0.40f, curve40),
                    new Keyframe(0.60f, curve60),
                    new Keyframe(0.80f, curve80),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static intCurveTree intCurve(int initial, Keyframe start, Keyframe end)
        {
            return new intCurveTree(new intCurve(initial, start, end));
        }

        public static intCurveTree intCurve(int initial, AnimationCurve curve)
        {
            return new intCurveTree(new intCurve(initial, curve));
        }

        public static AnimationCurveTree Curve(float curveStart, float curveEnd)
        {
            return new AnimationCurveTree(
                new AnimationCurve(new Keyframe(0f, curveStart), new Keyframe(1f, curveEnd))
            );
        }

        public static AnimationCurveTree Curve(float curveStart, float curve50, float curveEnd)
        {
            return new AnimationCurveTree(
                new AnimationCurve(
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.50f, curve50),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static AnimationCurveTree Curve(
            float curveStart,
            float curve33,
            float curve66,
            float curveEnd)
        {
            return new AnimationCurveTree(
                new AnimationCurve(
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.33f, curve33),
                    new Keyframe(0.66f, curve66),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static AnimationCurveTree Curve(
            float curveStart,
            float curve25,
            float curve50,
            float curve75,
            float curveEnd)
        {
            return new AnimationCurveTree(
                new AnimationCurve(
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.25f, curve25),
                    new Keyframe(0.50f, curve50),
                    new Keyframe(0.75f, curve75),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static AnimationCurveTree Curve(
            float curveStart,
            float curve20,
            float curve40,
            float curve60,
            float curve80,
            float curveEnd)
        {
            return new AnimationCurveTree(
                new AnimationCurve(
                    new Keyframe(0.00f, curveStart),
                    new Keyframe(0.20f, curve20),
                    new Keyframe(0.40f, curve40),
                    new Keyframe(0.60f, curve60),
                    new Keyframe(0.80f, curve80),
                    new Keyframe(1.00f, curveEnd)
                )
            );
        }

        public static AnimationCurveTree Curve(params Keyframe[] args)
        {
            return new AnimationCurveTree(new AnimationCurve(args));
        }

        protected TreeProperty() : base(ResponsiveSettingsType.Tree)
        {
        }

    }
}
