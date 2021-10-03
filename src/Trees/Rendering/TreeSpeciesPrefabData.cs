using System;
using System.Collections.Generic;
using Appalachia.Core.Trees.Metadata;

namespace Appalachia.Core.Trees.Rendering
{
    [Serializable]
    public class TreeSpeciesPrefabData
    {
        public TreeSpeciesMetadata speciesMetadata;

        public List<TreePrefabData> prefabs;
    }
}
