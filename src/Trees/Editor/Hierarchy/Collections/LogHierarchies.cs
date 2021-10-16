using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Shape.Collections;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Collections
{
    [Serializable]
    public class LogHierarchies : HierarchyCollection<LogHierarchies>
    {
        [HideInInspector] public List<BranchHierarchyData> branches;

        public LogHierarchies()
        {
            branches = new List<BranchHierarchyData>();
        }

        public override int Count =>
            (branches?.Count ?? 0) + (trunks?.Count ?? 0);

        protected override void ClearInternal()
        {
            trunks.Clear();
            branches.Clear();
        }

        public override IEnumerator<HierarchyData> GetEnumerator()
        {
            foreach (var hierarchy in trunks)
            {
                yield return hierarchy;
            }

            foreach (var hierarchy in branches)
            {
                yield return hierarchy;
            }
        }

        public override IEnumerable<HierarchyData> GetHierarchies(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Trunk:
                    return trunks;
                case TreeComponentType.Branch:
                    return branches;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        protected override void RemoveHierarchies(HierarchyData hierarchy)
        {
            switch (hierarchy.type)
            {
                case TreeComponentType.Trunk:
                    trunks.Remove(hierarchy as TrunkHierarchyData);
                    break;
                case TreeComponentType.Branch:
                    branches.Remove(hierarchy as BranchHierarchyData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hierarchy.type), hierarchy.type, null);
            }
        }

        public override void UpdateHierarchyParent(HierarchyData hierarchy, HierarchyData parent, bool rebuildState = true)
        {
            throw new NotImplementedException();
        }

        public override void DeleteHierarchyChain(int hierarchyID, bool rebuildState = true)
        {
            throw new NotImplementedException();
        }

        protected override HierarchyData AddHierarchy(int id, int parentID, TreeComponentType type)
        {
            HierarchyData newHierarchy;
            switch (type)
            {
                case TreeComponentType.Branch:
                    newHierarchy = new BranchHierarchyData(id, parentID, SettingsType);
                    branches.Add((BranchHierarchyData) newHierarchy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return newHierarchy;
        }

        protected override ResponsiveSettingsType SettingsType => ResponsiveSettingsType.Branch;

        protected override void DistributionSettingsChanged()
        {
            BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        public void DeleteHierarchyChain(BranchShapes shapes, int hierarchyID, bool rebuildState = true)
        {
            if (byParentID.ContainsKey(hierarchyID))
            {
                var children = byParentID[hierarchyID];

                foreach (var child in children)
                {
                    DeleteHierarchyChain(shapes, child.hierarchyID, false);
                }
            }

            shapes.DeleteAllShapeChainsInHierarchy(hierarchyID, false);

            var hierarchy = byID[hierarchyID];

            byID.Remove(hierarchyID);

            RemoveHierarchies(hierarchy);

            if (rebuildState)
            {
                Rebuild();
            }
        }
        
            
        
        public void CopyHierarchiesTo(LogHierarchies th)
        {
            if (th == this)
            {
                return;
            }
            
            var barkMat = th.trunks.Select(b => b.geometry.barkMaterial).FirstOrDefault(b => b != null);

            if (barkMat == null)
            {
                barkMat = th.branches.Select(b => b.geometry.barkMaterial).FirstOrDefault(b => b != null);
            }

            /*var breakMat = th.trunks.Select(b => b.limb.breakMaterial).FirstOrDefault(b => b != null);
            
            if (breakMat == null)
            {
                breakMat = th.branches.Select(b => b.limb.breakMaterial).FirstOrDefault(b => b != null);
            }*/
            
            th.trunks.Clear();
            th.branches.Clear();
            
            th.idGenerator.SetNextID(0);

            for (var i = 0; i < trunks.Count; i++)
            {
                var model = trunks[i];
                
                th.trunks.Add(
                    new TrunkHierarchyData(model.hierarchyID, SettingsType)
                );

                th.trunks[i].CopyGenerationSettings(model);
                th.trunks[i].geometry.barkMaterial = barkMat;
                //th.trunks[i].limb.breakMaterial = breakMat;
            }

            for (var i = 0; i < branches.Count; i++)
            {
                var model = branches[i];
                
                th.branches.Add(
                    new BranchHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                th.branches[i].CopyGenerationSettings(model);
                th.branches[i].geometry.barkMaterial = barkMat;
                //th.branches[i].limb.breakMaterial = breakMat;
            }

            th.Rebuild();
        }
    }
}