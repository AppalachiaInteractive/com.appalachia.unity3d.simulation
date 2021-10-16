using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    public static class TreeUV
    {
        public const int DOT_SIZE = 6;
        public const int HALF_DOT = 3;
        public const int LEFT_MOUSE_BUTTON = 0;
        public const int RIGHT_MOUSE_BUTTON = 1;
        public const int MIDDLE_MOUSE_BUTTON = 2;
        public const int PAD = 4;
        public const float SCROLL_MODIFIER = 1f;
        public const float ALT_SCROLL_MODIFIER = .07f;
        public const int HANDLE_SIZE = 96;
        public const int MIN_ACTION_WINDOW_SIZE = 128;
        public const float MIN_GRAPH_SCALE = .0001f;
        public const float MAX_GRAPH_SCALE = 250f;
        public const float MAX_GRAPH_SCALE_SCROLL = 20f;
        public const float MAX_PROXIMITY_SNAP_DIST_UV = .15f;
        public const float MAX_PROXIMITY_SNAP_DIST_CANVAS = 12f;
        public const float MIN_DIST_MOUSE_EDGE = 8f;
        public const float GRID_SNAP_INCREMENT = 0.01f;
        public const int UV_GRID_SIZE = 256;
        public const int INSPECTOR_MIN_WIDTH = 240;
        public const int ACTION_WINDOW_WIDTH = 80;
        public const int ACTION_WINDOW_HEIGHT = 200;


        public static bool ControlKey => Event.current.modifiers == EventModifiers.Control;
        public static bool ShiftKey => Event.current.modifiers == EventModifiers.Shift;
        public static bool AltKey => Event.current.modifiers == EventModifiers.Alt;

        
        public static readonly Color DRAG_BOX_COLOR_BASIC = new Color(0f, .7f, 1f, .2f);
        public static readonly Color DRAG_BOX_COLOR_PRO = new Color(0f, .7f, 1f, 1f);

        public static Color DRAG_BOX_COLOR => EditorGUIUtility.isProSkin ? DRAG_BOX_COLOR_PRO : DRAG_BOX_COLOR_BASIC;

        public static readonly Color HOVER_COLOR_MANUAL = new Color(1f, .68f, 0f, .23f);
        public static readonly Color HOVER_COLOR_AUTO = new Color(0f, 1f, 1f, .23f);

        public static readonly Color SELECTED_COLOR_MANUAL = new Color(1f, .68f, 0f, .39f);
        public static readonly Color SELECTED_COLOR_AUTO = new Color(0f, .785f, 1f, .39f);

        public static readonly Color proBuilderBlue = new Color(0f, .682f, .937f, 1f);
        public static readonly Color proBuilderLightGray = new Color(.35f, .35f, .35f, .4f);
        public static readonly Color proBuilderDarkGray = new Color(.1f, .1f, .1f, .3f);

        public static Color GridColorPrimary = new Color(1f, 1f, 1f, .2f);
        public static Color BasicBackgroundColor = new Color(.24f, .24f, .24f, 1f);
        public static Color UVColorPrimary = new Color(0.96f, 1f, 0.73f, 1f);
        public static Color UVColorSecondary = new Color(0.75f, 0.49f, 0.12f, 1f);
        public static Color UVHandlesPrimary = new Color(0.78f, 0.34f, 0.31f, 1f);
        public static Color UVHandlesSecondary = new Color(0.78f, 0.31f, 0.1f, 1f);

        private static Texture2D _dot;
        public static Texture2D dot
        {
            get
            {
                if (_dot == null)
                {
                    _dot = EditorGUIUtility.whiteTexture;
                }

                return _dot;
            }
        }
        
        
        // Convert a point on the UV canvas (0,1 scaled to guisize) to a GUI coordinate.
        public static Vector2 UVToGUIPoint(Vector2 v, Vector2 graphCenter, Vector2 graphOffset, float graphScale)
        {
            var p = new Vector2(v.x, -v.y);
            return graphCenter + (p * (UV_GRID_SIZE * graphScale)) + graphOffset;
        }

        public static Vector2 GUIToUVPoint(Vector2 v, Vector2 graphCenter, Vector2 graphOffset, float graphScale)
        {
            var p = (v - (graphCenter + graphOffset)) / (graphScale * UV_GRID_SIZE);
            p.y = -p.y;
            return p;
        }


    }
}
