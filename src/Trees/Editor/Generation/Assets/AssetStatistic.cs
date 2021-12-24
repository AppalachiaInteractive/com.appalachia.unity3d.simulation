using System;
using Appalachia.Core.Objects.Root;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class AssetStatistic : AppalachiaSimpleBase
    {
        public int submeshes;
        public int triangles;
        public int vertices;
    }
}
