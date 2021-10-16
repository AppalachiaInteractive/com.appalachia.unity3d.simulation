using System;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class TreeSelection : TSESelection<TreeDataContainer, TreeDataContainerSelection>
    {
        public AgeType age;
        public StageType stage;

        public override TreeDataContainerSelection selection 
        {
            get
            {
                if (_selection == null)
                {
                    _selection = TreeDataContainerSelection.instance;
                }

                return _selection;
            }
            set { _selection = value; }
        }
    }
}