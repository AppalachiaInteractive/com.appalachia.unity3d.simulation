using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class
        BranchSettingsSelection : TSESelection<BranchDataContainer, BranchSettingsDataContainerSelection>
    {
        static BranchSettingsSelection()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<BranchSettingsDataContainerSelection>()
                                     .IsAvailableThen(i => _branchSettingsDataContainerSelection = i);
        }

        #region Static Fields and Autoproperties

        private static BranchSettingsDataContainerSelection _branchSettingsDataContainerSelection;

        #endregion

        /// <inheritdoc />
        public override BranchSettingsDataContainerSelection selection =>
            _branchSettingsDataContainerSelection;
    }
}
