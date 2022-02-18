using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Utility.Async;
using Appalachia.Utility.Execution;
using Sirenix.OdinInspector;
using Unity.Profiling;

namespace Appalachia.Simulation.Trees.Core
{
    public class TreeRuntimeMetadataCollection : SingletonAppalachiaTreeObject<TreeRuntimeMetadataCollection>
    {
        #region Fields and Autoproperties

        public List<TreeRuntimeInstanceMetadata> treeRuntimeInstanceMetadatas = new();

        #endregion

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

#if UNITY_EDITOR
            if (!AppalachiaApplication.IsPlayingOrWillPlay)
            {
                UpdateLists();
            }
#endif
        }

#if UNITY_EDITOR

        private static readonly ProfilerMarker _PRF_UpdateLists =
            new ProfilerMarker(_PRF_PFX + nameof(UpdateLists));

        [Button]
        private void UpdateLists()
        {
            using (_PRF_UpdateLists.Auto())
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
        }

        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(TreeRuntimeMetadataCollection),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew();
        }
#endif
    }
}
