#region

using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Generation.Texturing.Transmission;
using Appalachia.Simulation.Trees.Prefabs;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management
{
    [Serializable]
    public class TreeMaterialCollection : MaterialCollection<TreeMaterialCollection>
    {
        [TabGroup("Transmission", Paddingless = true)]
        [InlineProperty, HideLabel]
        public MaterialTransmissionValues transmission;
        
        public static TreeMaterialCollection Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("materials");
            var instance = LoadOrCreateNew(folder, assetName);
            instance.transmission = new MaterialTransmissionValues();
            instance.Initialize(ResponsiveSettingsType.Tree);
            return instance;
        }

        
        public void UpdateMaterials(TreeSpecies species, TreePrefabCollection prefabs, int lodCount, bool shadowCaster)
        {
            inputMaterialCache.Update(species, prefabs, _ids);
            outputMaterialCache.Update(_ids, inputMaterialCache, settingsType, lodCount, shadowCaster);
        }
    }
}
