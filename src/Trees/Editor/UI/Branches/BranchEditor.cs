#region

using System;
using System.Collections.Generic;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Snapshot;
using Appalachia.Simulation.Trees.UI.Selections;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;
using Appalachia.Simulation.Trees.UI.Selections.Icons.Branch;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Appalachia.Simulation.Trees.UI.Branches
{
    [CallStaticConstructorInEditor]
    [CustomEditor(typeof(BranchDataContainer))]
    public class BranchEditor : OdinEditor
    {
        static BranchEditor()
        {
            TreeSpeciesEditorSelection.InstanceAvailable += i => _treeSpeciesEditorSelection = i;
        }

        #region Static Fields and Autoproperties

        public static BranchDataContainer branchData;
        private static int _previewRenderUtilityCount;

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        #region Fields and Autoproperties

        public bool _first = true;
        public BranchEditorSidebarCollection _sidebar;

        public float previewSize = .5f;
        public GUITabGroup _tabGroup;
        public GUITabPage _hierarchyPage;
        public GUITabPage _materialsPage;
        public GUITabPage _scenePage;
        public GUITabPage _settingsPage;
        public PropertyTree hierarchyProperty;
        public PropertyTree materialsProperty;
        public PropertyTree outputProperty;

        public PropertyTree sceneProperty;
        public PropertyTree settingsProperty;

        private readonly float buttonHeightMultiplier = .06f;
        private readonly float initialMenu1Width = 250f;

        private readonly float menu1AHeight = 120f;

        private readonly float menuWidthMultiplier = .50f;

        private readonly float miniButtonScale = .65f;

        private readonly float smallButtonScale = .75f;
        private readonly int initialButtonHeight = 32;

        [NonSerialized] private AppaContext _context;
        private int _previewRenderUtilityID;

        private PreviewRenderUtility _previewRenderUtility;

        private SnapshotRenderer.SnapshotMode renderMode = SnapshotRenderer.SnapshotMode.Albedo;

        private SnapshotRenderer.SnapshotMode[][] modeGroups =
        {
            new[] { SnapshotRenderer.SnapshotMode.Lit, SnapshotRenderer.SnapshotMode.Sample },
            new[]
            {
                SnapshotRenderer.SnapshotMode.Albedo,
                SnapshotRenderer.SnapshotMode.Normal,
                SnapshotRenderer.SnapshotMode.Surface
            }
        };

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

        protected override void OnEnable()
        {
            base.OnEnable();

            BranchBuildManager._enabled = true;
        }

        protected override void OnDisable()
        {
            if (_previewRenderUtility != null)
            {
                //Context.Log.Warn($"Cleaning up preview utility {_previewRenderUtilityID} from branch editor.");
                _previewRenderUtility.Cleanup();
                _previewRenderUtility = null;
            }

            sceneProperty?.Dispose();
            hierarchyProperty?.Dispose();
            settingsProperty?.Dispose();
            materialsProperty?.Dispose();
            outputProperty?.Dispose();

            if (branchData != null)
            {
                if (branchData.dataState == TSEDataContainer.DataState.PendingSave)
                {
                    branchData.Save();
                }

                AssetManager.OnFinalize();
            }

            base.OnDisable();

            BranchBuildManager._enabled = false;
        }

        protected void OnDestroy()
        {
            if (_previewRenderUtility != null)
            {
                //Context.Log.Warn($"Cleaning up preview utility {_previewRenderUtilityID} from branch editor.");
                _previewRenderUtility.Cleanup();
                _previewRenderUtility = null;
            }
        }

        #endregion

        public override void OnInspectorGUI()
        {
            branchData = target as BranchDataContainer;

            if (branchData == null)
            {
                return;
            }

            try
            {
                TreeIcons.Initialize();

                if (!branchData.initialized)
                {
                    if (string.IsNullOrWhiteSpace(branchData.initializationSettings.name))
                    {
                        branchData.initializationSettings.name = branchData.name;
                    }

                    base.OnInspectorGUI();
                    return;
                }

                if (branchData.subfolders == null)
                {
                    branchData.subfolders = TreeAssetSubfolders.CreateNested(branchData, false);
                }

                branchData.subfolders.nameBasis = branchData.GetNameBasis();
                branchData.subfolders.Initialize(branchData);

                _treeSpeciesEditorSelection.branch.selection.Set(branchData);

                if (branchData.snapshots == null)
                {
                    branchData.snapshots = new List<BranchSnapshotParameters>();
                }

                if (branchData.snapshots.Count == 0)
                {
                    if ((_sidebar != null) && (_sidebar.snapshotMenu.Selected != null))
                    {
                        branchData.snapshots.Add(_sidebar.snapshotMenu.Selected);
                    }
                    else
                    {
                        branchData.snapshots.Add(
                            BranchSnapshotParameters.Create(
                                branchData.subfolders.data,
                                branchData.branch.nameBasis,
                                0
                            )
                        );
                    }
                }

                if (Event.current.type == EventType.Layout)
                {
                    if (_sidebar == null)
                    {
                        _sidebar = new BranchEditorSidebarCollection();
                    }

                    _sidebar.snapshotMenu.RepopulateMenus(false, false);
                    _sidebar.hierarchyMenu.RepopulateMenus(false, false);
                }

                if (_previewRenderUtility == null)
                {
                    _previewRenderUtilityCount += 1;
                    _previewRenderUtilityID = _previewRenderUtilityCount;

                    //Context.Log.Warn($"Creating new preview utility {_previewRenderUtilityID} for branch editor.");
                    _previewRenderUtility = new PreviewRenderUtility();
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
                        //_treeSpeciesEditorSelection.branch.DrawAndSelect();

                        _sidebar.snapshotMenu.DrawMenu(
                            sideToolbarWidth,
                            menu1AHeight,
                            buttonHeight,
                            miniButtonHeight
                        );

                        if (_first || !_sidebar.snapshotMenu.HasSelection)
                        {
                            _sidebar.snapshotMenu.SelectFirst();

                            _first = false;
                        }

                        branchData.UpdateSettingsType(ResponsiveSettingsType.Branch);

                        if (branchData.branch.hierarchies.Count == 0)
                        {
                            branchData.branch.hierarchies.CreateTrunkHierarchy(
                                branchData.materials.inputMaterialCache
                            );
                        }

                        if (!_sidebar.hierarchyMenu.HasSelection)
                        {
                            _sidebar.hierarchyMenu.SelectFirst();
                        }

                        _sidebar.hierarchyMenu.DrawMenu(
                            sideToolbarWidth,
                            menu1AHeight,
                            buttonHeight,
                            miniButtonHeight
                        );

                        EditorGUILayout.Space();

                        DrawActionMenu();

                        EditorGUILayout.Space();

                        CopySettingsToolbar
                           .DrawCopyToolbar<BranchDataContainer, BranchSettingsSelection,
                                BranchSettingsDataContainerSelection>(
                                branchData,
                                _treeSpeciesEditorSelection.branchCopySettings,
                                !_sidebar.snapshotMenu.Selected.locked,
                                null
                            );
                    }

                    using (var rect = TreeGUI.Layout.Vertical())
                    {
                        previewSize = SirenixEditorFields.RangeFloatField(
                            TreeGUI.Content.Label("Preview Size"),
                            previewSize,
                            0.1f,
                            1f,
                            TreeGUI.Styles.LeftAlignedGreyMiniLabel
                        );

                        var size = .9f * editWindowWidth * previewSize;

                        using (TreeGUI.Layout.Vertical(
                                   false,
                                   TreeGUI.Layout.Options.ExpandWidth(false).Width(size)
                               ))
                        {
                            var aspectRect = GUILayoutUtility.GetAspectRect(1);

                            var center = rect.rect.x + (rect.rect.width / 2);
                            aspectRect.x = center - (aspectRect.width * .5f);

                            if ((branchData != null) &&
                                (branchData.branchAsset != null) &&
                                (branchData.branchAsset.mesh != null) &&
                                (_previewRenderUtility != null))
                            {
                                HandlePreviewInput(branchData.branchAsset.mesh, aspectRect);
                            }

                            if (Event.current.type == EventType.Repaint)
                            {
                                var snapshot = _sidebar.snapshotMenu.Selected;

                                if (_sidebar.snapshotMenu.HasSelection)
                                {
                                    var matElement =
                                        snapshot.branchOutputMaterial.GetMaterialElementByIndex(0);

                                    var matAssetPath = AssetDatabaseManager.GetAssetPath(matElement.asset);

                                    if (string.IsNullOrWhiteSpace(matAssetPath))
                                    {
                                        var assetGUIDs = AssetDatabaseManager.FindAssets(
                                            ZString.Format("t:Material {0}_snapshot", snapshot.name)
                                        );

                                        if (assetGUIDs.Length > 0)
                                        {
                                            var assetPath =
                                                AssetDatabaseManager.GUIDToAssetPath(assetGUIDs[0]);

                                            matElement.SetMaterial(
                                                AssetDatabaseManager.LoadAssetAtPath<Material>(assetPath),
                                                true
                                            );
                                        }
                                    }

                                    if ((snapshot.branchOutputMaterial.textureSet.outputTextures.Count ==
                                         0) &&
                                        (branchData.branchAsset != null) &&
                                        (branchData.branchAsset.materials != null) &&
                                        (branchData.branchAsset.materials.Length > 0) &&
                                        (snapshot.branchOutputMaterial.Count > 0) &&
                                        (snapshot.branchOutputMaterial.GetMaterialElementByIndex(0).asset !=
                                         null) &&
                                        (branchData.branchAsset.mesh != null))
                                    {
                                        snapshot.branchOutputMaterial.RebuildTextureSets();
                                    }
                                }

                                if (_sidebar.snapshotMenu.HasSelection && snapshot.locked)
                                {
                                    GUI.DrawTexture(
                                        aspectRect,
                                        snapshot.branchOutputMaterial.textureSet.outputTextures[0].texture,
                                        ScaleMode.StretchToFill,
                                        true
                                    );
                                }
                                else if ((branchData != null) &&
                                         (branchData.branchAsset != null) &&
                                         (branchData.branchAsset.mesh != null) &&
                                         (_previewRenderUtility != null))
                                {
                                    Texture resultRender;

                                    SnapshotRenderer.SetReplacementShader(_previewRenderUtility, renderMode);

                                    if ((renderMode == SnapshotRenderer.SnapshotMode.Lit) ||
                                        (renderMode == SnapshotRenderer.SnapshotMode.Sample))
                                    {
                                        resultRender = SnapshotRenderer.RenderMeshPreview(
                                            _previewRenderUtility,
                                            branchData,
                                            snapshot,
                                            aspectRect
                                        );
                                    }
                                    else
                                    {
                                        var bg = SnapshotRenderer.GetBackgroundForRenderMode(renderMode);

                                        resultRender = SnapshotRenderer.RenderMeshPreview(
                                            _previewRenderUtility,
                                            branchData,
                                            snapshot,
                                            aspectRect,
                                            bg
                                        );
                                    }

                                    GUI.DrawTexture(aspectRect, resultRender, ScaleMode.StretchToFill, false);
                                }
                                else
                                {
                                    EditorGUI.DropShadowLabel(aspectRect, "Nothing to render.");
                                }
                            }
                        }

                        //using (TreeGUI.Layout.Vertical())
                        {
                            using (TreeGUI.Layout.Horizontal())
                            {
                                BranchBuildToolbar.instance.DrawBuildToolbar(
                                    branchData,
                                    smallButtonHeight,
                                    32f
                                );
                            }

                            if (_tabGroup == null)
                            {
                                _tabGroup = new GUITabGroup();
                                _hierarchyPage = _tabGroup.RegisterTab("Hierarchy");
                                _settingsPage = _tabGroup.RegisterTab("Settings");
                                _materialsPage = _tabGroup.RegisterTab("Materials");
                                _scenePage = _tabGroup.RegisterTab("Scene");
                            }

                            if (_sidebar != null)
                            {
                                if (hierarchyProperty == null)
                                {
                                    if (_sidebar.hierarchyMenu.HasSelection)
                                    {
                                        hierarchyProperty =
                                            PropertyTree.Create(_sidebar.hierarchyMenu.Selected);
                                    }
                                }
                                else if (_sidebar.hierarchyMenu.HasSelection)
                                {
                                    if (!ReferenceEquals(
                                            _sidebar.hierarchyMenu.Selected,
                                            hierarchyProperty.WeakTargets[0]
                                        ))
                                    {
                                        hierarchyProperty =
                                            PropertyTree.Create(_sidebar.hierarchyMenu.Selected);
                                    }
                                }
                            }

                            foreach (var parameter in branchData.snapshots)
                            {
                                if (_sidebar.snapshotMenu.HasSelection &&
                                    (parameter == _sidebar.snapshotMenu.Selected))
                                {
                                    parameter.active = true;
                                }
                                else
                                {
                                    parameter.active = false;
                                }
                            }

                            if ((sceneProperty == null) ||
                                !ReferenceEquals(
                                    sceneProperty.WeakTargets[0],
                                    _sidebar.snapshotMenu.Selected
                                ))
                            {
                                sceneProperty = PropertyTree.Create(_sidebar.snapshotMenu.Selected);
                            }

                            if (settingsProperty == null)
                            {
                                settingsProperty = PropertyTree.Create(branchData.settings);
                            }

                            if (materialsProperty == null)
                            {
                                materialsProperty = PropertyTree.Create(branchData.materials);
                            }

                            if (_sidebar.snapshotMenu.HasSelection && !_sidebar.snapshotMenu.Selected.locked)
                            {
                                _tabGroup.BeginGroup();

                                if (_hierarchyPage.BeginPage())
                                {
                                    if (hierarchyProperty != null)
                                    {
                                        hierarchyProperty.Draw(false);
                                    }
                                }

                                _hierarchyPage.EndPage();

                                if (_settingsPage.BeginPage())
                                {
                                    if (settingsProperty != null)
                                    {
                                        settingsProperty.Draw(false);
                                    }
                                }

                                _settingsPage.EndPage();

                                if (_materialsPage.BeginPage())
                                {
                                    if (materialsProperty != null)
                                    {
                                        materialsProperty.Draw(false);
                                    }
                                }

                                _materialsPage.EndPage();

                                if (_scenePage.BeginPage())
                                {
                                    sceneProperty.Draw(false);
                                }

                                _scenePage.EndPage();

                                _tabGroup.EndGroup();
                            }
                            else
                            {
                                if (outputProperty == null)
                                {
                                    outputProperty = PropertyTree.Create(
                                        _sidebar.snapshotMenu.Selected.branchOutputMaterial
                                    );
                                }

                                outputProperty.Draw(false);
                            }
                        }
                    }
                }

                if (branchData != null)
                {
                    if ((branchData.dataState == TSEDataContainer.DataState.PendingSave) &&
                        (branchData.progressTracker.timeSinceBuildComplete > 60f))
                    {
                        branchData.Save();
                    }
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                Context.Log.Error(ex);

                if ((branchData != null) &&
                    (branchData.branch != null) &&
                    (branchData.branch.hierarchies != null) &&
                    (branchData.branch.shapes != null))
                {
                    branchData.RebuildStructures();
                }

                //throw;
            }
        }

        // ReSharper disable once UnusedParameter.Global
        public void HandlePreviewInput(Mesh mesh, Rect rect)
        {
            var controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            var current = Event.current;
            var useEvent = false;

            var delta = current.delta;

            if (current.shift)
            {
                delta *= 3;
            }

            if (current.control)
            {
                delta *= .333f;
            }

            if (current.alt)
            {
                delta *= .1f;
            }

            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (rect.Contains(current.mousePosition) && (rect.width > 50f))
                    {
                        GUIUtility.hotControl = controlID;
                        useEvent = true;
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }

                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        if (((current.button == 1) || (current.button == 2)) &&
                            !_sidebar.snapshotMenu.Selected.lockCameraPerspective)
                        {
                            var multiplier = .01f;

                            if (current.button == 2)
                            {
                                multiplier = .0025f;
                            }

                            _sidebar.snapshotMenu.Selected.translationXOffset += multiplier * delta.x;
                            _sidebar.snapshotMenu.Selected.translationYOffset += multiplier * delta.y;

                            _sidebar.snapshotMenu.Selected.translationXOffset = Mathf.Clamp(
                                _sidebar.snapshotMenu.Selected.translationXOffset,
                                -1f,
                                1f
                            );
                            _sidebar.snapshotMenu.Selected.translationYOffset = Mathf.Clamp(
                                _sidebar.snapshotMenu.Selected.translationYOffset,
                                -1f,
                                1f
                            );
                        }
                        else
                        {
                            var old = _sidebar.snapshotMenu.Selected.rotationOffset;

                            _sidebar.snapshotMenu.Selected.rotationOffset -=
                                (delta / Mathf.Min(rect.width, rect.height)) * 140f;

                            _sidebar.snapshotMenu.Selected.rotationOffset.y = Mathf.Clamp(
                                _sidebar.snapshotMenu.Selected.rotationOffset.y,
                                -90f,
                                90f
                            );

                            if (_sidebar.snapshotMenu.Selected.lockHorizontalRotation)
                            {
                                _sidebar.snapshotMenu.Selected.rotationOffset.x = old.x;
                            }

                            if (_sidebar.snapshotMenu.Selected.lockVerticalRotation)
                            {
                                _sidebar.snapshotMenu.Selected.rotationOffset.y = old.y;
                            }
                        }

                        useEvent = true;
                        GUI.changed = true;
                    }

                    break;
            }

            if (rect.Contains(current.mousePosition))
            {
                if (current.isScrollWheel && !_sidebar.snapshotMenu.Selected.lockCameraPerspective)
                {
                    useEvent = true;
                    _sidebar.snapshotMenu.Selected.translationDistance -= .005f * delta.y;
                }

                if (current.keyCode == KeyCode.L)
                {
                    useEvent = true;
                    _sidebar.snapshotMenu.Selected.ResetLighting();
                }

                if ((current.keyCode == KeyCode.F) && !_sidebar.snapshotMenu.Selected.lockCameraPerspective)
                {
                    useEvent = true;
                    _sidebar.snapshotMenu.Selected.ResetCamera();
                }

                if ((current.keyCode == KeyCode.R) && !_sidebar.snapshotMenu.Selected.lockCameraPerspective)
                {
                    useEvent = true;
                    _sidebar.snapshotMenu.Selected.ResetCamera();
                    _sidebar.snapshotMenu.Selected.ResetLighting();
                }
            }

            if (useEvent)
            {
                current.Use();
            }
        }

        protected override void OnHeaderGUI()
        {
        }

        private void DrawActionMenu()
        {
            using (TreeGUI.Layout.Vertical(true))
            {
                foreach (var modeGroup in modeGroups)
                {
                    using (TreeGUI.Layout.Horizontal(true))
                    {
                        foreach (var mode in modeGroup)
                        {
                            TreeGUI.Button.Toolbar(
                                _previewRenderUtility != null,
                                renderMode == mode,
                                mode.ToString(),
                                ZString.Format("Show {0}", mode.ToString().ToLower()),
                                () => { renderMode = mode; },
                                TreeGUI.Styles.ButtonSelected,
                                TreeGUI.Styles.Button,
                                TreeGUI.Layout.Options.None
                            );
                        }
                    }
                }

                EditorGUILayout.Space();

                using (TreeGUI.Layout.Horizontal(true))
                {
                    TreeGUI.Button.Toolbar(
                        _sidebar.snapshotMenu.Selected != null,
                        _sidebar.snapshotMenu.Selected.textureSize == TextureSize.k256,
                        "256",
                        "256 pixels",
                        () => { _sidebar.snapshotMenu.Selected.textureSize = TextureSize.k256; },
                        TreeGUI.Styles.ButtonLeftSelected,
                        TreeGUI.Styles.ButtonLeft,
                        TreeGUI.Layout.Options.None
                    );

                    TreeGUI.Button.Toolbar(
                        _sidebar.snapshotMenu.Selected != null,
                        _sidebar.snapshotMenu.Selected.textureSize == TextureSize.k512,
                        "512",
                        "512 pixels",
                        () => { _sidebar.snapshotMenu.Selected.textureSize = TextureSize.k512; },
                        TreeGUI.Styles.ButtonMidSelected,
                        TreeGUI.Styles.ButtonMid,
                        TreeGUI.Layout.Options.None
                    );

                    TreeGUI.Button.Toolbar(
                        _sidebar.snapshotMenu.Selected != null,
                        _sidebar.snapshotMenu.Selected.textureSize == TextureSize.k1024,
                        "1024",
                        "1024 pixels",
                        () => { _sidebar.snapshotMenu.Selected.textureSize = TextureSize.k1024; },
                        TreeGUI.Styles.ButtonMidSelected,
                        TreeGUI.Styles.ButtonMid,
                        TreeGUI.Layout.Options.None
                    );

                    TreeGUI.Button.Toolbar(
                        _sidebar.snapshotMenu.Selected != null,
                        _sidebar.snapshotMenu.Selected.textureSize == TextureSize.k2048,
                        "2048",
                        "2048 pixels",
                        () => { _sidebar.snapshotMenu.Selected.textureSize = TextureSize.k2048; },
                        TreeGUI.Styles.ButtonRightSelected,
                        TreeGUI.Styles.ButtonRight,
                        TreeGUI.Layout.Options.None
                    );
                }

                EditorGUILayout.Space();

                var old = GUI.backgroundColor;
                GUI.backgroundColor = TreeGUI.Colors.LightGreen;

                TreeGUI.Button.EnableDisable(
                    (_sidebar.snapshotMenu.Selected != null) && !_sidebar.snapshotMenu.Selected.locked,
                    "Render",
                    "Render the texture",
                    () =>
                    {
                        _sidebar.snapshotMenu.Selected.active = true;
                        BranchBuildRequestManager.Default();
                    },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.None
                );

                GUI.backgroundColor = TreeGUI.Colors.BurntLightOrange;

                TreeGUI.Button.ContextEnableDisable(
                    _sidebar.snapshotMenu.Selected != null,
                    (_sidebar.snapshotMenu.Selected != null) && !_sidebar.snapshotMenu.Selected.locked,
                    "Lock",
                    "Lock the snapshot",
                    () =>
                    {
                        _sidebar.snapshotMenu.Selected.locked = true;
                        branchData.dataState = TSEDataContainer.DataState.PendingSave;
                        _sidebar.snapshotMenu.Selected.MarkAsModified();
                        branchData.MarkAsModified();

                        branchData.Save();
                    },
                    TreeGUI.Styles.Button,
                    "Unlock",
                    "Unlock the snapshot",
                    () =>
                    {
                        _sidebar.snapshotMenu.Selected.locked = false;
                        branchData.dataState = TSEDataContainer.DataState.PendingSave;
                        _sidebar.snapshotMenu.Selected.MarkAsModified();
                        branchData.MarkAsModified();

                        branchData.Save();
                    },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.None
                );

                TreeGUI.Button.EnableDisable(
                    (Selection.objects.Length != 1) || (Selection.objects[0] != branchData),
                    "Select",
                    "Select the branch data container",
                    () => { Selection.objects = new Object[] { branchData }; },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.None
                );

                TreeGUI.Button.EnableDisable(
                    (_sidebar?.snapshotMenu != null) &&
                    (_sidebar.snapshotMenu.Selected != null) &&
                    (_sidebar.snapshotMenu.Selected.branchOutputMaterial?.GetMaterialElementByIndex(0) !=
                     null) &&
                    (_sidebar.snapshotMenu.Selected.branchOutputMaterial.GetMaterialElementByIndex(0).asset !=
                     null) &&
                    TreeMaterialUsageTracker.IsMaterialUsedInTrees(
                        _sidebar.snapshotMenu.Selected.branchOutputMaterial.GetMaterialElementByIndex(0).asset
                    ),
                    "Build Dependent Trees",
                    "Builds trees depending on this branch texture",
                    () =>
                    {
                        branchData.dataState = TSEDataContainer.DataState.PendingSave;
                        _sidebar.snapshotMenu.Selected.MarkAsModified();
                        branchData.MarkAsModified();
                        branchData.Save();

                        var material = _sidebar.snapshotMenu.Selected.branchOutputMaterial
                                               .GetMaterialElementByIndex(0)
                                               .asset;

                        var trees = TreeMaterialUsageTracker.GetTreeDataContainers(material);

                        TreeBuildManager._executing = true;
                        TreeBuildManager._autobuilding = true;
                        TreeBuildManager._coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(
                            TreeBuildManager.ExecuteAllBuildsEnumerator(
                                QualityMode.Finalized,
                                () => TreeBuildRequestManager.TextureOnly(),
                                trees
                            )
                        );
                    },
                    TreeGUI.Styles.Button,
                    TreeGUI.Layout.Options.None
                );

                EditorGUILayout.Space();

                GUI.backgroundColor = old;
            }
        }
    }
}
