using System;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class TreeSelection : TSESelection<TreeDataContainer, TreeDataContainerSelection>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static TreeSelection()
        {
            TreeDataContainerSelection.InstanceAvailable += i => _treeDataContainerSelection = i;
        }

        private static TreeDataContainerSelection _treeDataContainerSelection;
        public AgeType age;
        public StageType stage;

        public override TreeDataContainerSelection selection 
        {
            get
            {
                if (_selection == null)
                {
                    _selection = _treeDataContainerSelection;
                }

                return _selection;
            }
        }
    }
}