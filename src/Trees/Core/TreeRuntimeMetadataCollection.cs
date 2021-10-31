using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core
{
    public class
        TreeRuntimeMetadataCollection : SingletonAppalachiaObject<
            TreeRuntimeMetadataCollection>
    {
        public List<TreeRuntimeInstanceMetadata> treeRuntimeInstanceMetadatas = new();

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying)
            {
                return;
            }

            UpdateLists();
        }

        [Button]
        private void UpdateLists()
        {
            treeRuntimeInstanceMetadatas.Clear();

            var ts = AssetDatabaseManager.FindAssets("t: TreeRuntimeInstanceMetadata");

            foreach (var t in ts)
            {
                var path = AssetDatabaseManager.GUIDToAssetPath(t);
                var i = AssetDatabaseManager.LoadAssetAtPath<TreeRuntimeInstanceMetadata>(path);
                treeRuntimeInstanceMetadatas.Add(i);
            }
        }

        [UnityEditor.MenuItem(PKG.Menu.Assets.Base + nameof(TreeRuntimeMetadataCollection), priority = PKG.Menu.Assets.Priority)]
        public static void CreateAsset()
        {
            CreateNew();
        }
#endif
    }
}
