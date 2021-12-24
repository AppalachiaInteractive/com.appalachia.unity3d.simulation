using Appalachia.Core.Attributes;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    [CallStaticConstructorInEditor]
    public abstract class LogContextEditorSidebarMenuContainer<T> : TreeEditorSidebarMenuContainer<T>
        where T : class
    {
        protected LogDataContainer log => _treeSpeciesEditorSelection.log.selection.selected;

        protected LogContextEditorSidebarMenuContainer(OdinMenuStyle menuStyle, OdinMenuTreeDrawingConfig menuConfig, Color menuBackgroundColor, string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }
    }
}