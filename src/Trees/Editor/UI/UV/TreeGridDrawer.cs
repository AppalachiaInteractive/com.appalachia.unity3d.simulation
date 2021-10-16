using Appalachia.Core.Shading;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    public static class TreeGridDrawer
    {
        // Remember that Unity GUI coordinates Y origin is the bottom
        private static Vector2 UpperLeft = new Vector2(0f, -1f);
        private static Vector2 UpperRight = new Vector2(1f, -1f);
        private static Vector2 LowerLeft = new Vector2(0f, 0f);
        private static Vector2 LowerRight = new Vector2(1f, 0f);

        private static Rect UVGraphZeroZero = new Rect(0, 0, 40, 40);
        private static Rect UVGraphOneOne = new Rect(0, 0, 40, 40);
        private static Rect UVRectIdentity = new Rect(0, 0, 1, 1);

        // re-usable rect for drawing graphs
        private static Rect r = new Rect(0, 0, 0, 0);

        private static LazyShader _previewShader = new LazyShader("internal/core/trees/TexturePreview");
        private static Material _previewMaterial;

        private static Material previewMaterial
        {
            get
            {
                if (_previewMaterial == null)
                {
                    _previewMaterial = new Material(_previewShader);
                }

                return _previewMaterial;
            }
        }
        
        // Must be called inside GL immediate mode context
        public static void DrawUVGridTexture(
            Texture texture,
            Vector2 center,
            float uvGraphScale,
            Vector2 uvGraphOffset)
        {
            UVRectIdentity.width = TreeUV.UV_GRID_SIZE * uvGraphScale;
            UVRectIdentity.height = UVRectIdentity.width;

            UVRectIdentity.x = center.x + uvGraphOffset.x;
            UVRectIdentity.y = (center.y + uvGraphOffset.y) - UVRectIdentity.height;

            if (texture)
            {
                previewMaterial.SetTexture(GSPL.Get(GSC.GENERAL._MainTex), texture);
                
                EditorGUI.DrawPreviewTexture(UVRectIdentity, texture, previewMaterial, ScaleMode.StretchToFill, 0);
            }
        }
        
        // Must be called inside GL immediate mode context
        public static void DrawUVGridTextureLabels()
        {
            var col = UnityEngine.GUI.color;
            var gridColor = TreeUV.GridColorPrimary;
            gridColor.a = .1f;

            UnityEngine.GUI.color = gridColor;

            UVGraphZeroZero.x = UVRectIdentity.x + 4;
            UVGraphZeroZero.y = UVRectIdentity.y + UVRectIdentity.height + 1;

            UVGraphOneOne.x = UVRectIdentity.x + UVRectIdentity.width + 4;
            UVGraphOneOne.y = UVRectIdentity.y;

            UnityEditor.Handles.BeginGUI();
            UnityEngine.GUI.Label(UVGraphZeroZero, "0, 0");
            UnityEngine.GUI.Label(UVGraphOneOne,   "1, 1");
            UnityEditor.Handles.EndGUI();

            UnityEngine.GUI.color = col;
        }
        
        // Must be called inside GL immediate mode context
        public static void DrawUVGrid(
            Vector2 center,
            float uvGraphScale,
            Vector2 uvGraphOffset)
        {
            var col = UnityEngine.GUI.color;
            var gridColor = TreeUV.GridColorPrimary;
            gridColor.a = .1f;

            if (Event.current.type == EventType.Repaint)
            {
                GL.PushMatrix();
                EditorHandleUtility.handleMaterial.SetPass(0);
                GL.MultMatrix(Handles.matrix);

                GL.Begin(GL.LINES);
                GL.Color(gridColor);

                // Grid temp vars
                var GridLines = 64;
                var StepSize = TreeUV.GRID_SNAP_INCREMENT; // In UV coordinates

                // Exponentially scale grid size
                while ((StepSize * TreeUV.UV_GRID_SIZE * uvGraphScale) < (TreeUV.UV_GRID_SIZE / 10f))
                {
                    StepSize *= 2f;
                }

                // Calculate what offset the grid should be (different from uvGraphOffset in that we always want to render the grid)
                var gridOffset = uvGraphOffset;
                gridOffset.x = gridOffset.x % (StepSize * TreeUV.UV_GRID_SIZE * uvGraphScale); // (uvGridSize * uvGraphScale);
                gridOffset.y = gridOffset.y % (StepSize * TreeUV.UV_GRID_SIZE * uvGraphScale); // (uvGridSize * uvGraphScale);

                Vector2 p0 = Vector2.zero, p1 = Vector2.zero;

                //==== X axis lines
                p0.x = (StepSize * (GridLines / 2f) * TreeUV.UV_GRID_SIZE * uvGraphScale) + center.x + gridOffset.x;
                p1.x = (-StepSize * (GridLines / 2f) * TreeUV.UV_GRID_SIZE * uvGraphScale) + center.x + gridOffset.x;

                for (var i = 0; i < (GridLines + 1); i++)
                {
                    p0.y = (((StepSize * i) - ((GridLines * StepSize) / 2f)) * TreeUV.UV_GRID_SIZE * uvGraphScale) +
                        center.y +
                        gridOffset.y;
                    p1.y = p0.y;

                    GL.Vertex(p0);
                    GL.Vertex(p1);
                }

                // Y axis lines
                p0.y = (StepSize * (GridLines / 2f) * TreeUV.UV_GRID_SIZE * uvGraphScale) + center.y + gridOffset.y;
                p1.y = (-StepSize * (GridLines / 2f) * TreeUV.UV_GRID_SIZE * uvGraphScale) + center.y + gridOffset.y;

                for (var i = 0; i < (GridLines + 1); i++)
                {
                    p0.x = (((StepSize * i) - ((GridLines * StepSize) / 2)) * TreeUV.UV_GRID_SIZE * uvGraphScale) +
                        center.x +
                        gridOffset.x;
                    p1.x = p0.x;

                    GL.Vertex(p0);
                    GL.Vertex(p1);
                }

                GL.Color(Color.gray);

                GL.Vertex(center + (UpperLeft * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);
                GL.Vertex(center + (UpperRight * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);

                GL.Vertex(center + (UpperRight * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);
                GL.Vertex(center + (LowerRight * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);

                GL.Color(TreeUV.proBuilderBlue);

                GL.Vertex(center + (LowerRight * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);
                GL.Vertex(center + (LowerLeft * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);

                GL.Vertex(center + (LowerLeft * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);
                GL.Vertex(center + (UpperLeft * (TreeUV.UV_GRID_SIZE * uvGraphScale)) + uvGraphOffset);

                GL.End();
                GL.PopMatrix(); // Pop pop!
            }

            UnityEngine.GUI.color = col;
        }

        public static void DrawUVGraph(
            Vector2 center,
            float uvGraphScale,
            Vector2 uvGraphOffset,
            LeafUVRect uvRect,
            Color outlineColor,
            Color handleColor,
            bool drawLabels)
        {
            if (uvRect == null)
            {
                return;
            }

            var evt = Event.current;
            
            /*Vector2[] uv;*/
            r.width = TreeUV.DOT_SIZE;
            r.height = TreeUV.DOT_SIZE;

            UnityEngine.GUI.color = handleColor;

            for (var n = 0; n < 4; n++)
            {
                var p = TreeUV.UVToGUIPoint(uvRect.rect[n], center, uvGraphOffset, uvGraphScale);
                r.x = p.x - TreeUV.HALF_DOT;
                r.y = p.y - TreeUV.HALF_DOT;
                UnityEngine.GUI.DrawTexture(r, TreeUV.dot, ScaleMode.ScaleToFit);
            }

            UnityEngine.GUI.color = outlineColor;

            if (evt.type == EventType.Repaint)
            {
                GL.PushMatrix();
                EditorHandleUtility.handleMaterial.SetPass(0);
                GL.MultMatrix(Handles.matrix);

                GL.Begin(GL.LINES);

                UnityEngine.GUI.color = outlineColor;

                for (var n = 0; n < 4; n++)
                {
                    var p1 = TreeUV.UVToGUIPoint(uvRect.rect[n], center, uvGraphOffset, uvGraphScale);
                    var p2 = TreeUV.UVToGUIPoint(uvRect.rect[n == 3 ? 0 : n + 1], center, uvGraphOffset, uvGraphScale);
                    GL.Vertex(p1);
                    GL.Vertex(p2);
                }

                GL.End();

                GL.PopMatrix();
            }

            UnityEditor.Handles.BeginGUI();

            Rect labelRect = new Rect(0, 0, 50, 50);
            
            if (drawLabels)
            {
                for (var n = 0; n < 4; n++)
                {
                    var p0 = uvRect.rect[n];
                    var p1 = TreeUV.UVToGUIPoint(p0, center, uvGraphOffset, uvGraphScale);
                
                    labelRect.x = p1.x + TreeUV.PAD;
                    labelRect.y = p1.y + TreeUV.PAD;

                    UnityEngine.GUI.Label(labelRect, $"{p0.x:F1}, {p0.y:F1}");
                }
            }

            UnityEngine.GUI.color = handleColor;
            
            var pt = .5f * (uvRect.rect[1] + uvRect.rect[2]);
            var pt1 = TreeUV.UVToGUIPoint(pt, center, uvGraphOffset, uvGraphScale);

            labelRect.x = pt1.x + TreeUV.PAD;
            labelRect.y = pt1.y + TreeUV.PAD;

            var originalMatrix = UnityEngine.GUI.matrix;
            GUIUtility.RotateAroundPivot(uvRect.rect.rotation, pt1);

            UnityEngine.GUI.Label(labelRect, $"TOP");

            UnityEngine.GUI.matrix = originalMatrix;

            if (drawLabels)
            {
                var pb = .5f * (uvRect.rect[0] + uvRect.rect[3]);
                var pb1 = TreeUV.UVToGUIPoint(pb, center, uvGraphOffset, uvGraphScale);
                labelRect.x = pb1.x;
                labelRect.y = pb1.y + TreeUV.PAD;
                UnityEngine.GUI.Label(labelRect, $"{uvRect.rect.size.x:F1}");

                var pr = .5f * (uvRect.rect[2] + uvRect.rect[3]);
                var pr1 = TreeUV.UVToGUIPoint(pr, center, uvGraphOffset, uvGraphScale);
                labelRect.x = pr1.x + TreeUV.PAD;
                labelRect.y = pr1.y;
                UnityEngine.GUI.Label(labelRect, $"{uvRect.rect.size.y:F1}");
                
            }

            UnityEditor.Handles.EndGUI();
        }
    }
}
