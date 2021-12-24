using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Snapshot;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Branch
{
    
    public class BranchSnapshotSidebarMenu : BranchContextEditorSidebarMenuContainer<BranchSnapshotParameters>
    {
        public BranchSnapshotSidebarMenu(OdinMenuStyle menuStyle, OdinMenuTreeDrawingConfig menuConfig, Color menuBackgroundColor, string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }

        protected override IList<BranchSnapshotParameters> GetMenuItems()
        {
            for (var i = branch.snapshots.Count - 1; i >= 0; i--)
            {
                if (branch.snapshots[i] == null)
                {
                    branch.snapshots.RemoveAt(i);
                }
            }
            
            return branch.snapshots;
        }

        protected override string GetMenuName(BranchSnapshotParameters menuItem)
        {
            if (menuItem.nameBasis == null)
            {
                menuItem.nameBasis = branch.branch.nameBasis;
            }
            
            return menuItem.name;
        }

        protected override TreeIcon GetMenuIcon(BranchSnapshotParameters menuItem)
        {
            return TreeIcons.branch2;
        }

        protected override bool HasAdditionalToolbar => true;

        protected override void DrawAdditionalToolbar(float width, float height)
        {
            using (TreeGUI.Layout.Horizontal())
            {
                TreeGUI.Button.Standard(
                    TreeIcons.plus,
                    "Add new snapshot",
                    () =>
                    {
                        var newName = ZString.Format(
                            "{0}_{1:yyyyMMddHHmmss}",
                            branch.branch.nameBasis.safeName,
                            DateTime.Now
                        );
                        
                        branch.subfolders.ResetEmptyPaths();
                        var folder = branch.subfolders.snapshots;

                        var snapshot = TreeGUI.Assets.CreateAndSaveInFolder<BranchSnapshotParameters>(folder, newName);

                        branch.snapshots.Add(snapshot);
                    },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options.MaxWidth(width / 3f).Height(height)
                );

                TreeGUI.Button.EnableDisable(
                    HasSelection,
                    TreeIcons.copy,
                    TreeIcons.disabledCopy,
                    "Duplicate snapshot",
                    () =>
                    {
                        var newName = ZString.Format(
                            "{0}_{1:yyyyMMddHHmmss}",
                            branch.branch.nameBasis.safeName,
                            DateTime.Now
                        );
                        var folder = branch.subfolders.snapshots;

                        var snapshot = TreeGUI.Assets.CreateAndSaveInFolder<BranchSnapshotParameters>(folder, newName);

                        branch.snapshots.Add(snapshot);

                        snapshot.CopySettings(Selected);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.MaxWidth(width / 3f).Height(height)
                );                

                TreeGUI.Button.EnableDisable(
                    HasSelection,
                    TreeIcons.x,
                    TreeIcons.disabledX,
                    "Delete snapshot",
                    () =>
                    {
                        branch.snapshots.Remove(Selected);
                        menu.Selection.Clear();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.MaxWidth(width / 3f).Height(height)
                );
            }
        }
    }
}
