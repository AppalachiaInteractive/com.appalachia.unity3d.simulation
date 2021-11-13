using System;
using Appalachia.Core.Collections;
using Appalachia.Simulation.Core.Metadata.Materials;

namespace Appalachia.Simulation.Core.Metadata.Collections
{
    [Serializable]
    public sealed class AppaList_PhysicMaterialWrapper : AppaList<PhysicMaterialWrapper>
    {
        public AppaList_PhysicMaterialWrapper()
        {
        }

        public AppaList_PhysicMaterialWrapper(int capacity, float capacityIncreaseMultiplier = 2, bool noTracking = false) : base(
            capacity,
            capacityIncreaseMultiplier,
            noTracking
        )
        {
        }

        public AppaList_PhysicMaterialWrapper(AppaList<PhysicMaterialWrapper> list) : base(list)
        {
        }

        public AppaList_PhysicMaterialWrapper(PhysicMaterialWrapper[] values) : base(values)
        {
        }
    }
}