#region

using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Simulation.Core.Metadata.Tree;
using Appalachia.Simulation.Trees.Core.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;



#endregion

namespace Appalachia.Simulation.Trees.Core
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class TreeRuntimeInstance : MonoBehaviour
    {
        [InlineEditor]
        [SmartLabel]
        public TreeSpeciesMetadata speciesMetadata;

        [InlineEditor]
        [SmartLabel]
        public TreeRuntimeInstanceMetadata metadata;

        [SmartLabel] public LODGroup lodGroup;

        [NonSerialized] private TreeAgeMetadata _ageMetadata;

        //public Rigidbody rigidbody;

        [NonSerialized] private TreeIndividualMetadata _individualMetadata;

        [NonSerialized] private TreeStageMetadata _stageMetadata;

        public TreeIndividualMetadata individualMetadata
        {
            get
            {
                if (_individualMetadata != null)
                {
                    return _individualMetadata;
                }

                _individualMetadata = speciesMetadata.GetIndividual(metadata.individualID);

                return _individualMetadata;
            }
        }

        public TreeAgeMetadata ageMetadata
        {
            get
            {
                if (_ageMetadata != null)
                {
                    return _ageMetadata;
                }

                if (individualMetadata != null)
                {
                    _ageMetadata = individualMetadata[metadata.age];
                }

                return _ageMetadata;
            }
        }

        public TreeStageMetadata stageMetadata
        {
            get
            {
                if (_stageMetadata != null)
                {
                    return _stageMetadata;
                }

                if (ageMetadata != null)
                {
                    _stageMetadata = ageMetadata[metadata.stage];
                }

                return _stageMetadata;
            }
        }

        public bool CanCut => (ageMetadata != null) && ageMetadata.CanCut(metadata.stage);

        public bool CanBare => (ageMetadata != null) && ageMetadata.CanBare(metadata.stage);

        public bool CanRot => (ageMetadata != null) && ageMetadata.CanRot(metadata.stage);

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private TreeModel _model;

        private bool _missingModel => (_model == null) || _model.MissingContainer;

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        [PropertyOrder(-100)]
        [DisableIf(nameof(_missingModel))]
        public void OpenSpecies()
        {
            _model.OpenSpecies();
        }

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        [PropertyOrder(-100)]
        [DisableIf(nameof(_missingModel))]
        public void SelectModel()
        {
            UnityEditor.Selection.objects = new Object[] {_model.GameObject};
        }

        private void Update()
        {
            if (_model == null)
            {
                _model = GetComponentInParent<TreeModel>();
            }
        }

        [Button]
        public void ClearModels()
        {
            var runtimes = FindObjectsOfType<TreeRuntimeInstance>();

            foreach (var runtime in runtimes)
            {
                runtime._model = null;
            }
        }
#endif
    }
}
