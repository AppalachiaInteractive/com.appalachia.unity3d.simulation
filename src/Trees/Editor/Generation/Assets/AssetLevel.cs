using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class AssetLevel
    {
        public Mesh mesh;

        public Material[] materials;

        public AssetStatistic GetStatistics()
        {
            if (mesh == null)
            {
                return new AssetStatistic()
                {
                    submeshes = 0,
                    triangles = 0,
                    vertices = 0
                };
            }
            
            return new AssetStatistic()
            {
                submeshes = mesh.subMeshCount,
                triangles = mesh.triangles.Length / 3,
                vertices = mesh.vertexCount
            };
        }

        public AssetLevel(string meshName)
        {
            CreateMesh(meshName);
        }

        public void CreateMesh(string meshName)
        {
            mesh = new Mesh {name = meshName};
        }

        public void SetMaterials(Material[] materials)
        {
            this.materials = materials;
        }
    }
}
