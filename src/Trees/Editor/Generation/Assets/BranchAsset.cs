using System;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class BranchAsset : TypeBasedSettings<BranchAsset>
    {
        private BranchAsset()
        {
        }

        #region Fields and Autoproperties

        public Material[] materials;
        public Mesh mesh;

        #endregion

        public string CleanName => name.Replace("asset", string.Empty);

        public static BranchAsset Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("asset");
            var instance = BranchAsset.LoadOrCreateNew(folder, assetName);

            return instance;
        }

        public void CreateMesh()
        {
            mesh = new Mesh {name = CleanName};
        }

        public AssetStatistics GetStatistics()
        {
            var stats = new AssetStatistics();

            if (mesh == null)
            {
                stats.statistics.Add(new AssetStatistic {submeshes = 0, triangles = 0, vertices = 0});
            }
            else
            {
                stats.statistics.Add(
                    new AssetStatistic
                    {
                        submeshes = mesh.subMeshCount,
                        triangles = mesh.triangles.Length / 3,
                        vertices = mesh.vertexCount
                    }
                );
            }

            return stats;
        }

        public void Refresh()
        {
            if (mesh == null)
            {
                CreateMesh();
            }
        }

        public void SetMaterials(Material[] materials)
        {
            this.materials = materials;
        }
    }
}
