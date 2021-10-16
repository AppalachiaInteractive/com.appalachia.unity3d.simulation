using Appalachia.Simulation.Trees.UI.GUI;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Log
{
    public class LogEditorSidebarCollection
    {
        private LogInstanceSidebarMenu _instanceMenu;

        public LogInstanceSidebarMenu instanceMenu
        {
            get
            {
                if (_instanceMenu == null)
                {
                    _instanceMenu = new LogInstanceSidebarMenu(
                        TreeGUI.MenuStyles.LogEditorSidebarMenuStyle,
                        TreeGUI.MenuStyles.LogEditorSidebarMenuConfig,
                        TreeGUI.Colors.LogEditorMenuBackgroundColor,
                        "Select a log"
                    );
                }

                return _instanceMenu;
            }
        }
        
    }
}
