using System;
using Appalachia.Simulation.Buoyancy.Jobs;
using Appalachia.Spatial.Voxels.Persistence;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    [Serializable]
    public class BuoyancyVoxelsDataStore : VoxelPersistentObjectAndElementsDataStore<BuoyancyVoxels,
        BuoyancyVoxelsDataStore, BuoyancyObjectData, BuoyancyVoxel>
    {
    }
}
