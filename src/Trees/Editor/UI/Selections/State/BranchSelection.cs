using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class BranchSelection : TSESelection<BranchDataContainer, BranchDataContainerSelection>
    {
        public override BranchDataContainerSelection selection 
        {
            get
            {
                if (_selection == null)
                {
                    _selection = BranchDataContainerSelection.instance;
                }

                return _selection;
            }
            set
            {
                _selection = value;
            }
        }
    }
}