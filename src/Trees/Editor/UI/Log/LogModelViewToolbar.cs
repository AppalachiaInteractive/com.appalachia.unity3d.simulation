using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Debugging;
using Appalachia.Editing.Debugging;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Gizmos;
using Appalachia.Simulation.Trees.UI.Selections.State;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Log
{
    [CallStaticConstructorInEditor]
    public static class LogModelViewToolbar
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static LogModelViewToolbar()
        {
            TreeGizmoStyle.InstanceAvailable += i => _treeGizmoStyle = i;
            TreeGizmoDelegate.InstanceAvailable += i => _treeGizmoDelegate = i;
            TreeSpeciesEditorSelection.InstanceAvailable += i => _treeSpeciesEditorSelection = i;
            GlobalDebug.InstanceAvailable += i => _globalDebug = i;
        }

        #region Static Fields and Autoproperties

        private static GlobalDebug _globalDebug;
        private static TreeGizmoDelegate _treeGizmoDelegate;

        private static TreeGizmoStyle _treeGizmoStyle;
        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        public static void DrawToolbar(
            LogDataContainer log,
            LogModel logModel,
            Transform cameraTransform,
            float smallButtonHeight)
        {
            using (var buttons = TreeGUI.Layout.Horizontal())
            {
                TreeGUI.Button.ContextEnableDisable(
                    log.logInstances.Count > 0,
                    logModel == null,
                    TreeIcons.disabledNewTree,
                    TreeIcons.newTree,
                    "Remove model from scene",
                    "Add model to scene",
                    () =>
                    {
                        Object.DestroyImmediate(logModel);
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    () =>
                    {
                        logModel = LogModel.Find(log);

                        if (logModel == null)
                        {
                            logModel = LogModel.Create(log, _treeGizmoDelegate);
                        }

                        var pos = cameraTransform.position;
                        var spot = pos + (cameraTransform.forward * 30);
                        pos.y = 0;

                        logModel.transform.position = spot;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.ContextEnableDisable(
                    logModel != null,
                    logModel.visible,
                    TreeIcons.visible,
                    TreeIcons.disabledVisible,
                    "Hide tree model",
                    "Show tree model",
                    () =>
                    {
                        logModel.visible = false;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    () =>
                    {
                        logModel.visible = true;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnableDisable(
                    logModel != null,
                    TreeIcons.magnifyingGlass,
                    TreeIcons.disabledMagnifyingGlass,
                    "Select tree model",
                    () =>
                    {
                        Selection.objects = new Object[] { logModel.gameObject };
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.Toolbar(
                    (logModel != null) && logModel.visible,
                    log.drawGizmos,
                    TreeIcons.scene,
                    TreeIcons.disabledScene,
                    "Toggle Gizmo Drawing",
                    () =>
                    {
                        log.drawGizmos = !log.drawGizmos;
                        EditorApplication.QueuePlayerLoopUpdate();
                    },
                    TreeGUI.Styles.ButtonLeftSelected,
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.Toolbar(
                    (logModel != null) && logModel.visible,
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
                    (logModel != null) && logModel.visible,
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
                    _globalDebug.debugMode,
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
                        _globalDebug.debugMode = mode switch
                        {
                            DebugMode.Off         => DebugMode.DebugMotion,
                            DebugMode.DebugMotion => DebugMode.DebugMesh,
                            _                     => DebugMode.Off
                        };

                        _globalDebug.Update();
                    },
                    mode => mode == DebugMode.Off
                        ? TreeGUI.Styles.ButtonLeft
                        : TreeGUI.Styles.ButtonLeftSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext(
                    _globalDebug.debugMode,
                    mode => mode == DebugMode.DebugMotion,
                    mode => mode == DebugMode.DebugMotion ? TreeIcons.wind : TreeIcons.disabledWind,
                    mode => "Change motion debugging type",
                    mode =>
                    {
                        _globalDebug.debugMotion = _globalDebug.debugMotion.Next();

                        _globalDebug.Update();
                    },
                    mode => mode != DebugMode.DebugMotion
                        ? TreeGUI.Styles.ButtonMid
                        : TreeGUI.Styles.ButtonMidSelected,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                           .MaxHeight(smallButtonHeight)
                );

                TreeGUI.Button.EnumContext(
                    _globalDebug.debugMode,
                    mode => mode == DebugMode.DebugMesh,
                    mode => mode == DebugMode.DebugMesh ? TreeIcons.mesh : TreeIcons.disabledMesh,
                    mode => "Change mesh debugging type",
                    mode =>
                    {
                        _globalDebug.debugMesh = _globalDebug.debugMesh.Next();

                        _globalDebug.Update();
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
