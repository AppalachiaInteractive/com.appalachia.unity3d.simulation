using Appalachia.Core.Objects.Root;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Log
{
    public class LogEditorSidebarCollection : AppalachiaSimpleBase
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
