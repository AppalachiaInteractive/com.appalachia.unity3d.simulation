using Appalachia.Editing.Drawers;
using Appalachia.Simulation.Core.Metadata.Wood;
using Appalachia.Simulation.Core.Selections;
using UnityEditor;

namespace Appalachia.Simulation.Drawers
{
    [CustomEditor(typeof(WoodSimulationDataLookupSelection), true)]
    public sealed class WoodSimulationDataLookupSelectionDrawer : MetadataLookupSelectionBaseDrawer<
        WoodSimulationDataLookupSelection, WoodSimulationDataCollection, WoodSimulationData>
    {
    }
}
