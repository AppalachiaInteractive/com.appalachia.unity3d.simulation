using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class TreeSettingsSelection : TSESelection<TreeDataContainer, TreeSettingsDataContainerSelection>
    {
        static TreeSettingsSelection()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<TreeSettingsDataContainerSelection>().IsAvailableThen(
                i => _treeSettingsDataContainerSelection = i);
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
