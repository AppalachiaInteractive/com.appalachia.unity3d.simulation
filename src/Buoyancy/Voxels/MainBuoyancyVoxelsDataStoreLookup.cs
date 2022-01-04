using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Core.Objects.Scriptables;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    public class MainBuoyancyVoxelsDataStoreLookup : SingletonAppalachiaObjectLookupCollection<string,
        BuoyancyVoxelsDataStore, stringList, BuoyancyVoxelsDataStoreList, BuoyancyVoxelsLookup,
        BuoyancyVoxelsDataStoreLookup, MainBuoyancyVoxelsDataStoreLookup>
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(MainBuoyancyVoxelsDataStoreLookup),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew<MainBuoyancyVoxelsDataStoreLookup>();
        }
#endif
    }
}
