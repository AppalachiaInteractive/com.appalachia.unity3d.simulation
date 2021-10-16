using System;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Scriptables;
using Appalachia.Utility.Reflection.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Dropdown
{
    [Serializable]
    public abstract class TreeScriptableObjectContainerSelection <T, TS> : SelfSavingSingletonScriptableObject<TS>
        where TS : SelfSavingSingletonScriptableObject<TS> 
        where T : ScriptableObject//AppalachiaScriptableObject<T>
    {
        [SerializeField, HideInInspector] private T[] objects;
        [SerializeField, HideInInspector] private string[] objectNames;

        private string typeName
        {
            get
            {
                if (typeof(T) == typeof(BranchDataContainer))
                {
                    return "Branch";
                }
                else if (typeof(T) == typeof(TreeDataContainer))
                {
                    return "Tree";
                }
                else
                {
                    return typeof(T).GetReadableName();
                }
            }
        }
        
        [SerializeField, HideLabel]
        [TitleGroup(@"@""Selected "" + typeName", Alignment = TitleAlignments.Centered)]
        [ValueDropdown(nameof(objects),
            NumberOfItemsBeforeEnablingSearch = 5,
            AppendNextDrawer = false,
            IsUniqueList = true,
            SortDropdownItems = true, 
            DoubleClickToConfirm = false,
            ExpandAllMenuItems = true, 
            FlattenTreeView = false)
        ]
        private T _selected;

        public T selected => _selected;

        public void Refresh()
        {
            var searchString = $"t: {typeof(T).GetReadableName()}";
            var assetGuids = AssetDatabaseManager.FindAssets(searchString);

            objects = new T[assetGuids.Length];
            objectNames = new string[assetGuids.Length];

            for (var index = 0; index < assetGuids.Length; index++)
            {
                var assetGuid = assetGuids[index];
                var path = AssetDatabaseManager.GUIDToAssetPath(assetGuid);

                var t = AssetDatabaseManager.LoadAssetAtPath<T>(path);

                objects[index] = t;
                objectNames[index] = t.ToString();
            }
        }

        public void Set(T t)
        {
            if (_selected == t)
            {
                return;
            }
            
            _selected = t;
            Refresh();
        }

        public bool NeedsRefresh =>
            (objects == null) ||
            (objects.Length == 0) ||
            (objectNames == null) ||
            (objectNames.Length == 0) ||
            objects.Any(o => o == null);

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }
    }
}
