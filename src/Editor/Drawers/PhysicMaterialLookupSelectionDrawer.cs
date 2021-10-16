using Appalachia.Editing.Drawers;
using Appalachia.Simulation.Core.Metadata.Materials;
using Appalachia.Simulation.Core.Selections;
using UnityEditor;

namespace Appalachia.Simulation.Drawers
{
    [CustomEditor(typeof(PhysicMaterialLookupSelection), true)]
    public sealed class PhysicMaterialLookupSelectionDrawer : MetadataLookupSelectionBaseDrawer<
        PhysicMaterialLookupSelection, PhysicsMaterialsCollection, PhysicMaterialWrapper>
    {
    }
}
