using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Core.Metadata.Density
{
    public class
        DensityMetadataCollection : AppalachiaMetadataCollection<DensityMetadataCollection, DensityMetadata>
    {
        [FoldoutGroup("Named")] public DensityMetadata ground;
        [FoldoutGroup("Named")] public DensityMetadata air;
        [FoldoutGroup("Named")] public DensityMetadata water;
    }
}
