using System;
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

        [SerializeField] private string path;
        [SerializeField] private bool _current;
        
        public bool value => _current;

        public void Toggle()
        {
            SetValue(!_current);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            SetValue(_current);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            GetCachedValue();
        }

        private void SetValue(bool b)
        {
            _current = b;
            EditorPrefs.SetBool(path, _current);
            Menu.SetChecked(path, _current);
        }
        
        private void GetCachedValue()
        {
            _current = EditorPrefs.GetBool(path);
            Menu.SetChecked(path, _current);
        }
        
        public static implicit operator bool(CheckmarkMenuBoolean b) => b.value;
    }

}
