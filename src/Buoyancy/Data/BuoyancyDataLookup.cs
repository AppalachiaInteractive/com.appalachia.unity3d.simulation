using System;
using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Simulation.Buoyancy.Collections;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class BuoyancyDataLookup : AppaLookup<string, BuoyancyData, stringList, BuoyancyDataList>
    {
        /// <inheritdoc />
        protected override Color GetDisplayColor(string key, BuoyancyData value)
        {
            return Color.white;
        }

        /// <inheritdoc />
        protected override string GetDisplaySubtitle(string key, BuoyancyData value)
        {
            return value.name;
        }

        /// <inheritdoc />
        protected override string GetDisplayTitle(string key, BuoyancyData value)
        {
            return key;
        }
    }
}
