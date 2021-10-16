using Appalachia.Simulation.Trees.Build.Requests;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class LogDataContainerExtensions
    {
        public static LogDataContainer BuildDistribution(this LogDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(distribution: level);
            return tree;
        }
        
        public static LogDataContainer BuildMaterialProperties(this LogDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(materialProperties: level);
            return tree;
        }

        public static LogDataContainer BuildMaterialGeneration(this LogDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(materialGeneration: level);
            return tree;
        }
        
        public static LogDataContainer BuildStage(this LogDataContainer tree, BuildRequestLevel level)
        {
            tree.PushBuildRequestLevel(uv: level, highQualityGeometry: level, levelsOfDetail: level, vertex: level, collision: level);
            return tree;
        }
    }
}
