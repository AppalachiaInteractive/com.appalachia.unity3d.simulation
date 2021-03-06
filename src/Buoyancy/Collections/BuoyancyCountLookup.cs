using System;
using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Utility.Colors;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Collections
{
    [Serializable]
    public sealed class BuoyancyCountLookup : AppaLookup<Buoyant, int, AppaList_Buoyant, intList>
    {
        /// <inheritdoc />
        protected override Color GetDisplayColor(Buoyant key, int value)
        {
            return key.submerged ? Colors.Aquamarine3 : Colors.White;
        }

        /// <inheritdoc />
        protected override string GetDisplaySubtitle(Buoyant key, int value)
        {
            return ZString.Format("{0} colliders", value);
        }

        /// <inheritdoc />
        protected override string GetDisplayTitle(Buoyant key, int value)
        {
            return key.name;
        }
    }
}
