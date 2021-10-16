#region

using Appalachia.Simulation.Trees.Build.Execution;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    public static class LogMaterialPropertyManager
    {    

        public static void AssignMaterialProperties(LogDataContainer log)
        {
            using (BUILD_TIME.LOG_MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
            }
        }
        
    }
}
