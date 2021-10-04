using System;
using System.Collections.Generic;
using Appalachia.Base.Scriptables;
using Appalachia.Core.Comparisons;
using Appalachia.Editing.Preferences;
using Appalachia.Editing.Preferences.Globals;
using Appalachia.Simulation.Core;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Metadata.Materials;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Physical
{
    public abstract class CollectionButtonSelector
    {
        protected static Color GetButtonRowColor(int row, float drop, Color color)
        {
            for (var i = 0; i < row; i++)
            {
                color.r *= drop;
                color.g *= drop;
                color.b *= drop;
            }

            return color;
        }

        public static CollectionButtonSelector<PhysicsMaterials, PhysicMaterialWrapper> CreatePhysicMaterialSelector(Action<PhysicMaterialWrapper> select)
        {
            return new CollectionButtonSelector<PhysicsMaterials, PhysicMaterialWrapper>(
                PhysicsMaterials.instance,
                select,
                ColorPrefs.Instance.PhysicMaterialSelectorButton,
                ColorPrefs.Instance.PhysicMaterialSelectorColorDrop
            );
        }

        public static CollectionButtonSelector<PhysicsMaterials, PhysicMaterialWrapper> CreatePhysicMaterialSelector(Action<PhysicMaterialWrapper> select, PREF<Color> buttonColor)
        {
            return new CollectionButtonSelector<PhysicsMaterials, PhysicMaterialWrapper>(
                PhysicsMaterials.instance,
                select,
                buttonColor,
                ColorPrefs.Instance.PhysicMaterialSelectorColorDrop
            );
        }

        public static CollectionButtonSelector<DensityMetadataCollection, DensityMetadata> CreateDensityMetadataSelector(Action<DensityMetadata> select)
        {
            return new CollectionButtonSelector<DensityMetadataCollection, DensityMetadata>(
                DensityMetadataCollection.instance,
                select,
                ColorPrefs.Instance.DensitySelectorButton,
                ColorPrefs.Instance.DensitySelectorColorDrop
            );
        }

        public static CollectionButtonSelector<TC, TV> CreateGenericSelector<TC, TV>(TC instance, Action<TV> select)
            where TC : MetadataLookupBase<TC, TV>
            where TV : InternalScriptableObject<TV>, ICategorizable
        {
            return new CollectionButtonSelector<TC, TV>(
                instance,
                select,
                ColorPrefs.Instance.GenericSelectorButton,
                ColorPrefs.Instance.GenericSelectorColorDrop
            );
        }
    }
    
    [InlineProperty, Serializable, HideDuplicateReferenceBox, HideReferenceObjectPicker]
    public sealed class CollectionButtonSelector<T, TValue> : CollectionButtonSelector
    where T : MetadataLookupBase<T, TValue>
    where TValue : InternalScriptableObject<TValue>, ICategorizable
    {
        public CollectionButtonSelector(T instance, Action<TValue> select, PREF<Color> buttonColor, PREF<float> buttonColorDrop)
        {
            _instance = instance;
            _selection = select;
            ButtonColor = buttonColor;
            ButtonColorDrop = buttonColorDrop;
        }

        private T _instance;
        private Action<TValue> _selection;
        
        private PREF<Color> ButtonColor { get; }
        private PREF<float> ButtonColorDrop { get; }
        
        private static GUITabGroup _tabGroup;
        private static Dictionary<string, GUITabPage> _tabs;
        private static Dictionary<string, List<TValue>> _tabItems;
        private static List<string> _categories;

        private static PREF<int> _itemsPerRow;

        private void EnsureInitialized()
        {
            if (_itemsPerRow == null)
            {
                _itemsPerRow = PREFS.REG("Collection Buttons", "Items Per Row", 4, 1, 25);
            }
            
            if (_tabGroup == null)
            {
                _tabGroup = new GUITabGroup();
            }
            
            if (_tabs == null)
            {
                _tabs = new Dictionary<string, GUITabPage>();
            }

            if (_tabItems == null)
            {
                _tabItems = new Dictionary<string, List<TValue>>();
            }

            if (_categories == null)
            {
                _categories = new List<string>();
                
                var items = _instance.all;
                
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var category = item.Category;
                    
                    if (string.IsNullOrWhiteSpace(category))
                    {
                        category = "Uncategorized";
                    }

                    if (category.StartsWith("_"))
                    {
                        continue;
                    }
                    
                    if (!_tabs.ContainsKey(category))
                    {
                        _tabs.Add(category, _tabGroup.RegisterTab(category));
                        
                        _categories.Add(category);
                        
                        _tabItems.Add(category, new List<TValue>());
                    }
                    
                    _tabItems[category].Add(item);
                }
                
                _categories.Sort();

                for (var i = 0; i < _categories.Count; i++)
                {
                    var tabItems = _tabItems[_categories[i]];
                    
                    tabItems.Sort(ObjectComparer<TValue>.Instance);
                }
            }
        }

        [Button]
        public void Reset()
        {
            _tabGroup = null;
            _tabs = null;
            _tabItems = null;
            _categories = null;
        }

        [OnInspectorGUI]
        public void Draw()
        {
            EnsureInitialized();
                
           _tabGroup.BeginGroup();

           var originalColor = GUI.color;
           
            for (var categoryID = 0; categoryID < _categories.Count; categoryID++)
            {
                var category = _categories[categoryID];

                var tab = _tabs[category];
                
                var tabItems = _tabItems[category];
                
                if (tab.BeginPage())
                {
                    var tabRows = 0;

                    var open = false;
                    
                    for (var itemIndex = 0; itemIndex < tabItems.Count; itemIndex++)
                    {
                        var item = tabItems[itemIndex];

                        if (itemIndex == 0)
                        {
                            GUILayout.BeginHorizontal();
                            open = true;
                            tabRows += 1;
                        }
                        else if (itemIndex > 0 && itemIndex % _itemsPerRow.v == 0)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            open = true;
                            tabRows += 1;
                        }
                        
                        GUI.color = GetButtonRowColor(tabRows, ButtonColorDrop.v, ButtonColor.v);

                        if (item.NiceName != category && item.NiceName.StartsWith(category))
                        {
                            item.NiceName = item.NiceName.Substring(category.Length);
                        }
                        
                        if (GUILayout.Button(item.NiceName, SirenixGUIStyles.Button))
                        {
                            _selection(item);
                        }
                    }

                    if (open)
                    {
                        GUILayout.EndHorizontal();
                    }
                    
                }

                tab.EndPage();
            }
            
            _tabGroup.EndGroup();
            
            GUI.color = originalColor;
        }
    }
}