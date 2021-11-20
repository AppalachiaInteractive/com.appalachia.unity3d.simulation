using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.Fuel
{
    public class FuelBurnScale : AppalachiaObject
    {
        #region Fields and Autoproperties

        public Vector3 burnScale;

        #endregion

        #region Menu Items

#if UNITY_EDITOR
        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(FuelBurnScale),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew<FuelBurnScale>();
        }
#endif

        #endregion
    }
}
