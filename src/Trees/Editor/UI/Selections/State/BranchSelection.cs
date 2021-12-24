using System;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class BranchSelection : TSESelection<BranchDataContainer, BranchDataContainerSelection>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static BranchSelection()
        {
            BranchDataContainerSelection.InstanceAvailable += i => _branchDataContainerSelection = i;
        }

        private static BranchDataContainerSelection _branchDataContainerSelection;
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