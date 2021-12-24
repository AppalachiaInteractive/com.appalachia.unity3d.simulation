using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Core.Objects.Scriptables;
using Appalachia.Simulation.Buoyancy.Collections;

namespace Appalachia.Simulation.Buoyancy.Data
{
    public class MainBuoyancyDataCollection : SingletonAppalachiaObjectLookupCollection<string, BuoyancyData,
        stringList, BuoyancyDataList, BuoyancyDataLookup, BuoyancyDataCollection, MainBuoyancyDataCollection>
    {
    }
}
