using System;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Editing.Debugging;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Gizmos;
using Appalachia.Simulation.Trees.UI.Selections;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;
using Appalachia.Simulation.Trees.UI.Selections.Icons.Tree;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Species
{
    [CallStaticConstructorInEditor]
    [CustomEditor(typeof(TreeDataContainer))]
    public class TreeSpeciesEditor : OdinEditor
    {
        static TreeSpeciesEditor()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeSpeciesEditorSelection>()
                                     .IsAvailableThen(i => _treeSpeciesEditorSelection = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<RendererDebuggingSettings>()
                                     .IsAvailableThen(i => _rendererDebuggingSettings = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeGlobalSettings>()
                                     .IsAvailableThen(i => _treeGlobalSettings = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeGizmoStyle>()
                                     .IsAvailableThen(i => _treeGizmoStyle = i);
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeGizmoDelegate>()
                                     .IsAvailableThen(i => _treeGizmoDelegate = i);
        }

        #region Static Fields and Autoproperties

        public static TreeDataContainer _tree;
        public static TreeModel _treeModel;
        private static EditorInstanceState editorState;
        private static EditorInstanceState modelState;
        private static RendererDebuggingSettings _rendererDebuggingSettings;
        private static TreeGizmoDelegate _treeGizmoDelegate;
        private static TreeGizmoStyle _treeGizmoStyle;
        private static TreeGlobalSettings _treeGlobalSettings;

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        #region Fields and Autoproperties

        public bool _first = true;
        public bool _swallowMenuError;
        public GUITabGroup _globalTabGroup;

        public GUITabGroup _topLevelTabGroup;
        public GUITabGroup _treeTabGroup;
        public GUITabPage _global_DebugTabPage;
        public GUITabPage _global_SceneTabPage;

        public GUITabPage _global_SettingsTabPage;
        public GUITabPage _topLevel_GlobalTabPage;

        public GUITabPage _topLevel_TreeTabPage;
        public GUITabPage _tree_HierarchyTabPage;
        public GUITabPage _tree_IndividualTabPage;
        public GUITabPage _tree_MaterialTabPage;

        public GUITabPage _tree_SpeciesTabPage;

        public PropertyTree _debugProperty;
        public PropertyTree _globalProperty;
        public PropertyTree _materialsProperty;
        public PropertyTree _modelProperty;
        public PropertyTree _sceneProperty;

        public PropertyTree _speciesProperty;

        public Transform _cameraTransform;
        public TreeEditorSidebarCollection _sidebar;
        public TreeSpeciesHierarchyEditor _hierarchyEditor;

        private readonly float buttonHeightMultiplier = .05f;
        private readonly float drawSettingsWidth = 32f;
        private readonly float initialMenu1Width = 140f;

        private readonly float menu1AHeight = 120f;
        private readonly float menuButtonWidthMultiplier = .92f;

        private readonly float menuWidthMultiplier = .25f;
        private readonly float miniButtonScale = .65f;
        private readonly float sidebarButtonHeightMultiplier = .06f;

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

        private float sidebarButtonHeight =>
            Mathf.Min(initialButtonHeight, EditorGUIUtility.currentViewWidth * sidebarButtonHeightMultiplier);

        private float sidebarMiniButtonHeight => miniButtonScale * sidebarButtonHeight;

        private float sidebarSmallButtonHeight => smallButtonScale * sidebarButtonHeight;

        private float sideToolbarWidth =>
            Mathf.Min(initialMenu1Width, EditorGUIUtility.currentViewWidth * menuWidthMultiplier);

        private float smallButtonHeight => smallButtonScale * buttonHeight;

        #region Event Functions

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            TreeBuildManager._enabled = true;
        }

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

            _hierarchyEditor?.Dispose();

            _debugProperty?.Dispose();
            _globalProperty?.Dispose();
            _sceneProperty?.Dispose();

            _speciesProperty?.Dispose();
            _materialsProperty?.Dispose();
            _modelProperty?.Dispose();

            if (editorState.instanceID == instanceID)
            {
                if ((_tree != null) && !EditorApplication.isCompiling)
                {
                    if (_tree.dataState == TSEDataContainer.DataState.PendingSave)
                    {
                        _tree.Save();
                    }

                    AssetManager.OnFinalize();
                }

                editorState.instanceID = -1;
                editorState.activeWindowWidth = 0;
            }

            base.OnDisable();

            TreeBuildManager._enabled = false;
        }

        #endregion

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            try
            {
                if (!TreeModelViewToolbar.IsReady ||
                    !TreeSpeciesEditorSelection.IsInstanceAvailable ||
                    !RendererDebuggingSettings.IsInstanceAvailable ||
                    !TreeGlobalSettings.IsInstanceAvailable ||
                    !TreeGizmoStyle.IsInstanceAvailable)
                {
                    return;
                }

                var t = target as TreeDataContainer;

                if (t != _tree)
                {
                    ResetState();
                    _tree = t;
                }

                if (_tree == null)
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

                if (!_tree.initialized)
                {
                    if (string.IsNullOrWhiteSpace(_tree.initializationSettings.name))
                    {
                        _tree.initializationSettings.name = _tree.name;
                    }

                    base.OnInspectorGUI();
                    return;
                }

                _treeSpeciesEditorSelection.tree.selection.Set(_tree);

                if (_hierarchyEditor == null)
                {
                    _hierarchyEditor = new TreeSpeciesHierarchyEditor();
                }

                if (_tree.species.nameBasis == null)
                {
                    _tree.species.nameBasis = NameBasis.CreateNested(_tree);
                }

                if (_tree.subfolders == null)
                {
                    _tree.subfolders = TreeAssetSubfolders.CreateNested(_tree);
                }

                if (_treeModel == null)
                {
                    _treeModel = TreeModel.Find(_tree);

                    if (_treeModel == null)
                    {
                        _treeModel = TreeModel.Create(_tree, _treeGizmoDelegate);
                    }
                }

                _cameraTransform = SceneView.lastActiveSceneView.camera.transform;

                if (Event.current.type == EventType.Layout)
                {
                    if (_sidebar == null)
                    {
                        _sidebar = new TreeEditorSidebarCollection();
                    }

                    _sidebar.RepopulateMenus(_tree, false, false);
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
                        var oldI = _sidebar.SelectedIndividual;
                        var oldA = _sidebar.SelectedAge;
                        var oldS = _sidebar.SelectedStage;

                        var enabled = GUI.enabled;

                        try
                        {
                            if (_tree.progressTracker.buildActive)
                            {
                                GUI.enabled = false;
                            }

                            _sidebar.DrawIndividualMenu(
                                _tree,
                                sideToolbarWidth,
                                menu1AHeight,
                                sidebarButtonHeight,
                                sideToolbarWidth * menuButtonWidthMultiplier * .5f,
                                sidebarMiniButtonHeight
                            );

                            _sidebar.DrawAgeMenu(
                                _tree,
                                sideToolbarWidth,
                                sidebarButtonHeight,
                                sideToolbarWidth * menuButtonWidthMultiplier,
                                sidebarMiniButtonHeight
                            );

                            if (_sidebar.HasSelectedAge)
                            {
                                TreeProperty.SetActiveUIAge(_sidebar.SelectedAge.ageType);
                            }

                            _sidebar.DrawStageMenu(
                                _tree,
                                sideToolbarWidth,
                                sidebarButtonHeight,
                                sideToolbarWidth * menuButtonWidthMultiplier,
                                sidebarMiniButtonHeight
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
                            TreeBuildManager._enabled = true;

                            _sidebar.Select(
                                _treeSpeciesEditorSelection.tree.id,
                                _treeSpeciesEditorSelection.tree.age,
                                _treeSpeciesEditorSelection.tree.stage
                            );

                            //_sidebar.SelectFirst();

                            _tree.UpdateSettingsType(ResponsiveSettingsType.Tree);
                            _first = false;
                        }

                        if (_treeModel != null)
                        {
                            if ((_sidebar.SelectedIndividual != oldI) ||
                                (_sidebar.SelectedAge != oldA) ||
                                (_sidebar.SelectedStage != oldS))
                            {
                                _treeModel.selection.individualSelection =
                                    _tree.individuals.IndexOf(_sidebar.SelectedIndividual);

                                _treeSpeciesEditorSelection.tree.id =
                                    _sidebar.SelectedIndividual.individualID;

                                if (_sidebar.HasSelectedAge)
                                {
                                    _treeModel.selection.ageSelection = _sidebar.SelectedAge.ageType;
                                    _treeSpeciesEditorSelection.tree.id =
                                        _sidebar.SelectedIndividual.individualID;
                                }

                                if (_sidebar.HasSelectedStage)
                                {
                                    _treeModel.selection.stageSelection = _sidebar.SelectedStage.stageType;
                                    _treeSpeciesEditorSelection.tree.stage = _sidebar.SelectedStage.stageType;
                                }
                                else
                                {
                                    _treeModel.selection.stageSelection = StageType.Normal;
                                }

                                EditorApplication.QueuePlayerLoopUpdate();
                            }
                            else
                            {
                                if (_sidebar.SelectedIndividual != null)
                                {
                                    var index = _tree.individuals.IndexOf(_sidebar.SelectedIndividual);

                                    if ((index >= 0) && (index < _tree.individuals.Count))
                                    {
                                        var individual = _tree.individuals[index];

                                        if (!individual.HasType(_treeModel.selection.ageSelection))
                                        {
                                            var first = individual.ages.FirstOrDefault();

                                            _treeModel.selection.ageSelection = first.ageType;
                                            _treeModel.selection.stageSelection = StageType.Normal;
                                        }

                                        var age = individual[_treeModel.selection.ageSelection];

                                        if (!age.HasType(_treeModel.selection.stageSelection))
                                        {
                                            _treeModel.selection.stageSelection = StageType.Normal;
                                        }

                                        var modelSelection = age[_treeModel.selection.stageSelection];

                                        if (modelSelection != _sidebar.SelectedStage)
                                        {
                                            _sidebar.Select(modelSelection);
                                        }
                                    }
                                }
                            }
                        }

                        _tree.SetActive(
                            _sidebar.SelectedIndividual?.individualID ?? 0,
                            _sidebar.SelectedAge?.ageType ?? AgeType.None,
                            _sidebar.SelectedStage?.stageType ?? StageType.Normal
                        );

                        if (_sidebar.SelectedIndividual != null)
                        {
                            _sidebar.SelectedIndividual.UpdateMetadata(_tree.species);
                        }

                        EditorGUILayout.Space();

                        CopySettingsToolbar
                           .DrawCopyToolbar<TreeDataContainer, TreeSettingsSelection,
                                TreeSettingsDataContainerSelection>(
                                _tree,
                                _treeSpeciesEditorSelection.treeCopySettings,
                                _treeSpeciesEditorSelection.treeCopySettings.selection.enabled,
                                () => _treeSpeciesEditorSelection.treeCopySettings.selection.enabled = false
                            );
                    }

                    using (TreeGUI.Layout.Vertical())
                    {
                        using (TreeGUI.Layout.Horizontal())
                        {
                            TreeModelViewToolbar.DrawToolbar(
                                _tree,
                                _treeModel,
                                _cameraTransform,
                                smallButtonHeight
                            );
                        }

                        using (TreeGUI.Layout.Horizontal())
                        {
                            TreeSpeciesBuildToolbar.instance.DrawBuildToolbar(
                                _tree,
                                smallButtonHeight,
                                drawSettingsWidth
                            );
                        }

                        using (TreeGUI.Layout.Horizontal())
                        {
                            TreeSpeciesBuildToolbar.instance.DrawBuildProgress(
                                _tree,
                                smallButtonHeight * .75f
                            );
                        }

                        if ((_topLevelTabGroup == null) && (Event.current.type == EventType.Layout))
                        {
                            _topLevelTabGroup =
                                new GUITabGroup { FixedHeight = false, AnimationSpeed = 1000 };

                            _topLevel_TreeTabPage = _topLevelTabGroup.RegisterTab("Tree");
                            _topLevel_GlobalTabPage = _topLevelTabGroup.RegisterTab("Globals");

                            _treeTabGroup = new GUITabGroup { FixedHeight = false, AnimationSpeed = 1000 };

                            _globalTabGroup = new GUITabGroup { FixedHeight = false, AnimationSpeed = 1000 };

                            _tree_HierarchyTabPage = _treeTabGroup.RegisterTab("Shape");
                            _tree_SpeciesTabPage = _treeTabGroup.RegisterTab("Species");
                            _tree_IndividualTabPage = _treeTabGroup.RegisterTab("Individual");
                            _tree_MaterialTabPage = _treeTabGroup.RegisterTab("Materials");

                            _global_SettingsTabPage = _globalTabGroup.RegisterTab("Global");
                            _global_DebugTabPage = _globalTabGroup.RegisterTab("Debug");
                            _global_SceneTabPage = _globalTabGroup.RegisterTab("Scene");

                            if (_treeSpeciesEditorSelection.tree.tab == 0)
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevel_TreeTabPage);

                                if (_treeSpeciesEditorSelection.tree.subtab == 0)
                                {
                                    _treeTabGroup.SetCurrentPage(_tree_HierarchyTabPage);
                                }
                                else if (_treeSpeciesEditorSelection.tree.subtab == 1)
                                {
                                    _treeTabGroup.SetCurrentPage(_tree_SpeciesTabPage);
                                }
                                else if (_treeSpeciesEditorSelection.tree.subtab == 2)
                                {
                                    _treeTabGroup.SetCurrentPage(_tree_IndividualTabPage);
                                }
                                else
                                {
                                    _treeTabGroup.SetCurrentPage(_tree_MaterialTabPage);
                                }
                            }
                            else
                            {
                                _topLevelTabGroup.SetCurrentPage(_topLevel_GlobalTabPage);

                                if (_treeSpeciesEditorSelection.tree.subtab == 0)
                                {
                                    _globalTabGroup.SetCurrentPage(_global_SettingsTabPage);
                                }
                                else if (_treeSpeciesEditorSelection.tree.subtab == 1)
                                {
                                    _globalTabGroup.SetCurrentPage(_global_DebugTabPage);
                                }
                                else
                                {
                                    _globalTabGroup.SetCurrentPage(_global_SceneTabPage);
                                }
                            }
                        }

                        _topLevelTabGroup.BeginGroup(true, GUIStyle.none);

                        if (_topLevel_TreeTabPage.BeginPage())
                        {
                            _treeTabGroup.BeginGroup(true, GUIStyle.none);

                            _treeSpeciesEditorSelection.tree.tab = 0;

                            if (_tree_HierarchyTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 0;

                                using (TreeGUI.Layout.Horizontal())
                                {
                                    if ((_sidebar.SelectedAge != null) &&
                                        (_sidebar.SelectedAge.ageType != AgeType.None) &&
                                        (_sidebar.SelectedStage != null))
                                    {
                                        _hierarchyEditor?.DrawAll(
                                            _tree,
                                            _sidebar.SelectedStage.asset,
                                            _tree.species.hierarchies,
                                            _sidebar.SelectedStage.shapes,
                                            smallButtonHeight
                                        );
                                    }
                                }
                            }

                            _tree_HierarchyTabPage.EndPage();

                            if (_tree_SpeciesTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 1;

                                DrawSpecies();
                            }

                            _tree_SpeciesTabPage.EndPage();

                            if (_tree_IndividualTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 2;

                                TreeEditorGenerationToggleMenuManager.DrawEditorAgeToolbar(
                                    _tree,
                                    _sidebar.SelectedIndividual,
                                    buttonHeight
                                );

                                TreeEditorGenerationToggleMenuManager.DrawEditorStageToolbar(
                                    _tree,
                                    _sidebar.SelectedIndividual,
                                    _sidebar.SelectedAge,
                                    buttonHeight
                                );
                            }

                            _tree_IndividualTabPage.EndPage();

                            if (_tree_MaterialTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 3;

                                DrawMaterials();
                            }

                            _tree_MaterialTabPage.EndPage();

                            _treeTabGroup.EndGroup();
                        }

                        _topLevel_TreeTabPage.EndPage();

                        if (_topLevel_GlobalTabPage.BeginPage())
                        {
                            _globalTabGroup.BeginGroup(true, GUIStyle.none);

                            _treeSpeciesEditorSelection.tree.tab = 1;

                            if (_global_SettingsTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 0;

                                DrawGlobal();
                            }

                            _global_SettingsTabPage.EndPage();

                            if (_global_DebugTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 1;

                                DrawDebug();
                            }

                            _global_DebugTabPage.EndPage();

                            if (_global_SceneTabPage.BeginPage())
                            {
                                _treeSpeciesEditorSelection.tree.subtab = 2;

                                DrawScene();
                            }

                            _global_SceneTabPage.EndPage();

                            _globalTabGroup.EndGroup();
                        }

                        _topLevel_GlobalTabPage.EndPage();

                        _topLevelTabGroup.EndGroup();
                    }
                }

                if (_tree != null)
                {
                    /*if (_tree.dataState == TSEDataContainer.DataState.PendingSave && 
                        _tree.progressTracker.timeSinceBuildComplete > 60f
                        && _tree.settings.qualityMode > QualityMode.Working)
                    {
                        _tree.Save();
                    }*/
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                if (_tree != null)
                {
                    _tree.RebuildStructures();
                }

                Context.Log.Error(ex);
                _tree.dataState = TSEDataContainer.DataState.Normal;

                //throw;
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

        private void DrawGlobal()
        {
            if ((Event.current.type == EventType.Layout) && (_globalProperty == null))
            {
                _globalProperty = PropertyTree.Create(_treeGlobalSettings);
            }

            if (_globalProperty != null)
            {
                _globalProperty.Draw(false);
            }
        }

        private void DrawMaterials()
        {
            if ((Event.current.type == EventType.Layout) && (_materialsProperty == null))
            {
                _materialsProperty = PropertyTree.Create(_tree.materials);
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
                if (_treeModel == null)
                {
                    return;
                }

                _modelProperty = PropertyTree.Create(_treeModel);
            }

            if (_modelProperty != null)
            {
                _modelProperty.Draw(false);
            }
        }

        private void DrawNonActive()
        {
            SirenixEditorGUI.WarningMessageBox("This window is not the active tree editing window.");

            TreeGUI.Button.Standard(
                "Click to enable tree editing in this window",
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

        private void DrawSpecies()
        {
            if ((Event.current.type == EventType.Layout) && (_speciesProperty == null))
            {
                _speciesProperty = PropertyTree.Create(_tree.species);
            }

            if (_speciesProperty != null)
            {
                _speciesProperty.Draw(false);

                base.OnInspectorGUI();
            }
        }

        private void ResetState()
        {
            _treeModel = null;
            _tree = null;
            _first = true;
            _sidebar = null;
            _hierarchyEditor = null;
            _tree_IndividualTabPage = null;

            //_topLevelAssetsPage = null;
            _tree_MaterialTabPage = null;
            _tree_SpeciesTabPage = null;
            _tree_HierarchyTabPage = null;
            _topLevelTabGroup = null;

            //_assetsProperty = null;
            _cameraTransform = null;
        }
    }
}
