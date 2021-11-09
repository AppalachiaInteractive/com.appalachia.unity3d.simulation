using System;
using System.Diagnostics;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public abstract class TreeProperty<T> : TreeProperty, IElementCloner<T>
    {
        [ShowInInspector]
        [HideLabel] 
        [InlineProperty, HideReferenceObjectPicker]
        [HorizontalGroup]
        //[OnValueChanged(nameof(DistributionSettingsChanged))]
        public T accessor
        {
            get => GetActiveUIProperty();
            set
            {
                var activeUIAge = settingsType == ResponsiveSettingsType.Branch ? AgeType.Mature : _activeUIAge;

                if (activeUIAge != AgeType.Mature)
                {
                    toggler = true;
                }
                
                SetActiveUIValue(value);
            }
        }

        [HideInInspector]
        public T adult;

        [HideInInspector]
        public T mature;
        
        [HideInInspector]
        public T sapling;

        [HideInInspector]
        public T spirit;

        [HideInInspector]
        public T young;

        [HideInInspector]
        public T defaultValue;


        public T UIValue => GetActiveUIProperty();

        public T Value => GetActiveGenerationProperty();

        private T GetActiveUIProperty()
        {
            var activeUIAge = settingsType == ResponsiveSettingsType.Branch ? AgeType.Mature : _activeUIAge;

            return GetProperty(activeUIAge);
        }

        private T GetActiveGenerationProperty()
        {
            return GetProperty(_activeGenerationAge);
        }

        private T GetProperty(AgeType age)
        {
            switch (age)
            {
                case AgeType.Sapling:
                    return overrideSapling ? sapling : defaultValue;
                case AgeType.Young:
                    return overrideYoung ? young : defaultValue;
                case AgeType.Adult:
                    return overrideAdult ? adult : defaultValue;
                case AgeType.Mature:
                    return overrideMature ? mature : defaultValue;
                case AgeType.Spirit:
                    return overrideSpirit ? spirit : defaultValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public T GetValue(AgeType age) => GetProperty(age);

        public void SetActiveUIValue(T val)
        {
            var activeUIAge = settingsType == ResponsiveSettingsType.Branch ? AgeType.Mature : _activeUIAge;
            SetValue(val, activeUIAge);
        }


        public void SetValue(T v, AgeType age)
        {
            switch (age)
            {
                case AgeType.Sapling:
                    if (overrideSapling)
                    {
                        sapling = v;
                    }
                    else
                    {
                        SetValue(v);
                    }
                    break;
                case AgeType.Young:
                    if (overrideYoung)
                    {
                        young = v;
                    }
                    else
                    {
                        SetValue(v);
                    }
                    break;
                case AgeType.Adult:
                    if (overrideAdult)
                    {
                        adult = v;
                    }
                    else
                    {
                        SetValue(v);
                    }
                    break;
                case AgeType.Mature:
                    if (overrideMature)
                    {
                        mature = v;
                    }
                    else
                    {
                        SetValue(v);
                    }
                    break;
                case AgeType.Spirit:
                    if (overrideSpirit)
                    {
                        spirit = v;
                    }
                    else
                    {
                        SetValue(v);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(age), age, null);
            }
        }
        
        public void SetValue(T v)
        {
            if (!overrideSapling) sapling = v;
            if (!overrideYoung) young = v;
            if (!overrideAdult) adult = v;
            if (!overrideMature) mature = v;
            if (!overrideSpirit) spirit = v;

            defaultValue = v;
        }

        [DebuggerStepThrough] public static implicit operator T(TreeProperty<T> t) => t.Value;

        protected TreeProperty(T defaultValue) 
        {
            sapling = defaultValue;
            young = defaultValue;
            adult = defaultValue;
            mature = defaultValue;
            spirit = defaultValue;
            this.defaultValue = defaultValue;
        }

        public abstract T CloneElement(T model);

        protected void Clone(TreeProperty<T> clone)
        {
            clone.sapling = sapling;
            clone.young = young;
            clone.adult = adult;
            clone.mature = mature;
            clone.spirit = spirit;

            clone.overrideSapling = overrideSapling;
            clone.overrideYoung = overrideYoung;
            clone.overrideAdult = overrideAdult;
            clone.overrideMature = overrideMature;
            clone.overrideSpirit = overrideSpirit;
        }

        public void CheckInitialization(T dv)
        {
            if (!initialized)
            {
                initialized = true;
                
                defaultValue = dv;
                sapling = dv;
                young = dv;
                adult = dv;
                mature = dv;
                spirit = dv;
            }
        }
    }
}