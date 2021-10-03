#region

using Appalachia.Core.Editing.Attributes;
using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Density
{

    public class DensityMetadata : SelfCategorizingNamingSavingAndIdentifyingScriptableObject<DensityMetadata>
    {

        [SerializeField, SmartLabel]
        public float densityGramPerCubicCentimeter;
        
        [ShowInInspector, SmartLabel]
        public float densityKGPerCubicMeter
        {
            get => densityGramPerCubicCentimeter * 1000.0f;
            set => densityGramPerCubicCentimeter = value / 1000.0f;
        }

        [SerializeField, SmartLabel]
        public PhysicMaterialWrapper materialWrapper;

    }
}