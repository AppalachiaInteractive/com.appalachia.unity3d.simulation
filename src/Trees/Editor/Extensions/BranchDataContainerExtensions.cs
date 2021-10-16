namespace Appalachia.Simulation.Trees.Extensions
{
    public static class BranchDataContainerExtensions
    {
        public static BranchDataContainer BuildDistribution(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(distribution: true);
            return tree;
        }
        
        public static BranchDataContainer BuildMaterialProperties(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(materialProperties: true);
            return tree;
        }
        public static BranchDataContainer BuildMaterialGeneration(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(materialGeneration: true);
            return tree;
        }
        
        public static BranchDataContainer BuildUv(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(uv: true);
            return tree;
        }
        
        public static BranchDataContainer BuildHighQualityGeometry(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(highQualityGeometry: true);
            return tree;
        }

        public static BranchDataContainer BuildAmbientOcclusion(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(ambientOcclusion: true);
            return tree;
        }
        
        public static BranchDataContainer BuildStage(this BranchDataContainer tree)
        {
            tree.PushBuildRequestLevel(distribution: true, uv: true, highQualityGeometry: true, ambientOcclusion: true);
            return tree;
        }
   }
}
