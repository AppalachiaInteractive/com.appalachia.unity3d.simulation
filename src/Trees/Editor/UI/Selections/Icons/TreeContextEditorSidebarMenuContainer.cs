using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    public abstract class TreeContextEditorSidebarMenuContainer<T> : TreeEditorSidebarMenuContainer<T>
        where T : class
    {
        protected TreeDataContainer tree => _treeSpeciesEditorSelection.tree.selection.selected;

        protected TreeContextEditorSidebarMenuContainer(OdinMenuStyle menuStyle, OdinMenuTreeDrawingConfig menuConfig, Color menuBackgroundColor, string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }
    }
}