using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.Fuel
{
    [CreateAssetMenu(menuName = "Internal/Metadata/Simulation/Heat/Fuel/FuelBurnScale", order = 0)]
    public class FuelBurnScale : AppalachiaScriptableObject<FuelBurnScale>
    {
        public Vector3 burnScale;
    }
}
