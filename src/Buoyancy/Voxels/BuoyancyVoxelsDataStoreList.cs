using System;
using Appalachia.Core.Collections;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    [Serializable]
    public sealed class BuoyancyVoxelsDataStoreList : AppaList<BuoyancyVoxelsDataStore>
    {
        public BuoyancyVoxelsDataStoreList()
        {
        }

        public BuoyancyVoxelsDataStoreList(
            int capacity,
            float capacityIncreaseMultiplier = 2,
            bool noTracking = false) : base(capacity, capacityIncreaseMultiplier, noTracking)
        {
        }

        public BuoyancyVoxelsDataStoreList(AppaList<BuoyancyVoxelsDataStore> list) : base(list)
        {
        }

        public BuoyancyVoxelsDataStoreList(BuoyancyVoxelsDataStore[] values) : base(values)
        {
        }
    }
}
