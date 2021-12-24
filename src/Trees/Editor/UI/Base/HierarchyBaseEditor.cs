using System;
using System.Linq;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.UI.Graph;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Base
{
    [CallStaticConstructorInEditor]
    public abstract class HierarchyBaseEditor<T> : IDisposable
        where T : TSEDataContainer
    {
        static HierarchyBaseEditor()
        {
            TreeGraphSettings.InstanceAvailable += i => _treeGraphSettings = i;
        }

        #region Static Fields and Autoproperties

        protected static TreeGraphSettings _treeGraphSettings;

        #endregion

        #region Fields and Autoproperties

        private HierarchyData lastHierarchySelection;

        private PropertyTree hierarchyProperty;
        private TreeGraphSettings settings;
        private Vector2 hierarchyScrollPosition;

        #endregion

        public HierarchyData current => TreeGraph.selectedHierarchy;

        public void DrawAll(
            T so,
            ITreeStatistics statistics,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            float buttonHeight)
        {
            using (var vert = TreeGUI.Layout.Vertical())
            {
                PrepareDrawing(so, hierarchies);

                DrawGraph(so, statistics, hierarchies, shapes);

                DrawHierarchyManagementToolbar(so, hierarchies, shapes, buttonHeight, vert.rect.width);

                TreeGUI.Draw.Space(1, true);

                DrawHierarchyData(so);
            }
        }

        public void DrawGraph(
            T so,
            ITreeStatistics statistics,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes)
        {
            using (var rect = TreeGUI.Layout.Horizontal())
            {
                var selection = GetSelection();

                if (so.graphDirty)
                {
                    TreeGraph.Rebuild(hierarchies, shapes, selection, _treeGraphSettings);
                    so.graphDirty = false;
                }

                if (shapes != null)
                {
                    try
                    {
                        TreeGraph.Draw(
                            so,
                            selection,
                            hierarchies,
                            shapes,
                            _treeGraphSettings,
                            statistics,
                            rect.rect
                        );
                    }
                    catch
                    {
                        TreeGraph.Rebuild(hierarchies, shapes, selection, _treeGraphSettings);
                        throw;
                    }
                }
            }
        }

        public void DrawHierarchyData(T so)
        {
            if ((hierarchyProperty == null) && (TreeGraph.selectedHierarchy == null))
            {
                return;
            }

            if (hierarchyProperty == null)
            {
                hierarchyProperty = PropertyTree.Create(new TrunkHierarchyData(0, so.settingsType));
            }

            if (lastHierarchySelection != TreeGraph.selectedHierarchy)
            {
                if (TreeGraph.selectedHierarchy != null)
                {
                    hierarchyProperty = PropertyTree.Create(TreeGraph.selectedHierarchy);
                    hierarchyProperty.OnPropertyValueChanged += (property, index) =>
                        so.RecordUndo(TreeEditMode.UpdateHierarchy);

                    if ((TreeGraph.selectedHierarchy.type == TreeComponentType.Branch) ||
                        (TreeGraph.selectedHierarchy.type == TreeComponentType.Root))
                    {
                        var bh = TreeGraph.selectedHierarchy as BarkHierarchyData;

                        bh.geometry.relativeToParentAllowed = true;
                    }
                }

                so.UpdateSettingsType(so.settingsType);
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

        public void DrawHierarchyManagementToolbar(
            T so,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            float buttonHeight,
            float buttonSpaceWidth)
        {
            var buttonsEnabled = AreHierarchyButtonsEnabled();

            var selection = GetSelection();
            var buttonSize = (buttonSpaceWidth - 18) / 10;
            using (TreeGUI.Layout.Horizontal(false))
            {
                var preAction = new Action(() => { so.RecordUndo(TreeEditMode.UpdateHierarchy); });

                var copyAction = new Action<HierarchyData, HierarchyData>(
                    (ho, hn) =>
                    {
                        so.graphDirty = true;
                        so.dataState = TSEDataContainer.DataState.Dirty;

                        hn.CopyGenerationSettings(ho);

                        SettingsChanged();

                        so.UpdateSettingsType(so.settingsType);
                        TreeGraph.Rebuild(hierarchies, shapes, selection, _treeGraphSettings);
                    }
                );

                var postAction = new Action<HierarchyData>(
                    h =>
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
                                    var siblings2 = hierarchies.GetHierarchiesByParent(sibling.hierarchyID)
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

                        SettingsChanged();

                        so.UpdateSettingsType(so.settingsType);

                        TreeGraph.Rebuild(hierarchies, shapes, selection, _treeGraphSettings);
                    }
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) && buttonsEnabled,
                    TreeIcons.copy,
                    TreeIcons.disabledCopy,
                    ZString.Format("Duplicate {0}", TreeGraph.selectedHierarchy?.type.ToString().ToLower()),
                    () =>
                    {
                        preAction();

                        var h = hierarchies.CreateHierarchy(
                            TreeGraph.selectedHierarchy.type,
                            hierarchies.GetHierarchyData(TreeGraph.selectedHierarchy.parentHierarchyID),
                            null
                        );

                        SeedManager.UpdateSeeds(so);
                        h.seed.value = UnityEngine.Random.Range(1, BaseSeed.HIGH_ELEMENT);

                        copyAction(TreeGraph.selectedHierarchy, h);
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                EditorGUILayout.Space();

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) && buttonsEnabled,
                    TreeIcons.trash,
                    TreeIcons.disabledTrash,
                    "Remove hierarchy",
                    () =>
                    {
                        so.RecordUndo(TreeEditMode.DeleteHierarchy);

                        hierarchies.DeleteHierarchyChain(TreeGraph.selectedHierarchy.hierarchyID);

                        so.graphDirty = true;
                        so.dataState = TSEDataContainer.DataState.Dirty;

                        so.RebuildStructures();
                        TreeGraph.Rebuild(hierarchies, shapes, selection, _treeGraphSettings);

                        SettingsChanged();
                    },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                EditorGUILayout.Space();

                TreeGUI.Button.EnableDisable(
                    !TreeGraph.internalRebuild,
                    TreeIcons.refresh,
                    TreeIcons.disabledRefresh,
                    "Rebuild graph",
                    () => { TreeGraph.Rebuild(hierarchies, shapes, selection, _treeGraphSettings); },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                EditorGUILayout.Space();

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy == null) && buttonsEnabled,
                    TreeIcons.addTrunk,
                    TreeIcons.disabledTrunk,
                    "Add new trunk",
                    () =>
                    {
                        preAction();

                        var h = CreateTrunkHierarchy(hierarchies);

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsRootChild &&
                    buttonsEnabled,
                    TreeIcons.addRoot,
                    TreeIcons.disabledRoot,
                    "Add new roots",
                    () =>
                    {
                        preAction();

                        var h = CreateHierarchy(
                            hierarchies,
                            TreeComponentType.Root,
                            TreeGraph.selectedHierarchy
                        );

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsKnotChild &&
                    buttonsEnabled,
                    TreeIcons.addKnot,
                    TreeIcons.disabledKnot,
                    "Add new knots",
                    () =>
                    {
                        preAction();

                        var h = hierarchies.CreateHierarchy(
                            TreeComponentType.Knot,
                            TreeGraph.selectedHierarchy,
                            null
                        );

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsFungusChild &&
                    buttonsEnabled,
                    TreeIcons.addFungus,
                    TreeIcons.disabledFungus,
                    "Add new fungi",
                    () =>
                    {
                        preAction();

                        var h = CreateHierarchy(
                            hierarchies,
                            TreeComponentType.Fungus,
                            TreeGraph.selectedHierarchy
                        );

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsBranchChild &&
                    buttonsEnabled,
                    TreeIcons.addBranch,
                    TreeIcons.disabledBranch,
                    "Add new branches",
                    () =>
                    {
                        preAction();

                        var h = CreateHierarchy(
                            hierarchies,
                            TreeComponentType.Branch,
                            TreeGraph.selectedHierarchy
                        );

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsLeafChild &&
                    buttonsEnabled,
                    TreeIcons.addLeaf,
                    TreeIcons.disabledLeaf,
                    "Add new leaves",
                    () =>
                    {
                        preAction();

                        var h = CreateHierarchy(
                            hierarchies,
                            TreeComponentType.Leaf,
                            TreeGraph.selectedHierarchy
                        );

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );

                TreeGUI.Button.EnableDisable(
                    (TreeGraph.selectedHierarchy != null) &&
                    TreeGraph.selectedHierarchy.SupportsFruitChild &&
                    buttonsEnabled,
                    TreeIcons.addFruit,
                    TreeIcons.disabledFruit,
                    "Add new fruit",
                    () =>
                    {
                        preAction();

                        var h = CreateHierarchy(
                            hierarchies,
                            TreeComponentType.Fruit,
                            TreeGraph.selectedHierarchy
                        );

                        postAction(h);
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.Height(buttonHeight)
                           .ExpandHeight(false)
                           .ExpandWidth()
                           .MaxWidth(buttonSize)
                );
            }
        }

        public void PrepareDrawing(T so, IHierarchyReadWrite hierarchies)
        {
            var trunks = hierarchies.GetHierarchyCount(TreeComponentType.Trunk);
            if (trunks == 0)
            {
                var h = CreateTrunkHierarchy(hierarchies);

                SeedManager.UpdateSeeds(so);
                h.seed.value = UnityEngine.Random.Range(1, BaseSeed.HIGH_ELEMENT);
            }

            foreach (var trunk in hierarchies.GetHierarchies(TreeComponentType.Trunk))
            {
                trunk.parentHierarchyID = -1;
            }
        }

        protected abstract bool AreHierarchyButtonsEnabled();

        protected abstract HierarchyData CreateHierarchy(
            IHierarchyWrite hierarchies,
            TreeComponentType type,
            HierarchyData parent);

        protected abstract HierarchyData CreateTrunkHierarchy(IHierarchyWrite hierarchies);

        protected abstract IBasicSelection GetSelection();

        protected abstract void SettingsChanged();

        #region IDisposable Members

        public void Dispose()
        {
            hierarchyProperty?.Dispose();
        }

        #endregion
    }
}
