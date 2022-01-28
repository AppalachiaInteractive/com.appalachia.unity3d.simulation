using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class BranchSelection : TSESelection<BranchDataContainer, BranchDataContainerSelection>
    {
        static BranchSelection()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<BranchDataContainerSelection>().IsAvailableThen( i => _branchDataContainerSelection = i);
        }

        #region Static Fields and Autoproperties

        private static BranchDataContainerSelection _branchDataContainerSelection;

        #endregion

        public override BranchDataContainerSelection selection
        {
            get
            {
                if (_selection == null)
                {
                    _selection = _branchDataContainerSelection;
                }

                return _selection;
            }
        }
    }
}
