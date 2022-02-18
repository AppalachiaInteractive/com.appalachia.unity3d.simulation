using System;
using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Collections
{
    [Serializable]
    public sealed class
        BuoyancyLookup : AppaLookup<GameObject, Buoyant, AppaList_GameObject, AppaList_Buoyant>
    {
        /// <inheritdoc />
        protected override Color GetDisplayColor(GameObject key, Buoyant value)
        {
            return value.submerged ? Colors.Aquamarine3 : Colors.White;
        }

        /// <inheritdoc />
        protected override string GetDisplaySubtitle(GameObject key, Buoyant value)
        {
            return value.buoyancyData.name;
        }

        /// <inheritdoc />
        protected override string GetDisplayTitle(GameObject key, Buoyant value)
        {
            return key.name;
        }
    }
}
