#region

using System;
using Appalachia.Core.Collections;
using Appalachia.Simulation.Buoyancy.Data;

#endregion

namespace Appalachia.Simulation.Buoyancy.Collections
{
    [Serializable]
    public sealed class BuoyancyDataList : AppaList<BuoyancyData>
    {
        public BuoyancyDataList()
        {
        }

        public BuoyancyDataList(
            int capacity,
            float capacityIncreaseMultiplier = 2,
            bool noTracking = false) : base(capacity, capacityIncreaseMultiplier, noTracking)
        {
        }

        public BuoyancyDataList(AppaList<BuoyancyData> list) : base(list)
        {
        }

        public BuoyancyDataList(BuoyancyData[] values) : base(values)
        {
        }
    }
}
