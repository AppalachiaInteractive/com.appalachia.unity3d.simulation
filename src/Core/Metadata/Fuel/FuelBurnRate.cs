﻿using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.Fuel
{
    [CreateAssetMenu(menuName = "Internal/Metadata/Simulation/Heat/Fuel/FuelBurnRate", order = 0)]
    public class FuelBurnRate : AppalachiaScriptableObject<FuelBurnRate>
    {
        public float kgCharPerHour = 2f;
        public float kgBurnPerHour = 1f;
        public float ignitionRate = 0.05f;
    }
}
