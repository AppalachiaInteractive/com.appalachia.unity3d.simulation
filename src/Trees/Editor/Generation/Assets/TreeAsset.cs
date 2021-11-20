using System;
using System.Collections.Generic;
using AmplifyImpostors;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class TreeAsset : PrefabAsset<TreeAsset>, ITreeStatistics
    {
        #region Fields and Autoproperties

        public AmplifyImpostorAsset impostor;

        public CollisionMeshCollection collisionMeshes;
        public List<AssetLevel> levels;

        public Mesh shadowCasterMesh;

        #endregion

        public static TreeAsset Create(
            string folder,
            NameBasis nameBasis,
            int individualID,
            AgeType age,
            StageType stage)
        {
            var assetName = nameBasis.FileNameAssetSO(individualID, age, stage);
            var instance = LoadOrCreateNew<TreeAsset>(folder, assetName);

            instance.levels = new List<AssetLevel>();

            return instance;
        }

        public void Refresh(TreeSettings settings)
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

            if (settings.lod.shadowCaster)
            {
                if (shadowCasterMesh == null)
                {
                    shadowCasterMesh = new Mesh {name = GetShadowCasterMeshName()};
                }

                shadowCasterMesh.name = GetShadowCasterMeshName();
            }

            levels = new List<AssetLevel>();

            for (var i = 0; i < settings.lod.levels; i++)
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
