using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Core.Objects.Scriptables;

namespace Appalachia.Simulation.Buoyancy.Voxels
{
    public class MainBuoyancyVoxelsDataStoreLookup : SingletonAppalachiaObjectLookupCollection<string,
        BuoyancyVoxelsDataStore, stringList, BuoyancyVoxelsDataStoreList, BuoyancyVoxelsLookup,
        BuoyancyVoxelsDataStoreLookup, MainBuoyancyVoxelsDataStoreLookup>
    {
    }
}
