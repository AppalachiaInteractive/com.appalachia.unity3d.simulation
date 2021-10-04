using System;
using System.Collections.Generic;
using Appalachia.Simulation.Core.Metadata.Tree;

namespace Appalachia.Simulation.Trees.Rendering
{
    [Serializable]
    public class TreeSpeciesPrefabData
    {
        public TreeSpeciesMetadata speciesMetadata;

        public List<TreePrefabData> prefabs;
    }
}
