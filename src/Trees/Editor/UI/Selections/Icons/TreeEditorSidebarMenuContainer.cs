using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Selections.State;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Icons
{
    [CallStaticConstructorInEditor]
    public abstract class TreeEditorSidebarMenuContainer<T>
        where T : class
    {
        static TreeEditorSidebarMenuContainer()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<TreeSpeciesEditorSelection>().IsAvailableThen( i => _treeSpeciesEditorSelection = i);
        }

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

        #region Static Fields and Autoproperties

        protected static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        #region Fields and Autoproperties

        protected readonly Color menuBackgroundColor;

        protected readonly OdinMenuStyle menuStyle;
        protected readonly OdinMenuTreeDrawingConfig menuConfig;
        protected string menuTitle;

        protected T lastSelection;
        protected TreeEditorSidebarMenu menu;

        #endregion

        protected abstract bool HasAdditionalToolbar { get; }

        public bool HasSelection => menu?.HasSelection ?? false;

        public T Selected => menu?.Selection?.SelectedValue as T;

        public void DrawMenu(float menuWidth, float menuHeight, float menuItemHeight, float menuButtonHeight)
        {
            TreeGUI.Draw.SmallTitle(menuTitle);
            menu.Draw(menuItemHeight, maxWidth: menuWidth, maxHeight: menuHeight);

            if (HasAdditionalToolbar)
            {
                DrawAdditionalToolbar(menuWidth, menuButtonHeight);
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

        public void SelectFirst()
        {
            menu.SelectFirst();
        }

        protected abstract void DrawAdditionalToolbar(float width, float height);

        protected abstract TreeIcon GetMenuIcon(T menuItem);

        protected abstract IList<T> GetMenuItems();

        protected abstract string GetMenuName(T menuItem);

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

        private void RecreateMenusIfNull()
        {
            if ((menu?.MenuItems == null) || menu.MenuItems.Any(m => m == null))
            {
                menu = new TreeEditorSidebarMenu(menuStyle, menuConfig, menuBackgroundColor);

                menu.DefaultMenuStyle = menuStyle;
            }
        }
    }
}
