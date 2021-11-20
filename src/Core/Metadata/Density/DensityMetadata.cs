#region

using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Core.Metadata.Materials;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core.Metadata.Density
{
    public class DensityMetadata : CategorizableAutonamedIdentifiableAppalachiaObject
    {
        [SerializeField]
        [SmartLabel]
        public float densityGramPerCubicCentimeter;

        [SerializeField]
        [SmartLabel]
        public PhysicMaterialWrapper materialWrapper;

        [ShowInInspector]
        [SmartLabel]
        public float densityKGPerCubicMeter
        {
            get => densityGramPerCubicCentimeter * 1000.0f;
            set => densityGramPerCubicCentimeter = value / 1000.0f;
        }
    }
}
