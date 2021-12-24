using System;
using Appalachia.Rendering.Prefabs.Rendering.MultiStage.Trees;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class IntegrationAsset : PrefabAsset<IntegrationAsset>
    {
        #region Fields and Autoproperties

        public GameObject dead;
        public GameObject deadFelled;
        public GameObject deadFelledRotted;
        public GameObject felled;
        public GameObject felledBare;
        public GameObject felledBareRotted;

        public GameObject normal;
        public GameObject stump;
        public GameObject stumpRotted;
        public TreeGPUInstancerPrefab integrationPrefab;

        #endregion

        public static IntegrationAsset Create(
            string folder,
            NameBasis nameBasis,
            int individualID,
            AgeType age)
        {
            var assetName = nameBasis.FileNameAgeSO(individualID, age, "Integration");
            var instance = LoadOrCreateNew(folder, assetName);

            return instance;
        }

        public void Refresh(TreeSettings settings)
        {
            if (prefab != null)
            {
                prefab.name = CleanName;
            }

            integrationPrefab = prefab.GetComponent<TreeGPUInstancerPrefab>();

            if (!integrationPrefab)
            {
            }
        }
    }
}
