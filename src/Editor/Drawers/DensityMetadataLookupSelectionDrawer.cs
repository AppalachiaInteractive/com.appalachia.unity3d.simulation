using Appalachia.Editing.Drawers;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Selections;
using UnityEditor;

namespace Appalachia.Simulation.Drawers
{
    [CustomEditor(typeof(DensityMetadataLookupSelection), true)]
    public sealed class DensityMetadataLookupSelectionDrawer : MetadataLookupSelectionBaseDrawer<
        DensityMetadataLookupSelection, DensityMetadataCollection, DensityMetadata>
    {
    }
}
