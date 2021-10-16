using Appalachia.Simulation.Trees.UI.Selections.State;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    public abstract class BranchContextEditorSidebarMenuContainer<T> : TreeEditorSidebarMenuContainer<T>
        where T : class
    {
        protected BranchDataContainer branch => TreeSpeciesEditorSelection.instance.branch.selection.selected;

        protected BranchContextEditorSidebarMenuContainer(OdinMenuStyle menuStyle, OdinMenuTreeDrawingConfig menuConfig, Color menuBackgroundColor, string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }
    }
}