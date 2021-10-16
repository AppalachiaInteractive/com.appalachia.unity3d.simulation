using Appalachia.Simulation.Trees.UI.GUI;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Branch
{
    public class BranchEditorSidebarCollection
    {
        private BranchSnapshotSidebarMenu _snapshotMenu;

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
        
        private BranchHierarchySidebarMenu _hierarchyMenu;

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
        
    }
}
