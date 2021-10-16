#region

using System;
using System.Collections.Generic;
using System.Text;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy.Collections;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Simulation.Trees.UI.Graph;
using Appalachia.Simulation.Trees.UI.GUI;
using Appalachia.Simulation.Trees.UI.Selections.State;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.UI.Gizmos
{
    public static class TreeGizmoDrawer
    {
        public static void Draw(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            Transform transform,
            Mesh mesh)
        {
            var treeMatrix = transform.localToWorldMatrix;

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            shapes.RecurseSplines(hierarchies,
                data =>
                {
                    var spline = data.spline;

                    var branchMatrix = treeMatrix * data.shape.effectiveMatrix;

                    // Draw Spline
                    var previousPosition = branchMatrix.MultiplyPoint(SplineModeler.GetPositionAtTime(spline, 0.0f));

                    for (var i = 0; i < TreeGizmoStyle.instance.splineAccuracy; i++)
                    {
                        var t = i / (float) TreeGizmoStyle.instance.splineAccuracy;

                        var position = branchMatrix.MultiplyPoint(SplineModeler.GetPositionAtTime(spline, t));

                        var radius = SplineModeler.GetRadiusWithCollarAtTime(
                            hierarchies, data.shape, data.hierarchy, t);

                        var rotation = SplineModeler.GetRotationAtTime(spline, t);

                        if (TreeGizmoStyle.instance.drawSplines)
                        {
                            var splineColor = TreeGizmoStyle.instance.splineColor;
                            splineColor.a = TreeGizmoStyle.instance.splineTransparency;

                            SmartHandles.DrawLine(position, previousPosition, splineColor);
                        }

                        if (((i % TreeGizmoStyle.instance.splineDiscInterval) == 0) &&
                            TreeGizmoStyle.instance.drawSplineDisks)
                        {
                            var diskInner = TreeGizmoStyle.instance.diskInnerColor;
                            diskInner.a = TreeGizmoStyle.instance.diskInnerTransparency;

                            var diskOuter = TreeGizmoStyle.instance.diskOuterColor;
                            diskOuter.a = TreeGizmoStyle.instance.diskOuterTransparency;

                            SmartHandles.DrawWireDisc(position, rotation.eulerAngles, radius, diskInner);

                            SmartHandles.DrawWireDisc(
                                position,
                                rotation.eulerAngles,
                                TreeGizmoStyle.instance.diskOuterRadiusMultiplier * radius,
                                diskOuter
                            );
                        }

                        previousPosition = position;
                    }
                }
            );
            
            if (TreeGizmoStyle.instance.drawNodes)
            {
                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (!data.shape.exportGeometry)
                        {
                            return;
                        }
                        
                        Color cNode;

                        switch (data.type)
                        {
                            case TreeComponentType.Root:
                                cNode = TreeGizmoStyle.instance.rootColor;
                                break;
                            case TreeComponentType.Trunk:
                                cNode = TreeGizmoStyle.instance.trunkColor;
                                break;
                            case TreeComponentType.Branch:
                                cNode = TreeGizmoStyle.instance.branchColor;
                                break;
                            case TreeComponentType.Leaf:
                                cNode = TreeGizmoStyle.instance.leafColor;
                                break;
                            case TreeComponentType.Fruit:
                                cNode = TreeGizmoStyle.instance.fruitColor;
                                break;
                            case TreeComponentType.Knot:
                                cNode = TreeGizmoStyle.instance.knotColor;
                                break;
                            case TreeComponentType.Fungus:
                                cNode = TreeGizmoStyle.instance.fungusColor;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var nodeMatrix = treeMatrix * data.shape.effectiveMatrix;
                        cNode.a = TreeGizmoStyle.instance.nodeOpacity;

                        SmartHandles.DrawWireCube(
                            nodeMatrix.MultiplyPoint(Vector3.zero),
                            Vector3.one * (TreeGizmoStyle.instance.nodeSize * data.shape.effectiveSize),
                            cNode
                        );
                    }
                );
            }

            if (TreeGizmoStyle.instance.drawGroundOffset)
            {
                var baseColor = TreeGizmoStyle.instance.groundOffsetBaseColor;
                baseColor.a = TreeGizmoStyle.instance.groundOffsetBaseOpacity;

                SmartHandles.DrawSolidDisc(
                    transform.position,
                    Vector3.up,
                    TreeGizmoStyle.instance.groundOffsetRadius,
                    baseColor
                );

                var ringColor = TreeGizmoStyle.instance.groundOffsetRingColor;
                ringColor.a = TreeGizmoStyle.instance.groundOffsetRingOpacity;

                for (var i = 0; i < TreeGizmoStyle.instance.groundOffsetRings; i++)
                {
                    var time = i / TreeGizmoStyle.instance.groundOffsetRings;
                    var radiusStep = TreeGizmoStyle.instance.groundOffsetRadius /
                        TreeGizmoStyle.instance.groundOffsetRings;
                    var radius = i * radiusStep;

                    SmartHandles.DrawWireDisc(transform.position, Vector3.up, radius, ringColor * (1 - time));
                }
            }

            if (TreeGizmoStyle.instance.drawNormals && (mesh != null))
            {
                DrawNormals(hierarchies, shapes, transform, mesh);
            }
            

            if (TreeGizmoStyle.instance.drawShapeLabels)
            {
                DrawShapeLabels(hierarchies, shapes, transform, mesh);
            }
        }
     
          public static void DrawWeldGizmo(
            Vector3 rayOrigin,
            Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Vector3 edge_ab = v1 - v0;
            Vector3 edge_ac = v2 - v0;

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 normal = Vector3.Cross(edge_ab, edge_ac);

            var target = (v0 + v1 + v2) / 3f;
            var direction = (target - rayOrigin).normalized;

            var ray1Color = Color.black;
            var ray2Color = Color.black;

            float dot = Vector3.Dot(direction, normal);

            var frontFaceHit = dot < 0.0f;

            if (dot == 0.0f) // ray is perpendicular to triangle plane
            {
                ray1Color = Color.white;
            }
            else if (frontFaceHit)
            {
                ray1Color = Color.green;
            }
            else if (!frontFaceHit) // ray is going in same
            {
                ray1Color = Color.blue;
            }
          
            SmartHandles.DrawLine(rayOrigin, target, ray1Color);
        }
          
          public static object IntersectRayTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, bool bidirectional)
          {
              Vector3 ab = v1 - v0;
              Vector3 ac = v2 - v0;

              // Compute triangle normal. Can be precalculated or cached if
              // intersecting multiple segments against the same triangle
              Vector3 n = Vector3.Cross(ab, ac);

              // Compute denominator d. If d <= 0, segment is parallel to or points
              // away from triangle, so exit early
              float d = Vector3.Dot(-ray.direction, n);
              if (d <= 0.0f) return null;

              // Compute intersection t value of pq with plane of triangle. A ray
              // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
              // dividing by d until intersection has been found to pierce triangle
              Vector3 ap = ray.origin - v0;
              float t = Vector3.Dot(ap, n);
              if ((t < 0.0f) && (!bidirectional)) return null;
              //if (t > d) return null; // For segment; exclude this code line for a ray test

              // Compute barycentric coordinate components and test if within bounds
              Vector3 e = Vector3.Cross(-ray.direction, ap);
              float v = Vector3.Dot(ac, e);
              if ((v < 0.0f) || (v > d)) return null;

              float w = -Vector3.Dot(ab, e);
              if ((w < 0.0f) || ((v + w) > d)) return null;

              // Segment/ray intersects triangle. Perform delayed division and
              // compute the last barycentric coordinate component
              float ood = 1.0f / d;
              t *= ood;
              v *= ood;
              w *= ood;
              float u = 1.0f - v - w;

              RaycastHit hit = new RaycastHit();

              hit.point = ray.origin + (t * ray.direction);
              hit.distance = t;
              hit.barycentricCoordinate = new Vector3(u, v, w);
              hit.normal = Vector3.Normalize(n);

              return hit;
          }

          public static void DrawShapeLabels(
              IHierarchyRead hierarchies,
              IShapeRead shapes,
              Transform transform,
              Mesh mesh)
          {
              var builder = new StringBuilder();

              var rootPosition = transform.position;
              
              foreach (var shape in shapes)
              {
                  builder.Clear();
                  
                  if (shape.hierarchyID != TreeGraph.selected?.data?.hierarchyID)
                  {
                      continue;
                  }
                  
                  if (!shape.exportGeometry)
                  {
                      continue;
                  }
                  
                  if ((shape.type == TreeComponentType.Trunk) || (shape.type == TreeComponentType.Branch))
                  {
                      var s = shape as BarkShapeData;

                      var point = SplineModeler.GetPositionAtTime(s.spline, .5f);

                      var c = UnityEngine.GUI.color;
                      var bg = UnityEngine.GUI.backgroundColor;
                      var cc = UnityEngine.GUI.backgroundColor;

                      UnityEngine.GUI.contentColor = TreeGUI.Colors.LightBeige;
                      UnityEngine.GUI.backgroundColor = TreeGUI.Colors.DarkGray;
                      
                      var position = shape.effectiveMatrix.MultiplyPoint(point);

                      if (hierarchies is LogHierarchies)
                      {
                          var id = TreeSpeciesEditorSelection.instance.log.id;
                          var log = TreeSpeciesEditorSelection.instance.log.selection.selected;
                          var instance = log.GetLogInstance(id);
                              
                          builder.AppendLine($"Diameter: {instance.actualDiameter:0.00}m");
                          builder.AppendLine($"Length: {instance.actualLength:0.00}m");
                          builder.Append($"Volume: {instance.volume:0.000}m");
                          
                          var centerP = rootPosition +  instance.center;
                          var centerMassP = rootPosition + instance.centerOfMass;
                          
                          SmartHandles.DrawSphere(centerP, .05f, Color.white);
                          SmartHandles.DrawSphere(centerMassP, .05f, Color.green);

                          position = rootPosition + instance.centerOfMass;
                      }
                      else
                      {
                          builder.AppendLine($"Diameter: {(shape.effectiveSize*2):0.00}m");
                          builder.Append($"Length: {shape.effectiveLength:0.00}m");                          
                      }
                      
                      UnityEditor.Handles.Label(position, builder.ToString(), TreeGUI.Styles.TreeLabel);

                      UnityEngine.GUI.backgroundColor = bg;
                      UnityEngine.GUI.contentColor = cc;
                      UnityEngine.GUI.color = c;
                  }
              }
          }

          

        public static void DrawNormals(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            Transform transform,
            Mesh mesh)
        {            
            switch (TreeGizmoStyle.instance.normalStyle)
            {
                case TreeGizmoStyle.DrawNormalStyle.All:
                    DrawNormals_All(transform, mesh);
                    break;
                case TreeGizmoStyle.DrawNormalStyle.Caps:
                    DrawNormals_Caps(hierarchies, shapes, transform, mesh);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
       

        public static void DrawNormals_All(Transform transform, Mesh mesh)
        {
            DrawNormals_Range(transform, mesh, TreeGizmoStyle.instance.normalOffset, TreeGizmoStyle.instance.normalOffset+TreeGizmoStyle.instance.normalLimit);
        }
        
        public static void DrawNormals_Range(
            Transform transform, Mesh mesh, int vertexStart, int vertexEnd)
        {
            vertexEnd = Mathf.Clamp(vertexEnd, vertexStart, vertexStart + TreeGizmoStyle.instance.normalLimit);

            var vertices = mesh.vertices;
            var normals = mesh.normals;
            
            for (var i = vertexStart; i < vertexEnd; i++)
            {
                if (i > (mesh.vertexCount - 1))
                {
                    break;
                }
                
                var point = vertices[i];
                var position = point + transform.position;
                var normal = normals[i];
                var target = position + (normal * TreeGizmoStyle.instance.normalLength);

                var color = TreeGizmoStyle.instance.useNormalAsColor
                    ? new Color((normal.x * .5f) + .5f, (normal.y * .5f) + .5f, (normal.z * .5f) + .5f, 1)
                    : TreeGizmoStyle.instance.normalColor;

                SmartHandles.DrawLine(position, target, color);
            }
        }
        

        public static void DrawNormals_Caps(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            Transform transform, Mesh mesh)
        {
            shapes.RecurseSplines(hierarchies,
                data =>
                {
                    var geo = data.shape.geometry[0];

                    var vertices = mesh.vertices;
                    var normals = mesh.normals;
                    var pos = transform.position;
                    
                    foreach (var vertex in geo.actualVertices)
                    {
                        
                        var point = vertices[vertex];
                        var position = point + pos;
                        var normal = normals[vertex];
                        var target = position + (normal * TreeGizmoStyle.instance.normalLength);
                
                        var color = TreeGizmoStyle.instance.useNormalAsColor
                            ? new Color((normal.x * .5f) + .5f, (normal.y * .5f) + .5f, (normal.z * .5f) + .5f, 1)
                            : TreeGizmoStyle.instance.normalColor;

                        SmartHandles.DrawLine(position, target, color);
                    }
                });
        }
       
       

        public static void DrawVertex(
            Transform transform,
            Mesh mesh,
            ref int selectedVertex)
        {
            selectedVertex = Mathf.Clamp(selectedVertex, 0, mesh.vertexCount - 1);

            var builder = new StringBuilder();
            
            var pos = mesh.vertices[selectedVertex];
            var nor = mesh.normals[selectedVertex];
            var tan = mesh.tangents[selectedVertex];
            var col = mesh.colors[selectedVertex];

            builder.AppendLine($"Position: {pos.x:0.000}, {pos.y:0.000}, {pos.z:0.000}");
            builder.AppendLine($"Normal:   {nor.x:0.000}, {nor.y:0.000}, {nor.z:0.000}");
            builder.AppendLine($"Tangent:  {tan.x:0.000}, {tan.y:0.000}, {tan.z:0.000}, {tan.w:0.000}");
            builder.AppendLine($"Color:    {col.r:0.000}, {col.g:0.000}, {col.b:0.000}, {col.a:0.000}");
            
            var uv = new List<Vector4>();
            
            for (var i = 0; i < 8; i++)
            {
                mesh.GetUVs(i, uv);

                if (uv.Count > selectedVertex)
                {
                    var uvid = i == 0 ? 0 : i + 1;
                    var uvx = uv[selectedVertex];
                    builder.AppendLine($"UV{uvid}:      {uvx.x:0.000}, {uvx.y:0.000}, {uvx.z:0.000}, {uvx.w:0.000}");
                }
            }


            var labelPosition = transform.position + pos;
            
            var bg = UnityEngine.GUI.backgroundColor;
            var cc = UnityEngine.GUI.contentColor;

            UnityEngine.GUI.contentColor = TreeGUI.Colors.LightBeige;
            UnityEngine.GUI.backgroundColor = TreeGUI.Colors.DarkGray;

            UnityEditor.Handles.Label(labelPosition, builder.ToString(), TreeGUI.Styles.TreeLabel);

            UnityEngine.GUI.backgroundColor = bg;
            UnityEngine.GUI.contentColor = cc;
        }

        
        /*
         *   void OnSceneGUI()
        {
            // make sure it's a tree
            Tree tree = target as Tree;
            TreeData treeData = GetTreeData(tree);
            if (!treeData)
                return;

// make sure selection is ok
            VerifySelection(treeData);
            if (s_SelectedGroup == null)
            {
                return;
            }
    
            // Check for hotkey event
            OnCheckHotkeys(treeData, true);
    
            Transform treeTransform = tree.transform;
            Matrix4x4 treeMatrix = tree.transform.localToWorldMatrix;
    
            Event evt = Event.current;
    
            #region Do Root UnityEditor.Handles
            if (s_SelectedGroup.GetType() == typeof(TreeGroupRoot))
            {
                Tools.s_Hidden = false;
    
                UnityEditor.Handles.color = s_NormalColor;
                UnityEditor.Handles.DrawWireDisc(treeTransform.position, treeTransform.up, treeData.root.rootSpread);
            }
            else
            {
                Tools.s_Hidden = true;
    
                UnityEditor.Handles.color = UnityEditor.Handles.secondaryColor;
                UnityEditor.Handles.DrawWireDisc(treeTransform.position, treeTransform.up, treeData.root.rootSpread);
            }
            #endregion
    
            #region Do Branch UnityEditor.Handles
            if (s_SelectedGroup != null && s_SelectedGroup.GetType() == typeof(TreeGroupBranch))
            {
                EventType oldEventType = evt.type;
    
                // we want ignored mouse up events to check for dragging off of scene view
                if (evt.type == EventType.Ignore && evt.rawType == EventType.MouseUp)
                    oldEventType = evt.rawType;
    
                // Draw all splines in a single GL.Begin / GL.End
                UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.zero);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(GL.LINES);
                for (int nodeIndex = 0; nodeIndex < s_SelectedGroup.nodeIDs.Length; nodeIndex++)
                {
                    TreeNode branch = treeData.GetNode(s_SelectedGroup.nodeIDs[nodeIndex]);
                    TreeSpline spline = branch.spline;
                    if (spline == null) continue;
    
                    UnityEditor.Handles.color = (branch == s_SelectedNode) ? s_NormalColor : s_GroupColor;
    
                    Matrix4x4 branchMatrix = treeMatrix * branch.matrix;
    
                    // Draw Spline
                    Vector3 prevPos = branchMatrix.MultiplyPoint(spline.GetPositionAtTime(0.0f));
    
                    GL.Color(Handles.color);
                    for (float t = 0.01f; t <= 1.0f; t += 0.01f)
                    {
                        Vector3 currPos = branchMatrix.MultiplyPoint(spline.GetPositionAtTime(t));
                        //Handles.DrawLine(prevPos, currPos);
    
                        GL.Vertex(prevPos);
                        GL.Vertex(currPos);
    
                        prevPos = currPos;
                    }
                }
                GL.End();
    
                // Draw all UnityEditor.Handles
                for (int nodeIndex = 0; nodeIndex < s_SelectedGroup.nodeIDs.Length; nodeIndex++)
                {
                    TreeNode branch = treeData.GetNode(s_SelectedGroup.nodeIDs[nodeIndex]);
                    TreeSpline spline = branch.spline;
                    if (spline == null) continue;
    
                    UnityEditor.Handles.color = (branch == s_SelectedNode) ? s_NormalColor : s_GroupColor;
    
                    Matrix4x4 branchMatrix = treeMatrix * branch.matrix;
    
    
                    // Draw Points
                    for (int pointIndex = 0; pointIndex < spline.nodes.Length; pointIndex++)
                    {
                        SplineNode point = spline.nodes[pointIndex];
                        Vector3 worldPos = branchMatrix.MultiplyPoint(point.point);
                        float UnityEditor.Handlesize = HandleUtility.GetHandleSize(worldPos) * 0.08f;
    
                        UnityEditor.Handles.color = UnityEditor.Handles.centerColor;
    
                        int oldKeyboardControl = GUIUtility.keyboardControl;
    
                        switch (editMode)
                        {
                            case EditMode.MoveNode:
                                if (pointIndex == 0)
                                    worldPos = UnityEditor.Handles.FreeMoveHandle(worldPos, Quaternion.identity, UnityEditor.Handlesize, Vector3.zero, UnityEditor.Handles.CircleHandleCap);
                                else
                                    worldPos = UnityEditor.Handles.FreeMoveHandle(worldPos, Quaternion.identity, UnityEditor.Handlesize, Vector3.zero, UnityEditor.Handles.RectangleHandleCap);
    
                                // check if point was just selected
                                if (oldEventType == EventType.MouseDown && evt.type == EventType.Used && oldKeyboardControl != GUIUtility.keyboardControl)
                                {
                                    SelectNode(branch, treeData);
                                    s_SelectedPoint = pointIndex;
                                    m_StartPointRotation = MathUtils.QuaternionFromMatrix(branchMatrix) * point.rot;
                                }
    
                                if ((oldEventType == EventType.MouseDown || oldEventType == EventType.MouseUp) && evt.type == EventType.Used)
                                {
                                    m_StartPointRotation = MathUtils.QuaternionFromMatrix(branchMatrix) * point.rot;
                                }
    
                                // check if we're done dragging handle (so we can change ids)
                                if (oldEventType == EventType.MouseUp && evt.type == EventType.Used)
                                {
                                    if (treeData.isInPreviewMode)
                                    {
                                        // We need a complete rebuild..
                                        UpdateMesh(tree);
                                    }
                                }
    
                                if (GUI.changed)
                                {
                                    Undo.RegisterCompleteObjectUndo(treeData, "Move");
    
                                    s_SelectedGroup.Lock();
    
                                    // Snap root branch to parent (overrides position received from UnityEditor.Handles, uses mouse raycasts instead)
                                    float angle = branch.baseAngle;
                                    if (pointIndex == 0)
                                    {
                                        TreeNode parentNode = treeData.GetNode(s_SelectedNode.parentID);
    
                                        Ray mouseRay = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
                                        float hitDist = 0f;
    
                                        if (parentNode != null)
                                        {
                                            TreeGroup parentGroup = treeData.GetGroup(s_SelectedGroup.parentGroupID);
                                            if (parentGroup.GetType() == typeof(TreeGroupBranch))
                                            {
                                                // Snap to parent branch
                                                s_SelectedNode.offset = FindClosestOffset(treeData, treeMatrix, parentNode,
                                                    mouseRay, ref angle);
                                                worldPos = branchMatrix.MultiplyPoint(Vector3.zero);
                                            }
                                            else if (parentGroup.GetType() == typeof(TreeGroupRoot))
                                            {
                                                // Snap to ground
                                                Vector3 mid = treeMatrix.MultiplyPoint(Vector3.zero);
                                                Plane p = new Plane(treeMatrix.MultiplyVector(Vector3.up), mid);
                                                if (p.Raycast(mouseRay, out hitDist))
                                                {
                                                    worldPos = mouseRay.origin + mouseRay.direction * hitDist;
                                                    Vector3 delta = worldPos - mid;
                                                    delta = treeMatrix.inverse.MultiplyVector(delta);
                                                    s_SelectedNode.offset =
                                                        Mathf.Clamp01(delta.magnitude / treeData.root.rootSpread);
                                                    angle = Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
                                                    worldPos = branchMatrix.MultiplyPoint(Vector3.zero);
                                                }
                                                else
                                                {
                                                    worldPos = branchMatrix.MultiplyPoint(point.point);
                                                }
                                            }
                                        }
                                    }
    
                                    branch.baseAngle = angle;
                                    point.point = branchMatrix.inverse.MultiplyPoint(worldPos);
    
                                    spline.UpdateTime();
                                    spline.UpdateRotations();
    
                                    PreviewMesh(tree);
    
                                    GUI.changed = false;
                                }
    
                                break;
                            case EditMode.RotateNode:
                                UnityEditor.Handles.FreeMoveHandle(worldPos, Quaternion.identity, UnityEditor.Handlesize, Vector3.zero, UnityEditor.Handles.CircleHandleCap);
    
                                // check if point was just selected
                                if (oldEventType == EventType.MouseDown && evt.type == EventType.Used && oldKeyboardControl != GUIUtility.keyboardControl)
                                {
                                    SelectNode(branch, treeData);
                                    s_SelectedPoint = pointIndex;
                                    m_GlobalToolRotation = Quaternion.identity;
                                    m_TempSpline = new TreeSpline(branch.spline);
                                }
    
                                GUI.changed = false;
                                break;
                            case EditMode.Freehand:
                                UnityEditor.Handles.FreeMoveHandle(worldPos, Quaternion.identity, UnityEditor.Handlesize, Vector3.zero, UnityEditor.Handles.CircleHandleCap);
    
                                // check if point was just selected
                                if (oldEventType == EventType.MouseDown && evt.type == EventType.Used && oldKeyboardControl != GUIUtility.keyboardControl)
                                {
                                    Undo.RegisterCompleteObjectUndo(treeData, "Free Hand");
    
                                    SelectNode(branch, treeData);
                                    s_SelectedPoint = pointIndex;
                                    s_StartPosition = worldPos;
    
                                    int cutCount = Mathf.Max(2, s_SelectedPoint + 1);
                                    branch.spline.SetNodeCount(cutCount);
    
                                    evt.Use();
                                }
    
                                if (s_SelectedPoint == pointIndex && s_SelectedNode == branch && oldEventType == EventType.MouseDrag)
                                {
                                    Ray mouseRay = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
    
                                    // In draw mode.. move current spline node to mouse position
                                    // trace ray on a plane placed at the original position of the selected node and aligned to the camera..
                                    Vector3 camFront = Camera.current.transform.forward;
                                    Plane p = new Plane(camFront, s_StartPosition);
                                    float hitDist = 0.0f;
    
                                    if (p.Raycast(mouseRay, out hitDist))
                                    {
                                        Vector3 hitPos = mouseRay.origin + hitDist * mouseRay.direction;
    
                                        if (s_SelectedPoint == 0)
                                        {
                                            s_SelectedPoint = 1;
                                        }
    
                                        // lock shape
                                        s_SelectedGroup.Lock();
    
                                        s_SelectedNode.spline.nodes[s_SelectedPoint].point = (branchMatrix.inverse).MultiplyPoint(hitPos);
    
                                        Vector3 delta = s_SelectedNode.spline.nodes[s_SelectedPoint].point -
                                            s_SelectedNode.spline.nodes[s_SelectedPoint - 1].point;
                                        if (delta.magnitude > 1.0f)
                                        {
                                            s_SelectedNode.spline.nodes[s_SelectedPoint].point =
                                                s_SelectedNode.spline.nodes[s_SelectedPoint - 1].point + delta;
                                            // move on to the next node
                                            s_SelectedPoint++;
                                            if (s_SelectedPoint >= s_SelectedNode.spline.nodes.Length)
                                            {
                                                s_SelectedNode.spline.AddPoint(branchMatrix.inverse.MultiplyPoint(hitPos), 1.1f);
                                            }
                                        }
    
                                        s_SelectedNode.spline.UpdateTime();
                                        s_SelectedNode.spline.UpdateRotations();
    
                                        // Make sure changes are saved
                                        // EditorUtility.SetDirty( selectedGroup );
                                        evt.Use();
                                        PreviewMesh(tree);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
    
                        // Handle undo
                        if (s_SelectedPoint == pointIndex && s_SelectedNode == branch && m_StartPointRotationDirty)
                        {
                            spline.UpdateTime();
                            spline.UpdateRotations();
    
                            m_StartPointRotation = MathUtils.QuaternionFromMatrix(branchMatrix) * point.rot;
                            m_GlobalToolRotation = Quaternion.identity;
    
                            m_StartPointRotationDirty = false;
                        }
                    }
                }
    
                if (oldEventType == EventType.MouseUp && editMode == EditMode.Freehand)
                {
                    s_SelectedPoint = -1;
    
                    if (treeData.isInPreviewMode)
                    {
                        // We need a complete rebuild..
                        UpdateMesh(tree);
                    }
                }
    
                // Draw Position UnityEditor.Handles for Selected Point
                if (s_SelectedPoint > 0 && editMode == EditMode.MoveNode && s_SelectedNode != null)
                {
                    TreeNode branch = s_SelectedNode;
                    SplineNode point = branch.spline.nodes[s_SelectedPoint];
                    Matrix4x4 branchMatrix = treeMatrix * branch.matrix;
    
                    Vector3 worldPos = branchMatrix.MultiplyPoint(point.point);
                    Quaternion toolRotation = Quaternion.identity;
                    if (Tools.pivotRotation == PivotRotation.Local)
                    {
                        if (oldEventType == EventType.MouseUp || oldEventType == EventType.MouseDown)
                            m_StartPointRotation = MathUtils.QuaternionFromMatrix(branchMatrix) * point.rot;
                        toolRotation = m_StartPointRotation;
                    }
                    worldPos = DoPositionHandle(worldPos, toolRotation, false);
    
                    if (GUI.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(treeData, "Move");
    
                        s_SelectedGroup.Lock();
    
                        point.point = branchMatrix.inverse.MultiplyPoint(worldPos);
    
                        branch.spline.UpdateTime();
                        branch.spline.UpdateRotations();
    
                        PreviewMesh(tree);
                    }
    
                    // check if we're done dragging handle (so we can change ids)
                    if (oldEventType == EventType.MouseUp && evt.type == EventType.Used)
                    {
                        if (treeData.isInPreviewMode)
                        {
                            // We need a complete rebuild..
                            UpdateMesh(tree);
                        }
                    }
                }
    
                // Draw Rotation UnityEditor.Handles for selected Point
                if (s_SelectedPoint >= 0 && editMode == EditMode.RotateNode && s_SelectedNode != null)
                {
                    TreeNode branch = s_SelectedNode;
                    SplineNode point = branch.spline.nodes[s_SelectedPoint];
                    Matrix4x4 branchMatrix = treeMatrix * branch.matrix;
    
                    if (m_TempSpline == null)
                    {
                        m_TempSpline = new TreeSpline(branch.spline);
                    }
    
                    Vector3 worldPos = branchMatrix.MultiplyPoint(point.point);
                    Quaternion rotation = Quaternion.identity;
                    m_GlobalToolRotation = UnityEditor.Handles.RotationHandle(m_GlobalToolRotation, worldPos);
                    rotation = m_GlobalToolRotation;
    
                    if (GUI.changed)
                    {
                        Undo.RegisterCompleteObjectUndo(treeData, "Move");
    
                        s_SelectedGroup.Lock();
    
                        for (int i = s_SelectedPoint + 1; i < m_TempSpline.nodes.Length; i++)
                        {
                            Vector3 pointVector = (m_TempSpline.nodes[i].point - point.point);
                            pointVector = branchMatrix.MultiplyVector(pointVector);
                            pointVector = rotation * pointVector;
                            pointVector = branchMatrix.inverse.MultiplyVector(pointVector);
                            Vector3 newPos = point.point + pointVector;
                            s_SelectedNode.spline.nodes[i].point = newPos;
                        }
    
                        branch.spline.UpdateTime();
                        branch.spline.UpdateRotations();
    
                        PreviewMesh(tree);
                    }
    
                    // check if we're done dragging handle (so we can change ids)
                    if (oldEventType == EventType.MouseUp && evt.type == EventType.Used)
                    {
                        if (treeData.isInPreviewMode)
                        {
                            // We need a complete rebuild..
                            UpdateMesh(tree);
                        }
                    }
                }
            }
            #endregion
    
            #region Do Leaf UnityEditor.Handles
            if (s_SelectedGroup != null && s_SelectedGroup.GetType() == typeof(TreeGroupLeaf))
            {
                for (int nodeIndex = 0; nodeIndex < s_SelectedGroup.nodeIDs.Length; nodeIndex++)
                {
                    TreeNode leaf = treeData.GetNode(s_SelectedGroup.nodeIDs[nodeIndex]);
                    Matrix4x4 leafMatrix = treeMatrix * leaf.matrix;
                    Vector3 worldPos = leafMatrix.MultiplyPoint(Vector3.zero);
                    float UnityEditor.Handlesize = HandleUtility.GetHandleSize(worldPos) * 0.08f;
    
                    UnityEditor.Handles.color = UnityEditor.Handles.centerColor;
    
                    EventType oldEventType = evt.type;
                    int oldKeyboardControl = GUIUtility.keyboardControl;
    
                    switch (editMode)
                    {
                        case EditMode.MoveNode:
                            UnityEditor.Handles.FreeMoveHandle(worldPos, Quaternion.identity, UnityEditor.Handlesize, Vector3.zero, UnityEditor.Handles.CircleHandleCap);
    
                            // check if point was just selected
                            if (oldEventType == EventType.MouseDown && evt.type == EventType.Used && oldKeyboardControl != GUIUtility.keyboardControl)
                            {
                                SelectNode(leaf, treeData);
                                m_GlobalToolRotation = MathUtils.QuaternionFromMatrix(leafMatrix);
                                m_StartMatrix = leafMatrix;
                                m_StartPointRotation = leaf.rotation;
                                m_LockedWorldPos = new Vector3(m_StartMatrix.m03, m_StartMatrix.m13, m_StartMatrix.m23);
                            }
    
                            // check if we're done dragging handle (so we can change ids)
                            if (oldEventType == EventType.MouseUp && evt.type == EventType.Used)
                            {
                                if (treeData.isInPreviewMode)
                                {
                                    // We need a complete rebuild..
                                    UpdateMesh(tree);
                                }
                            }
    
                            if (GUI.changed)
                            {
                                s_SelectedGroup.Lock();
    
                                TreeNode parentNode = treeData.GetNode(leaf.parentID);
                                TreeGroup parentGroup = treeData.GetGroup(s_SelectedGroup.parentGroupID);
    
                                Ray mouseRay = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
                                float hitDist = 0f;
                                float angle = leaf.baseAngle;
    
                                if (parentGroup.GetType() == typeof(TreeGroupBranch))
                                {
                                    // Snap to branch
                                    leaf.offset = FindClosestOffset(treeData, treeMatrix, parentNode, mouseRay, ref angle);
                                    leaf.baseAngle = angle;
    
                                    PreviewMesh(tree);
                                }
                                else if (parentGroup.GetType() == typeof(TreeGroupRoot))
                                {
                                    // Snap to ground
                                    Vector3 mid = treeMatrix.MultiplyPoint(Vector3.zero);
                                    Plane p = new Plane(treeMatrix.MultiplyVector(Vector3.up), mid);
                                    if (p.Raycast(mouseRay, out hitDist))
                                    {
                                        worldPos = mouseRay.origin + mouseRay.direction * hitDist;
                                        Vector3 delta = worldPos - mid;
                                        delta = treeMatrix.inverse.MultiplyVector(delta);
                                        leaf.offset =
                                            Mathf.Clamp01(delta.magnitude / treeData.root.rootSpread);
                                        angle = Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
                                    }
                                    leaf.baseAngle = angle;
    
                                    PreviewMesh(tree);
                                }
                            }
                            break;
    
                        case EditMode.RotateNode:
                        {
                            UnityEditor.Handles.FreeMoveHandle(worldPos, Quaternion.identity, UnityEditor.Handlesize, Vector3.zero, UnityEditor.Handles.CircleHandleCap);
    
                            // check if point was just selected
                            if (oldEventType == EventType.MouseDown && evt.type == EventType.Used && oldKeyboardControl != GUIUtility.keyboardControl)
                            {
                                SelectNode(leaf, treeData);
                                m_GlobalToolRotation = MathUtils.QuaternionFromMatrix(leafMatrix);
                                m_StartMatrix = leafMatrix;
                                m_StartPointRotation = leaf.rotation;
                                m_LockedWorldPos = new Vector3(leafMatrix.m03, leafMatrix.m13, leafMatrix.m23);
                            }
    
                            // Rotation handle for selected leaf
                            if (s_SelectedNode == leaf)
                            {
                                oldEventType = evt.GetTypeForControl(GUIUtility.hotControl);
                                m_GlobalToolRotation = UnityEditor.Handles.RotationHandle(m_GlobalToolRotation, m_LockedWorldPos);
    
                                // check if we're done dragging handle (so we can change ids)
                                if (oldEventType == EventType.MouseUp && evt.type == EventType.Used)
                                {
                                    // update position of gizmo
                                    m_LockedWorldPos = new Vector3(leafMatrix.m03, leafMatrix.m13, leafMatrix.m23);
    
                                    if (treeData.isInPreviewMode)
                                    {
                                        // We need a complete rebuild..
                                        UpdateMesh(tree);
                                    }
                                }
    
                                if (GUI.changed)
                                {
                                    s_SelectedGroup.Lock();
                                    Quaternion invStart = Quaternion.Inverse(MathUtils.QuaternionFromMatrix(m_StartMatrix));
                                    leaf.rotation = m_StartPointRotation * (invStart * m_GlobalToolRotation);
                                    MathUtils.QuaternionNormalize(ref leaf.rotation);
                                    PreviewMesh(tree);
                                }
                            }
                        }
                        break;
    
                        default:
                            break;
                    }
                }
            }
            #endregion
        }
         */
    }
}
