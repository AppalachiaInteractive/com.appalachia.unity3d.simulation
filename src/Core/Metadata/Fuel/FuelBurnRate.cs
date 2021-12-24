namespace Appalachia.Simulation.Core.Metadata.Fuel
{
    public class FuelBurnRate : AppalachiaSimulationObject
    {        
        #region Fields and Autoproperties

        public float ignitionRate = 0.05f;
        public float kgBurnPerHour = 1f;
        public float kgCharPerHour = 2f;

        #endregion

        #region Menu Items

#if UNITY_EDITOR
        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(FuelBurnRate),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew<FuelBurnRate>();
        }
#endif

        #endregion
    }
}
