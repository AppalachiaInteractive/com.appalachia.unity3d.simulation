using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class TreeSettingsSelection : TSESelection<TreeDataContainer, TreeSettingsDataContainerSelection>
    {
        public override TreeSettingsDataContainerSelection selection 
        {
            get
            {
                if (_selection == null)
                {
                    _selection = TreeSettingsDataContainerSelection.instance;
                }

                return _selection;
            }
            set { _selection = value; }
        }
    }
}