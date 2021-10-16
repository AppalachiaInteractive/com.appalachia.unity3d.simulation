using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.GUI
{
    public static partial class TreeGUI
    {
        public static class Colors
        {
            public static readonly Color BurntOrange = new Color(0.510f, 0.157f, 0.039f, 1.000f);

            public static readonly Color BurntLightOrange = new Color(0.78f, 0.24f, 0.04f);

            public static readonly Color BurntOrange_AHALF = new Color(0.510f, 0.157f, 0.039f, 0.500f);

            public static readonly Color DarkGreen = new Color(0.11f, 0.23f, 0.12f, 1.000f);

            public static readonly Color LightGreen = new Color(0.17f, 0.51f, 0.35f);

            public static readonly Color DarkRed = new Color(0.4f, 0f, 0.08f, 1.000f);

            public static readonly Color LightRed = new Color(0.7f, 0f, 0.09f);

            public static readonly Color LightBeige = new Color(0.96f, 1.00f, 0.73f, 1.00f);

            public static readonly Color DarkGray = new Color(0.192f, 0.192f, 0.192f, 1f);

            public static readonly Color BorderColor = EditorGUIUtility.isProSkin
                ? new Color(0.11f, 0.11f, 0.11f, 0.8f)
                : new Color(0.38f, 0.38f, 0.38f, 0.6f);

            public static readonly Color BoxBackgroundColor = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.05f)
                : new Color(1f, 1f, 1f, 0.5f);

            public static readonly Color DarkEditorBackground = EditorGUIUtility.isProSkin
                ? new Color(0.192f, 0.192f, 0.192f, 1f)
                : new Color(0.0f, 0.0f, 0.0f, 0.0f);

            public static readonly Color LightBorderColor = new Color32(90, 90, 90, byte.MaxValue);

            public static readonly Color TreeUVMenuBackgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0f, 0f, 0f, 0.25f)
                : new Color(0.87f, 0.87f, 0.87f, 1f);

            public static readonly Color BranchEditorMenuBackgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0f, 0f, 0f, 0.25f)
                : new Color(0.87f, 0.87f, 0.87f, 1f);

            public static readonly Color LogEditorMenuBackgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0f, 0f, 0f, 0.25f)
                : new Color(0.87f, 0.87f, 0.87f, 1f);

            public static readonly Color SidebarMenuBackgroundColor = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.035f)
                : new Color(0.87f, 0.87f, 0.87f, 1f);
        }

    }
}
