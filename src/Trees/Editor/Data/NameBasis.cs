using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Serialization;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Data
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public class NameBasis : ResponsiveNestedAppalachiaObject<NameBasis>
    {
        #region Constants and Static Readonly

        private const string DEFAULT_NAME = "name_basis";

        #endregion

        #region Fields and Autoproperties

        [LabelText("Name"), LabelWidth(100)]
        [DelayedProperty, OnValueChanged(nameof(ValidateName))]
        [DisableIf(nameof(_locked))]
        public string nameBasis;

        [SerializeField, HideInInspector]
        private string _friendlyName;

        [SerializeField, HideInInspector]
        private string _safeName;

        [SerializeField, HideInInspector]
        private bool _locked;

        #endregion

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

        /// <inheritdoc />
        protected override string DefaultName => DEFAULT_NAME;

        /// <inheritdoc />
        public override void InitializeFromParent(AppalachiaObject parent)
        {
        }

        /// <inheritdoc />
        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
        }

        public string FileNameAgeSO(int individualID, AgeType age, string prefix = null)
        {
            var p = prefix == null ? string.Empty : ZString.Format("{0}_", prefix);

            return ZString.Format("{0}{1}_{2:00}_{3}", p, safeName, individualID, age)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameAssetSO(int individualID, AgeType age, StageType stage)
        {
            return ZString.Format("{0}_{1:00}_{2}_{3}_asset", safeName, individualID, age, stage)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameBranchIndividualSO(NameBasis branchName)
        {
            return ZString.Format("{0}_{1}", safeName, branchName.safeName);
        }

        public string FileNameIndividualMetadataSO(int individualID)
        {
            return ZString.Format("{0}_{1:00}_individual-metadata", safeName, individualID)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameIndividualSO(int individualID)
        {
            return ZString.Format("{0}_{1:00}", safeName, individualID)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameLogAssetSO(int logID)
        {
            return ZString.Format("{0}_{1:00}_asset", safeName, logID)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameLogSO(int logID)
        {
            return ZString.Format("{0}_log-{1:00}", safeName, logID)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameSO(string value)
        {
            return ZString.Format("{0}_{1}", safeName, value.ToSafe())
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public string FileNameStageSO(int individualID, AgeType age, StageType stage)
        {
            return ZString.Format("{0}_{1:00}_{2}_{3}", safeName, individualID, age, stage)
                          .ToLowerInvariant()
                          .TrimEnd(',', '.', '_', '-');
        }

        public void Lock()
        {
            if (!_locked)
            {
                _locked = true;
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
    }
}
