using System.Collections.Generic;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Icons;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Log
{
    public class LogInstanceSidebarMenu : LogContextEditorSidebarMenuContainer<LogInstance>
    {
        public LogInstanceSidebarMenu(
            OdinMenuStyle menuStyle,
            OdinMenuTreeDrawingConfig menuConfig,
            Color menuBackgroundColor,
            string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }

        /// <inheritdoc />
        protected override bool HasAdditionalToolbar => true;

        /// <inheritdoc />
        protected override void DrawAdditionalToolbar(float width, float height)
        {
            using (TreeGUI.Layout.Horizontal())
            {
                TreeGUI.Button.Standard(
                    TreeIcons.plus,
                    "Add new log",
                    () => { log.AddNewLog(); },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options.MaxWidth(width / 2f).Height(height)
                );

                TreeGUI.Button.EnableDisable(
                    HasSelection,
                    TreeIcons.x,
                    TreeIcons.disabledX,
                    "Delete log",
                    () =>
                    {
                        log.logInstances.Remove(Selected);
                        menu.Selection.Clear();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.MaxWidth(width / 2f).Height(height)
                );
            }
        }

        /// <inheritdoc />
        protected override TreeIcon GetMenuIcon(LogInstance menuItem)
        {
            return TreeIcons.knot;
        }

        /// <inheritdoc />
        protected override IList<LogInstance> GetMenuItems()
        {
            return log.logInstances;
        }

        /// <inheritdoc />
        protected override string GetMenuName(LogInstance menuItem)
        {
            return menuItem.GetMenuString();
        }
    }
}
