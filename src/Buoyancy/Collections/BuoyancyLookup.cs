using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Collections
{
    public sealed class
        BuoyancyLookup : AppaLookup<GameObject, Buoyant, AppaList_GameObject, AppaList_Buoyant>
    {
        protected override Color GetDisplayColor(GameObject key, Buoyant value)
        {
            return value.submerged ? Colors.Aquamarine3 : Colors.White;
        }

        protected override string GetDisplaySubtitle(GameObject key, Buoyant value)
        {
            return value.buoyancyData.name;
        }

        protected override string GetDisplayTitle(GameObject key, Buoyant value)
        {
            return key.name;
        }
    }
}
