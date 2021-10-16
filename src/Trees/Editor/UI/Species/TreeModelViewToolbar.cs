using Appalachia.CI.Integration.Assets;
using Appalachia.Editing.Debugging;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Gizmos;
using Appalachia.Simulation.Trees.UI.GUI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.UI.Species
{
    public static class TreeModelViewToolbar
    {

        public static void DrawToolbar(
            TreeDataContainer tree,
            TreeModel treeModel,
            Transform cameraTransform,
            float smallButtonHeight)
        { 
            using (var buttons = TreeGUI.Layout.Horizontal())
            {
                TreeGUI.Button.ContextEnableDisable(
                    tree.individuals.Count > 0,
                    treeModel == null,
                    TreeIcons.disabledNewTree,
                    TreeIcons.newTree,
                    "Remove model from scene",
                    "Add model to scene",
                    () =>
                    {
                        Object.DestroyImmediate(treeModel);
                        EditorApplication.QueuePlayerLoopUpdate();

                    },
                    () =>
                    {
                        treeModel = TreeModel.Find(tree);

                        if (treeModel == null)
                        {
                            treeModel = TreeModel.Create(tree, TreeGizmoDelegate.instance);
                        }

                        var pos = cameraTransform.position;
                        var spot = pos + (cameraTransform.forward * 30);
                        pos.y = 0;

                        treeModel.transform.position = spot;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.ContextEnableDisable(
                    treeModel != null,
                    treeModel.visible,
                    TreeIcons.visible,
                    TreeIcons.disabledVisible,
                    "Hide tree model",
                    "Show tree model",
                    () =>
                    {
                        treeModel.visible = false;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    () =>
                    {
                        treeModel.visible = true;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnableDisable(
                    treeModel != null,
                    TreeIcons.magnifyingGlass,
                    TreeIcons.disabledMagnifyingGlass,
                    "Select tree model",
                    () =>
                    {
                        Selection.objects = new Object[] {treeModel.gameObject};
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );


                TreeGUI.Button.Toolbar(
                    (treeModel != null) && treeModel.visible,
                    tree.drawGizmos,
                    TreeIcons.scene,
                    TreeIcons.disabledScene,
                    "Toggle Gizmo Drawing",
                    () =>
                    {
                        tree.drawGizmos = !tree.drawGizmos;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonLeftSelected,
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.Toolbar(
                    (treeModel != null) && treeModel.visible,
                    TreeGizmoStyle.instance.drawGroundOffset,
                    TreeIcons.ground,
                    TreeIcons.disabledGround,
                    "Toggle ground gizmos",
                    () =>
                    {
                        TreeGizmoStyle.instance.drawGroundOffset = !TreeGizmoStyle.instance.drawGroundOffset;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonMidSelected,
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.Toolbar(
                    (treeModel != null) && treeModel.visible,
                    TreeGizmoStyle.instance.drawShapeLabels,
                    TreeIcons.label,
                    TreeIcons.disabledLabel,
                    "Toggle label gizmos",
                    () =>
                    {
                        TreeGizmoStyle.instance.drawShapeLabels = !TreeGizmoStyle.instance.drawShapeLabels;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonMidSelected,
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.Standard(
                    TreeIcons.gear,
                    "Open Gizmo Drawing Settings",
                    () =>
                    {
                        AssetDatabaseManager.OpenAsset(TreeGizmoStyle.instance);
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext<DebugMode>(
                    GlobalDebug.instance.debugMode,
                    (mode) => true,
                    (mode) => mode == DebugMode.Off ? TreeIcons.disabledColor : TreeIcons.color,
                    mode => mode switch
                    {
                        DebugMode.Off         => "Enable motion debugging",
                        DebugMode.DebugMotion => "Enable mesh debugging",
                        _                     => "Disable debugging"
                    },
                    mode =>
                    {
                        GlobalDebug.instance.debugMode = (mode switch
                        {
                            DebugMode.Off         => DebugMode.DebugMotion,
                            DebugMode.DebugMotion => DebugMode.DebugMesh,
                            _                     => DebugMode.Off
                        });

                        GlobalDebug.instance.Update();
                    },
                    (mode) => mode == DebugMode.Off
                        ? TreeGUI.Styles.ButtonLeft
                        : TreeGUI.Styles.ButtonLeftSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext<DebugMode>(
                    GlobalDebug.instance.debugMode,
                    (mode) => mode == DebugMode.DebugMotion,
                    (mode) => mode == DebugMode.DebugMotion ? TreeIcons.wind : TreeIcons.disabledWind,
                    mode => "Change motion debugging type",
                    mode =>
                    {
                        GlobalDebug.instance.debugMotion = GlobalDebug.instance.debugMotion.Next();

                        GlobalDebug.instance.Update();
                    },
                    (mode) => mode != DebugMode.DebugMotion
                        ? TreeGUI.Styles.ButtonMid
                        : TreeGUI.Styles.ButtonMidSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext<DebugMode>(
                    GlobalDebug.instance.debugMode,
                    (mode) => mode == DebugMode.DebugMesh,
                    (mode) => mode == DebugMode.DebugMesh ? TreeIcons.mesh : TreeIcons.disabledMesh,
                    mode => "Change mesh debugging type",
                    mode =>
                    {
                        GlobalDebug.instance.debugMesh = GlobalDebug.instance.debugMesh.Next();

                        GlobalDebug.instance.Update();
                    },
                    (mode) => mode != DebugMode.DebugMesh
                        ? TreeGUI.Styles.ButtonRight
                        : TreeGUI.Styles.ButtonRightSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(smallButtonHeight)
                );
            }
        }
    }


}
