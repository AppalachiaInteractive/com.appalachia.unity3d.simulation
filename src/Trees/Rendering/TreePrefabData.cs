using System;
using Appalachia.Simulation.Core.Metadata.Tree.Types;

namespace Appalachia.Simulation.Trees.Rendering
{
    [Serializable]
    public class TreePrefabData
    {
        public int treePrefabID;
        
        public TreePrefabGPUInstancerPrototypeSet prototypes;
        public TreePrefabGPUInstancerPrototypeSet ngoPrototypes;
        public int individualID;
        public AgeType age;
    }
}