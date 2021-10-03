using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Simulation.Buoyancy;
using Appalachia.Simulation.Buoyancy.Collections;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.Core.Collections.Implementations.Lookups
{
    public class BuoyancyCountLookup : AppaLookup<Buoyant, int, AppaList_Buoyant, AppaList_int>
    {
        protected override string GetDisplayTitle(Buoyant key, int value)
        {
            return key.name;
        }

        protected override string GetDisplaySubtitle(Buoyant key, int value)
        {
            return $"{value} colliders";
        }

        protected override Color GetDisplayColor(Buoyant key, int value)
        {
            return key.submerged ? Colors.Aquamarine3 : Colors.White;
        }
    }
}