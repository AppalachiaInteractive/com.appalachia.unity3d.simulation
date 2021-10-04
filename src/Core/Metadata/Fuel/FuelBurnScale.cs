using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Internal.Core.Trees.Simulation.Fuel
{
    [CreateAssetMenu(menuName = "Internal/Metadata/Simulation/Heat/Fuel/FuelBurnScale", order = 0)]
    public class FuelBurnScale : InternalScriptableObject<FuelBurnScale>
    {
        public Vector3 burnScale;
    }
}