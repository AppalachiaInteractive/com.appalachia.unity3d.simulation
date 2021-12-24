using System;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Core.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Model
{
    [Serializable]
    public class LogModelSelection : AppalachiaSimpleBase
    {
        #if UNITY_EDITOR
        
        private ILogDataProvider _container;
        [SerializeField] private ScriptableObject _containerSO;
        
        public ILogDataProvider container
        {
            get
            {
                if (_container == null)
                {
                    if (_containerSO == null)
                    {
                        return null;
                    }
                    
                    _container = _containerSO as ILogDataProvider;
                }

                return _container;
            }
            set
            {
                _container = value;
                _containerSO = value?.GetSerializable();
            }
        }

        private bool _enabled => container != null;
        
        [PropertyRange(0, nameof(maxIndividual))]
        public int instanceSelection;

        public int maxIndividual => (container == null ? 0 : container.GetLogCount() -1 );
        
        public bool HasIndividual =>
            container?.HasLog(instanceSelection) ?? false;

        public LogModelSelection(ILogDataProvider container)
        {
            _container = container;
            _containerSO = container.GetSerializable();
        }
        
        #endif
    }
}
