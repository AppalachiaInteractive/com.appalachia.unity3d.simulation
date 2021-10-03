
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Simulation.Buoyancy;
using Appalachia.Simulation.Buoyancy.Collections;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.Core.Collections.Implementations.Lookups
{
    public class BuoyancyLookup : AppaLookup<GameObject, Buoyant, AppaList_GameObject, AppaList_Buoyant>
    {
        protected override string GetDisplayTitle(GameObject key, Buoyant value)
        {
            return key.name;
        }

        protected override string GetDisplaySubtitle(GameObject key, Buoyant value)
        {
            return value.buoyancyData.name;
        }

        protected override Color GetDisplayColor(GameObject key, Buoyant value)
        {
            return value.submerged ? Colors.Aquamarine3 : Colors.White;
        }
    }
}