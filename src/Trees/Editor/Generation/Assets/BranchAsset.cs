using System;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{

    [Serializable]
    public class BranchAsset : TypeBasedSettings<BranchAsset>
    {
        public Mesh mesh;
        
        public Material[] materials;

        private BranchAsset()
        {
        }

        public static BranchAsset Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("asset");
            var instance = LoadOrCreateNew(folder, assetName);

            return instance;
        }

        public void Refresh()
        {
            if (mesh == null)
            {
                CreateMesh();
            }
        }
        
        public void CreateMesh()
        {
            mesh = new Mesh {name = CleanName};
        }

        public void SetMaterials(Material[] materials)
        {
            this.materials = materials;
        }
        
        public AssetStatistics GetStatistics()
        {
            var stats = new AssetStatistics();

            if (mesh == null)
            {
                stats.statistics.Add(new AssetStatistic
                {
                    submeshes = 0,
                    triangles = 0,
                    vertices = 0
                });
            }
            else
            {
                stats.statistics.Add(new AssetStatistic()
                {
                    submeshes = mesh.subMeshCount,
                    triangles = mesh.triangles.Length / 3,
                    vertices = mesh.vertexCount
                });
            }
            
            return stats;
        }
        
        public string CleanName => name.Replace("asset", string.Empty);
    }
}


