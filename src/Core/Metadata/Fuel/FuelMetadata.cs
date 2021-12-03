/*using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Simulation.Fuel
{
    [CreateAssetMenu(menuName = "Internal/Metadata/Simulation/Heat/Fuel/FuelMetadata", order = 0)]
    public class FuelMetadata : AppalachiaScriptableObject<FuelMetadata>
    {
        public float btuPerKgGreen;
        public float btuPerKgDry;
        [Range(0.5f, 2.2f)] public float densityMultiplier;
        [Range(0.5f, 2.2f)] public float smokeMultiplier;
        [Range(120, 200)] public int charTemperature = 120;
        [Range(150, 400)] public int ignitionTemperature = 200;
        public FuelBurnRate burnRate;
        public FuelBurnScale burnScale;
        
        #if UNITY_EDITOR
        public void ToWood()
        {
            var wood = AppalachiaObject.LoadOrCreateNew<WoodSimulationData>($"{name}.asset", true, false, false);

            wood.btuGreen = btuPerKgGreen;
            wood.btuDry = btuPerKgDry;

            var dTime = (densityMultiplier - 0.5772006f) / (2.195011f - 0.5772006f);

            wood.density = (int)Mathf.Lerp(400, 1000, dTime);
            wood.charTemperature = charTemperature;
            wood.ignitionTemperature = ignitionTemperature;
            wood.burnRate = burnRate;
            wood.burnScale = burnScale;
            wood.smokeAmount = smokeMultiplier;

            wood.MarkAsModified();
        }

        [Button, ShowInInspector]
        public static void ConvertAllToWood()
        {
            var metadatas = AssetDatabaseManager.FindAssets("t: FuelMetadata");

            foreach (var asset in metadatas)
            {
                var path = AssetDatabaseManager.GUIDToAssetPath(asset);

                var load = AssetDatabaseManager.LoadAssetAtPath<FuelMetadata>(path);
                
                load.ToWood();
            }
            
            AssetDatabaseManager.SaveAssetsNextFrame();
        }
        
        #endif
    }
}*/


