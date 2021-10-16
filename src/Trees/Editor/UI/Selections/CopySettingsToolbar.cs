using System;
using Appalachia.Simulation.Trees.UI.GUI;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;
using Appalachia.Simulation.Trees.UI.Selections.State;
using UnityEditor;

namespace Appalachia.Simulation.Trees.UI.Selections
{
    public static class CopySettingsToolbar
    {
        public static void DrawCopyToolbar<T, TS, TSE>(T instance, TS selection, bool enabled, Action post)
            where T : TSEDataContainer
            where TS : TSESelection<T, TSE>
            where TSE : TreeScriptableObjectContainerSelection<T, TSE>
        {
            var old = UnityEngine.GUI.backgroundColor;
            selection.DrawAndSelect();

            EditorGUILayout.Space();

            UnityEngine.GUI.backgroundColor = TreeGUI.Colors.BurntLightOrange;

            TreeGUI.Button.EnableDisable(
                (selection.selection != null) &&
                (selection.selection.selected != null) &&
                (selection.selection.selected != instance) &&
                selection.selection.selected.initialized,
                "Select", "Select the branch data container",
                () => { Selection.objects = new UnityEngine.Object[] {selection.selection.selected}; },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.None
            );
            
            TreeGUI.Button.EnableDisable(
                (selection.selection != null) &&
                (selection.selection.selected != null) &&
                (selection.selection.selected != instance) &&
                selection.selection.selected.initialized &&
                (selection.selection.selected.dataState == TSEDataContainer.DataState.Normal) &&
                enabled,
                "Copy All",
                "Copy Hierarchies & Settings",
                () =>
                {
                    instance.CopyHierarchiesFrom(selection.selection.selected);
                    instance.CopySettingsFrom(selection.selection.selected);
                    post?.Invoke();
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.None
            );

            TreeGUI.Button.EnableDisable(
                (selection.selection != null) &&
                (selection.selection.selected != null) &&
                (selection.selection.selected != instance) &&
                selection.selection.selected.initialized &&
                (selection.selection.selected.dataState == TSEDataContainer.DataState.Normal) &&
                enabled,
                "Copy Hierarchies",
                "Copy Hierarchies",
                () =>
                {
                    instance.CopyHierarchiesFrom(selection.selection.selected);
                    post?.Invoke();
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.None
            );

            TreeGUI.Button.EnableDisable(
                (selection.selection != null) &&
                (selection.selection.selected != null) &&
                (selection.selection.selected != instance) &&
                selection.selection.selected.initialized &&
                (selection.selection.selected.dataState == TSEDataContainer.DataState.Normal) &&
                enabled,
                "Copy Settings",
                "Copy Settings",
                () =>
                {
                    instance.CopySettingsFrom(selection.selection.selected);
                    post?.Invoke();
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.None
            );
            
            EditorGUILayout.Space();
            
            UnityEngine.GUI.backgroundColor = TreeGUI.Colors.DarkRed;


            TreeGUI.Button.EnableDisable(
                (selection.selection != null) &&
                (selection.selection.selected != null) &&
                (selection.selection.selected != instance) &&
                selection.selection.selected.initialized &&
                (selection.selection.selected.dataState == TSEDataContainer.DataState.Normal) &&
                enabled,
                "Reset",
                "Reset initialization",
                () =>
                {
                    instance.initialized = false;
                    post?.Invoke();
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.None
            );


            UnityEngine.GUI.backgroundColor = old;
        }
    }
}