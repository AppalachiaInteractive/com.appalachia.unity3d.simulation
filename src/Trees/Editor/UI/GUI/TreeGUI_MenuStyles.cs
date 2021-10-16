using System;
using Sirenix.OdinInspector.Editor;

namespace Appalachia.Simulation.Trees.UI.GUI
{
    public static partial class TreeGUI
    {
        public static class MenuStyles
        {
            private static OdinMenuTreeDrawingConfig sidebarMenuConfig;
            private static OdinMenuTreeDrawingConfig treeUVMenuConfig;
            private static OdinMenuTreeDrawingConfig branchEditorMenuConfig;
            private static OdinMenuTreeDrawingConfig materialSelectionMenuConfig;

            private static OdinMenuStyle sidebarMenuStyle;
            private static OdinMenuStyle treeUVMenuStyle;
            private static OdinMenuStyle branchEditorMenuStyle;
            private static OdinMenuStyle materialSelectionMenuStyle;

            public static OdinMenuTreeDrawingConfig SidebarMenuConfig
            {
                get
                {
                    if (sidebarMenuConfig == null)
                    {
                        sidebarMenuConfig = new OdinMenuTreeDrawingConfig
                        {
                            AutoFocusSearchBar = false,
                            AutoHandleKeyboardNavigation = true,
                            AutoScrollOnSelectionChanged = false,
                            ConfirmSelectionOnDoubleClick = false,
                            DrawSearchToolbar = false,
                            DrawScrollView = true,
                            DefaultMenuStyle = SidebarMenuStyle
                        };
                    }

                    return sidebarMenuConfig;
                }
            }

            public static OdinMenuStyle SidebarMenuStyle
            {
                get
                {
                    if (sidebarMenuStyle == null)
                    {
                        sidebarMenuStyle = new OdinMenuStyle
                        {
                            Height = 36,
                            Offset = 0.00f,
                            IndentAmount = 0.00f,
                            IconSize = 32.00f,
                            IconOffset = 6.00f,
                            NotSelectedIconAlpha = 0.35f,
                            IconPadding = 12.00f,
                            TriangleSize = 17.00f,
                            TrianglePadding = 8.00f,
                            AlignTriangleLeft = false,
                            Borders = true,
                            BorderPadding = 12.00f,
                            BorderAlpha = 0.50f,
                            SelectedColorDarkSkin = Colors.BurntOrange,
                            SelectedColorLightSkin = Colors.BurntOrange,
                            SelectedInactiveColorDarkSkin = Colors.BurntOrange,
                            SelectedInactiveColorLightSkin = Colors.BurntOrange
                        };
                    }

                    return sidebarMenuStyle;
                }
            }

            public static OdinMenuTreeDrawingConfig TreeUVMenuConfig
            {
                get
                {
                    if (treeUVMenuConfig == null)
                    {
                        treeUVMenuConfig = new OdinMenuTreeDrawingConfig
                        {
                            AutoFocusSearchBar = false,
                            AutoHandleKeyboardNavigation = true,
                            AutoScrollOnSelectionChanged = false,
                            ConfirmSelectionOnDoubleClick = false,
                            DrawSearchToolbar = false,
                            DrawScrollView = true,
                            DefaultMenuStyle = TreeUVMenuStyle
                        };
                    }

                    return treeUVMenuConfig;
                }
            }

            public static OdinMenuStyle TreeUVMenuStyle
            {
                get
                {
                    if (treeUVMenuStyle == null)
                    {
                        treeUVMenuStyle = new OdinMenuStyle
                        {
                            Height = 36,
                            Offset = 0.00f,
                            IndentAmount = 0.00f,
                            IconSize = 32.00f,
                            IconOffset = 6.00f,
                            NotSelectedIconAlpha = 0.65f,
                            IconPadding = 12.00f,
                            TriangleSize = 17.00f,
                            TrianglePadding = 8.00f,
                            AlignTriangleLeft = false,
                            Borders = true,
                            BorderPadding = 12.00f,
                            BorderAlpha = 0.50f,
                            SelectedColorDarkSkin = Colors.BurntOrange,
                            SelectedColorLightSkin = Colors.BurntOrange,
                            SelectedInactiveColorDarkSkin = Colors.BurntOrange,
                            SelectedInactiveColorLightSkin = Colors.BurntOrange
                        };
                    }

                    return treeUVMenuStyle;
                }
            }

            public static OdinMenuTreeDrawingConfig BranchEditorMenuConfig
            {
                get
                {
                    if (branchEditorMenuConfig == null)
                    {
                        branchEditorMenuConfig = new OdinMenuTreeDrawingConfig
                        {
                            AutoFocusSearchBar = false,
                            AutoHandleKeyboardNavigation = true,
                            AutoScrollOnSelectionChanged = false,
                            ConfirmSelectionOnDoubleClick = false,
                            DrawSearchToolbar = false,
                            DrawScrollView = true,
                            UseCachedExpandedStates = true,
                            DefaultMenuStyle = BranchEditorMenuStyle
                        };
                    }

                    return branchEditorMenuConfig;
                }
            }

