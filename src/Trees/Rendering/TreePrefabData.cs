using System;
using Appalachia.Core.Trees.Types;

namespace Appalachia.Core.Trees.Rendering
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