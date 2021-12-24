using System;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class
        BranchSettingsSelection : TSESelection<BranchDataContainer, BranchSettingsDataContainerSelection>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static BranchSettingsSelection()
        {
            BranchSettingsDataContainerSelection.InstanceAvailable +=
                i => _branchSettingsDataContainerSelection = i;
        }

        #region Static Fields and Autoproperties

        private static BranchSettingsDataContainerSelection _branchSettingsDataContainerSelection;

        #endregion

        public override BranchSettingsDataContainerSelection selection =>
            _branchSettingsDataContainerSelection;
    }
}
