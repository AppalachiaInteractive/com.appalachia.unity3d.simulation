using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Build.Execution
{
    public static class TreeMaterialUsageTracker
    {
        private static Dictionary<Material, HashSet<TreeDataContainer>> _treeUsages;
        
        private static Dictionary<Material, BranchDataContainer> _branches;

        public static void Refresh()
        {
            if (_treeUsages == null)
            {
                _treeUsages = new Dictionary<Material, HashSet<TreeDataContainer>>();
            }
            
            _treeUsages.Clear();

            
            if (_branches == null)
            {
                _branches = new Dictionary<Material, BranchDataContainer>();
            }
            
            _branches.Clear();



            var trees = AssetDatabaseManager.FindAssets<TreeDataContainer>();

            for (var i = 0; i < trees.Count; i++)
            {
                var tree = trees[i];

                foreach (var material in tree.materials.inputMaterialCache.atlasInputMaterials)
                {
                    if (material.material == null) continue;
                    
                    if (!_treeUsages.ContainsKey(material.material))
                    {
                        _treeUsages.Add(material.material, new HashSet<TreeDataContainer>());
                    }

                    _treeUsages[material.material].Add(tree);
                }
            }
            
            var branches = AssetDatabaseManager.FindAssets<BranchDataContainer>();

            for (var i = 0; i < branches.Count; i++)
            {
                var branch = branches[i];

                foreach (var snapshot in branch.snapshots)
                {
                    if ((snapshot == null) ||
                        (snapshot.branchOutputMaterial == null) ||
                        (snapshot.branchOutputMaterial.Count == 0) ||
                        (snapshot.branchOutputMaterial.GetMaterialElementByIndex(0) == null) ||
                        (snapshot.branchOutputMaterial.GetMaterialElementByIndex(0).asset == null))
                    {
                        continue;
                    }

                    var mat = snapshot.branchOutputMaterial.GetMaterialElementByIndex(0);

                    _branches.Add(mat.asset, branch);
                }
            }
        }

        public static bool IsMaterialABranch(Material m)
        {
            CheckInitialization();
            return _branches.ContainsKey(m);
        }
        
        public static bool IsMaterialUsedInTrees(Material m)
        {
            CheckInitialization();
            return _treeUsages.ContainsKey(m);
        }
        
        public static BranchDataContainer GetBranchDataContainer(Material m)
        {
            CheckInitialization();
            return _branches[m];
        }
        
        public static IEnumerable<TreeDataContainer> GetTreeDataContainers(Material m)
        {
            CheckInitialization();
            return _treeUsages[m];
        }

        private static void CheckInitialization()
        {
            if ((_branches == null) || (_treeUsages == null))
            {
                Refresh();
            }
        }
    }
}
