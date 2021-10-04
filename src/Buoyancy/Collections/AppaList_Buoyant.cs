#region

using System;
using Appalachia.Core.Collections;

#endregion

namespace Appalachia.Simulation.Buoyancy.Collections
{
    [Serializable]
    public sealed class AppaList_Buoyant : AppaList<Buoyant>
    {
        public AppaList_Buoyant()
        {
        }

        public AppaList_Buoyant(
            int capacity,
            float capacityIncreaseMultiplier = 2,
            bool noTracking = false) : base(capacity, capacityIncreaseMultiplier, noTracking)
        {
        }

        public AppaList_Buoyant(AppaList<Buoyant> list) : base(list)
        {
        }

        public AppaList_Buoyant(Buoyant[] values) : base(values)
        {
        }
    }
}
