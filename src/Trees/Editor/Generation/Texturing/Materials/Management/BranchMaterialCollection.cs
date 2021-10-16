#region

using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Prefabs;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management
{
    [Serializable]
    public class BranchMaterialCollection : MaterialCollection<BranchMaterialCollection>
    {
        /*[TitleGroup("Branch Material", Alignment = TitleAlignments.Centered)]
        public BranchOutputMaterial branchOutputMaterial;*/

        public static BranchMaterialCollection Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("materials");
            var instance = LoadOrCreateNew(folder, assetName);

            instance.Initialize(ResponsiveSettingsType.Branch);
            //instance.branchOutputMaterial = new BranchOutputMaterial(instance._ids.GetNextIdAndIncrement());
            return instance;
        }
        
        public void UpdateMaterials(TreeBranch species, TreePrefabCollection prefabs)
        {
            inputMaterialCache.Update(species, prefabs, _ids);
            outputMaterialCache.Update(_ids, inputMaterialCache, settingsType, 
                1, false);
        }

    }
}