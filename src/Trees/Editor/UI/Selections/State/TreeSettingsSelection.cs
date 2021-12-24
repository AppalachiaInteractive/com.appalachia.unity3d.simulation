using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class TreeSettingsSelection : TSESelection<TreeDataContainer, TreeSettingsDataContainerSelection>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static TreeSettingsSelection()
        {
            TreeSettingsDataContainerSelection.InstanceAvailable +=
                i => _treeSettingsDataContainerSelection = i;
        }

        #region Static Fields and Autoproperties

        private static TreeSettingsDataContainerSelection _treeSettingsDataContainerSelection;

        #endregion

        public override TreeSettingsDataContainerSelection selection
        {
            get
            {
                if (_selection == null)
                {
                    _selection = _treeSettingsDataContainerSelection;
                }

                return _selection;
            }
        }
    }
}
