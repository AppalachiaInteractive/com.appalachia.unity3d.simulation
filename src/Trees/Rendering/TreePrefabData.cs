using System;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Tree.Types;

namespace Appalachia.Simulation.Trees.Rendering
{
    [Serializable]
    public class TreePrefabData : AppalachiaSimpleBase
    {
        public int treePrefabID;

        public TreePrefabGPUInstancerPrototypeSet prototypes;
        public TreePrefabGPUInstancerPrototypeSet ngoPrototypes;
        public int individualID;
        public AgeType age;
    }
}
