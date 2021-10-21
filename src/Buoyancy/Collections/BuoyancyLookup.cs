using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Utility.src.Colors;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Collections
{
    public class
        BuoyancyLookup : AppaLookup<GameObject, Buoyant, AppaList_GameObject, AppaList_Buoyant>
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
