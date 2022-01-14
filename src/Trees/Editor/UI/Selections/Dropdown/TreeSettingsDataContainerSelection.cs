using System;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;

namespace Appalachia.Simulation.Trees.UI.Selections.Dropdown
{
    [Serializable]
    public class TreeSettingsDataContainerSelection : TreeScriptableObjectContainerSelection<TreeDataContainer
        , TreeSettingsDataContainerSelection>
    {
        #region Fields and Autoproperties

        [TreeProperty] public bool enabled;

        #endregion
    }
}
