using System;
using Appalachia.Core.Collections;
using Appalachia.Simulation.Core.Metadata.Density;

namespace Appalachia.Simulation.Core.Metadata.Collections
{
    [Serializable]
    public sealed class AppaList_DensityMetadata : AppaList<DensityMetadata>
    {
        public AppaList_DensityMetadata()
        {
        }

        public AppaList_DensityMetadata(int capacity, float capacityIncreaseMultiplier = 2, bool noTracking = false) : base(
            capacity,
            capacityIncreaseMultiplier,
            noTracking
        )
        {
        }

        public AppaList_DensityMetadata(AppaList<DensityMetadata> list) : base(list)
        {
        }

        public AppaList_DensityMetadata(DensityMetadata[] values) : base(values)
        {
        }
    }
}