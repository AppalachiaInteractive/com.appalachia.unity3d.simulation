using System;
using Appalachia.Core.Collections;
using Appalachia.Simulation.Core.Metadata.Wood;

namespace Appalachia.Simulation.Core.Metadata.Collections
{
    [Serializable]
    public sealed class AppaList_WoodSimulationData : AppaList<WoodSimulationData>
    {
        public AppaList_WoodSimulationData()
        {
        }

        public AppaList_WoodSimulationData(int capacity, float capacityIncreaseMultiplier = 2, bool noTracking = false) : base(
            capacity,
            capacityIncreaseMultiplier,
            noTracking
        )
        {
        }

        public AppaList_WoodSimulationData(AppaList<WoodSimulationData> list) : base(list)
        {
        }

        public AppaList_WoodSimulationData(WoodSimulationData[] values) : base(values)
        {
        }
    }
}