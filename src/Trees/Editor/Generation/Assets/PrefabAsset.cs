using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Editing.Scene.Prefabs;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    
    [Serializable]
    public abstract class PrefabAsset<T> : TypeBasedSettings<T>, IPrefabSaveable
        where T : PrefabAsset<T>
    {
        public GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
            set { prefab = value; }
        }

        public string prefabPath => AssetDatabaseManager.GetAssetPath(prefab);

        public GameObject ToInstance() => PrefabUtility.InstantiatePrefab(prefab) as GameObject;


        public string CleanName => name.Replace("asset", string.Empty).TrimEnd(new[] {'.', '_', '-'});
        

        public string GetMeshName(int meshLevel)
        {
            return ZString.Format("{0}_LOD{1}", CleanName, meshLevel);
        }

        public string GetShadowCasterMeshName()
        {
            return ZString.Format("{0}_SHADOW", CleanName);
        }
    }
}
