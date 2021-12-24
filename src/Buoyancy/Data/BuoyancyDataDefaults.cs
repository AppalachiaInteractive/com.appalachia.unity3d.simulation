using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Root;
using Appalachia.Spatial.Voxels;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Buoyancy.Data
{
    public class BuoyancyDataDefaults : SingletonAppalachiaObject<BuoyancyDataDefaults>
    {
        
            
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        public float wetnessAccumulationSpeed = .001f;

        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        public float wetnessDispersalSpeed = .0003f;

        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        public float submergednessAccumulationSpeed = .001f;

        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        public float submergednessDispersalSpeed = .0003f;

        [SmartLabel]
        public BuoyancyType
            buoyancyType = BuoyancyType.PhysicalVoxel; // type of buoyancy to calculate

        /// <summary>
        ///     type of voxel population to use
        /// </summary>
        [SmartLabel]
        public VoxelPopulationStyle voxelPopulationStyle;

        /// <summary>
        ///     voxel resolution, represents the half size of a voxel when creating the voxel representation
        /// </summary>
        [PropertyRange(0.01f, 2f)]
        [SmartLabel]
        public float voxelResolution = 0.251f; // 

        [PropertyRange(-10f, 10f)]
        [SmartLabel]
        public float waterLevelOffset;

        [PropertyRange(0.01f, 1.0f)]
        [SmartLabel]
        public float submersionEngageSmoothing = 0.15f;

        [PropertyRange(0.01f, 1.0f)]
        [SmartLabel]
        public float submersionDisengageSmoothing = 0.35f;
    }
}
