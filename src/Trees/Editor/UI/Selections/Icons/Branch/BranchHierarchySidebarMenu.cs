using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.UI.Graph;
using Appalachia.Simulation.Trees.UI.GUI;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Branch
{
    public class BranchHierarchySidebarMenu : BranchContextEditorSidebarMenuContainer<HierarchyData>
    {
        private TreeGraphNode root;
        private Dictionary<HierarchyData, string> nodeNames;

        protected override IList<HierarchyData> GetMenuItems()
        {
            root = GetMenuForHierarchies();

            nodeNames = new Dictionary<HierarchyData, string>();

            root.Recursive(
                node =>
                {
                    if (node.data == null)
                    {
                        return;
                    }

                    if (node.parent == null)
                    {
                        nodeNames.Add(node.data, $"{node.data.type}_{node.data.hierarchyID}");
                    }
                    else
                    {
                        nodeNames.Add(
                            node.data,
                            $"{nodeNames[node.parent.data]}_" + $"{node.data.type}_{node.data.hierarchyID}"
                        );
                    }
                },
                false
            );

            return branch.branch.hierarchies.ToList();
        }

        protected override string GetMenuName(HierarchyData menuItem)
        {
            return nodeNames[menuItem];
        }

        protected override TreeIcon GetMenuIcon(HierarchyData menuItem)
        {
            return menuItem.GetIcon(!menuItem.hidden);
        }

        private TreeGraphNode GetMenuForHierarchies()
        {
            var lookup = new Dictionary<int, TreeGraphNode>();
            var r = new TreeGraphNode(null, null);

            foreach (var data in branch.branch)
            {
                TreeGraphNode newNode;

                if (data.parentHierarchyID == -1)
                {
                    newNode = new TreeGraphNode(data, null);
                    r.children.Add(newNode);
                }
                else
                {
                    var parentNode = lookup[data.parentHierarchyID];
                    newNode = new TreeGraphNode(data, parentNode);
                }

                lookup.Add(data.hierarchyID, newNode);
            }

            return r;
        }

        protected override bool HasAdditionalToolbar => true;

        protected override void DrawAdditionalToolbar(float width, float height)
        {
            var branchCreateHierarchyAction = new Action<TreeComponentType>(
                (type) =>
                {
                    branch.RecordUndo(TreeEditMode.CreateHierarchy);

                    HierarchyData h;

                    if (type == TreeComponentType.Trunk)
                    {
                        h = branch.branch.hierarchies.CreateTrunkHierarchy(branch.materials.inputMaterialCache);
                    }
                    else
                    {
                        h = branch.branch.hierarchies.CreateHierarchy(
                            type,
                            Selected,
                            branch.materials.inputMaterialCache
                        );
                    }

                    SeedManager.UpdateSeeds(branch.branch);
                    h.seed.value = UnityEngine.Random.Range(1, BaseSeed.HIGH_ELEMENT);

                    branch.dataState = TSEDataContainer.DataState.Dirty;

                    var parent = branch.branch.hierarchies.GetHierarchyData(h.parentHierarchyID);

                    if (parent != null)
                    {
                        if (h.type == parent.type)
                        {
                            h.CopyGenerationSettings(parent);
                        }
                        else
                        {
                            var siblings = branch.branch.hierarchies.GetHierarchiesByParent(parent.hierarchyID)
                                .Where(ho => ho.hierarchyID != h.hierarchyID);

                            var found = false;
                            foreach (var sibling in siblings)
                            {
                                if (sibling.type == h.type)
                                {
                                    h.CopyGenerationSettings(sibling);
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                foreach (var sibling in siblings)
                                {
                                    var siblings2 = branch.branch.hierarchies
                                        .GetHierarchiesByParent(sibling.hierarchyID)
                                        .Where(ho => ho.hierarchyID != h.hierarchyID);

                                    foreach (var sibling2 in siblings2)
                                    {
                                        if (sibling2.type == h.type)
                                        {
                                            h.CopyGenerationSettings(sibling2);
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (found)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
                }
            );

            var addTrunkEnabled = HasSelection &&
                (branch != null) &&
                (!HasSelection || (Selected.type == TreeComponentType.Trunk));

            var addBranchEnabled = HasSelection &&
                (branch != null) &&
                HasSelection &&
                ((Selected.type == TreeComponentType.Trunk) || (Selected.type == TreeComponentType.Branch));

            var addLeavesEnabled = HasSelection &&
                (branch != null) &&
                HasSelection &&
                ((Selected.type == TreeComponentType.Trunk) || (Selected.type == TreeComponentType.Branch));

            var addFruitEnabled = HasSelection &&
                (branch != null) &&
                HasSelection &&
                ((Selected.type == TreeComponentType.Trunk) || (Selected.type == TreeComponentType.Branch));


            using (TreeGUI.Layout.Vertical())
            {
                

                using (TreeGUI.Enabled.If(HasSelection))
                {
                    if ((Selected != null) && GUILayout.Button(
                        Selected.hidden
                            ? TreeIcons.disabledVisible.Get("Show")
                            : TreeIcons.visible.Get("Hide"),
                        TreeGUI.Styles.Button,
                        TreeGUI.Layout.Options.MaxWidth(width).Height(height)
                    ))
                    {
                        Selected.hidden = !Selected.hidden;
                        branch.dataState = TSEDataContainer.DataState.Dirty;
                        BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
                        branch.RecordUndo(TreeEditMode.UpdateHierarchy);

                        RepopulateMenus(true, true);
                    }
                }
                EditorGUILayout.Space();
                
                using (TreeGUI.Layout.Horizontal())
                {
                    using (TreeGUI.Enabled.If(addTrunkEnabled))
                    {
                        if (GUILayout.Button(
                            TreeIcons.addTrunk.Get("Add new trunk"),
                            TreeGUI.Styles.ButtonLeft,
                            TreeGUI.Layout.Options.MaxWidth(width / 2f).Height(height)
                        ))
                        {
                            branchCreateHierarchyAction(TreeComponentType.Trunk);
                        }
                    }

                    using (TreeGUI.Enabled.If(addBranchEnabled))
                    {
                        if (GUILayout.Button(
                            TreeIcons.addBranch.Get("Add new branch"),
                            TreeGUI.Styles.ButtonLeft,
                            TreeGUI.Layout.Options.MaxWidth(width / 2f).Height(height)
                        ))
                        {
                            branchCreateHierarchyAction(TreeComponentType.Branch);
                        }
                    }
                }

                using (TreeGUI.Layout.Horizontal())
                {

                    using (TreeGUI.Enabled.If(addLeavesEnabled))
                    {
                        if (GUILayout.Button(
                            TreeIcons.addLeaf.Get("Add new leaf"),
                            TreeGUI.Styles.ButtonLeft,
                            TreeGUI.Layout.Options.MaxWidth(width / 2f).Height(height)
                        ))
                        {
                            branchCreateHierarchyAction(TreeComponentType.Leaf);
                        }
                    }

                    using (TreeGUI.Enabled.If(addFruitEnabled))
                    {
                        if (GUILayout.Button(
                            TreeIcons.addFruit.Get("Add new fruit"),
                            TreeGUI.Styles.ButtonLeft,
                            TreeGUI.Layout.Options.MaxWidth(width / 2f).Height(height)
                        ))
                        {
                            branchCreateHierarchyAction(TreeComponentType.Fruit);
                        }
                    }
                }

                using (TreeGUI.Enabled.If(HasSelection))
                {
                    if (GUILayout.Button(
                        (HasSelection ? TreeIcons.x : TreeIcons.disabledX).Get("Remove"),
                        TreeGUI.Styles.Button,
                        TreeGUI.Layout.Options.MaxWidth(width).Height(height)
                    ))
                    {
                        branch.RecordUndo(TreeEditMode.DeleteHierarchy);
                        branch.branch.hierarchies.DeleteHierarchyChain(branch.branch.shapes, Selected.hierarchyID);
                        branch.dataState = TSEDataContainer.DataState.Dirty;
                        BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
                    }
                }
            }
        }


        public BranchHierarchySidebarMenu(
            OdinMenuStyle menuStyle,
            OdinMenuTreeDrawingConfig menuConfig,
            Color menuBackgroundColor,
            string menuTitle) : base(menuStyle, menuConfig, menuBackgroundColor, menuTitle)
        {
        }
    }
}
   