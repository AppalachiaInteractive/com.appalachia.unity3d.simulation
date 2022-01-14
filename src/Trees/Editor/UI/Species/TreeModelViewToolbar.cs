using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Debugging;
using Appalachia.Editing.Debugging;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Gizmos;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Species
{
    [CallStaticConstructorInEditor]
    public static class TreeModelViewToolbar
    {
        static TreeModelViewToolbar()
        {
            TreeGizmoDelegate.InstanceAvailable += i => _treeGizmoDelegate = i;
            TreeGizmoStyle.InstanceAvailable += i => _treeGizmoStyle = i;
            RendererDebuggingSettings.InstanceAvailable += i => _rendererDebuggingSettings = i;
        }

        #region Static Fields and Autoproperties

        private static RendererDebuggingSettings _rendererDebuggingSettings;

        private static TreeGizmoDelegate _treeGizmoDelegate;
        private static TreeGizmoStyle _treeGizmoStyle;

        #endregion

        public static bool IsReady =>
            (_treeGizmoDelegate != null) && (_treeGizmoStyle != null) && (_rendererDebuggingSettings != null);

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
                            treeModel = TreeModel.Create(tree, _treeGizmoDelegate);
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
                        Selection.objects = new Object[] { treeModel.gameObject };
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
                    _treeGizmoStyle.drawGroundOffset,
                    TreeIcons.ground,
                    TreeIcons.disabledGround,
                    "Toggle ground gizmos",
                    () =>
                    {
                        _treeGizmoStyle.drawGroundOffset = !_treeGizmoStyle.drawGroundOffset;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonMidSelected,
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.Toolbar(
                    (treeModel != null) && treeModel.visible,
                    _treeGizmoStyle.drawShapeLabels,
                    TreeIcons.label,
                    TreeIcons.disabledLabel,
                    "Toggle label gizmos",
                    () =>
                    {
                        _treeGizmoStyle.drawShapeLabels = !_treeGizmoStyle.drawShapeLabels;
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
                        AssetDatabaseManager.OpenAsset(_treeGizmoStyle);
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext(
                    _rendererDebuggingSettings.debugMode,
                    mode => true,
                    mode => mode == DebugMode.Off ? TreeIcons.disabledColor : TreeIcons.color,
                    mode => mode switch
                    {
                        DebugMode.Off         => "Enable motion debugging",
                        DebugMode.DebugMotion => "Enable mesh debugging",
                        _                     => "Disable debugging"
                    },
                    mode =>
                    {
                        _rendererDebuggingSettings.debugMode = mode switch
                        {
                            DebugMode.Off         => DebugMode.DebugMotion,
                            DebugMode.DebugMotion => DebugMode.DebugMesh,
                            _                     => DebugMode.Off
                        };

                        _rendererDebuggingSettings.Update();
                    },
                    mode => mode == DebugMode.Off
                        ? TreeGUI.Styles.ButtonLeft
                        : TreeGUI.Styles.ButtonLeftSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext(
                    _rendererDebuggingSettings.debugMode,
                    mode => mode == DebugMode.DebugMotion,
                    mode => mode == DebugMode.DebugMotion ? TreeIcons.wind : TreeIcons.disabledWind,
                    mode => "Change motion debugging type",
                    mode =>
                    {
                        _rendererDebuggingSettings.debugMotion =
                            _rendererDebuggingSettings.debugMotion.Next();

                        _rendererDebuggingSettings.Update();
                    },
                    mode => mode != DebugMode.DebugMotion
                        ? TreeGUI.Styles.ButtonMid
                        : TreeGUI.Styles.ButtonMidSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext(
                    _rendererDebuggingSettings.debugMode,
                    mode => mode == DebugMode.DebugMesh,
                    mode => mode == DebugMode.DebugMesh ? TreeIcons.mesh : TreeIcons.disabledMesh,
                    mode => "Change mesh debugging type",
                    mode =>
                    {
                        _rendererDebuggingSettings.debugMesh = _rendererDebuggingSettings.debugMesh.Next();

                        _rendererDebuggingSettings.Update();
                    },
                    mode => mode != DebugMode.DebugMesh
                        ? TreeGUI.Styles.ButtonRight
                        : TreeGUI.Styles.ButtonRightSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );
            }
        }
    }
}
