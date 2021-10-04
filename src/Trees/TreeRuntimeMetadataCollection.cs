using System.Collections.Generic;
using Appalachia.Base.Scriptables;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees
{
    [CreateAssetMenu(menuName = "Internal/System/TreeRuntimeMetadataCollection", order = 0)]
    public class TreeRuntimeMetadataCollection : SelfSavingSingletonScriptableObject<TreeRuntimeMetadataCollection>
    {
        public List<TreeRuntimeInstanceMetadata> treeRuntimeInstanceMetadatas = new List<TreeRuntimeInstanceMetadata>();

#if  UNITY_EDITOR
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
        
            var ts = AssetDatabase.FindAssets("t: TreeRuntimeInstanceMetadata");

            foreach (var t in ts)
            {
                var path = AssetDatabase.GUIDToAssetPath(t);
                var i = AssetDatabase.LoadAssetAtPath<TreeRuntimeInstanceMetadata>(path);
                treeRuntimeInstanceMetadatas.Add(i);
            }
        }
#endif
    }
}
