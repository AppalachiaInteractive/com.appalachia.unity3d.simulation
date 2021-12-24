using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Tree;

namespace Appalachia.Simulation.Trees.Rendering
{
    [Serializable]
    public class TreeSpeciesPrefabData : AppalachiaSimpleBase
    {
        public TreeSpeciesMetadata speciesMetadata;

        public List<TreePrefabData> prefabs;
    }
}
