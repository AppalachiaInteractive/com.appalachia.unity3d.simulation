#region

using System;
using Appalachia.Core.Collections;
using Appalachia.Simulation.Buoyancy.Data;

#endregion

namespace Appalachia.Simulation.Buoyancy.Collections
{
    [Serializable]
    public sealed class AppaList_BuoyancyData : AppaList<BuoyancyData>
    {
        public AppaList_BuoyancyData()
        {
        }

        public AppaList_BuoyancyData(
            int capacity,
            float capacityIncreaseMultiplier = 2,
            bool noTracking = false) : base(capacity, capacityIncreaseMultiplier, noTracking)
        {
        }

        public AppaList_BuoyancyData(AppaList<BuoyancyData> list) : base(list)
        {
        }

        public AppaList_BuoyancyData(BuoyancyData[] values) : base(values)
        {
        }
    }
}
