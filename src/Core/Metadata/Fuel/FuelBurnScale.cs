using Appalachia.Base.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.Fuel
{
    [CreateAssetMenu(menuName = "Internal/Metadata/Simulation/Heat/Fuel/FuelBurnScale", order = 0)]
    public class FuelBurnScale : InternalScriptableObject<FuelBurnScale>
    {
        public Vector3 burnScale;
    }
}
