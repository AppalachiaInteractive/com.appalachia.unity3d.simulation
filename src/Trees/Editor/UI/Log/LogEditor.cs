using System;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Editing.Debugging;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Gizmos;
using Appalachia.Simulation.Trees.UI.Selections.Icons.Log;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Simulation.Trees.UI.Species;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Log
{
    [CallStaticConstructorInEditor]
    [CustomEditor(typeof(LogDataContainer))]
    public class LogEditor : OdinEditor
    {
        static LogEditor()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeSpeciesEditorSelection>()
                                     .IsAvailableThen(i => _treeSpeciesEditorSelection = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeGizmoDelegate>()
                                     .IsAvailableThen(i => _treeGizmoDelegate = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeGizmoStyle>()
                                     .IsAvailableThen(i => _treeGizmoStyle = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<RendererDebuggingSettings>()
                                     .IsAvailableThen(i => _rendererDebuggingSettings = i);
        }

        #region Static Fields and Autoproperties

        public static LogDataContainer _log;
        public static LogModel _logModel;
        private static EditorInstanceState editorState;
        private static EditorInstanceState modelState;
        private static RendererDebuggingSettings _rendererDebuggingSettings;
        private static TreeGizmoDelegate _treeGizmoDelegate;
        private static TreeGizmoStyle _treeGizmoStyle;

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        #region Fields and Autoproperties

        public bool _first = true;
        public bool _swallowMenuError;
        public GUITabGroup _topLevelTabGroup;
        public GUITabPage _topLevelDebugPage;
        public GUITabPage _topLevelHierarchyPage;
        public GUITabPage _topLevelMaterialsPage;
        public GUITabPage _topLevelScenePage;

        public GUITabPage _topLevelSettingsPage;
        public LogEditorSidebarCollection _sidebar;
        public LogHierarchyEditor _hierarchyEditor;
        public PropertyTree _debugProperty;
        public PropertyTree _instanceProperty;
        public PropertyTree _logProperty;
        public PropertyTree _materialsProperty;
        public PropertyTree _modelProperty;
        public PropertyTree _sceneProperty;

        public Transform _cameraTransform;

        private readonly float buttonHeightMultiplier = .06f;
        private readonly float drawSettingsWidth = 32f;
        private readonly float initialMenu1Width = 140f;

        private readonly float menu1AHeight = 120f;

        //private readonly float menuButtonWidthMultiplier = .92f;

        private readonly float menuWidthMultiplier = .25f;
        private readonly float miniButtonScale = .65f;

        private readonly float smallButtonScale = .75f;
        private readonly int initialButtonHeight = 32;

        [NonSerialized] private AppaContext _context;

        #endregion

        protected AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(this);
                }

                return _context;
            }
        }

        private float buttonHeight =>
            Mathf.Min(initialButtonHeight, EditorGUIUtility.currentViewWidth * buttonHeightMultiplier);

        private float editWindowWidth => EditorGUIUtility.currentViewWidth - sideToolbarWidth - 6;

        private float miniButtonHeight => miniButtonScale * buttonHeight;

        private float sideToolbarWidth =>
            Mathf.Min(initialMenu1Width, EditorGUIUtility.currentViewWidth * menuWidthMultiplier);

        private float smallButtonHeight => smallButtonScale * buttonHeight;

        #region Event Functions

        /// <inheritdoc />
        protected override void OnDisable()
        {
            var instanceID = GetInstanceID();

            if (editorState == null)
            {
                editorState = new EditorInstanceState();
            }

            if (modelState == null)
            {
                modelState = new EditorInstanceState();
            }

            if (editorState.instanceID == instanceID)
            {
                if ((_log != null) && !EditorApplication.isCompiling)
                {
                    if (_log.dataState == TSEDataContainer.DataState.PendingSave)
                    {
                        _log.Save();
                    }

                    AssetManager.OnFinalize();
                }

                editorState.instanceID = -1;
                editorState.activeWindowWidth = 0;
            }

            _instanceProperty?.Dispose();
            _debugProperty?.Dispose();
            _sceneProperty?.Dispose();
            _logProperty?.Dispose();
            _materialsProperty?.Dispose();
            _modelProperty?.Dispose();

            base.OnDisable();
        }

        #endregion

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            try
            {
                var l = target as LogDataContainer;

                if (l != _log)
                {
                    ResetState();
                    _log = l;
                }

                if (_log == null)
                {
                    return;
                }

                if (editorState == null)
                {
                    editorState = new EditorInstanceState();
                }

                if (modelState == null)
                {
                    modelState = new EditorInstanceState();
                }

                if (!editorState.HandleStateChange(this))
                {
                    if (modelState.HandleStateChange(this))
                    {
                        DrawNonActive();
                        DrawModel();
                        return;
                    }

                    DrawNonActive();
                    return;
                }

                if (editorState.instanceID == modelState.instanceID)
                {
                    modelState.instanceID = -1;
                }

                TreeIcons.Initialize();

                if (!_log.initialized)
                {
                    if (string.IsNullOrWhiteSpace(_log.initializationSettings.name))
                    {
                        _log.initializationSettings.name = _log.name;
                    }

                    base.OnInspectorGUI();
                    return;
                }

                _treeSpeciesEditorSelection.log.selection.Set(_log);

                if (_hierarchyEditor == null)
                {
                    _hierarchyEditor = new LogHierarchyEditor();
                }

                if (_log.log.nameBasis == null)
                {
                    _log.log.nameBasis = NameBasis.CreateNested(_log);
                }

                if (_log.subfolders == null)
                {
                    _log.subfolders = TreeAssetSubfolders.CreateNested(_log);
                }

                if (_logModel == null)
                {
                    _logModel = LogModel.Find(_log);

                    if (_logModel == null)
                    {
                        _logModel = LogModel.Create(_log, _treeGizmoDelegate);
                    }
                }

                _cameraTransform = SceneView.lastActiveSceneView.camera.transform;

                if (Event.current.type == EventType.Layout)
                {
                    if (_sidebar == null)
                    {
                        _sidebar = new LogEditorSidebarCollection();
                    }

                    _sidebar.instanceMenu.RepopulateMenus(false, false);
                }

                EditorGUILayout.Space();

                using (TreeGUI.Layout.Horizontal())
                {
                    using (TreeGUI.Layout.Vertical(
                               true,
                               TreeGUI.Layout.Options.ExpandWidth(false)
                                      .MinWidth(sideToolbarWidth)
                                      .MaxWidth(sideToolbarWidth)
                           ))
                    {
                        var oldLogID = _sidebar.instanceMenu.Selected.logID;

                        var enabled = GUI.enabled;

                        try
                        {
                            if (_log.progressTracker.buildActive)
                            {
                                GUI.enabled = false;
                            }

                            _sidebar.instanceMenu.DrawMenu(
                                sideToolbarWidth,
                                menu1AHeight,
                                buttonHeight,
                                miniButtonHeight
                            );

                            _swallowMenuError = false;
                            GUI.enabled = enabled;
                        }
                        catch (Exception ex)
                        {
                            GUI.enabled = enabled;
                            if (_swallowMenuError)
                            {
                                throw;
                            }

                            Context.Log.Error(ex);
                            _swallowMenuError = true;
                            return;
                        }

                        if (_first)
                        {
                            _sidebar.instanceMenu.Select(
                                _log.logInstances.FirstOrDefault(
                                    lo => lo.logID == _treeSpeciesEditorSelection.log.id
                                )
                            );

                            //_sidebar.SelectFirst();

                            _log.UpdateSettingsType(ResponsiveSettingsType.Log);
                            _first = false;
                        }

                        if (_logModel != null)
                        {
                            if (_sidebar.instanceMenu.HasSelection)
                            {
                                _treeSpeciesEditorSelection.log.id = _sidebar.instanceMenu.Selected.logID;

                                if (_sidebar.instanceMenu.Selected.logID != oldLogID)
                                {
                                    EditorApplication.QueuePlayerLoopUpdate();
                                }

                                _logModel.selection.container = _log;
                                _logModel.selection.instanceSelection = _sidebar.instanceMenu.Selected.logID;
                            }
                        }

                        _log.SetActive(_sidebar.instanceMenu.Selected.logID);
                    }

                    using (TreeGUI.Layout.Vertical())
                    {
                        using (TreeGUI.Layout.Horizontal())
                        {
                            LogModelViewToolbar.DrawToolbar(
                                _log,
                                _logModel,
                                _cameraTransform,
                                smallButtonHeight
                            );
                        }

                        using (TreeGUI.Layout.Horizontal())
                        {
                            LogBuildToolbar.instance.DrawBuildToolbar(
                                _log,
                                smallButtonHeight,
                                drawSettingsWidth
                            );
                        }

                        using (TreeGUI.Layout.Horizontal())
                        {
                            LogBuildToolbar.instance.DrawBuildProgress(_log, smallButtonHeight * .75f);
                        }

                        if ((_topLevelTabGroup == null) && (Event.current.type == EventType.Layout))
                        {
                            _topLevelTabGroup = new GUITabGroup { FixedHeight = false, AnimationSpeed = 20 };

                            _topLevelHierarchyPage = _topLevelTabGroup.RegisterTab("Shape");
                            _topLevelSettingsPage = _topLevelTabGroup.RegisterTab("Settings");
                            _topLevelMaterialsPage = _topLevelTabGroup.RegisterTab("Materials");
                            _topLevelDebugPage = _topLevelTabGroup.RegisterTab("Debug");
                            _topLevelScenePage = _topLevelTabGroup.RegisterTab("Scene");

                            if (_treeSpeciesEditorSelection.log.tab == 1)
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevelSettingsPage);
                            }
                            else if (_treeSpeciesEditorSelection.log.tab == 2)
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevelMaterialsPage);
                            }
                            else if (_treeSpeciesEditorSelection.log.tab == 3)
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevelDebugPage);
                            }
                            else if (_treeSpeciesEditorSelection.log.tab == 4)
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevelScenePage);
                            }
                            else
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevelHierarchyPage);
                            }
                        }

                        _topLevelTabGroup.BeginGroup();

                        if (_topLevelHierarchyPage.BeginPage())
                        {
                            _treeSpeciesEditorSelection.log.tab = 0;

                            using (TreeGUI.Layout.Horizontal())
                            {
                                if (_sidebar.instanceMenu.HasSelection)
                                {
                                    using (var vert = TreeGUI.Layout.Vertical())
                                    {
                                        _hierarchyEditor.PrepareDrawing(_log, _log.log.hierarchies);

                                        _hierarchyEditor.DrawGraph(
                                            _log,
                                            _sidebar.instanceMenu.Selected.asset,
                                            _log.log.hierarchies,
                                            _sidebar.instanceMenu.Selected.shapes
                                        );

                                        _hierarchyEditor.DrawHierarchyManagementToolbar(
                                            _log,
                                            _log.log.hierarchies,
                                            _sidebar.instanceMenu.Selected.shapes,
                                            smallButtonHeight,
                                            vert.rect.width
                                        );

                                        EditorGUILayout.Space();

                                        DrawInstance();

                                        _hierarchyEditor.DrawHierarchyData(_log);
                                    }
                                }
                            }
                        }

                        _topLevelHierarchyPage.EndPage();

                        if (_topLevelSettingsPage.BeginPage())
                        {
                            _treeSpeciesEditorSelection.log.tab = 1;

                            DrawSettings();
                        }

                        _topLevelSettingsPage.EndPage();

                        /*if (_topLevelMaterialsPage.BeginPage())
                        {
                            _treeSpeciesEditorSelection.log.tab = 2;
                            
                            DrawMaterials();
                        }

                        _topLevelMaterialsPage.EndPage();*/

                        if (_topLevelDebugPage.BeginPage())
                        {
                            _treeSpeciesEditorSelection.log.tab = 3;

                            DrawDebug();
                        }

                        _topLevelDebugPage.EndPage();

                        if (_topLevelScenePage.BeginPage())
                        {
                            _treeSpeciesEditorSelection.log.tab = 4;

                            DrawScene();
                        }

                        _topLevelScenePage.EndPage();

                        _topLevelTabGroup.EndGroup();
                    }
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.RebuildStructures();
                }

                Context.Log.Error(ex);
                _log.dataState = TSEDataContainer.DataState.Normal;

                throw;
            }
        }

        /// <inheritdoc />
        protected override void OnHeaderGUI()
        {
        }

        private void DrawDebug()
        {
            if ((Event.current.type == EventType.Layout) && (_debugProperty == null))
            {
                _debugProperty = PropertyTree.Create(_rendererDebuggingSettings);
            }

            if (_debugProperty != null)
            {
                _debugProperty.Draw(false);
            }
        }

        private void DrawInstance()
        {
            if (Event.current.type == EventType.Layout)
            {
                if (!_sidebar.instanceMenu.HasSelection)
                {
                    _instanceProperty = null;
                }
                else if (_instanceProperty == null)
                {
                    _instanceProperty = PropertyTree.Create(_sidebar.instanceMenu.Selected);
                }
#pragma warning disable 252,253
                else if (_instanceProperty.WeakTargets[0] != _sidebar.instanceMenu.Selected)
#pragma warning restore 252,253
                {
                    _instanceProperty = PropertyTree.Create(_sidebar.instanceMenu.Selected);
                }
            }

            if (_instanceProperty != null)
            {
                _instanceProperty.Draw(false);
            }
        }

        private void DrawMaterials()
        {
            if ((Event.current.type == EventType.Layout) && (_materialsProperty == null))
            {
                _materialsProperty = PropertyTree.Create(_log.material);
            }

            if (_materialsProperty != null)
            {
                _materialsProperty.Draw(false);
            }
        }

        private void DrawModel()
        {
            if ((Event.current.type == EventType.Layout) && (_modelProperty == null))
            {
                if (_logModel == null)
                {
                    return;
                }

                _modelProperty = PropertyTree.Create(_logModel);
            }

            if (_modelProperty != null)
            {
                _modelProperty.Draw(false);
            }
        }

        private void DrawNonActive()
        {
            SirenixEditorGUI.WarningMessageBox("This window is not the active log editing window.");

            TreeGUI.Button.Standard(
                "Click to enable log editing in this window",
                string.Empty,
                () => editorState.instanceID = GetInstanceID(),
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.None
            );
        }

        private void DrawScene()
        {
            if ((Event.current.type == EventType.Layout) && (_sceneProperty == null))
            {
                _sceneProperty = PropertyTree.Create(_treeGizmoStyle);
            }

            if (_sceneProperty != null)
            {
                _sceneProperty.Draw(false);
            }
        }

        private void DrawSettings()
        {
            if ((Event.current.type == EventType.Layout) && (_logProperty == null))
            {
                _logProperty = PropertyTree.Create(_log.log);
            }

            if (_logProperty != null)
            {
                _logProperty.Draw(false);

                base.OnInspectorGUI();
            }
        }

        private void ResetState()
        {
            _logModel = null;
            _log = null;
            _first = true;
            _sidebar = null;
            _hierarchyEditor = null;
            _topLevelMaterialsPage = null;
            _topLevelSettingsPage = null;
            _topLevelHierarchyPage = null;
            _topLevelTabGroup = null;
            _cameraTransform = null;
        }
    }
}
