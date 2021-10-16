using Appalachia.Simulation.Trees.Build.Requests;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class TreeDataContainerExtensions
    {
        public static TreeDataContainer BuildDistribution(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(distribution: level);
            return tree;
        }
        
        public static TreeDataContainer BuildMaterialProperties(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(materialProperties: level);
            return tree;
        }
        
        /*
        public static TreeDataContainer BuildPrefabCreation(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(prefabCreation: level);
            return tree;
        }
        */
        
        public static TreeDataContainer BuildMaterialGeneration(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(materialGeneration: level);
            return tree;
        }
        
        public static TreeDataContainer BuildUv(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(uv: level);
            return tree;
        }
        
        public static TreeDataContainer BuildHighQualityGeometry(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(highQualityGeometry: level);
            return tree;
        }

        public static TreeDataContainer BuildAmbientOcclusion(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(ambientOcclusion: level);
            return tree;
        }
        
        /*public static TreeDataContainer BuildLevelsOfDetail(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(levelsOfDetail: level);
            return tree;
        }*/
        
        public static TreeDataContainer BuildWind(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(wind: level);
            return tree;
        }      
        
        public static TreeDataContainer BuildLowQualityGeometry(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(lowQualityGeometry: level);
            return tree;
        }     
        
        
        public static TreeDataContainer BuildColliders(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(collision: level);
            return tree;
        }     
        
        public static TreeDataContainer BuildImpostor(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(impostors: level);
            return tree;
        }     
        public static TreeDataContainer BuildStage(this TreeDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(uv: level, highQualityGeometry: level, ambientOcclusion: level, /*levelsOfDetail: level,*/ wind: level, collision: level);
            return tree;
        }
    }
}
