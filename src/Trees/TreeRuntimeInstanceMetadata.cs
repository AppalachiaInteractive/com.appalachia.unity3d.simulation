using System.Collections.Generic;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Core.Metadata.POI;
using Appalachia.Simulation.Core.Metadata.Tree.Types;

namespace Appalachia.Simulation.Trees
{
    public class
        TreeRuntimeInstanceMetadata : SelfSavingAndIdentifyingScriptableObject<
            TreeRuntimeInstanceMetadata>
    {
        public string speciesName;

        public int individualID;

        public AgeType age;

        public StageType stage;

        public float rootDepth;

        public List<RuntimePointOfInterest> pointsOfInterest = new();
    }
}
