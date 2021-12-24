using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Root;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class AssetStatistics : AppalachiaSimpleBase
    {
        public List<AssetStatistic> statistics = new List<AssetStatistic>();
    }
}
