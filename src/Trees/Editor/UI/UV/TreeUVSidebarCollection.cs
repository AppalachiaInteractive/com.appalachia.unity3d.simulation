using System.Linq;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.GUI;
using Appalachia.Simulation.Trees.UI.Selections.Icons;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    public class TreeUVSidebarCollection
    {
        private object _lastLeafHierarchySelection;
        private object _lastLeafRectSelection;
        private TreeEditorSidebarMenu atlasMaterialMenu;
        private TreeEditorSidebarMenu leafRectMenu;

        public AtlasInputMaterial SelectedInputMaterial =>
            atlasMaterialMenu?.Selection?.SelectedValue as AtlasInputMaterial;

        public LeafUVRect SelectedLeafRect =>
            leafRectMenu?.Selection?.SelectedValue as LeafUVRect;

        public bool HasSelectedAtlasMaterial => atlasMaterialMenu?.HasSelection ?? false;
        public bool HasSelectedLeafRect => leafRectMenu?.HasSelection ?? false;

        public void RepopulateMenus(InputMaterialCache inputMaterials, bool force, bool clear)
        {
            if (force)
            {
                atlasMaterialMenu = null;
                leafRectMenu = null;
            }

            if ((atlasMaterialMenu?.MenuItems == null) || atlasMaterialMenu.MenuItems.Any(m => m == null))
            {
                atlasMaterialMenu = new TreeEditorSidebarMenu(
                    TreeGUI.MenuStyles.TreeUVMenuStyle,
                    TreeGUI.MenuStyles.TreeUVMenuConfig,
                    TreeGUI.Colors.TreeUVMenuBackgroundColor
                );

                atlasMaterialMenu.DefaultMenuStyle = TreeGUI.MenuStyles.SidebarMenuStyle;
                atlasMaterialMenu.Selection.SelectionChanged += HierarchyOnSelectionChanged;
            }

            if (atlasMaterialMenu.Selection.Any(s => s == null))
            {
                atlasMaterialMenu.Selection.Clear();
            }

            if ((leafRectMenu?.MenuItems == null) || leafRectMenu.MenuItems.Any(m => m == null))
            {
                leafRectMenu = new TreeEditorSidebarMenu(
                    TreeGUI.MenuStyles.TreeUVMenuStyle,
                    TreeGUI.MenuStyles.TreeUVMenuConfig,
                    TreeGUI.Colors.TreeUVMenuBackgroundColor
                );

                leafRectMenu.DefaultMenuStyle = TreeGUI.MenuStyles.SidebarMenuStyle;
                leafRectMenu.Selection.SelectionChanged += RectOnSelectionChanged;
            }

            if (leafRectMenu.Selection.Any(s => s == null))
            {
                leafRectMenu.Selection.Clear();
            }

            if (clear)
            {
                atlasMaterialMenu.Clear();
                leafRectMenu.Clear();
            }

            var materialCountValid = atlasMaterialMenu.MenuItems.Count == inputMaterials.atlasInputMaterials.Count;

            if (!materialCountValid)
            {
                atlasMaterialMenu.Selection.SelectionChanged -= HierarchyOnSelectionChanged;

                //var currentMaterialIndex = atlasMaterialMenu.GetSelectedIndex();

                atlasMaterialMenu.Clear();

                foreach (var atlasMaterial in inputMaterials.atlasInputMaterials.Where(m => m.material != null).OrderByDescending(l => l.proportionalArea))
                {
                    atlasMaterialMenu.Add(
                        atlasMaterial.material.name,
                        atlasMaterial,
                        atlasMaterial.material.primaryTexture()
                    );
                    
                    atlasMaterialMenu.SelectFirst();
                }

                atlasMaterialMenu.Selection.SelectionChanged += HierarchyOnSelectionChanged;
            }

            var uvRects = LeafUVRectCollection.instance.Get(SelectedInputMaterial.material);
            var uvRectCountValid = leafRectMenu.MenuItems.Count == uvRects.Count;

            if (SelectedInputMaterial == null)
            {
                leafRectMenu.Selection.SelectionChanged -= RectOnSelectionChanged;

                leafRectMenu.Clear();
                leafRectMenu.Selection.Clear();

                leafRectMenu.Selection.SelectionChanged += RectOnSelectionChanged;
            }
            else if (!uvRectCountValid)
            {
                leafRectMenu.Selection.SelectionChanged -= RectOnSelectionChanged;

                var currentRectSelectionIndex = leafRectMenu.GetSelectedIndex();

                leafRectMenu.Clear();
                leafRectMenu.Selection.Clear();

                for (var index = 0; index < uvRects.Count; index++)
                {
                    var rect = uvRects[index];
                    var name = $"{index}: {rect}";
                    
                    leafRectMenu.Add(name, rect);
                }

                currentRectSelectionIndex = Mathf.Max(
                    currentRectSelectionIndex,
                    leafRectMenu.MenuItems.Count - 1
                );

                if (currentRectSelectionIndex > 0)
                {
                    leafRectMenu.SelectByIndex(currentRectSelectionIndex);
                }

                leafRectMenu.Selection.SelectionChanged += RectOnSelectionChanged;
            }
            else if (leafRectMenu.MenuItems.Count == 0)
            {
                leafRectMenu.Selection.SelectionChanged -= RectOnSelectionChanged;

                for (var index = 0; index < uvRects.Count; index++)
                {
                    var rect = uvRects[index];
                    var name = $"{index}: {rect}";

                    leafRectMenu.Add(name, rect);
                }

                if (leafRectMenu.MenuItems.Count > 0)
                {
                    leafRectMenu.SelectFirst();
                }

                leafRectMenu.Selection.SelectionChanged += RectOnSelectionChanged;
            }
        }

        public void SelectFirst()
        {
            atlasMaterialMenu.SelectFirst();
            leafRectMenu.SelectFirst();
        }


        private void HierarchyOnSelectionChanged(SelectionChangedType obj)
        {
            if (obj == SelectionChangedType.ItemAdded)
            {
                if (atlasMaterialMenu.Selection.SelectedValue == _lastLeafHierarchySelection)
                {
                    return;
                }

                _lastLeafHierarchySelection = atlasMaterialMenu.Selection.SelectedValue;
                leafRectMenu.Clear();
                leafRectMenu.Selection.Clear();
            }
        }

        private void RectOnSelectionChanged(SelectionChangedType obj)
        {
            if (obj == SelectionChangedType.ItemAdded)
            {
                if (leafRectMenu.Selection.SelectedValue == _lastLeafRectSelection)
                {
                    return;
                }

                _lastLeafRectSelection = leafRectMenu.Selection.SelectedValue;
            }
        }

        public void DrawInputMaterialMenu(
            float menuWidth, 
            float menuHeight,
            float menuItemHeight)
        {
            TreeGUI.Draw.SmallTitle("Leaves");
            atlasMaterialMenu.Draw(menuItemHeight, maxWidth: menuWidth, maxHeight: menuHeight);
        }


        public void DrawLeafRectMenu(
            float menuWidth, 
            float menuHeight,
            float menuItemHeight,
            float menuButtonWidth,
            float menuButtonHeight)
        {
            TreeGUI.Draw.SmallTitle("Leaf Rectangles");
            leafRectMenu.Draw(menuItemHeight, maxWidth: menuWidth, maxHeight: menuHeight);

            DrawUVRectAddRemoveToolbar(
                menuButtonWidth,
                menuButtonHeight
            );
        }

        private void DrawUVRectAddRemoveToolbar(
            float width,
            float height)
        {
            using(TreeGUI.Layout.Horizontal())
            {
                var uvRects = LeafUVRectCollection.instance.Get(SelectedInputMaterial.material);

                if (GUILayout.Button(
                    TreeIcons.plus.Get("Add new rect"),
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/.MaxWidth(width).Height(height)
                ))
                {
                    uvRects.Add(new LeafUVRect());
                }

                using (TreeGUI.Enabled.If(SelectedInputMaterial != null))
                {
                    if (GUILayout.Button(
                        (UnityEngine.GUI.enabled ? TreeIcons.x : TreeIcons.disabledX).Get("Remove selected rect"),
                        TreeGUI.Styles.ButtonRight,
                        TreeGUI.Layout.Options

                            //.MinWidth(width)
                            .MaxWidth(width)
                            .Height(height)
                    ))
                    {
                        leafRectMenu.Clear();
                        uvRects.Remove(SelectedLeafRect);
                    }
                }
            }
        }
   
        public void Select(LeafHierarchyData data)
        {
            atlasMaterialMenu.SelectWhere(o => (o.Value as LeafHierarchyData) == data);
            leafRectMenu.Clear();
        }
    }
}
