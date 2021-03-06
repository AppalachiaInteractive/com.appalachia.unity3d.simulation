using System;
using Appalachia.Simulation.Buoyancy.Jobs;
using Appalachia.Spatial.Voxels.VoxelTypes;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    [Serializable]
    public class BuoyancyVoxels : PersistentVoxelsObjectAndElementsBase<BuoyancyVoxels,
        BuoyancyVoxelsDataStore, BuoyancyObjectData, BuoyancyVoxel>
    {
        public BuoyancyVoxels(string identifier) : base(identifier)
        {
        }
    }
}
