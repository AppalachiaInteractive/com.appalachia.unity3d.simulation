using System;
using System.Diagnostics;
using Appalachia.Utility.Constants;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Utilities
{
    [Serializable]
    public class CheckmarkMenuBoolean : ISerializationCallbackReceiver
    {
        public CheckmarkMenuBoolean(string path, bool initial)
        {
            this.path = path;
        }

        #region Fields and Autoproperties

        [SerializeField] private bool _current;

        [SerializeField] private string path;

        #endregion

        public bool value => _current;

        [DebuggerStepThrough]
        public static implicit operator bool(CheckmarkMenuBoolean b)
        {
            return b.value;
        }

        public void Toggle()
        {
            SetValue(!_current);
        }

        private void GetCachedValue()
        {
            _current = EditorPrefs.GetBool(path);
            Menu.SetChecked(path, _current);
        }

        private void SetValue(bool b)
        {
            _current = b;
            EditorPrefs.SetBool(path, _current);
            Menu.SetChecked(path, _current);
        }

        #region ISerializationCallbackReceiver Members

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            using var scope = APPASERIALIZE.OnBeforeSerialize();
            SetValue(_current);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            using var scope = APPASERIALIZE.OnAfterDeserialize();
            GetCachedValue();
        }

        #endregion
    }
}
