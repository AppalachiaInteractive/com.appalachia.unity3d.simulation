using Appalachia.Core.Objects.Scriptables;
using Appalachia.Simulation.Core.Metadata.Collections;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Core.Metadata.Density
{
    public class DensityMetadataCollection : AppalachiaMetadataCollection<DensityMetadataCollection,
        DensityMetadata, AppaList_DensityMetadata>
    {
        #region Fields and Autoproperties

        [FoldoutGroup("Named")] public DensityMetadata ground;
        [FoldoutGroup("Named")] public DensityMetadata air;
        [FoldoutGroup("Named")] public DensityMetadata water;

        #endregion

        
    }
}
