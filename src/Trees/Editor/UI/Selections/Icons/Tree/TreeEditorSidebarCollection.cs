using System.Linq;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Icons;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons.Tree
{
    public class TreeEditorSidebarCollection : AppalachiaSimpleBase
    {
        private bool _changing;
        private object _lastAgeSelection;
        private object _lastIndividualSelection;
        private object _lastStageSelection;
        private TreeEditorSidebarMenu individualMenu;
        private TreeEditorSidebarMenu ageMenu;
        private TreeEditorSidebarMenu stageMenu;

        public TreeIndividual SelectedIndividual => individualMenu?.Selection?.SelectedValue as TreeIndividual;

        public TreeAge SelectedAge => ageMenu?.Selection?.SelectedValue as TreeAge;

        public TreeStage SelectedStage => stageMenu?.Selection?.SelectedValue as TreeStage;

        public bool HasSelectedIndividual => individualMenu?.HasSelection ?? false;
        public bool HasSelectedAge => ageMenu?.HasSelection ?? false;
        public bool HasSelectedStage => stageMenu?.HasSelection ?? false;

        private void RecreateMenusIfNull()
        {
            if ((individualMenu?.MenuItems == null) || individualMenu.MenuItems.Any(m => m == null))
            {
                individualMenu = new TreeEditorSidebarMenu(
                    TreeGUI.MenuStyles.SidebarMenuStyle,
                    TreeGUI.MenuStyles.SidebarMenuConfig,
                    TreeGUI.Colors.SidebarMenuBackgroundColor
                );

                individualMenu.DefaultMenuStyle = TreeGUI.MenuStyles.SidebarMenuStyle;
                individualMenu.Selection.SelectionChanged += IndividualOnSelectionChanged;
            }

            if ((ageMenu?.MenuItems == null) || ageMenu.MenuItems.Any(m => m == null))
            {
                ageMenu = new TreeEditorSidebarMenu(
                    TreeGUI.MenuStyles.SidebarMenuStyle,
                    TreeGUI.MenuStyles.SidebarMenuConfig,
                    TreeGUI.Colors.SidebarMenuBackgroundColor
                );

                ageMenu.DefaultMenuStyle = TreeGUI.MenuStyles.SidebarMenuStyle;
                ageMenu.Selection.SelectionChanged += AgeOnSelectionChanged;
            }

            if ((stageMenu?.MenuItems == null) || stageMenu.MenuItems.Any(m => m == null))
            {
                stageMenu = new TreeEditorSidebarMenu(
                    TreeGUI.MenuStyles.SidebarMenuStyle,
                    TreeGUI.MenuStyles.SidebarMenuConfig,
                    TreeGUI.Colors.SidebarMenuBackgroundColor
                );

                stageMenu.DefaultMenuStyle = TreeGUI.MenuStyles.SidebarMenuStyle;
                stageMenu.Selection.SelectionChanged += StageOnSelectionChanged;
            }
        }

        private void ClearMenusIfInvalid(bool requested)
        {
            if (requested)
            {
                individualMenu.Selection.Clear();
                ageMenu.Selection.Clear();
                stageMenu.Selection.Clear();

                individualMenu.Clear();
                ageMenu.Clear();
                stageMenu.Clear();
            }

            if (individualMenu.Selection.Any(s => s == null))
            {
                individualMenu = null;
                ageMenu = null;
                stageMenu = null;

                return;
            }

            if (ageMenu.Selection.Any(s => s == null))
            {
                ageMenu = null;
                stageMenu = null;

                return;
            }

            if (HasSelectedIndividual &&
                ageMenu.MenuItems.Any(a => (a.Value as TreeAge).individualID != SelectedIndividual.individualID))
            {
                ageMenu = null;
                stageMenu = null;

                return;
            }

            if (stageMenu.Selection.Any(s => s == null))
            {
                stageMenu = null;

                return;
            }

            if ((stageMenu != null) &&
                HasSelectedIndividual &&
                stageMenu.MenuItems.Any(a => (a.Value as TreeStage).individualID != SelectedIndividual.individualID))
            {
                stageMenu = null;

                return;
            }


            if ((stageMenu != null) &&
                HasSelectedAge &&
                stageMenu.MenuItems.Any(a => (a.Value as TreeStage).ageType != SelectedAge.ageType))
            {
                stageMenu = null;
            }
        }

        public void RepopulateMenus(TreeDataContainer tree, bool force, bool clear)
        {
            if (force)
            {
                individualMenu = null;
                ageMenu = null;
                stageMenu = null;
            }

            if (Event.current.type == EventType.Layout)
            {
                RecreateMenusIfNull();
                ClearMenusIfInvalid(clear);
                RecreateMenusIfNull();
            }

            var individualCountValid = individualMenu.MenuItems.Count == tree.individuals.Count;

            if (!individualCountValid)
            {
                individualMenu.Selection.SelectionChanged -= IndividualOnSelectionChanged;

                var currentIndividualSelectionIndex = individualMenu.GetSelectedIndex();

                individualMenu.Clear();

                foreach (var individual in tree.individuals)
                {
                    individualMenu.Add(individual.GetMenuString(), individual, individual.GetIcon(true).icon);
                }

                currentIndividualSelectionIndex = Mathf.Max(
                    currentIndividualSelectionIndex,
                    individualMenu.MenuItems.Count - 1
                );

                if (currentIndividualSelectionIndex > 0)
                {
                    individualMenu.SelectByIndex(currentIndividualSelectionIndex);
                }

                individualMenu.SelectByIndex(currentIndividualSelectionIndex);

                individualMenu.Selection.SelectionChanged += IndividualOnSelectionChanged;
            }
            else if (individualMenu.MenuItems.Count == 0)
            {
                individualMenu.Selection.SelectionChanged -= IndividualOnSelectionChanged;

                foreach (var individual in tree.individuals)
                {
                    individualMenu.Add(individual.GetMenuString(), individual, individual.GetIcon(true).icon);
                }

                if (individualMenu.MenuItems.Count > 0)
                {
                    individualMenu.SelectFirst();
                }

                individualMenu.Selection.SelectionChanged += IndividualOnSelectionChanged;
            }

            var selectedIndividual = individualMenu.Selection.SelectedValue as TreeIndividual;

            var ageCountValid = ageMenu.MenuItems.Count == selectedIndividual?.ageCount;

            if (selectedIndividual == null)
            {
                ageMenu.Selection.SelectionChanged -= AgeOnSelectionChanged;

                ageMenu.Clear();

                ageMenu.Selection.SelectionChanged += AgeOnSelectionChanged;
            }
            else if (!ageCountValid)
            {
                ageMenu.Selection.SelectionChanged -= AgeOnSelectionChanged;

                var currentAgeSelectionIndex = ageMenu.GetSelectedIndex();

                ageMenu.Clear();

                foreach (var age in selectedIndividual.ages)
                {
                    ageMenu.Add(age.GetMenuString(), age, age.GetIcon(true).icon);
                }

                currentAgeSelectionIndex = Mathf.Max(currentAgeSelectionIndex, ageMenu.MenuItems.Count - 1);

                if (currentAgeSelectionIndex > 0)
                {
                    ageMenu.SelectByIndex(currentAgeSelectionIndex);
                }

                ageMenu.Selection.SelectionChanged += AgeOnSelectionChanged;
            }
            else if (ageMenu.MenuItems.Count == 0)
            {
                ageMenu.Selection.SelectionChanged -= AgeOnSelectionChanged;

                foreach (var age in selectedIndividual.ages)
                {
                    ageMenu.Add(age.GetMenuString(), age, age.GetIcon(true).icon);
                }

                if (ageMenu.MenuItems.Count > 0)
                {
                    ageMenu.SelectFirst();
                }

                ageMenu.Selection.SelectionChanged += AgeOnSelectionChanged;
            }

            var selectedAge = ageMenu.Selection.SelectedValue as TreeAge;

            if (selectedAge != null)
            {
                if (selectedAge.ageType == AgeType.None)
                {
                    selectedAge.ageType = AgeType.Mature;
                    selectedAge.individualID = selectedIndividual.individualID;
                }
            }

            var stageCountValid = stageMenu.MenuItems.Count == selectedAge?.StageCount;

            if (selectedAge == null)
            {
                stageMenu.Selection.SelectionChanged -= StageOnSelectionChanged;

                stageMenu.Clear();

                stageMenu.Selection.SelectionChanged += StageOnSelectionChanged;
            }
            else if (!stageCountValid)
            {
                stageMenu.Selection.SelectionChanged -= StageOnSelectionChanged;

                var currentStageSelectionIndex = stageMenu.GetSelectedIndex();

                stageMenu.Clear();

                if (selectedAge.normalStage != null)
                {
                    stageMenu.Add(
                        selectedAge.normalStage.GetMenuString(),
                        selectedAge.normalStage,
                        selectedAge.normalStage.GetIcon(true).icon
                    );
                }

                foreach (var variant in selectedAge.Variants)
                {
                    stageMenu.Add(variant.GetMenuString(), variant, variant.GetIcon(true).icon);
                }

                currentStageSelectionIndex = Mathf.Max(currentStageSelectionIndex, stageMenu.MenuItems.Count - 1);

                if (currentStageSelectionIndex > 0)
                {
                    stageMenu.SelectByIndex(currentStageSelectionIndex);
                }

                stageMenu.SelectByIndex(currentStageSelectionIndex);

                stageMenu.Selection.SelectionChanged += StageOnSelectionChanged;
            }
            else if (stageMenu.MenuItems.Count == 0)
            {
                stageMenu.Selection.SelectionChanged -= StageOnSelectionChanged;

                if (selectedAge.normalStage != null)
                {
                    stageMenu.Add(
                        selectedAge.normalStage.GetMenuString(),
                        selectedAge.normalStage,
                        selectedAge.normalStage.GetIcon(true).icon
                    );
                }

                foreach (var variant in selectedAge.Variants)
                {
                    stageMenu.Add(variant.GetMenuString(), variant, variant.GetIcon(true).icon);
                }

                if (stageMenu.MenuItems.Count > 0)
                {
                    stageMenu.SelectFirst();
                }

                stageMenu.Selection.SelectionChanged += StageOnSelectionChanged;
            }
        }

        public void SelectFirst()
        {
            individualMenu.SelectFirst();
            ageMenu.SelectFirst();
            stageMenu.SelectFirst();

        }


        private void IndividualOnSelectionChanged(SelectionChangedType obj)
        {
            if (obj == SelectionChangedType.ItemAdded)
            {
                if (individualMenu.Selection.SelectedValue == _lastIndividualSelection)
                {
                    return;
                }

                ageMenu.Clear();
                stageMenu.Clear();

                _lastIndividualSelection = individualMenu.Selection.SelectedValue;
            }
        }

        private void AgeOnSelectionChanged(SelectionChangedType obj)
        {
            if (obj == SelectionChangedType.ItemAdded)
            {
                if (ageMenu.Selection.SelectedValue == _lastAgeSelection)
                {
                    return;
                }

                stageMenu.Clear();

                _lastAgeSelection = ageMenu.Selection.SelectedValue;
            }
        }

        private void StageOnSelectionChanged(SelectionChangedType obj)
        {
            if ((obj == SelectionChangedType.ItemAdded) && stageMenu.HasSelection)
            {
                if (stageMenu.Selection.SelectedValue == _lastStageSelection)
                {
                    return;
                }

                _lastStageSelection = stageMenu.Selection.SelectedValue;
            }
        }


        public void DrawIndividualMenu(
            TreeDataContainer tree,
            float menuWidth,
            float menuHeight,
            float menuItemHeight,
            float menuButtonWidth,
            float menuButtonHeight)
        {
            TreeGUI.Draw.SmallTitle("Trees");
            individualMenu.Draw(menuItemHeight, maxWidth: menuWidth, maxHeight: menuHeight);

            DrawIndividualAddRemoveToolbar(
                tree,
                individualMenu.Selection.SelectedValue as TreeIndividual,
                (int) menuButtonWidth,
                menuButtonHeight
            );
        }

        private void DrawIndividualAddRemoveToolbar(
            TreeDataContainer tree,
            TreeIndividual selected,
            float width,
            float height)
        {
            using (TreeGUI.Layout.Horizontal())
            {

                if (GUILayout.Button(
                    TreeIcons.plus.Get("Add new tree"),
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/.MaxWidth(width).Height(height)
                ))
                {
                    tree.AddNewIndividual();
                    tree.RebuildStructures();

                    tree.dataState = TSEDataContainer.DataState.Dirty;
                }

                using (TreeGUI.Enabled.If(selected != null))
                {
                    if (GUILayout.Button(
                        (UnityEngine.GUI.enabled ? TreeIcons.x : TreeIcons.disabledX).Get("Remove selected tree"),
                        TreeGUI.Styles.ButtonRight,
                        TreeGUI.Layout.Options

                            //.MinWidth(width)
                            .MaxWidth(width)
                            .Height(height)
                    ))
                    {
                        individualMenu.Clear();
                        tree.RemoveIndividual(selected.individualID);

                        tree.RebuildStructures();

                        tree.dataState = TSEDataContainer.DataState.Dirty;
                    }
                }
            }
        }


        public void DrawAgeMenu(
            TreeDataContainer tree,
            float menuWidth,
            float menuItemHeight,
            float menuButtonWidth,
            float menuButtonHeight)
        {
            TreeGUI.Draw.SmallTitle("Ages");
            ageMenu.Draw(menuItemHeight, maxWidth: menuWidth);

            DrawAgeRemoveToolbar(
                tree,
                SelectedIndividual,
                SelectedAge,
                menuButtonWidth,
                menuButtonHeight
            );
        }

        private void DrawAgeRemoveToolbar(
            TreeDataContainer tree,
            TreeIndividual individual,
            TreeAge selected,
            float width,
            float height)
        {
            using (TreeGUI.Enabled.If(
                (selected != null) &&
                (individual != null) &&
                (individual.ageCount > 1)
                ))
            {
                if (GUILayout.Button(
                    (UnityEngine.GUI.enabled ? TreeIcons.x : TreeIcons.disabledX).Get("Remove selected age"),
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options

                        //.MinWidth(width)
                        .MaxWidth(width).Height(height)
                ))
                {
                    ageMenu.Clear();
                    individual.RemoveAge(selected.ageType);
                    tree.RebuildStructures();

                    tree.dataState = TSEDataContainer.DataState.Dirty;
                }
            }
        }

        public void DrawStageMenu(
            TreeDataContainer tree,
            float menuWidth,
            float menuItemHeight,
            float menuButtonWidth,
            float menuButtonHeight)
        {
            TreeGUI.Draw.SmallTitle("Stages");
            stageMenu.Draw(menuItemHeight, maxWidth: menuWidth);

            DrawStageRemoveToolbar(
                tree,
                SelectedAge,
                SelectedStage,
                menuButtonWidth,
                menuButtonHeight
            );
        }

        private void DrawStageRemoveToolbar(
            TreeDataContainer tree,
            TreeAge age,
            TreeStage selected,
            float width,
            float height)
        {
            using (TreeGUI.Enabled.If(
                (selected != null) && (selected.stageType != StageType.Normal) && (age != null)
            ))
            {
                if (GUILayout.Button(
                    (UnityEngine.GUI.enabled ? TreeIcons.x : TreeIcons.disabledX).Get("Remove selected stage"),
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options

                        //.MinWidth(width)
                        .MaxWidth(width).Height(height)
                ))
                {
                    stageMenu.Clear();
                    age.RemoveVariant(selected.stageType);
                    tree.RebuildStructures();

                    tree.dataState = TSEDataContainer.DataState.Dirty;
                }
            }
        }

        
        private object GetCurrentMenuSelection(MenuType change)
        {
            var set = individualMenu.Selection.SelectedValue as TreeIndividual;
            var age = ageMenu.Selection.SelectedValue as TreeAge;

            if (change == MenuType.Individual)
            {
                ageMenu.Selection.Clear();
                ageMenu.Clear();

                if (set == null)
                {
                    return null;
                }

                foreach (var a in set.ages)
                {
                    ageMenu.Add(a.GetMenuString(), a, a.GetIcon(true).icon);
                }

                var selection = ageMenu.MenuItems.FirstOrDefault(
                    i => (i.Value as TreeAge)?.ageType == AgeType.Mature
                );

                if (selection == null)
                {
                    selection = ageMenu.MenuItems.FirstOrDefault();
                }

                if (selection != null)
                {
                    ageMenu.Selection.Add(selection);
                }
            }

            if ((change == MenuType.Individual) || (change == MenuType.Age))
            {
                stageMenu.Selection.Clear();
                stageMenu.Clear();

                if (age == null)
                {
                    return null;
                }

                if (age.normalStage != null)
                {
                    stageMenu.Add(
                        age.normalStage.GetMenuString(),
                        age.normalStage,
                        age.normalStage.GetIcon(true).icon
                    );
                }

                foreach (var v in age.Variants)
                {
                    stageMenu.Add(v.GetMenuString(), v, v.GetIcon(true).icon);
                }

                if (stageMenu.MenuItems.Count > 0)
                {
                    stageMenu.Selection.Add(stageMenu.MenuItems.FirstOrDefault());
                }
            }

            return stageMenu.Selection.SelectedValue;
        }

        private enum MenuType
        {
            Individual,
            Age,
            Stage
        }

        public void Select(int individualID, AgeType ageType, StageType stageType)
        {
            individualMenu.SelectWhere(o => (o.Value as TreeIndividual)?.individualID == individualID);
            ageMenu.SelectWhere(o => (o.Value as TreeAge)?.ageType == ageType);
            stageMenu.SelectWhere(o => (o.Value as TreeStage)?.stageType == stageType);
        }

        public void Select(TreeStage stage)
        {
            Select(stage.individualID, stage.ageType, stage.stageType);
        }
    }
}