            public static OdinMenuStyle BranchEditorMenuStyle
            {
                get
                {
                    var iconSize = 16f;
                    
                    if ((branchEditorMenuStyle == null) || (Math.Abs(branchEditorMenuStyle.IconSize - iconSize) > float.Epsilon))
                    {
                        branchEditorMenuStyle = new OdinMenuStyle
                        {
                            Height = 8,
                            Offset = 0.00f,
                            IndentAmount = 0.00f,
                            IconSize = iconSize,
                            IconOffset = iconSize*.5f,
                            NotSelectedIconAlpha = 0.65f,
                            IconPadding = 8.00f,
                            TriangleSize = 6.00f,
                            TrianglePadding = 8.00f,
                            AlignTriangleLeft = false,
                            Borders = true,
                            BorderPadding = 12.00f,
                            BorderAlpha = 0.50f,
                            SelectedColorDarkSkin = Colors.BurntOrange,
                            SelectedColorLightSkin = Colors.BurntOrange,
                            SelectedInactiveColorDarkSkin = Colors.BurntOrange,
                            SelectedInactiveColorLightSkin = Colors.BurntOrange
                        };
                    }

                    return branchEditorMenuStyle;
                }
            }

            
            public static OdinMenuTreeDrawingConfig LogEditorSidebarMenuConfig
            {
                get
                {
                    if (sidebarMenuConfig == null)
                    {
                        sidebarMenuConfig = new OdinMenuTreeDrawingConfig
                        {
                            AutoFocusSearchBar = false,
                            AutoHandleKeyboardNavigation = true,
                            AutoScrollOnSelectionChanged = false,
                            ConfirmSelectionOnDoubleClick = false,
                            DrawSearchToolbar = false,
                            DrawScrollView = true,
                            DefaultMenuStyle = SidebarMenuStyle
                        };
                    }

                    return sidebarMenuConfig;
                }
            }

            public static OdinMenuStyle LogEditorSidebarMenuStyle
            {
                get
                {
                    if (sidebarMenuStyle == null)
                    {
                        sidebarMenuStyle = new OdinMenuStyle
                        {
                            Height = 36,
                            Offset = 0.00f,
                            IndentAmount = 0.00f,
                            IconSize = 32.00f,
                            IconOffset = 6.00f,
                            NotSelectedIconAlpha = 0.35f,
                            IconPadding = 12.00f,
                            TriangleSize = 17.00f,
                            TrianglePadding = 8.00f,
                            AlignTriangleLeft = false,
                            Borders = true,
                            BorderPadding = 12.00f,
                            BorderAlpha = 0.50f,
                            SelectedColorDarkSkin = Colors.BurntOrange,
                            SelectedColorLightSkin = Colors.BurntOrange,
                            SelectedInactiveColorDarkSkin = Colors.BurntOrange,
                            SelectedInactiveColorLightSkin = Colors.BurntOrange
                        };
                    }

                    return sidebarMenuStyle;
                }
            }

            public static OdinMenuStyle MaterialSelectionMenuStyle
            {
                get
                {
                    if (materialSelectionMenuStyle == null)
                    {
                        materialSelectionMenuStyle = new OdinMenuStyle
                        {
                            Height = 20,
                            Offset = 16.00f,
                            IndentAmount = 16.00f,
                            IconSize = 32.00f,
                            IconOffset = 6.00f,
                            NotSelectedIconAlpha = 0.35f,
                            IconPadding = 12.00f,
                            TriangleSize = 17.00f,
                            TrianglePadding = 8.00f,
                            AlignTriangleLeft = false,
                            Borders = true,
                            BorderPadding = 12.00f,
                            BorderAlpha = 0.50f,
                            SelectedColorDarkSkin = Colors.BurntOrange,
                            SelectedColorLightSkin = Colors.BurntOrange,
                            SelectedInactiveColorDarkSkin = Colors.BurntOrange,
                            SelectedInactiveColorLightSkin = Colors.BurntOrange
                        };
                    }

                    return materialSelectionMenuStyle;
                }
            }

            public static OdinMenuTreeDrawingConfig MaterialSelectionMenuConfig
            {
                get
                {
                    if (materialSelectionMenuConfig == null)
                    {
                        materialSelectionMenuConfig = new OdinMenuTreeDrawingConfig
                        {
                            AutoFocusSearchBar = false,
                            AutoHandleKeyboardNavigation = true,
                            AutoScrollOnSelectionChanged = false,
                            ConfirmSelectionOnDoubleClick = false,
                            DrawSearchToolbar = false,
                            DrawScrollView = true,
                            UseCachedExpandedStates = true,
                            DefaultMenuStyle = MaterialSelectionMenuStyle
                        };
                    }

                    return materialSelectionMenuConfig;
                }
            }
        }

    }
}
