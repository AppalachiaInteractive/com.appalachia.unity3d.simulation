using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Utility;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Collections
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