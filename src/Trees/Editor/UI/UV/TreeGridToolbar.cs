using Appalachia.Simulation.Trees.Icons;
using UnityEditor;

namespace Appalachia.Simulation.Trees.UI.UV
{
    public static class TreeGridToolbar
    {
        public static Tool tool = Tool.Move;

        public static void Draw<T>(float height, T selection) where T : class
        {
            TreeGUI.Button.Toolbar(
                selection != null,
                tool == Tool.View,
                TreeIcons.grab,
                TreeIcons.disabledGrab,
                "View",
                () => tool = Tool.View,
                TreeGUI.Styles.ButtonLeftSelected,
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(height)
            );

            TreeGUI.Button.Toolbar(
                selection != null,
                tool == Tool.Move,
                TreeIcons.move,
                TreeIcons.disabledMove,
                "Move",
                () => tool = Tool.Move,
                TreeGUI.Styles.ButtonMidSelected,
                TreeGUI.Styles.ButtonMid,
                TreeGUI.Layout.Options.MaxHeight(height)
            );
           
            TreeGUI.Button.Toolbar(
                selection != null,
                tool == Tool.Rotate,
                TreeIcons.rotate,
                TreeIcons.disabledRotate,
                "Rotate",
                () => tool = Tool.Rotate,
                TreeGUI.Styles.ButtonMidSelected,
                TreeGUI.Styles.ButtonMid,
                TreeGUI.Layout.Options.MaxHeight(height)
            );
            
            TreeGUI.Button.Toolbar(
                selection != null,
                tool == Tool.Scale,
                TreeIcons.scale,
                TreeIcons.disabledScale,
                "Scale",
                () => tool = Tool.Scale,
                TreeGUI.Styles.ButtonRightSelected,
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(height)
            );
            
            if (tool == Tool.View)
            {
                Tools.current = Tool.View;
            }
            else
            {
                Tools.current = Tool.None;
            }
        }
        
        public static void SetTool(Tool t)
        {
            tool = t;
        }
    }
}
