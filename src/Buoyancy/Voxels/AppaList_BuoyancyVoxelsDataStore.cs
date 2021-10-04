using System;
using Appalachia.Core.Collections;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    [Serializable]
    public sealed class AppaList_BuoyancyVoxelsDataStore : AppaList<BuoyancyVoxelsDataStore>
    {
        public AppaList_BuoyancyVoxelsDataStore()
        {
        }

        public AppaList_BuoyancyVoxelsDataStore(
            int capacity,
            float capacityIncreaseMultiplier = 2,
            bool noTracking = false) : base(capacity, capacityIncreaseMultiplier, noTracking)
        {
        }

        public AppaList_BuoyancyVoxelsDataStore(AppaList<BuoyancyVoxelsDataStore> list) : base(list)
        {
        }

        public AppaList_BuoyancyVoxelsDataStore(BuoyancyVoxelsDataStore[] values) : base(values)
        {
        }
    }
}
