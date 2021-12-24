using System;
using Appalachia.Core.Collections.Implementations.Lists;
using Appalachia.Core.Objects.Scriptables;
using Appalachia.Simulation.Buoyancy.Collections;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class BuoyancyDataCollection : AppalachiaObjectLookupCollection<string, BuoyancyData, stringList,
        BuoyancyDataList, BuoyancyDataLookup, BuoyancyDataCollection>
    {
        
        
        public override bool HasDefault => false;

        protected override string GetUniqueKeyFromValue(BuoyancyData value)
        {
            return value.meshGUID;
        }
    }
}
