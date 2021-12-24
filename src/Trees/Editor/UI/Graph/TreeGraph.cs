using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Graph
{
    public static class TreeGraph
    {
        // [CallStaticConstructorInEditor] should be added to the class
        static TreeGraph()
        {
            TreeSpeciesEditorSelection.InstanceAvailable += i => _treeSpeciesEditorSelection = i;
        }

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;
        private static readonly string _assetString =
            "LOD{0} Stats|{1} vertices|{2} triangles|{3} materials";

        private static List<TreeGraphNode> _nodes;

        
        private static float _height;
        private static float _width;
        
        private static bool _first = true;
        private static bool _internalRebuild;
        private static bool _isDragging;
        
        private static Vector2 _dragClickPosition;
        
        private static Vector2 _graphScrollPosition;
        private static Vector2 _statsScrollPosition;
        
        private static TreeGraphNode _rootDownward;
        private static TreeGraphNode _rootUpward;
        private static TreeGraphNode dragNode;
        private static TreeGraphNode dropNode;

        private static IHierarchyReadWrite _current;
        
        public static TreeGraphNode selected;
        public static float buildTime;
        
        public static HierarchyData selectedHierarchy => selected?.data;
        public static bool internalRebuild => _internalRebuild;


        public static void Rebuild(
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            IBasicSelection selection,
            TreeGraphSettings settings)
        {
            dragNode = null;
            dropNode = null;
            _isDragging = false;
            _dragClickPosition = Vector2.zero;

            if (_nodes == null)
            {
                _nodes = new List<TreeGraphNode>();
            }

            _nodes.Clear();
            _rootUpward = null;
            _rootDownward = null;
            _width = 0;
            _height = 0;
            _first = true;
            _graphScrollPosition = Vector2.zero;
            _statsScrollPosition = Vector2.zero;
            selected = null;
            _internalRebuild = false;

            Build(hierarchies, shapes, selection, settings);
        }

        private static void Build(
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            IBasicSelection selection,
             TreeGraphSettings settings)
        {
            _current = hierarchies;
            
            if (_nodes == null)
            {
                _nodes = new List<TreeGraphNode>();
            }
            _nodes.Clear();

            if (_rootUpward == null)
            {
                _rootUpward = new TreeGraphNode(null, null);
            }

            if (_rootDownward == null)
            {
                _rootDownward = new TreeGraphNode(null, null);
            }

            _rootUpward.children.Clear();
            _rootDownward.children.Clear();

            var lookup = new Dictionary<int, TreeGraphNode>();

            hierarchies.RecurseHierarchiesWithData(
                data =>
                {
                    TreeGraphNode newNode;

                    if (data.type == TreeComponentType.Root)
                    {
                        return;
                    }

                    if ((data.parentHierarchy != null) &&
                        (data.parentHierarchy.type == TreeComponentType.Root))
                    {
                        return;
                    }

                    if (data.parentHierarchy == null)
                    {
                        newNode = new TreeGraphNode(data.hierarchy, null);
                        _rootUpward.children.Add(newNode);
                    }
                    else
                    {
                        var parentNode = lookup[data.hierarchy.parentHierarchyID];
                        newNode = new TreeGraphNode(data.hierarchy, parentNode);
                    }

                    _nodes.Add(newNode);
                    lookup.Add(data.hierarchy.hierarchyID, newNode);
                }
            );

            _rootUpward.Recursive(
                node => node.children.Sort(
                    (a, b) =>
                    {
                        var typeComparison = a.data.type.CompareTo(b.data.type);
                        
                        var idComparison = a.data.GetSortKey().CompareTo(b.data.GetSortKey());

                        if (typeComparison == 0)
                        {
                            return idComparison;
                        }

                        return typeComparison;
                    }
                ),
                true
            );

            TreeGraphGenerator.LayoutTree(settings, _rootUpward);

            foreach (var node in _nodes)
            {
                node.position.y = -node.position.y;
            }

            lookup.Clear();

            var tempNodes = new List<TreeGraphNode>();

            hierarchies.RecurseHierarchiesWithData(
                data =>
                {
                    TreeGraphNode newNode;

                    if (data.parentHierarchy == null)
                    {
                        newNode = new TreeGraphNode(data.hierarchy, null);
                        _rootDownward.children.Add(newNode);
                        lookup.Add(data.hierarchy.hierarchyID, newNode);
                    }
                    else if ((data.type == TreeComponentType.Root) ||
                        ((data.parentHierarchy != null) &&
                            (data.parentHierarchy.type == TreeComponentType.Root)))
                    {
                        var parentNode = lookup[data.hierarchy.parentHierarchyID];
                        newNode = new TreeGraphNode(data.hierarchy, parentNode);
                        tempNodes.Add(newNode);
                        lookup.Add(data.hierarchy.hierarchyID, newNode);
                    }
                }
            );

            _rootDownward.Recursive(
                node => node.children.Sort(
                    (a, b) =>
                    {
                        var typeComparison = a.data.type.CompareTo(b.data.type);
                        
                        var idComparison = a.data.GetSortKey().CompareTo(b.data.GetSortKey());

                        if (typeComparison == 0)
                        {
                            return idComparison;
                        }

                        return typeComparison;
                    }
                ),
                true
            );

            TreeGraphGenerator.LayoutTree(settings, _rootDownward);

            foreach (var node in tempNodes)
            {
                node.position.y -= (settings.verticalBufferPixels + settings.nodeHeight) * 2;
                _nodes.Add(node);
            }

            foreach (var trunk in _rootDownward.children)
            {
                var matching = _rootUpward.children.First(c => c.data == trunk.data);

                var targetX = Mathf.Max(matching.position.x, trunk.position.x);

                var matchingDelta = targetX - matching.position.x;
                matching.Recursive(node => node.position.x += matchingDelta, true);

                var trunkDelta = targetX - trunk.position.x;
                trunk.Recursive(node => node.position.x += trunkDelta, true);

                foreach (var root in trunk.children)
                {
                    matching.children.Add(root);
                }
            }

            var bounds = new Bounds();

            foreach (var node in _nodes)
            {
                var upperLeft = node.position - (Vector2.one * settings.nodePadding);
                var bottomRight = node.position +
                    new Vector2(settings.nodeWidth, settings.nodeHeight) +
                    (Vector2.one * settings.nodePadding);

                bounds.Encapsulate(upperLeft);
                bounds.Encapsulate(bottomRight);
            }

            _width = bounds.size.x;
            _height = bounds.size.y;
            
            foreach (var node in _nodes)
            {
                node.position.x -= bounds.min.x;
                node.position.y -= bounds.min.y;
            }

            if ((selected != null) && !_nodes.Contains(selected))
            {
                selected = null;
            }

            if (selected == null)
            {
                var selectedHierarchyID = _treeSpeciesEditorSelection.tree.hierarchyID;

                var match = _nodes.FirstOrDefault(n => n.data.hierarchyID == selectedHierarchyID);

                if (match == null)
                {
                    selected = _nodes.First();
                    _treeSpeciesEditorSelection.tree.hierarchyID = selected?.data?.hierarchyID ?? 0;
                }
                else
                {
                    selected = match;
                }
            }

            buildTime = Time.realtimeSinceStartup;
        }

        public static void Draw(
            TSEDataContainer so,
            IBasicSelection selection,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            TreeGraphSettings settings,
            ITreeStatistics stats,
            Rect sizeRect)
        {
            if (_internalRebuild || 
                (_current != hierarchies) ||
                (_rootUpward == null) || 
                ((so.progressTracker.buildCompleteTime > buildTime) && (so.progressTracker.timeSinceBuildComplete > 5f) && !_isDragging))
            {
                if ((_current != hierarchies) || (selected != null))
                {
                    selection.HierarchyID = selected?.data?.hierarchyID ?? 0;
                    
                    Rebuild(hierarchies, shapes, selection, settings);

                    selected = _nodes.FirstOrDefault(
                        n => n.data.hierarchyID == selection.HierarchyID
                    );
                    
                    dragNode = null;
                }
                else
                {
                    selection.HierarchyID = 0;
                    Rebuild(hierarchies, shapes, selection, settings);
                }
            }
            
            var clear = UnityEngine.GUI.changed;

            var hierarchyRect = new Rect(0, 0, _width, _height);

            var graphOffset = Vector2.zero;

            if (sizeRect.width < hierarchyRect.width)
            {
                graphOffset.y -= 16;
            }

            if (sizeRect.height < hierarchyRect.height)
            {
                graphOffset.x -= 16;
            }

            var viewRect = GUILayoutUtility.GetRect(
                sizeRect.width - settings.statsWindowWidth, 
                2000,
                settings.graphSize + graphOffset.y,
                settings.graphSize + graphOffset.y,
                TreeGUI.Layout.Options.ExpandWidth());

            // background
            TreeGUI.Draw.Solid(viewRect, TreeGUI.Colors.DarkEditorBackground);

            if (stats != null)
            {
                if (_first && (Event.current.type == EventType.Repaint))
                {
                    _first = false;

                    if (graphOffset != Vector2.zero)
                    {
                        var ratioX = viewRect.width / hierarchyRect.width;
                        var ratioY = viewRect.height / hierarchyRect.height;

                        var offsetX = hierarchyRect.width * ratioX * .5f;
                        var offsetY = hierarchyRect.height * ratioY * .5f;
                        
                        var x = _rootUpward.children.Average(c => c.position.x);
                        var y = _rootUpward.children.Average(c => c.position.y);
                        
                        _graphScrollPosition.x = x - offsetX;
                        _graphScrollPosition.y = y - offsetY;
                    }
                }

                if ((Event.current.type == EventType.ScrollWheel) &&
                    viewRect.Contains(Event.current.mousePosition) && (graphOffset != Vector2.zero))
                {
                    if ((_graphScrollPosition.y == 0) && (Event.current.delta.y < 0))
                    {

                    }
                    else
                    {
                        var multiplier = Vector2.one * ((Event.current.control || Event.current.alt)
                            ? settings.fastScrollSpeed
                            : settings.scrollSpeed);

                        if (Event.current.shift)
                        {
                            multiplier.y = 0;
                        }
                        else
                        {
                            multiplier.x = 0;
                        }
                    
                        _graphScrollPosition += (multiplier * Event.current.delta.y);
                        Event.current.Use();
                    }
                }
                
                _graphScrollPosition = UnityEngine.GUI.BeginScrollView(
                    viewRect,
                    _graphScrollPosition,
                    hierarchyRect,
                    false,
                    false
                );

                UnityEngine.GUI.changed = clear;


                // handle dragging
                HandleDragHierarchyNodes(so, hierarchies, shapes, settings, graphOffset);

                // draw nodes
                foreach (var trunk in _rootUpward.children)
                {
                    DrawHierarchyNode(so, hierarchies, shapes, selection, settings, trunk, graphOffset / 2, 1.0f, 1.0f);
                }

                // draw drag nodes
                if ((dragNode != null) && _isDragging)
                {
                    var dragOffset = Event.current.mousePosition - _dragClickPosition;

                    DrawHierarchyNode(so, hierarchies, shapes, selection, settings, dragNode, dragOffset + (graphOffset / 2), 0.5f, 0.5f);
                }

                UnityEngine.GUI.EndScrollView();


                var statsWidth = settings.statsWindowWidth + settings.statsPaddingX;
                var statsHeight = (settings.statsHeight + settings.statsPaddingY) *
                    stats.GetStatsCount();

                var statsRect = new Rect(
                    0, 
                    0, 
                    statsWidth,
                    statsHeight
                );

                                
                var statsViewRect = GUILayoutUtility.GetRect(
                    settings.statsWindowWidth,                    
                    settings.statsWindowWidth,
                    settings.graphSize + graphOffset.y,
                    settings.graphSize + graphOffset.y
                );
                
                var statsOffset = Vector2.zero;

                if (statsViewRect.height < statsHeight)
                {
                    statsOffset.x -= 16;
                }

                statsRect.width += statsOffset.x;
                
                _statsScrollPosition = UnityEngine.GUI.BeginScrollView(
                    statsViewRect,
                    _statsScrollPosition,
                    statsRect,
                    false,
                    false
                );
                
                // background
                TreeGUI.Draw.Solid(statsViewRect, TreeGUI.Colors.DarkEditorBackground);

                DrawAssetStatistics(settings, statsRect, stats, statsOffset);
                UnityEngine.GUI.EndScrollView();
            }
        }

        private static void HandleDragHierarchyNodes(
            TSEDataContainer so,
            IHierarchyReadWrite hierarchies,
            IShapeRead shapes,
            TreeGraphSettings settings,
            Vector2 offset)
        {           
            if (Event.current.keyCode == KeyCode.Escape)
            {
                dragNode = null;
            }

            if (dragNode == null)
            {
                _isDragging = false;
                dropNode = null;
            }

            var handleID = GUIUtility.GetControlID(FocusType.Passive);
            var eventType = Event.current.GetTypeForControl(handleID);


            // on left mouse button down
            if ((eventType == EventType.MouseDown) && (Event.current.button == 0))
            {
                var node = GetNodeFromPosition(settings, Event.current.mousePosition, offset, false);

                if (node != null)
                {
                    // start dragging
                    _dragClickPosition = Event.current.mousePosition;
                    dragNode = node;
                    GUIUtility.hotControl = handleID;
                    Event.current.Use();
                }
            }

            if (dragNode != null)
            {
                dropNode = GetDropNode(hierarchies, shapes, settings, offset);

                if ((eventType == EventType.MouseMove) || (eventType == EventType.MouseDrag))
                {
                    var delta = _dragClickPosition - Event.current.mousePosition;
                    if (delta.magnitude > 10.0f)
                    {
                        _isDragging = true;
                    }

                    Event.current.Use();
                }
                else if (eventType == EventType.MouseUp)
                {
                    if (GUIUtility.hotControl == handleID)
                    {
                        // finish dragging
                        if (dropNode != null)
                        {
                            so.RecordUndo(TreeEditMode.UpdateHierarchy);
                            // Relink
                            hierarchies.UpdateHierarchyParent(
                                dragNode.data,
                                dropNode.data
                            );
                            
                            _internalRebuild = true;

                            so.BuildFull();                            
                        }
                        else
                        {
                            // the nodes have not been dropped on a drop node so e.g.
                            // they landed outside of the hierarchy view
                            // ask for a repaint
                            TreeGUI.Repaint();
                        }

                        // clear and exit
                        dragNode = null;
                        dropNode = null;

                        // cleanup drag
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                }
            }
        }

        private static TreeGraphNode GetDropNode(
            IHierarchyRead hierarchies,
            IShapeRead shapes, 
            TreeGraphSettings settings, 
            Vector2 offset)
        {
            var node = GetNodeFromPosition(settings, Event.current.mousePosition, offset, true);

            if (node == null)
            {
                return null;
            }

            // Verify drop-target is valid
            var sourceHierarchy = dragNode.data;
            var targetHierarchy = node.data;

            if (targetHierarchy == sourceHierarchy)

                // Dropping on itself.. do nothing
            {
                return null;
            }

            if (!targetHierarchy.SupportsChildren)

                // Drop target cannot have a sub group
            {
                return null;
            }

            if (!targetHierarchy.SupportsChildType(sourceHierarchy.type))

                // Context.Log.Info("Drop target cannot have subGroups");
            {
                return null;
            }

            if (sourceHierarchy.parentHierarchyID == targetHierarchy.hierarchyID)

                // No need to do anything
                // Context.Log.Info("Drop target already parent of Drag node .. IGNORING");
            {
                return null;
            }

            if (hierarchies.IsAncestor(sourceHierarchy, targetHierarchy))

                // Would create cyclic-dependency
                // Context.Log.Info("Drop target is a subGroup of Drag node");
            {
                return null;
            }

            return node;
        }

        private static Rect GetHierarchyNodeVisRect(TreeGraphSettings settings, Rect rect)
        {
            return new Rect(
                (rect.x + settings.nodeWidth) - settings.visibleIconSize -
                settings.visibleIconPadding,
                rect.y + settings.visibleIconPadding,
                settings.visibleIconSize,
                settings.visibleIconSize
            );
        }

        private static void DrawHierarchyNode(
            TSEDataContainer so,
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            IBasicSelection selection,
            TreeGraphSettings settings, 
            TreeGraphNode node,
            Vector2 offset, 
            float alpha, 
            float fade)
        {
            // fade
            if ((dragNode != null) && _isDragging)
            {
                if (dragNode.data == node.data)
                {
                    alpha = 0.5f;
                    fade = 0.75f;
                }
            }

            UnityEditor.Handles.color = new Color(0.0f, 0.0f, 0.0f, 0.75f * alpha);

            if (EditorGUIUtility.isProSkin)
            {
                UnityEditor.Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.75f * alpha);
            }

            var deltaX = settings.nodeWidth * .5f;
            var deltaY = settings.nodeHeight;

            foreach (var child in node.children)
            {
                var parentPoint = new Vector2(node.position.x, node.position.y) + offset;
                var childPoint = new Vector2(child.position.x, child.position.y) + offset;

                parentPoint.x += deltaX;
                childPoint.x += deltaX;

                if (node.data.IsTrunk)
                {
                    if (child.data.IsRoot)
                    {
                        parentPoint.y += deltaY;
                    }
                    else
                    {
                        childPoint.y += deltaY;
                    }
                }
                else if (node.data.IsRoot)
                {
                    parentPoint.y += deltaY;
                }
                else
                {
                    childPoint.y += deltaY;
                }

                UnityEditor.Handles.DrawLine(parentPoint, childPoint);
            }

            var rect = node.GetRect(settings, offset);

            var nodeBoxIndex = 0;

            if (node == dropNode)

                // hovering over this node which is a valid drop-target
            {
                nodeBoxIndex = 1;
            }
            else if (selectedHierarchy == node.data)
            {
                if (selectedHierarchy != null)
                {
                    nodeBoxIndex = 1;
                }
            }

            UnityEngine.GUI.backgroundColor = new Color(1, 1, 1, alpha);
            UnityEngine.GUI.contentColor = new Color(1,    1, 1, alpha);
            UnityEngine.GUI.Label(rect, GUIContent.none, TreeGUI.Styles.NodeBoxes[nodeBoxIndex]);

            var pinRectTop = new Rect((rect.x + (rect.width / 2f)) - 4.0f, rect.y - 2.0f, 0f, 0f);

            var pinRectBot = new Rect(
                (rect.x + (rect.width / 2f)) - 4.0f,
                (rect.y + rect.height) - 2.0f,
                0f,
                0f
            );

            var iconRect = new Rect(
                rect.x + settings.nodeIconPaddingX,
                (rect.y + rect.height) - settings.nodeIconSize - settings.nodeIconPaddingY,
                settings.nodeIconSize,
                settings.nodeIconSize
            );

            var labelRect = new Rect(
                rect.x,
                (rect.y + rect.height) - settings.countSize - settings.countPadding,
                rect.width - settings.countPadding,
                settings.countSize
            );

            var buttonContent = node.data.GetIcon(!node.data.hidden).GetDefault();

            var visRect = GetHierarchyNodeVisRect(settings, rect);

            var visContent = node.data.hidden
                ? TreeIcons.disabledVisible.Get("Show")
                : TreeIcons.visible.Get("Hide");

            UnityEngine.GUI.contentColor = new Color(1, 1, 1, 0.7f);

            if (UnityEngine.GUI.Button(visRect, visContent, GUIStyle.none))
            {
                node.data.hidden = !node.data.hidden;
                so.SettingsChanged(SettingsUpdateTarget.Mesh);
                
                UnityEngine.GUI.changed = true;
            }

            UnityEngine.GUI.contentColor = Color.white;

            UnityEngine.GUI.contentColor = new Color(1, 1, 1, node.data.hidden ? 0.5f : 1f);

            if ((dragNode == node) | UnityEngine.GUI.Button(iconRect, buttonContent, GUIStyle.none))
            {
                selected = node;
                selection.HierarchyID = node.data.hierarchyID;
            }

            UnityEngine.GUI.contentColor = Color.white;

            // only show top pin if needed
            if (node.data.SupportsChildren)
            {
                UnityEngine.GUI.Label(pinRectTop, GUIContent.none, TreeGUI.Styles.PinLabel);
            }

            var nodeContent = TreeGUI.Content.Tooltip("Shape Count");
            
           nodeContent.text = 
               shapes.GetHierarchyShapeCount(
                    node.data.hierarchyID,
                    true
                ).ToString();

            var labelStyle = TreeGUI.Styles.NodeLabelTop;
            labelStyle.fontSize = settings.countSize;

            UnityEngine.GUI.Label(labelRect, nodeContent, labelStyle);

            UnityEngine.GUI.Label(pinRectBot, GUIContent.none, TreeGUI.Styles.PinLabel);

            foreach (var child in node.children)
            {
                DrawHierarchyNode(so, hierarchies, shapes, selection, settings, child, offset, alpha * fade, fade);
            }

            UnityEngine.GUI.backgroundColor = Color.white;
            UnityEngine.GUI.contentColor = Color.white;
        }

        private static void DrawAssetStatistics(TreeGraphSettings settings, Rect rect, ITreeStatistics asset, Vector2 offset)
        {
            var statistics = asset.GetStatistics();

            var strings = GetAssetStatisticStrings(statistics);

            for (var i = 0; i < strings.Length; i++)
            {
                var labelrect = new Rect(
                    (rect.width/2) - (settings.statsWidth/2) - settings.statsPaddingX,
                    
                    (rect.yMax + offset.y) - settings.statsHeight -
                    ((settings.statsHeight + settings.statsPaddingY) * i) // LOD offset
                    - settings.statsPaddingY,
                    
                    settings.statsWidth,
                    settings.statsHeight
                );

                TreeGUI.Draw.Solid(labelrect, TreeGUI.Colors.BoxBackgroundColor);
                TreeGUI.Draw.Borders(labelrect, 1);
                UnityEngine.GUI.Label(labelrect, strings[i], TreeGUI.Styles.RightAlignedGreyMiniLabel);
            }
        }

        private static string[] GetAssetStatisticStrings(AssetStatistics stats)
        {
            var strings = new string[stats.statistics.Count];

            for (var i = 0; i < stats.statistics.Count; i++)
            {
                var stat = stats.statistics[i];

                strings[i] = ZString.Format(
                    _assetString,
                    i,
                    stat.vertices,
                    stat.triangles,
                    stat.submeshes
                ).Replace("|", "\n");
            }

            return strings;
        }

        private static TreeGraphNode GetNodeFromPosition(TreeGraphSettings settings, Vector2 position, Vector2 offset, bool includeTrunk)
        {
            for (var i = 0; i < _nodes.Count; i++)
            {
                // continue if not in the node's rect
                if (!_nodes[i].GetRect(settings, offset).Contains(position))
                {
                    continue;
                }

                // don't drag from visibility checkbox rect
                if (GetHierarchyNodeVisRect(settings, _nodes[i].GetRect(settings, offset)).Contains(position))
                {
                    continue;
                }

                if (_nodes[i].data.type == TreeComponentType.Trunk)
                {
                    if (!includeTrunk)
                    {
                        continue;                        
                    }
                    
                }

                return _nodes[i];
            }

            return null;
        }
    }
}
