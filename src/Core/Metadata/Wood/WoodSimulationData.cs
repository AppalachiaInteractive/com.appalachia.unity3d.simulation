using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Metadata.Fuel;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Core.Metadata.Wood
{
    public class WoodSimulationData : CategorizableIdentifiableAppalachiaObject
    {
        #region Fields and Autoproperties

        [BoxGroup("Physical")] public DensityMetadata densityMetadata;

        [BoxGroup("Burning")]
        [SuffixLabel("/kg")]
        public double btuDry = 15000.0f;

        [BoxGroup("Burning")]
        [SuffixLabel("/kg")]
        public double btuGreen = 10000.0f;

        [BoxGroup("Burning")]
        [PropertyRange(0.5f, 2.2f)]
        public float smokeAmount = 1.0f;

        [BoxGroup("Burning")] public FuelBurnRate burnRate;

        [BoxGroup("Burning")] public FuelBurnScale burnScale;

        [BoxGroup("Ignition")]
        [PropertyRange(120, 200)]
        [SuffixLabel("°C")]
        public int charTemperature = 120;

        [BoxGroup("Ignition")]
        [PropertyRange(150, 400)]
        [SuffixLabel("°C")]
        public int ignitionTemperature = 200;

        #endregion

        [BoxGroup("Physical")]
        [PropertyRange(400, 1000)]
        [ReadOnly]
        [ShowInInspector]
        public float densityKgM3 => densityMetadata == null ? 650f : densityMetadata.densityKGPerCubicMeter;

        [BoxGroup("Physical")]
        [ReadOnly]
        [ShowInInspector]
        public float woodHardness =>
            (densityMetadata == null ? 650f : densityMetadata.densityKGPerCubicMeter) / 1250f;

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

            var densities = AssetDatabaseManager.FindAssets<DensityMetadata>();

            for (var i = 0; i < densities.Count; i++)
            {
                var d = densities[i];

                if (d.name == $"wood_{name}")
                {
                    densityMetadata = d;
                    SetDirty();
                    return;
                }
            }

            densityMetadata = LoadOrCreateNew<DensityMetadata>($"wood_{name}");
#pragma warning disable 612
            densityMetadata.densityKGPerCubicMeter = 650f;
#pragma warning restore 612
        }

        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(WoodSimulationData),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew<WoodSimulationData>();
        }
#endif
    }
}
