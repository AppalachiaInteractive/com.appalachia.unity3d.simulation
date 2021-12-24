using System;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Interfaces;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Model
{
    [Serializable]
    public class ModelSelection : AppalachiaSimpleBase
    {
#if UNITY_EDITOR
        
        private ISpeciesDataProvider _container;
        [SerializeField] private ScriptableObject _containerSO;
        
        public ISpeciesDataProvider container
        {
            get
            {
                if (_container == null)
                {
                    if (_containerSO == null)
                    {
                        return null;
                    }
                    
                    _container = _containerSO as ISpeciesDataProvider;
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
        
        public int individualSelection;

        public int maxIndividual => container?.GetIndividualCount() ?? 0;

        public AgeType ageSelection;
        
        public StageType stageSelection;

        public bool HasIndividual =>
            container?.HasIndividual(individualSelection, ageSelection, stageSelection) ?? false;

        public ModelSelection(ISpeciesDataProvider container)
        {
            _container = container;
            _containerSO = container.GetSerializable();
        }
        
#endif
    }
}
