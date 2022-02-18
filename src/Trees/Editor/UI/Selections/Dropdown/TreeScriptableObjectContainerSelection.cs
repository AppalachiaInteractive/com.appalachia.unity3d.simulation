using System;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Utility.Async;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Reflection.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Selections.Dropdown
{
    [Serializable]
    public abstract class TreeScriptableObjectContainerSelection<T, TS> : SingletonAppalachiaTreeObject<TS>
        where TS : SingletonAppalachiaTreeObject<TS>
        where T : ScriptableObject
    {
        #region Fields and Autoproperties

        [SerializeField, HideInInspector]
        private T[] objects;

        [SerializeField, HideInInspector]
        private string[] objectNames;

        [SerializeField, HideLabel]
        [TitleGroup(@"@""Selected "" + typeName", Alignment = TitleAlignments.Centered)]
        [ValueDropdown(
            nameof(objects),
            NumberOfItemsBeforeEnablingSearch = 5,
            AppendNextDrawer = false,
            IsUniqueList = true,
            SortDropdownItems = true,
            DoubleClickToConfirm = false,
            ExpandAllMenuItems = true,
            FlattenTreeView = false
        )]
        private T _selected;

        #endregion

        public bool NeedsRefresh =>
            (objects == null) ||
            (objects.Length == 0) ||
            (objectNames == null) ||
            (objectNames.Length == 0) ||
            objects.Any(o => o == null);

        public T selected => _selected;

        private string typeName
        {
            get
            {
                if (typeof(T) == typeof(BranchDataContainer))
                {
                    return "Branch";
                }

                if (typeof(T) == typeof(TreeDataContainer))
                {
                    return "Tree";
                }

                return typeof(T).GetReadableName();
            }
        }

        public void Refresh()
        {
            var searchString = ZString.Format("t: {0}", typeof(T).GetReadableName());
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

        /// <inheritdoc />
        protected override async AppaTask WhenEnabled()
        {
            if (AppalachiaApplication.IsPlayingOrWillPlay)
            {
                return;
            }

            await base.WhenEnabled();
            Refresh();
        }
    }
}
