using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Root;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Branch
{
    [NonSerializable]
    public class BranchEditorSidebarCollection : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        private BranchSnapshotSidebarMenu _snapshotMenu;

        private BranchHierarchySidebarMenu _hierarchyMenu;

        #endregion

        public BranchHierarchySidebarMenu hierarchyMenu
        {
            get
            {
                if (_hierarchyMenu == null)
                {
                    _hierarchyMenu = new BranchHierarchySidebarMenu(
                        TreeGUI.MenuStyles.BranchEditorMenuStyle,
                        TreeGUI.MenuStyles.BranchEditorMenuConfig,
                        TreeGUI.Colors.BranchEditorMenuBackgroundColor,
                        "Select a hierarchy"
                    );
                }

                return _hierarchyMenu;
            }
        }

        public BranchSnapshotSidebarMenu snapshotMenu
        {
            get
            {
                if (_snapshotMenu == null)
                {
                    _snapshotMenu = new BranchSnapshotSidebarMenu(
                        TreeGUI.MenuStyles.BranchEditorMenuStyle,
                        TreeGUI.MenuStyles.BranchEditorMenuConfig,
                        TreeGUI.Colors.BranchEditorMenuBackgroundColor,
                        "Select a snapshot"
                    );
                }

                return _snapshotMenu;
            }
        }
    }
}
