/*
#region

using System;
using Appalachia.Simulation.Trees.Metadata;
using Appalachia.Simulation.Trees.Metadata.Definition;
using Appalachia.Simulation.Trees.Metadata.Extensions;
using Appalachia.Simulation.Trees.Metadata.Hierarchy.Settings;
using Appalachia.Simulation.Trees.Metadata.Icons;
using Appalachia.Simulation.Trees.Metadata.Texturing.Materials;
using Appalachia.Simulation.Trees.UV;
using Sirenix.OdinInspector.Editor;


using UnityEditor;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees
{
    public abstract class UVEditor<TT, TD> : AppalachiaEditorWindow
        where TT : IBranch
        where TD : TSEDataContainer
    {
        private static UVEditor<TT, TD> instance;

        protected abstract TD container { get; set; }

        public bool _first = true;
        public TreeUVSidebarCollection _sidebar;
        public Texture2D _previewTexture;
        public Material _previewMaterial;

        private readonly float buttonHeightMultiplier = .06f;
        private readonly int initialButtonHeight = 32;
        private readonly float initialMenu1Width = 295f;

        private readonly float menu1AHeight = 120f;
        private readonly float menuButtonWidthMultiplier = .97f;

        private readonly float menuWidthMultiplier = .50f;

        private readonly float smallButtonScale = .75f;

        private readonly float miniButtonScale = .65f;

        private Rect uvGraph;
        private float uvGraphScale = 1f;
        private Vector2 uvGraphOffset = Vector2.zero;
        private bool modifyingUVs;
        private bool eatNextKeyUp;

        private bool m_mouseDragging;
        private bool needsRepaint;

        private bool m_ignore;
        private bool m_rightMouseDrag;
        private bool m_draggingCanvas;

        private float buttonHeight =>
            Mathf.Min(initialButtonHeight, EditorGUIUtility.currentViewWidth * buttonHeightMultiplier);

        private float miniButtonHeight => miniButtonScale * buttonHeight;

        private float smallButtonHeight => smallButtonScale * buttonHeight;

        private float sideToolbarWidth =>
            Mathf.Min(initialMenu1Width, EditorGUIUtility.currentViewWidth * menuWidthMultiplier);

        private void ResetState()
        {
            container = null;
            _first = true;
            _sidebar = null;
        }

        protected abstract TD GetContainer();
        
        protected abstract TT GetData();

        protected override void OnGUI()
        {
            try
            {
                TreeIcons.Initialize();

                container = GetContainer();

                if (container == null)
                {
                    return;
                }

                if (_sidebar == null)
                {
                    _sidebar = new TreeUVSidebarCollection();
                }

                var data = GetData();
                
                _sidebar.RepopulateMenus(data, false, false);

                EditorGUILayout.Space();

                using (TreeGUI.Layout.Horizontal())
                {
                    using (TreeGUI.Layout.Vertical(
                        true,
                        TreeGUI.Layout.Options.ExpandWidth(false).MinWidth(sideToolbarWidth).MaxWidth(sideToolbarWidth)
                    ))
                    {
                        _sidebar.DrawLeafHierarchyMenu(
                            data,
                            sideToolbarWidth,
                            menu1AHeight,
                            buttonHeight
                        );

                        _sidebar.DrawLeafRectMenu(
                            sideToolbarWidth,
                            menu1AHeight,
                            buttonHeight,
                            sideToolbarWidth * menuButtonWidthMultiplier * .5f,
                            miniButtonHeight
                        );

                        if (_first)
                        {
                            _sidebar.SelectFirst();
                            _first = false;
                        }

                        DrawActionSection(_sidebar.SelectedLeafRect?.rect);
                    }

                    using (var section = TreeGUI.Layout.Vertical())
                    {
                        /*using (TreeGUI.Layout.Horizontal())
                        {
                            TreeSpeciesBuildToolbar.DrawBuildToolbar(data, smallButtonHeight, 32f);
                        }#1#

                        using (TreeGUI.Layout.Horizontal())
                        {
                            TreeGridToolbar.Draw(smallButtonHeight, _sidebar.SelectedLeafRect);
                        }

                        using (var rect = TreeGUI.Layout.Horizontal())
                        {
                            DrawUVGraph(rect.rect);
                        }
                    }
                }
            }
            catch (ExitGUIException)
            {
            }
            catch (Exception ex)
            {
                /*if (_tree != null)
                {
                    _tree.RebuildStructures();
                    
                    _tree.dataState = TSEDataContainer.DataState.Normal;
                }#1#

                Debug.LogError(ex);

                throw;
            }
        }

        private void MoveTool(Rect rect, UVRect uvRect)
        {
            var e = Event.current;

            EditorHandleUtility.limitToLeftButton = false; // enable right click drag

            var point = TreeUV.UVToGUIPoint(uvRect.center, rect.center, uvGraphOffset, uvGraphScale);
            //var point = TreeUV.UVToGUIPoint(handlePosition, rect.center, uvGraphOffset, uvGraphScale);
            
            var pos = EditorHandleUtility.PositionHandle2d(1, point, TreeUV.HANDLE_SIZE);
            uvRect.center = TreeUV.GUIToUVPoint(pos, rect.center, uvGraphOffset, uvGraphScale);

            EditorHandleUtility.limitToLeftButton = true;

            if (!e.isMouse)
            {
                return;
            }

            // Setting a custom pivot
            if (((e.button == TreeUV.RIGHT_MOUSE_BUTTON) || (e.alt && (e.button == TreeUV.LEFT_MOUSE_BUTTON))) &&
                !pos.Approx2(uvRect.center))
            {
                //userPivot = true; // flag the handle as having been user set.

                if (TreeUV.ControlKey)
                {
                    uvRect.center = Snapping.SnapValue(pos, TreeUV.GRID_SNAP_INCREMENT);
                }
                
                uvRect.center = TreeUV.GUIToUVPoint(pos, rect.center, uvGraphOffset, uvGraphScale);

                //SetHandlePosition(handlePosition, true);

                return;
            }
            
            if (!pos.Approx2(point))
            {
                // Start of move UV operation
                if (!modifyingUVs)
                {
                    modifyingUVs = true;
                }

                needsRepaint = true;
            }
        }

        private void RotateTool(Rect rect, UVRect uvRect)
        {
            var handlePoint = TreeUV.UVToGUIPoint(uvRect.center, rect.center, uvGraphOffset, uvGraphScale);

            var modifier = _sidebar.SelectedLeafRect.rect.GetRotationModifier();
            
            var newRotation = EditorHandleUtility.RotationHandle2d(0, handlePoint, modifier, TreeUV.HANDLE_SIZE);

            var diff = newRotation - modifier;

            if (Mathf.Abs(diff) > 1f)
            {
                if (!modifyingUVs)
                {
                    modifyingUVs = true;
                }

                if (TreeUV.ControlKey)
                {
                    newRotation = Snapping.SnapValue(newRotation, 15f);
                }
                
                _sidebar.SelectedLeafRect.rect.SetRotationModifier(newRotation);
            }

            needsRepaint = true;
        }

        private void ScaleTool(Rect rect, UVRect uvRect)
        {
            var point = TreeUV.UVToGUIPoint(uvRect.center, rect.center, uvGraphOffset, uvGraphScale);

            var modifier = _sidebar.SelectedLeafRect.rect.GetScaleModifier();

            var uvScale = EditorHandleUtility.ScaleHandle2d(2, point, modifier, TreeUV.HANDLE_SIZE);
            
            if (TreeUV.ControlKey)
            {
                uvScale = Snapping.SnapValue(uvScale, TreeUV.GRID_SNAP_INCREMENT);
            }

            if (uvScale.x.Approx(0f, Mathf.Epsilon))
            {
                uvScale.x = .0001f;
            }

            if (uvScale.y.Approx(0f, Mathf.Epsilon))
            {
                uvScale.y = .0001f;
            }

            if (modifier != uvScale)
            {
                if (!modifyingUVs)
                {
                    modifyingUVs = true;
                }

                _sidebar.SelectedLeafRect.rect.SetScaleModifier(uvScale);
                    
                needsRepaint = true;
            }
        }
        
        // Internal because pb_Editor needs to call this sometimes.
        internal void OnFinishUVModification(UVRect uvRect)
        {
            if (modifyingUVs)
            {
                if (TreeGridToolbar.tool == Tool.Rotate || TreeGridToolbar.tool == Tool.Scale)
                {
                    uvRect.Finish();
                }
            }
            
            modifyingUVs = false;
        }

        private void DrawUVGraph(Rect rect)
        {
            if ((TreeGridToolbar.tool == Tool.View) || m_draggingCanvas)
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Pan);
            }

            HandleInput(rect);

            var mat = _sidebar.SelectedLeafHierarchy.geometry.leafMaterial;

            if (mat != _previewMaterial)
            {
                _previewMaterial = mat;
                _previewTexture = mat.primaryTexture();
            }

            TreeGridDrawer.DrawUVGridTexture(_previewTexture, rect.center, uvGraphScale, uvGraphOffset);
            TreeGridDrawer.DrawUVGrid(rect.center, uvGraphScale, uvGraphOffset);
            
            TreeGridDrawer.DrawUVGridTextureLabels();

            if (_sidebar.SelectedLeafHierarchy == null)
            {
                return;
            }

            foreach (var uvRect in _sidebar.SelectedLeafHierarchy.geometry.uvRects)
            {
                var outlineColor = TreeUV.UVColorSecondary;
                var handleColor = TreeUV.UVHandlesSecondary;
                var drawLabels = false;

                if (_sidebar.HasSelectedLeafRect && uvRect == _sidebar.SelectedLeafRect)
                {
                    outlineColor = TreeUV.UVColorPrimary;
                    handleColor = TreeUV.UVHandlesPrimary;
                    drawLabels = true;
                }

                TreeGridDrawer.DrawUVGraph(rect.center, uvGraphScale, uvGraphOffset, uvRect, outlineColor, handleColor, drawLabels);
            }

            if (_sidebar.HasSelectedLeafRect)
            {

                switch (TreeGridToolbar.tool)
                {
                    case Tool.Move:
                        MoveTool(rect, _sidebar.SelectedLeafRect.rect);
                        break;

                    case Tool.Rotate:
                        RotateTool(rect, _sidebar.SelectedLeafRect.rect);
                        break;

                    case Tool.Scale:
                        ScaleTool(rect, _sidebar.SelectedLeafRect.rect);
                        break;
                }

                if (m_mouseDragging && (EditorHandleUtility.CurrentID < 0) && !m_draggingCanvas && !m_rightMouseDrag)
                {
                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = TreeUV.DRAG_BOX_COLOR;
                    GUI.backgroundColor = oldColor;
                }
            }

            if (needsRepaint)
            {
                Repaint();
                needsRepaint = false;
            }
        }

        private void HandleInput(Rect rect)
        {
            var e = Event.current;

            if (e.isKey)
            {
                HandleKeyInput(e, rect);
                return;
            }

            switch (e.type)
            {
                case EventType.MouseDrag:

                    if (m_ignore)
                    {
                        break;
                    }

                    m_mouseDragging = true;

                    if ((e.button == TreeUV.RIGHT_MOUSE_BUTTON) || ((e.button == TreeUV.LEFT_MOUSE_BUTTON) && e.alt))
                    {
                        m_rightMouseDrag = true;
                    }

                    needsRepaint = true;

                    // If no handle is selected, do other stuff
                    if (EditorHandleUtility.CurrentID < 0)
                    {
                        if ((TreeUV.AltKey && (e.button == TreeUV.LEFT_MOUSE_BUTTON)) ||
                            (e.button == TreeUV.MIDDLE_MOUSE_BUTTON) ||
                            (Tools.current == Tool.View))
                        {
                            m_draggingCanvas = true;
                            uvGraphOffset += e.delta;
                        }
                        else if (TreeUV.AltKey && (e.button == TreeUV.RIGHT_MOUSE_BUTTON))
                        {
                            SetCanvasScale(
                                uvGraphScale +
                                ((e.delta.x - e.delta.y) *
                                    ((uvGraphScale / TreeUV.MAX_GRAPH_SCALE_SCROLL) * TreeUV.ALT_SCROLL_MODIFIER))
                            );
                        }
                    }

                    break;

                case EventType.Ignore:
                case EventType.MouseUp:

                    if (m_ignore)
                    {
                        m_ignore = false;
                        m_mouseDragging = false;
                        m_draggingCanvas = false;
                        needsRepaint = true;
                        return;
                    }

                    if ((e.button == TreeUV.LEFT_MOUSE_BUTTON) &&
                        !m_rightMouseDrag &&
                        !modifyingUVs &&
                        !m_draggingCanvas)
                    {
                        if (m_mouseDragging)
                        {
                            e.Use();
                        }
                    }

                    if (e.button != TreeUV.RIGHT_MOUSE_BUTTON)
                    {
                        m_rightMouseDrag = false;
                    }

                    m_mouseDragging = false;
                    m_draggingCanvas = false;

                    if (modifyingUVs)
                    {
                        OnFinishUVModification(_sidebar.SelectedLeafRect.rect);
                    }
                    
                    needsRepaint = true;
                    break;

                case EventType.ScrollWheel:

                    SetCanvasScale(
                        uvGraphScale -
                        (e.delta.y * ((uvGraphScale / TreeUV.MAX_GRAPH_SCALE_SCROLL) * TreeUV.SCROLL_MODIFIER))
                    );
                    e.Use();
                    needsRepaint = true;

                    break;

                case EventType.ContextClick:
                    m_rightMouseDrag = false;

                    break;

                default:
                    return;
            }
        }

        private void HandleKeyInput(Event e, Rect rect)
        {
            if ((e.type != EventType.KeyUp) || eatNextKeyUp)
            {
                eatNextKeyUp = false;
                return;
            }

            switch (e.keyCode)
            {
                case KeyCode.Keypad0:
                case KeyCode.Alpha0:
                    uvGraphScale = 1f;
                    SetCanvasCenter(new Vector2(.5f, -.5f) * (TreeUV.UV_GRID_SIZE * uvGraphScale));
                    
                    e.Use();
                    needsRepaint = true;
                    break;

                case KeyCode.Q:
                    TreeGridToolbar.SetTool(Tool.View);
                    needsRepaint = true;
                    break;

                case KeyCode.W:
                    TreeGridToolbar.SetTool(Tool.Move);
                    needsRepaint = true;
                    break;

                case KeyCode.E:
                    TreeGridToolbar.SetTool(Tool.Rotate);
                    needsRepaint = true;
                    break;

                case KeyCode.R:
                    TreeGridToolbar.SetTool(Tool.Scale);
                    needsRepaint = true;
                    break;

                case KeyCode.F:
                    FrameSelection(rect, _sidebar.SelectedLeafRect.rect);
                    break;
            }
        }

        // Zooms in on the current UV selection
        private void FrameSelection(Rect rect, UVRect uvRect)
        {
            needsRepaint = true;

            if (uvRect == null)
            {
                SetCanvasCenter(Event.current.mousePosition - rect.center - uvGraphOffset);
                SetCanvasScale(1f);
                return;
            }

            SetCanvasScale(2f - uvRect.size.magnitude);

            var center = new Vector2(.5f, -.5f) * (TreeUV.UV_GRID_SIZE * uvGraphScale);
            center.y *= 2f;
            SetCanvasCenter(center);
        }

        // Sets the canvas scale.  1 is full size, .1 is super zoomed, and 2 would be 2x out.
        private void SetCanvasScale(float zoom)
        {
            var center = -(uvGraphOffset / uvGraphScale);
            uvGraphScale = Mathf.Clamp(zoom, TreeUV.MIN_GRAPH_SCALE, TreeUV.MAX_GRAPH_SCALE);
            SetCanvasCenter(center * uvGraphScale);
        }

        // Center the canvas on this point.  Should be in GUI coordinates.
        private void SetCanvasCenter(Vector2 center)
        {
            uvGraphOffset = center;
            uvGraphOffset.x = -uvGraphOffset.x;
            uvGraphOffset.y = -uvGraphOffset.y;
        }

        private void DrawActionSection(UVRect uvRect)
        {
            GUI.enabled = uvRect != null;

            GUILayout.Space(4);
            TreeGUI.Draw.Title("Actions");

            GUILayout.Space(4);

            TreeGUI.Button.Standard(
                "Flip Horizontal",
                "Flip Horizontal",
                () => uvRect.flipX = !uvRect.flipX,
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
            GUILayout.Space(4);

            TreeGUI.Button.Standard(
                "Flip Vertical",
                "Flip Vertical",
                () => uvRect.flipY = !uvRect.flipY,
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );

            GUILayout.Space(4);

            TreeGUI.Button.Standard(
                "Fit UVs Snug",
                "Fit UVs Snug",
                () =>
                {
                    var rect = FitUVs(.01f, _previewTexture);
                    uvRect.center = rect.center;
                    uvRect.size = rect.size;
                    uvRect.rotation = 0;
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
            
            TreeGUI.Button.Standard(
                "Fit UVs Relaxed",
                "Fit UVs Relaxed",
                () =>
                {
                    var rect = FitUVs(.05f, _previewTexture);
                    uvRect.center = rect.center;
                    uvRect.size = rect.size;
                    uvRect.rotation = 0;
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
            
            TreeGUI.Button.Standard(
                "Reset UVs",
                "Reset UVs",
                () =>
                {
                    uvRect.center = Vector2.one * .5f;
                    uvRect.rotation = 0f;
                    uvRect.size = Vector2.one;
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
        }
        
        private Rect FitUVs(float padding, Texture2D tex)
        {
            float minX = -1f;
            float maxX = -1f;
            float minY = -1f;
            float maxY = -1f;
            
            for (var y = 0; y < tex.height; y++) // bottom to top
            {
                var pixels = tex.GetPixels(0, y, tex.width, 1);
                
                for (var x = 0; x < tex.width; x++) // left to right
                {
                    var pixel = pixels[x];
                    
                    if (pixel.a > .005f)
                    {
                        if (minY == -1)
                        {
                            minY = y;
                        }

                        if (minX == -1 || x < minX)
                        {
                            minX = x;
                        }

                        maxY = y;

                        if (x > maxX)
                        {
                            maxX = x;
                        }
                    }
                }
            }

            var paddingX = padding * tex.width;
            var paddingY = padding * tex.height;

            minX -= paddingX;
            minY -= paddingY;
            maxX += paddingX;
            maxY += paddingY;

            minX /= tex.width;
            minY /= tex.height;
            maxX /= tex.width;
            maxY /= tex.height;

            minX = Mathf.Clamp01(minX);
            minY = Mathf.Clamp01(minY);
            maxX = Mathf.Clamp01(maxX);
            maxY = Mathf.Clamp01(maxY);

            return new Rect(minX, minY, maxX - minX, maxY - minY);

        }
    }
    
    [InitializeOnLoad]
    public class TreeUVEditor : AppalachiaEditorWindow
    {
        private static TreeUVEditor instance;

        public TreeDataContainer _tree;
        
        public bool _first = true;
        public TreeUVSidebarCollection _sidebar;
        public Texture2D _previewTexture;
        public Material _previewMaterial;

        private readonly float buttonHeightMultiplier = .06f;
        private readonly int initialButtonHeight = 32;
        private readonly float initialMenu1Width = 295f;

        private readonly float menu1AHeight = 120f;
        private readonly float menuButtonWidthMultiplier = .97f;

        private readonly float menuWidthMultiplier = .50f;

        private readonly float smallButtonScale = .75f;

        private readonly float miniButtonScale = .65f;

        //private Vector2 handlePosition = Vector2.zero, handlePosition_offset = Vector2.zero;

        private readonly Vector3 Vec3_Zero = Vector3.zero;

        private Rect uvGraph;
        private float uvGraphScale = 1f;
        private Vector2 uvGraphOffset = Vector2.zero;
        private bool modifyingUVs;
        private bool eatNextKeyUp;

        private bool m_mouseDragging;
        private bool needsRepaint;

        private bool m_ignore;
        private bool m_rightMouseDrag;
        private bool m_draggingCanvas;
        private bool m_doubleClick;

        // Set the handlePosition to this UV coordinate.
        //private bool userPivot;

        public static void OpenInstance()
        {
            if (instance == null)
            {
                instance = GetWindow<TreeUVEditor>();
            }
            
            instance.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            instance._tree = TreeSpeciesEditor._tree;
        }

        private float buttonHeight =>
            Mathf.Min(initialButtonHeight, EditorGUIUtility.currentViewWidth * buttonHeightMultiplier);

        private float miniButtonHeight => miniButtonScale * buttonHeight;

        private float smallButtonHeight => smallButtonScale * buttonHeight;

        private float sideToolbarWidth =>
            Mathf.Min(initialMenu1Width, EditorGUIUtility.currentViewWidth * menuWidthMultiplier);

        private void ResetState()
        {
            _tree = null;
            _first = true;
            _sidebar = null;
        }

        protected override void OnGUI()
        {
            try
            {
                TreeIcons.Initialize();

                if (TreeSpeciesEditor._tree == null)
                {
                    _tree = null;
                    return;
                }
                
                if (_tree != TreeSpeciesEditor._tree)
                {
                    ResetState();
                }

                _tree = TreeSpeciesEditor._tree;

                if (_sidebar == null)
                {
                    _sidebar = new TreeUVSidebarCollection();
                }

                _sidebar.RepopulateMenus(_tree.species, false, false);

                EditorGUILayout.Space();

                using (TreeGUI.Layout.Horizontal())
                {
                    using (TreeGUI.Layout.Vertical(
                        true,
                        TreeGUI.Layout.Options.ExpandWidth(false).MinWidth(sideToolbarWidth).MaxWidth(sideToolbarWidth)
                    ))
                    {
                        _sidebar.DrawLeafHierarchyMenu(
                            _tree.species,
                            sideToolbarWidth,
                            menu1AHeight,
                            buttonHeight
                        );

                        _sidebar.DrawLeafRectMenu(
                            sideToolbarWidth,
                            menu1AHeight,
                            buttonHeight,
                            sideToolbarWidth * menuButtonWidthMultiplier * .5f,
                            miniButtonHeight
                        );

                        if (_first)
                        {
                            _sidebar.SelectFirst();
                            _first = false;
                        }

                        DrawActionSection(_sidebar.SelectedLeafRect?.rect);
                    }

                    using (var section = TreeGUI.Layout.Vertical())
                    {
                        using (TreeGUI.Layout.Horizontal())
                        {
                            TreeSpeciesBuildToolbar.DrawBuildToolbar(_tree, smallButtonHeight, 32f);
                        }

                        using (TreeGUI.Layout.Horizontal())
                        {
                            TreeGridToolbar.Draw(smallButtonHeight, _sidebar.SelectedLeafRect);
                        }

                        using (var rect = TreeGUI.Layout.Horizontal())
                        {
                            DrawUVGraph(rect.rect);
                        }
                    }
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
                    
                    _tree.dataState = TSEDataContainer.DataState.Normal;
                }

                Debug.LogError(ex);

                throw;
            }
        }

        private void MoveTool(Rect rect, UVRect uvRect)
        {
            var e = Event.current;

            EditorHandleUtility.limitToLeftButton = false; // enable right click drag

            var point = TreeUV.UVToGUIPoint(uvRect.center, rect.center, uvGraphOffset, uvGraphScale);
            //var point = TreeUV.UVToGUIPoint(handlePosition, rect.center, uvGraphOffset, uvGraphScale);
            
            var pos = EditorHandleUtility.PositionHandle2d(1, point, TreeUV.HANDLE_SIZE);
            uvRect.center = TreeUV.GUIToUVPoint(pos, rect.center, uvGraphOffset, uvGraphScale);

            EditorHandleUtility.limitToLeftButton = true;

            if (!e.isMouse)
            {
                return;
            }

            // Setting a custom pivot
            if (((e.button == TreeUV.RIGHT_MOUSE_BUTTON) || (e.alt && (e.button == TreeUV.LEFT_MOUSE_BUTTON))) &&
                !pos.Approx2(uvRect.center))
            {
                //userPivot = true; // flag the handle as having been user set.

                if (TreeUV.ControlKey)
                {
                    uvRect.center = Snapping.SnapValue(pos, TreeUV.GRID_SNAP_INCREMENT);
                }
                
                uvRect.center = TreeUV.GUIToUVPoint(pos, rect.center, uvGraphOffset, uvGraphScale);

                //SetHandlePosition(handlePosition, true);

                return;
            }
            
            if (!pos.Approx2(point))
            {
                // Start of move UV operation
                if (!modifyingUVs)
                {
                    modifyingUVs = true;
                }

                needsRepaint = true;
            }
        }

        private void RotateTool(Rect rect, UVRect uvRect)
        {
            var handlePoint = TreeUV.UVToGUIPoint(uvRect.center, rect.center, uvGraphOffset, uvGraphScale);

            var modifier = _sidebar.SelectedLeafRect.rect.GetRotationModifier();
            
            var newRotation = EditorHandleUtility.RotationHandle2d(0, handlePoint, modifier, TreeUV.HANDLE_SIZE);

            var diff = newRotation - modifier;

            if (Mathf.Abs(diff) > 1f)
            {
                if (!modifyingUVs)
                {
                    modifyingUVs = true;
                }

                if (TreeUV.ControlKey)
                {
                    newRotation = Snapping.SnapValue(newRotation, 15f);
                }
                
                _sidebar.SelectedLeafRect.rect.SetRotationModifier(newRotation);
            }

            needsRepaint = true;
        }

        private void ScaleTool(Rect rect, UVRect uvRect)
        {
            var point = TreeUV.UVToGUIPoint(uvRect.center, rect.center, uvGraphOffset, uvGraphScale);

            var modifier = _sidebar.SelectedLeafRect.rect.GetScaleModifier();

            var uvScale = EditorHandleUtility.ScaleHandle2d(2, point, modifier, TreeUV.HANDLE_SIZE);
            
            if (TreeUV.ControlKey)
            {
                uvScale = Snapping.SnapValue(uvScale, TreeUV.GRID_SNAP_INCREMENT);
            }

            if (uvScale.x.Approx(0f, Mathf.Epsilon))
            {
                uvScale.x = .0001f;
            }

            if (uvScale.y.Approx(0f, Mathf.Epsilon))
            {
                uvScale.y = .0001f;
            }

            if (modifier != uvScale)
            {
                if (!modifyingUVs)
                {
                    modifyingUVs = true;
                }

                _sidebar.SelectedLeafRect.rect.SetScaleModifier(uvScale);
                    
                needsRepaint = true;
            }
        }
        
        // Internal because pb_Editor needs to call this sometimes.
        internal void OnFinishUVModification(UVRect uvRect)
        {
            if (modifyingUVs)
            {
                if (TreeGridToolbar.tool == Tool.Rotate || TreeGridToolbar.tool == Tool.Scale)
                {
                    uvRect.Finish();
                }
            }
            
            modifyingUVs = false;
        }

        private void DrawUVGraph(Rect rect)
        {
            if ((TreeGridToolbar.tool == Tool.View) || m_draggingCanvas)
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Pan);
            }

            HandleInput(rect);

            var mat = _sidebar.SelectedLeafHierarchy.geometry.leafMaterial;

            if (mat != _previewMaterial)
            {
                _previewMaterial = mat;
                _previewTexture = mat.primaryTexture();
            }

            TreeGridDrawer.DrawUVGridTexture(_previewTexture, rect.center, uvGraphScale, uvGraphOffset);
            TreeGridDrawer.DrawUVGrid(rect.center, uvGraphScale, uvGraphOffset);
            
            TreeGridDrawer.DrawUVGridTextureLabels();

            if (_sidebar.SelectedLeafHierarchy == null)
            {
                return;
            }

            foreach (var uvRect in _sidebar.SelectedLeafHierarchy.geometry.uvRects)
            {
                var outlineColor = TreeUV.UVColorSecondary;
                var handleColor = TreeUV.UVHandlesSecondary;
                var drawLabels = false;

                if (_sidebar.HasSelectedLeafRect && uvRect == _sidebar.SelectedLeafRect)
                {
                    outlineColor = TreeUV.UVColorPrimary;
                    handleColor = TreeUV.UVHandlesPrimary;
                    drawLabels = true;
                }

                TreeGridDrawer.DrawUVGraph(rect.center, uvGraphScale, uvGraphOffset, uvRect, outlineColor, handleColor, drawLabels);
            }

            if (_sidebar.HasSelectedLeafRect)
            {

                switch (TreeGridToolbar.tool)
                {
                    case Tool.Move:
                        MoveTool(rect, _sidebar.SelectedLeafRect.rect);
                        break;

                    case Tool.Rotate:
                        RotateTool(rect, _sidebar.SelectedLeafRect.rect);
                        break;

                    case Tool.Scale:
                        ScaleTool(rect, _sidebar.SelectedLeafRect.rect);
                        break;
                }

                if (m_mouseDragging && (EditorHandleUtility.CurrentID < 0) && !m_draggingCanvas && !m_rightMouseDrag)
                {
                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = TreeUV.DRAG_BOX_COLOR;
                    GUI.backgroundColor = oldColor;
                }
            }

            if (needsRepaint)
            {
                Repaint();
                needsRepaint = false;
            }
        }

        private void HandleInput(Rect rect)
        {
            var e = Event.current;

            if (e.isKey)
            {
                HandleKeyInput(e, rect);
                return;
            }

            switch (e.type)
            {
                case EventType.MouseDown:

                    if (e.clickCount > 1)
                    {
                        m_doubleClick = true;
                    }

                    break;

                case EventType.MouseDrag:

                    if (m_ignore)
                    {
                        break;
                    }

                    m_mouseDragging = true;

                    if ((e.button == TreeUV.RIGHT_MOUSE_BUTTON) || ((e.button == TreeUV.LEFT_MOUSE_BUTTON) && e.alt))
                    {
                        m_rightMouseDrag = true;
                    }

                    needsRepaint = true;

                    // If no handle is selected, do other stuff
                    if (EditorHandleUtility.CurrentID < 0)
                    {
                        if ((TreeUV.AltKey && (e.button == TreeUV.LEFT_MOUSE_BUTTON)) ||
                            (e.button == TreeUV.MIDDLE_MOUSE_BUTTON) ||
                            (Tools.current == Tool.View))
                        {
                            m_draggingCanvas = true;
                            uvGraphOffset += e.delta;
                        }
                        else if (TreeUV.AltKey && (e.button == TreeUV.RIGHT_MOUSE_BUTTON))
                        {
                            SetCanvasScale(
                                uvGraphScale +
                                ((e.delta.x - e.delta.y) *
                                    ((uvGraphScale / TreeUV.MAX_GRAPH_SCALE_SCROLL) * TreeUV.ALT_SCROLL_MODIFIER))
                            );
                        }
                    }

                    break;

                case EventType.Ignore:
                case EventType.MouseUp:

                    if (m_ignore)
                    {
                        m_ignore = false;
                        m_mouseDragging = false;
                        m_draggingCanvas = false;
                        m_doubleClick = false;
                        needsRepaint = true;
                        return;
                    }

                    if ((e.button == TreeUV.LEFT_MOUSE_BUTTON) &&
                        !m_rightMouseDrag &&
                        !modifyingUVs &&
                        !m_draggingCanvas)
                    {
                        if (m_mouseDragging)
                        {
                            e.Use();
                        }
                    }

                    if (e.button != TreeUV.RIGHT_MOUSE_BUTTON)
                    {
                        m_rightMouseDrag = false;
                    }

                    m_mouseDragging = false;
                    m_doubleClick = false;
                    m_draggingCanvas = false;

                    if (modifyingUVs)
                    {
                        OnFinishUVModification(_sidebar.SelectedLeafRect.rect);
                    }
                    
                    needsRepaint = true;
                    break;

                case EventType.ScrollWheel:

                    SetCanvasScale(
                        uvGraphScale -
                        (e.delta.y * ((uvGraphScale / TreeUV.MAX_GRAPH_SCALE_SCROLL) * TreeUV.SCROLL_MODIFIER))
                    );
                    e.Use();
                    needsRepaint = true;

                    break;

                case EventType.ContextClick:
                    m_rightMouseDrag = false;

                    break;

                default:
                    return;
            }
        }

        private void HandleKeyInput(Event e, Rect rect)
        {
            if ((e.type != EventType.KeyUp) || eatNextKeyUp)
            {
                eatNextKeyUp = false;
                return;
            }

            switch (e.keyCode)
            {
                case KeyCode.Keypad0:
                case KeyCode.Alpha0:
                    uvGraphScale = 1f;
                    SetCanvasCenter(new Vector2(.5f, -.5f) * (TreeUV.UV_GRID_SIZE * uvGraphScale));
                    
                    e.Use();
                    needsRepaint = true;
                    break;

                case KeyCode.Q:
                    TreeGridToolbar.SetTool(Tool.View);
                    needsRepaint = true;
                    break;

                case KeyCode.W:
                    TreeGridToolbar.SetTool(Tool.Move);
                    needsRepaint = true;
                    break;

                case KeyCode.E:
                    TreeGridToolbar.SetTool(Tool.Rotate);
                    needsRepaint = true;
                    break;

                case KeyCode.R:
                    TreeGridToolbar.SetTool(Tool.Scale);
                    needsRepaint = true;
                    break;

                case KeyCode.F:
                    FrameSelection(rect, _sidebar.SelectedLeafRect.rect);
                    break;
            }
        }

        // Zooms in on the current UV selection
        private void FrameSelection(Rect rect, UVRect uvRect)
        {
            needsRepaint = true;

            if (uvRect == null)
            {
                SetCanvasCenter(Event.current.mousePosition - rect.center - uvGraphOffset);
                SetCanvasScale(1f);
                return;
            }

            SetCanvasScale(2f - uvRect.size.magnitude);

            var center = new Vector2(.5f, -.5f) * (TreeUV.UV_GRID_SIZE * uvGraphScale);
            center.y *= 2f;
            SetCanvasCenter(center);
        }

        // Sets the canvas scale.  1 is full size, .1 is super zoomed, and 2 would be 2x out.
        private void SetCanvasScale(float zoom)
        {
            var center = -(uvGraphOffset / uvGraphScale);
            uvGraphScale = Mathf.Clamp(zoom, TreeUV.MIN_GRAPH_SCALE, TreeUV.MAX_GRAPH_SCALE);
            SetCanvasCenter(center * uvGraphScale);
        }

        // Center the canvas on this point.  Should be in GUI coordinates.
        private void SetCanvasCenter(Vector2 center)
        {
            uvGraphOffset = center;
            uvGraphOffset.x = -uvGraphOffset.x;
            uvGraphOffset.y = -uvGraphOffset.y;
        }

        private void DrawActionSection(UVRect uvRect)
        {
            GUI.enabled = uvRect != null;

            GUILayout.Space(4);
            TreeGUI.Draw.Title("Actions");

            GUILayout.Space(4);

            TreeGUI.Button.Standard(
                "Flip Horizontal",
                "Flip Horizontal",
                () => uvRect.flipX = !uvRect.flipX,
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
            GUILayout.Space(4);

            TreeGUI.Button.Standard(
                "Flip Vertical",
                "Flip Vertical",
                () => uvRect.flipY = !uvRect.flipY,
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );

            GUILayout.Space(4);

            TreeGUI.Button.Standard(
                "Fit UVs Snug",
                "Fit UVs Snug",
                () =>
                {
                    var rect = FitUVs(.01f, _previewTexture);
                    uvRect.center = rect.center;
                    uvRect.size = rect.size;
                    uvRect.rotation = 0;
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
            
            TreeGUI.Button.Standard(
                "Fit UVs Relaxed",
                "Fit UVs Relaxed",
                () =>
                {
                    var rect = FitUVs(.05f, _previewTexture);
                    uvRect.center = rect.center;
                    uvRect.size = rect.size;
                    uvRect.rotation = 0;
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
            
            TreeGUI.Button.Standard(
                "Reset UVs",
                "Reset UVs",
                () =>
                {
                    uvRect.center = Vector2.one * .5f;
                    uvRect.rotation = 0f;
                    uvRect.size = Vector2.one;
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.EmptyGUIOptions
            );
        }
        
        private Rect FitUVs(float padding, Texture2D tex)
        {
            float minX = -1f;
            float maxX = -1f;
            float minY = -1f;
            float maxY = -1f;
            
            for (var y = 0; y < tex.height; y++) // bottom to top
            {
                var pixels = tex.GetPixels(0, y, tex.width, 1);
                
                for (var x = 0; x < tex.width; x++) // left to right
                {
                    var pixel = pixels[x];
                    
                    if (pixel.a > .005f)
                    {
                        if (minY == -1)
                        {
                            minY = y;
                        }

                        if (minX == -1 || x < minX)
                        {
                            minX = x;
                        }

                        maxY = y;

                        if (x > maxX)
                        {
                            maxX = x;
                        }
                    }
                }
            }

            var paddingX = padding * tex.width;
            var paddingY = padding * tex.height;

            minX -= paddingX;
            minY -= paddingY;
            maxX += paddingX;
            maxY += paddingY;

            minX /= tex.width;
            minY /= tex.height;
            maxX /= tex.width;
            maxY /= tex.height;

            minX = Mathf.Clamp01(minX);
            minY = Mathf.Clamp01(minY);
            maxX = Mathf.Clamp01(maxX);
            maxY = Mathf.Clamp01(maxY);

            return new Rect(minX, minY, maxX - minX, maxY - minY);

        }
    }
}
*/
