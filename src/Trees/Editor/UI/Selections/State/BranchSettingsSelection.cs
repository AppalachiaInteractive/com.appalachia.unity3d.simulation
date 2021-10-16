using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class BranchSettingsSelection : TSESelection<BranchDataContainer, BranchSettingsDataContainerSelection>
    {
        public override BranchSettingsDataContainerSelection selection 
        {
            get
            {
                if (_selection == null)
                {
                    _selection = BranchSettingsDataContainerSelection.instance;
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