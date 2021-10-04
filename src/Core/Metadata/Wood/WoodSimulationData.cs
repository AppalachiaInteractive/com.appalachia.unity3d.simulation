using Appalachia.Base.Scriptables;
using Appalachia.Editing.Assets;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Metadata.Fuel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.Wood
{
    [CreateAssetMenu(menuName = "Tree Species Editor/Simulation/Wood", order = 10)]
    public class WoodSimulationData : SelfCategorizingIdentifyingAndSavingScriptableObject<WoodSimulationData>, ICategorizable
    {
        [BoxGroup("Physical")]
        [PropertyRange(400, 1000)]
        [ReadOnly, ShowInInspector] 
        public float densityKgM3 => densityMetadata == null ? 650f : densityMetadata.densityKGPerCubicMeter;

        [BoxGroup("Physical")]
        public DensityMetadata densityMetadata;
        
        [BoxGroup("Physical")]
        [ReadOnly, ShowInInspector] 
        public float woodHardness => (densityMetadata == null ? 650f : densityMetadata.densityKGPerCubicMeter) / 1250f;
        
        [BoxGroup("Ignition")]
        [PropertyRange(120, 200), SuffixLabel("°C")] 
        public int charTemperature = 120;
        
        [BoxGroup("Ignition")]
        [PropertyRange(150, 400), SuffixLabel("°C")] 
        public int ignitionTemperature = 200;
        
        [BoxGroup("Burning")]
        public FuelBurnRate burnRate;
        
        [BoxGroup("Burning")]
        public FuelBurnScale burnScale;
        
        [BoxGroup("Burning"), SuffixLabel("/kg")]
        public double btuGreen = 10000.0f;
        
        [BoxGroup("Burning"), SuffixLabel("/kg")]
        public double btuDry = 15000.0f;
        
        [BoxGroup("Burning")]
        [PropertyRange(0.5f, 2.2f)] 
        public float smokeAmount = 1.0f;

        #if UNITY_EDITOR
        private void OnEnable()
        {
            AssignBestDensity();
        }

        private void AssignBestDensity()
        {
            if (densityMetadata != null)
            {
                return;
            }
            
            var densities = AssetDatabaseHelper.FindAssets<DensityMetadata>();

            for (var i = 0; i < densities.Length; i++)
            {
                var d = densities[i];

                if (d.name == $"wood_{name}")
                {
                    densityMetadata = d;
                    SetDirty();
                    return;
                }
            }

            densityMetadata = DensityMetadata.LoadOrCreateNew($"wood_{name}");
#pragma warning disable 612
            densityMetadata.densityKGPerCubicMeter = 650f;
#pragma warning restore 612
        }
        
        #endif
    }
}
