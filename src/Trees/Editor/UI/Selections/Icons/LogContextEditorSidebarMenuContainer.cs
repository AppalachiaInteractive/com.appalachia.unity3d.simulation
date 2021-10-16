using Appalachia.Simulation.Trees.UI.Selections.State;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    public abstract class LogContextEditorSidebarMenuContainer<T> : TreeEditorSidebarMenuContainer<T>
        where T : class
    {
        protected LogDataContainer log => TreeSpeciesEditorSelection.instance.log.selection.selected;

        protected LogContextEditorSidebarMenuContainer(OdinMenuStyle menuStyle, OdinMenuTreeDrawingConfig menuConfig, Color menuBackgroundColor, string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }
    }
}