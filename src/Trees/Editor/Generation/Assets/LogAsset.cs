using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Editing.Scene.Prefabs;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class LogAsset : TypeBasedSettings<LogAsset>, ITreeStatistics, IPrefabSaveable
    {
        private LogAsset()
        {
        }

        #region Fields and Autoproperties

        public CollisionMeshCollection collisionMeshes;

        public GameObject prefab;
        public List<AssetLevel> levels;

        #endregion

        public string CleanName => name.Replace("asset", string.Empty).TrimEnd('.', '_', '-');

        public string prefabPath => AssetDatabaseManager.GetAssetPath(prefab);

        public static LogAsset Create(string folder, NameBasis nameBasis, int logID)
        {
            var assetName = nameBasis.FileNameLogAssetSO(logID);
            var instance = LogAsset.LoadOrCreateNew(folder, assetName);

            instance.levels = new List<AssetLevel>();

            return instance;
        }

        public string GetMeshName(int meshLevel)
        {
            return ZString.Format("{0}_LOD{1}", CleanName, meshLevel);
        }

        public void Refresh(LevelOfDetailSettingsCollection lod)
        {
            if (prefab != null)
            {
                prefab.name = CleanName;
            }

            var current = levels;

            if (collisionMeshes == null)
            {
                collisionMeshes = new CollisionMeshCollection();
                collisionMeshes.CreateCollisionMeshes(CleanName);
            }

            levels = new List<AssetLevel>();

            for (var i = 0; i < lod.levels; i++)
            {
                AssetLevel match;

                if (i >= current.Count)
                {
                    match = new AssetLevel(GetMeshName(i));
                }
                else
                {
                    match = current[i];
                }

                if (match.mesh == null)
                {
                    match.CreateMesh(GetMeshName(i));
                }

                match.mesh.name = GetMeshName(i);

                levels.Add(match);
            }
        }

        public GameObject ToInstance()
        {
            return PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        }

        #region IPrefabSaveable Members

        public GameObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }

        #endregion

        #region ITreeStatistics Members

        public AssetStatistics GetStatistics()
        {
            var stats = new AssetStatistics();

            if (levels == null)
            {
                levels = new List<AssetLevel>();
            }

            foreach (var level in levels)
            {
                stats.statistics.Add(level.GetStatistics());
            }

            return stats;
        }

        public int GetStatsCount()
        {
            return levels.Count;
        }

        #endregion
    }
}
