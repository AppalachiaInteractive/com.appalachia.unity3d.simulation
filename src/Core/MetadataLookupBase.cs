using System;
using System.Collections.Generic;
using Appalachia.Base.Scriptables;
using Appalachia.Core.Extensions;
using Appalachia.Editing.Assets;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Core
{
    public abstract class MetadataLookupBase<T,TValue> : SelfSavingSingletonScriptableObject<T>
        where T : MetadataLookupBase<T, TValue>
        where TValue : InternalScriptableObject<TValue>, ICategorizable
    {
        protected override void WhenEnabled()
        {
            if (defaultValue == null)
            {
                defaultValue = all_internal.FirstOrDefault_NoAlloc();
            }
        }

        [FormerlySerializedAs("generic")] 
        [FormerlySerializedAs("defaultWrapper")] 
        [FoldoutGroup("Default")] 
        public TValue defaultValue;
        
        [NonSerialized, HideInInspector]
        private List<TValue> _all;

        protected List<TValue> all_internal
        {
            get
            {
                if (_all != null && _all.Count > 0)
                {
                    return _all;
                }

                _all = new List<TValue>();

                PopulateAll(_all);                

                return _all;
            }
        }
        
        public IReadOnlyList<TValue> all
        {
            get
            {
                if (_all != null && _all.Count > 0)
                {
                    return _all;
                }

                _all = new List<TValue>();

                PopulateAll(_all);                

                return _all;
            }
        }
        
        protected void PopulateAll(List<TValue> values)
        {
#if UNITY_EDITOR
            var assets = AssetDatabaseHelper.FindAssets<TValue>();

            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                
                values.Add(asset);
            }
#endif
        }
    }
}