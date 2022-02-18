using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Root;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Log
{
    [NonSerializable]
    public class LogEditorSidebarCollection : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        private LogInstanceSidebarMenu _instanceMenu;

        #endregion

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
