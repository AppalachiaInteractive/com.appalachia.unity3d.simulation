using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Serialization;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Data
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public class NameBasis : ResponsiveNestedAppalachiaObject<NameBasis, TSEDataContainer>
    {
        [LabelText("Name"), LabelWidth(100)]
        [DelayedProperty, OnValueChanged(nameof(ValidateName))]
        [DisableIf(nameof(_locked))]
        public string nameBasis;

        [SerializeField, HideInInspector] private string _friendlyName;

        public string friendlyName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_friendlyName))
                {
                    ValidateName();
                }
                
                return _friendlyName;
            }
        }

        [SerializeField, HideInInspector] private string _safeName;

        [SerializeField, HideInInspector]
        private bool _locked;

        public string safeName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_safeName))
                {
                    ValidateName();
                }
                
                return _safeName;
            }
        }

        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(nameBasis))
            {
                nameBasis = null;
                _friendlyName = null;
                _safeName = null;
                return;
            }

            _friendlyName = nameBasis.ToFriendly();
            _safeName = nameBasis.ToSafe().ToLowerInvariant();
            name = DEFAULT_NAME;
        }

        public string FileNameSO(string value)
        {
            return $"{safeName}_{value.ToSafe()}".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }
        
        public string FileNameBranchIndividualSO(NameBasis branchName)
        {
            return $"{safeName}_{branchName.safeName}";
        }

        public string FileNameLogSO(int logID)
        {
            return $"{safeName}_log-{logID:00}".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }

        public string FileNameIndividualSO(int individualID)
        {
            return $"{safeName}_{individualID:00}".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }

        public string FileNameAgeSO(int individualID, AgeType age, string prefix = null)
        {
            var p = prefix == null ? string.Empty : $"{prefix}_";
            
            return $"{p}{safeName}_{individualID:00}_{age}".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }
        
        public string FileNameStageSO(int individualID, AgeType age, StageType stage)
        {
            return $"{safeName}_{individualID:00}_{age}_{stage}".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }
        
        public string FileNameAssetSO(int individualID, AgeType age, StageType stage)
        {
            return $"{safeName}_{individualID:00}_{age}_{stage}_asset".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }        
        
        public string FileNameIndividualMetadataSO(int individualID)
        {
            return $"{safeName}_{individualID:00}_individual-metadata".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }
        
        public string FileNameLogAssetSO(int logID)
        {
            return $"{safeName}_{logID:00}_asset".ToLowerInvariant().TrimEnd(',', '.', '_', '-');
        }
        
        public void Lock()
        {
            if (!_locked)
            {
                _locked = true;
            }
        }

        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
        }

        public override void Initialize(TSEDataContainer parent)
        {
        }

        private const string DEFAULT_NAME = "name_basis";
        
        protected override string DefaultName => DEFAULT_NAME;
    }
}
