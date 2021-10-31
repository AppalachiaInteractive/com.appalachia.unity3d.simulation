using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.Fuel
{
    public class FuelBurnRate : AppalachiaObject<FuelBurnRate>
    {
        public float kgCharPerHour = 2f;
        public float kgBurnPerHour = 1f;
        public float ignitionRate = 0.05f;

        [UnityEditor.MenuItem(PKG.Menu.Assets.Base + nameof(FuelBurnRate), priority = PKG.Menu.Assets.Priority)]
        public static void CreateAsset()
        {
            CreateNew();
        }
    }
}
