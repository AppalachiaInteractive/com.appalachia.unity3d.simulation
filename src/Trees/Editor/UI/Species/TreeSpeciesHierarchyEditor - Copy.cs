/*
using System;
using System.Linq;
using Appalachia.Simulation.Trees.Building;
using Appalachia.Simulation.Trees.Graph;
using Appalachia.Simulation.Trees.Metadata;
using Appalachia.Simulation.Trees.Metadata.AssetGeneration;
using Appalachia.Simulation.Trees.Metadata.Build;
using Appalachia.Simulation.Trees.Metadata.Data.Interfaces;
using Appalachia.Simulation.Trees.Metadata.Hierarchy;
using Appalachia.Simulation.Trees.Metadata.Icons;
using Appalachia.Simulation.Trees.Metadata.Individuals;
using Appalachia.Simulation.Trees.Metadata.ResponsiveUI;
using Appalachia.Simulation.Trees.Metadata.Seeds;
using Appalachia.Simulation.Trees.Runtime;
using Appalachia.Simulation.Trees.Runtime.Editing;
using Appalachia.Simulation.Trees.Runtime.Individuals.Types;
using Appalachia.Simulation.Trees.Runtime.Seeds;
using Appalachia.Simulation.Trees.Runtime.Serialization;
using Appalachia.Simulation.Trees.Runtime.Settings;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees
{
    public class TreeSpeciesHierarchyEditor
    {
        private PropertyTree hierarchyProperty;
        private HierarchyData lastHierarchySelection;
        private TreeGraphSettings settings;
        private Vector2 hierarchyScrollPosition;

        public HierarchyData current => TreeGraph.selectedHierarchy;

        public void Draw(
            TSEDataContainer so,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            float buttonHeight)
        {
            using (var vert = TreeGUI.Layout.Vertical())
            {
                if (hierarchies.trunks.Count == 0)
                {
                    var h =hierarchies.CreateTrunkHierarchy(tree.materials.inputMaterialCache);
                    
                    SeedManager.UpdateSeeds(tree.species);
                    h.seed.value = UnityEngine.Random.Range(1, BaseSeed.HIGH_ELEMENT);
                }

                foreach (var trunk in hierarchies.trunks)
                {
                    trunk.parentHierarchyID = -1;
                }

                DrawGraph(tree, stage);

                DrawHierarchyManagementToolbar(
                    tree, 
                    stage != null && stage.stageType == StageType.Normal,
                    buttonHeight,
                    vert.rect.width
                );

                DrawHierarchyData(tree);
            }
        }

        private void DrawGraph(
            TSEDataContainer so,
            ITreeStatistics statistics,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes)
        {
            using (var rect = TreeGUI.Layout.Horizontal())
            {
                if (settings == null)
                {
                    var setGuid = AssetDatabaseManager.FindAssets($"t: {typeof(TreeGraphSettings).Name}")
                        .FirstOrDefault();

                    if (setGuid != null)
                    {
                        settings =
                            AssetDatabaseManager.LoadAssetAtPath<TreeGraphSettings>(
                                AssetDatabaseManager.GUIDToAssetPath(setGuid)
                            );
                    }
                    else
                    {
                        settings = TreeGUI.Assets.CreateAndSave<TreeGraphSettings>();
                    }
                }

                 if (so.graphDirty)
                {
                    TreeGraph.Rebuild(hierarchies, shapes, TreeGraphSettings.instance);
                    so.graphDirty = false;
                }
                
                if (shapes != null)
                {
                    try
                    {
                        TreeGraph.Draw(
                            so,
                            hierarchies,
                            shapes,
                            TreeGraphSettings.instance,
                            statistics,
                            rect.rect
                        );
                    }
                    catch
                    {
                        TreeGraph.Rebuild(
                            hierarchies,
                            shapes, 
                            TreeGraphSettings.instance);
                        throw;
                    }
                    
                }
            }
        }

        private void DrawHierarchyManagementToolbar(
            TSEDataContainer so,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            bool buttonsEnabled,
            float buttonHeight,
            float buttonSpaceWidth)
        {
            var buttonSize = (buttonSpaceWidth - 18) / 10;
            using (TreeGUI.Layout.Horizontal(false))
            {
               var preAction = new Action(
                    () =>
                    {
                        so.RecordUndo(TreeEditMode.UpdateHierarchy);
                    }
                );
                
                var copyAction = new Action<HierarchyData, HierarchyData>(
                    (ho, hn) =>
                    {
                        so.graphDirty = true;
                        so.dataState = TSEDataContainer.DataState.Dirty;
                            
                        hn.CopyGenerationSettings(ho);                        
                        
                        TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
                        TreeGraph.Rebuild(hierarchies, shapes, TreeGraphSettings.instance);
                    }
                );
                
                 
                var postAction = new Action<HierarchyData>(
                    (h) =>
                    {
                        SeedManager.UpdateSeeds(so);
                        h.seed.value = UnityEngine.Random.Range(1, BaseSeed.HIGH_ELEMENT);
                        
                        so.graphDirty = true;
                        so.dataState = TSEDataContainer.DataState.Dirty;

                        var parent = hierarchies.GetHierarchyData(h.parentHierarchyID);

                        if (h.type == parent.type)
                        {
                            h.CopyGenerationSettings(parent);
                        }
                        else
                        {
                            var siblings = hierarchies.GetHierarchiesByParent(parent.hierarchyID)
                                .Where(ho => ho.hierarchyID != h.hierarchyID);

                            var found = false;
                            foreach(var sibling in siblings)
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
                                    var siblings2 = hierarchies.GetHierarchiesByParent(sibling.hierarchyID)
                            .Where(ho => ho.hierarchyID != h.hierarchyID);

                                    foreach(var sibling2 in siblings2)
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

                        if (so.settingsType == ResponsiveSettingsType.Tree)
                        {
                            TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);                            
                        }
                        else
                        {
                            LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
                        }
                        
                        TreeGraph.Rebuild(hierarchies, shapes, TreeGraphSettings.instance);
                    }
                );

                TreeGUI.Button.EnableDisable(
                    TreeGraph.selectedHierarchy != null && buttonsEnabled,
                    TreeIcons.copy,
                    TreeIcons.disabledCopy,
                    $"Duplicate {TreeGraph.selectedHierarchy?.type.ToString().ToLower()}",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeGraph.selectedHierarchy.type,
                            hierarchies.GetHierarchyData(TreeGraph.selectedHierarchy.parentHierarchyID),
                            tree.materials.inputMaterialCache);
                        
                        
                        SeedManager.UpdateSeeds(so);
                        h.seed.value = UnityEngine.Random.Range(1, BaseSeed.HIGH_ELEMENT);
                        
                        copyAction(TreeGraph.selectedHierarchy, h);
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );
                
                EditorGUILayout.Space();
                
                TreeGUI.Button.EnableDisable(
                    TreeGraph.selectedHierarchy != null && buttonsEnabled,
                    TreeIcons.trash,
                    TreeIcons.disabledTrash,
                    "Remove hierarchy",
                    () =>
                    {
                        so.RecordUndo(TreeEditMode.DeleteHierarchy);

                        hierarchies.DeleteHierarchyChain(
                            tree.individuals,
                            TreeGraph.selectedHierarchy.hierarchyID
                        );
                        
                        so.graphDirty = true;
                        so.dataState = TSEDataContainer.DataState.Dirty;
                        
                        so.RebuildStructures();
                        TreeGraph.Rebuild(hierarchies, shapes, TreeGraphSettings.instance);
                        
                        TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
                    },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                EditorGUILayout.Space();
                
                TreeGUI.Button.EnableDisable(
                     !TreeGraph.internalRebuild,
                    TreeIcons.refresh,
                    TreeIcons.disabledRefresh,
                    "Rebuild graph",
                    () =>
                    {
                        TreeGraph.Rebuild(tree, TreeGraphSettings.instance);
                    },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );
                
                EditorGUILayout.Space();
                

                TreeGUI.Button.EnableDisable(
                    TreeGraph.selectedHierarchy == null && buttonsEnabled,
                    TreeIcons.addTrunk,
                    TreeIcons.disabledTrunk,
                    "Add new trunk",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateTrunkHierarchy(tree.materials.inputMaterialCache);
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) && TreeGraph.selectedHierarchy.SupportsRootChild && buttonsEnabled,
                    TreeIcons.addRoot,
                    TreeIcons.disabledRoot,
                    "Add new roots",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Root,
                            TreeGraph.selectedHierarchy,
                            tree.materials.inputMaterialCache
                        );
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) && TreeGraph.selectedHierarchy.SupportsKnotChild && buttonsEnabled,
                    TreeIcons.addKnot,
                    TreeIcons.disabledKnot,
                    "Add new knots",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Knot,
                            TreeGraph.selectedHierarchy,
                            tree.materials.inputMaterialCache
                        );
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsFungusChild && buttonsEnabled,
                    TreeIcons.addFungus,
                    TreeIcons.disabledFungus,
                    "Add new fungi",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Fungus,
                            TreeGraph.selectedHierarchy,
                            tree.materials.inputMaterialCache
                        );
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsBranchChild && buttonsEnabled,
                    TreeIcons.addBranch,
                    TreeIcons.disabledBranch,
                    "Add new branches",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Branch,
                            TreeGraph.selectedHierarchy,
                            tree.materials.inputMaterialCache
                        );
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) && TreeGraph.selectedHierarchy.SupportsLeafChild && buttonsEnabled,
                    TreeIcons.addLeaf,
                    TreeIcons.disabledLeaf,
                    "Add new leaves",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Leaf,
                            TreeGraph.selectedHierarchy,
                            tree.materials.inputMaterialCache
                        );
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) && TreeGraph.selectedHierarchy.SupportsFruitChild && buttonsEnabled,
                    TreeIcons.addFruit,
                    TreeIcons.disabledFruit,
                    "Add new fruit",
                    () =>
                    {
                        preAction();
                        
                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Fruit,
                            TreeGraph.selectedHierarchy,
                            tree.materials.inputMaterialCache
                        );
                        
                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.Height(buttonHeight).ExpandHeight(false).ExpandWidth()
                        .MaxWidth(buttonSize)
                );
            }
        }

        private void DrawHierarchyData(TreeDataContainer tree)
        {
            if ((hierarchyProperty == null) && (TreeGraph.selectedHierarchy == null))
            {
                return;
            }

            if (hierarchyProperty == null)
            {
                hierarchyProperty = PropertyTree.Create(new TrunkHierarchyData(0, ResponsiveSettingsType.Tree));
            }

            if (lastHierarchySelection != TreeGraph.selectedHierarchy)
            {
                if (TreeGraph.selectedHierarchy != null)
                {
                    hierarchyProperty = PropertyTree.Create(TreeGraph.selectedHierarchy);
                    hierarchyProperty.OnPropertyValueChanged += (property, index) =>
                        tree.RecordUndo(TreeEditMode.UpdateHierarchy);

                    if (TreeGraph.selectedHierarchy.type == TreeComponentType.Branch ||
                        TreeGraph.selectedHierarchy.type == TreeComponentType.Root)
                    {
                        var bh = TreeGraph.selectedHierarchy as BarkHierarchyData;
                        bh.geometry.relativeToParentAllowed = true;
                    }
                }
            }

            lastHierarchySelection = TreeGraph.selectedHierarchy;

            
            using (TreeGUI.Enabled.IfNotNull(TreeGraph.selectedHierarchy))
            {
                if (hierarchyProperty != null)
                {
                    hierarchyProperty.Draw(false);
                }
            }
        }
    }
}
*/
