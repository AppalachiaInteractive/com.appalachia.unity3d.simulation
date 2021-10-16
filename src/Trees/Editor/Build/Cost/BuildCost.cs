using System;

namespace Appalachia.Simulation.Trees.Build.Cost
{
    public struct BuildCost
    {
        public BuildCost(BuildCategory category)
        {
            this.category = category;
            cost = GetBuildCost(category);
        }
        
        public readonly BuildCategory category;
        
        public readonly float cost;

        public static float GetBuildCost(BuildCategory category)
        {
            switch (category)
            {
                case BuildCategory.None:
                    return 0f;
                case BuildCategory.MaterialGeneration:
                    return 30f;
                case BuildCategory.SavingTextures:
                    return 15f;
                case BuildCategory.MaterialProperties:
                    return 5f;
                case BuildCategory.LevelsOfDetail:
                    return 0f;
                case BuildCategory.VertexData:
                    return 2f;
                /*case BuildCategory.PrefabCreation:
                    return 10f;*/
                case BuildCategory.Distribution:
                    return 12f;
                case BuildCategory.HighQualityGeometry:
                    return 20f;
                case BuildCategory.LowQualityGeometry:
                    return 10f;
                case BuildCategory.UV:
                    return 4f;
                case BuildCategory.Collision:
                    return 25f;
                case BuildCategory.AmbientOcclusion:
                    return 15f;
                case BuildCategory.Mesh:
                    return 3f;
                case BuildCategory.Impostor:
                    return 10f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}
