using System;
using Appalachia.Core.Collections;
using Appalachia.Core.Collections.Implementations.Lists;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    [Serializable]
    public class BuoyancyVoxelsLookup : AppaLookup<string, BuoyancyVoxelsDataStore, stringList,
        BuoyancyVoxelsDataStoreList>
    {
        /// <inheritdoc />
        protected override Color GetDisplayColor(string key, BuoyancyVoxelsDataStore value)
        {
            return Color.white;
        }

        /// <inheritdoc />
        protected override string GetDisplaySubtitle(string key, BuoyancyVoxelsDataStore value)
        {
            return value.resolution.ToString();
        }

        /// <inheritdoc />
        protected override string GetDisplayTitle(string key, BuoyancyVoxelsDataStore value)
        {
            return value.identifier;
        }
    }
}
