using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.GUI;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    public abstract class TreeEditorSidebarMenuContainer<T>
        where T : class
    {
        protected TreeEditorSidebarMenuContainer(
            OdinMenuStyle menuStyle,
            OdinMenuTreeDrawingConfig menuConfig,
            Color menuBackgroundColor,
            string menuTitle)
        {
            this.menuStyle = menuStyle;
            this.menuConfig = menuConfig;
            this.menuBackgroundColor = menuBackgroundColor;
            this.menuTitle = menuTitle;
        }

        protected readonly OdinMenuStyle menuStyle;
        protected readonly OdinMenuTreeDrawingConfig menuConfig;
        protected readonly Color menuBackgroundColor;
        protected string menuTitle;
        
        protected T lastSelection;
        protected TreeEditorSidebarMenu menu;

        public T Selected => menu?.Selection?.SelectedValue as T;

        public bool HasSelection => menu?.HasSelection ?? false;
        
        private void RecreateMenusIfNull()
        {
            if ((menu?.MenuItems == null) || menu.MenuItems.Any(m => m == null))
            {
                menu = new TreeEditorSidebarMenu(menuStyle, menuConfig, menuBackgroundColor);
                
                menu.DefaultMenuStyle = menuStyle;
            }
        }

        private void ClearMenusIfInvalid(bool requested)
        {
            if (requested)
            {
                menu.Selection.Clear();
                menu.Clear();
            }

            if (menu.Selection.Any(s => s == null))
            {
                menu = null;
            }
        }

        public void RepopulateMenus(bool force, bool clear)
        {
            if (force)
            {
                menu = null;
            }

            if (Event.current.type == EventType.Layout)
            {
                RecreateMenusIfNull();
                ClearMenusIfInvalid(clear);
                RecreateMenusIfNull();
            }

            var menuItems = GetMenuItems();

            var menuCountValid = menu.MenuItems.Count == menuItems.Count;

            if (!menuCountValid)
            {
                var selectionIndex = menu.GetSelectedIndex();

                menu.Clear();

                foreach (var menuItem in menuItems)
                {
                    menu.Add(GetMenuName(menuItem), menuItem, GetMenuIcon(menuItem)?.icon);
                }

                selectionIndex = Mathf.Max(selectionIndex, menu.MenuItems.Count - 1);

                if (selectionIndex > 0)
                {
                    menu.SelectByIndex(selectionIndex);
                }

                menu.SelectByIndex(selectionIndex);
            }
            else if (menu.MenuItems.Count == 0)
            { 
                foreach (var menuItem in menuItems)
                {
                    menu.Add(GetMenuName(menuItem), menuItem, GetMenuIcon(menuItem)?.icon);
                }

                if (menu.MenuItems.Count > 0)
                {
                    menu.SelectFirst();
                }
            }
        }

        protected abstract IList<T> GetMenuItems();

        protected abstract string GetMenuName(T menuItem);
        
        protected abstract TreeIcon GetMenuIcon(T menuItem);

        public void SelectFirst()
        {
            menu.SelectFirst();
        }

        public void Select(T item)
        {
            foreach (var selection in menu.MenuItems)
            {
                if (selection.Value == item)
                {
                    selection.Select();
                }
                else
                {
                    selection.Deselect();
                }
            }
        }

        public void DrawMenu(float menuWidth, float menuHeight, float menuItemHeight, float menuButtonHeight)
        {
            TreeGUI.Draw.SmallTitle(menuTitle);
            menu.Draw(menuItemHeight, maxWidth: menuWidth, maxHeight: menuHeight);

            if (HasAdditionalToolbar)
            {
                DrawAdditionalToolbar(menuWidth, menuButtonHeight);
            }
        }

        protected abstract bool HasAdditionalToolbar { get; }
        
        protected abstract  void DrawAdditionalToolbar(float width, float height);
    }
}
