using Appalachia.Simulation.Buoyancy.Jobs;
using Appalachia.Spatial.Voxels.Casting;
using Appalachia.Spatial.Voxels.Persistence;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    public class BuoyancyVoxelsDataStoreLookup : VoxelDataStoreLookup<BuoyancyVoxels, VoxelRaycastHit<BuoyancyVoxel>, BuoyancyVoxelsDataStoreLookup,
        BuoyancyVoxelsLookup, BuoyancyVoxelsDataStore, AppaList_BuoyancyVoxelsDataStore>
    {

    }
}