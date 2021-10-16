using System;
using System.Linq;
using Appalachia.Simulation.Trees.UI.GUI;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    public class TreeEditorSidebarMenu : OdinMenuTree
    {
        private class FinishToken : IDisposable
        {
            private readonly TreeEditorSidebarMenu menu;
            private readonly IDisposable token;

            public void Dispose()
            {
                token.Dispose();
                menu.FinishDrawing();
            }

            public FinishToken(IDisposable token, TreeEditorSidebarMenu menu)
            {
                this.token = token;
                this.menu = menu;
            }
        }

        private readonly OdinMenuTreeDrawingConfig _config;
        private readonly Color _backgroundColor;
        private readonly bool _editStyles;
        private bool _pending;

        private readonly OdinMenuStyle _style;

        public bool HasSelection => (Selection != null) && Selection.Any() && Selection.SelectedValues.Any();

        public bool TrySelection<T>(out T value)
            where T : class
        {
            if (!HasSelection)
            {
                value = null;
                return false;
            }

            value = Selection.SelectedValue as T;

            return value != null;
        }

        public void Draw(float buttonSize, float minWidth = 0, float maxWidth = 0, float minHeight = 0, float maxHeight = 0)
        {
            using (DrawUnfinished(buttonSize, minWidth, maxWidth, minHeight, maxHeight))
            {
            }
        }

        public IDisposable DrawUnfinished(float buttonSize, float minWidth = 0, float maxWidth = 0, float minHeight = 0, float maxHeight = 0)
        {
            DefaultMenuStyle.IconSize = buttonSize;
            DefaultMenuStyle.Height = (int) (buttonSize * 1.25f);
            _pending = true;

            var options = TreeGUI.Layout.Options.ExpandHeight(maxWidth == 0);
            options.ExpandWidth(maxWidth == 0);

            if (minWidth != 0)
            {
                options.MinWidth(minWidth);
            }

            if (maxWidth != 0)
            {
                options.MaxWidth(maxWidth);
            }

            if (minHeight != 0)
            {
                options.MinHeight(minHeight);
            }

            if (maxHeight != 0)
            {
                options.MaxHeight(maxHeight);
            }

            var token = TreeGUI.Layout.Vertical(
                true,
                options
            );

            var currentLayoutRect = GUIHelper.GetCurrentLayoutRect();

            TreeGUI.Draw.Solid(currentLayoutRect, _backgroundColor);

            UpdateMenuTree();
            DrawMenuTree();

            return new FinishToken(token, this);
        }

        public void FinishDrawing()
        {
            if (_pending)
            {
                HandleKeybaordMenuNavigation();
                _pending = false;
            }
        }

        public void Clear()
        {
            MenuItems.Clear();

            if (_editStyles)
            {
                Add("Style", _style, EditorIcons.Pen);
                Add("Config", _config, EditorIcons.SettingsCog);
            }

            UpdateMenuTree();
            MarkDirty();
        }

        public void SelectFirst()
        
        {
            Selection.Clear();

            var item = MenuItems.FirstOrDefault();

            if (item != null)
            {  
                Selection.Add(item);
            }
        }

        public void SelectWhere(Func<OdinMenuItem, bool> predicate)
        {
            Selection.Clear();

            if (MenuItems.Count == 0)
            {
                return;
            }

            var item = MenuItems.FirstOrDefault(predicate);

            if (item != null)
            {
                Selection.Add(item);
            }
        }

        public void SelectWhereWithFallback(Func<OdinMenuItem, bool> predicate, bool first)
        {
            SelectWhere(predicate);
            if (Selection.SelectedValue != null)
            {
                return;
            }

            if (MenuItems.Count == 0)
            {
                return;
            }

            if (MenuItems.Count == 1)
            {
                Selection.Add(MenuItems[0]);
                return;
            }

            OdinMenuItem item = null;

            if (first)
            {
                item = MenuItems.FirstOrDefault();
            }
            else
            {
                item = MenuItems.LastOrDefault();
            }

            if (item != null)
            {
                Selection.Add(item);
            }
        }

        public void SelectWhereWithIndexFallback(Func<OdinMenuItem, bool> predicate, int index)
        {
            SelectWhere(predicate);
            if (Selection.SelectedValue != null)
            {
                return;
            }

            if (MenuItems.Count == 0)
            {
                return;
            }

            if (MenuItems.Count == 1)
            {
                Selection.Add(MenuItems[0]);
                return;
            }

            var item = MenuItems.FirstOrDefault();

            if (item != null)
            {
                Selection.Add(item);
                return;
            }

            SelectByIndex(index);
        }

        public void SelectByIndex(int index)
        {
            if (index < 0)
            {
                return;
            }

            if (index >= MenuItems.Count)
            {
                index = MenuItems.Count - 1;
            }

            Selection.Add(MenuItems[index]);
        }

        public int GetSelectedIndex()
        {
            if (!HasSelection)
            {
                return -1;
            }

            var selection = Selection[0];

            for (var i = 0; i < MenuItems.Count; i++)
            {
                if (MenuItems[i] == selection)
                {
                    return i;
                }
            }

            return -1;
        }

        public TreeEditorSidebarMenu(
            OdinMenuStyle style,
            OdinMenuTreeDrawingConfig config,
            Color backgroundColor,
            bool editStyles = false)
        {
            DefaultMenuStyle = style;
            Config = config;
            _style = style;
            _config = config;
            _editStyles = editStyles;
            _backgroundColor = backgroundColor;

            if (editStyles)
            {
                Add("Style", style, EditorIcons.Pen);
                Add("Config", config, EditorIcons.SettingsCog);
            }
        }
    }
}
