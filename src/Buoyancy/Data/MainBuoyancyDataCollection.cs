using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Core.Objects.Scriptables;
using Appalachia.Simulation.Buoyancy.Collections;

namespace Appalachia.Simulation.Buoyancy.Data
{
    public class MainBuoyancyDataCollection : SingletonAppalachiaObjectLookupCollection<string, BuoyancyData,
        stringList, BuoyancyDataList, BuoyancyDataLookup, BuoyancyDataCollection, MainBuoyancyDataCollection>
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(MainBuoyancyDataCollection),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew<MainBuoyancyDataCollection>();
        }
#endif
    }
}
