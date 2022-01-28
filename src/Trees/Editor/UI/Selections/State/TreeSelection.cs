using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class TreeSelection : TSESelection<TreeDataContainer, TreeDataContainerSelection>
    {
        static TreeSelection()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<TreeDataContainerSelection>().IsAvailableThen( i => _treeDataContainerSelection = i);
        }

        #region Static Fields and Autoproperties

        private static TreeDataContainerSelection _treeDataContainerSelection;

        #endregion

        #region Fields and Autoproperties

        public AgeType age;
        public StageType stage;

        #endregion

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
