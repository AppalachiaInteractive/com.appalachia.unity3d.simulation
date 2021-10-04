/*using Internal.Core.Base;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Internal.Core.Trees.Simulation.Fuel
{
    [CreateAssetMenu(menuName = "Internal/Metadata/Simulation/Heat/Fuel/FuelMetadata", order = 0)]
    public class FuelMetadata : InternalScriptableObject<FuelMetadata>
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
            var wood = WoodSimulationData.LoadOrCreateNew($"{name}.asset", true, false, false);

            wood.btuGreen = btuPerKgGreen;
            wood.btuDry = btuPerKgDry;

            var dTime = (densityMultiplier - 0.5772006f) / (2.195011f - 0.5772006f);

            wood.density = (int)Mathf.Lerp(400, 1000, dTime);
            wood.charTemperature = charTemperature;
            wood.ignitionTemperature = ignitionTemperature;
            wood.burnRate = burnRate;
            wood.burnScale = burnScale;
            wood.smokeAmount = smokeMultiplier;

            EditorUtility.SetDirty(wood);
        }

        [Button, ShowInInspector]
        public static void ConvertAllToWood()
        {
            var metadatas = AssetDatabase.FindAssets("t: FuelMetadata");

            foreach (var asset in metadatas)
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);

                var load = AssetDatabase.LoadAssetAtPath<FuelMetadata>(path);
                
                load.ToWood();
            }
            
            AssetDatabaseSaveManager.SaveAssetsNextFrame();
        }
        
        #endif
    }
}*/


