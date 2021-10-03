using Appalachia.Simulation.Buoyancy.Jobs;
using Appalachia.Voxels.VoxelTypes;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    public class BuoyancyVoxels : PersistentVoxelsObjectAndElementsBase<BuoyancyVoxels, BuoyancyVoxelsDataStore, BuoyancyObjectData, BuoyancyVoxel>
    {
        public BuoyancyVoxels(string identifier) : base(identifier)
        {
        }
    }
}