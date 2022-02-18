using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Shape.Collections;
using Appalachia.Simulation.Trees.UI.Selections.State;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Collections
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class BranchHierarchies : HierarchyCollection<BranchHierarchies>
    {
        static BranchHierarchies()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<TreeSpeciesEditorSelection>()
                                     .IsAvailableThen(i => _treeSpeciesEditorSelection = i);
        }

        public BranchHierarchies()
        {
            branches = new List<BranchHierarchyData>();
            leaves = new List<LeafHierarchyData>();
            fruits = new List<FruitHierarchyData>();
        }

        #region Static Fields and Autoproperties

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        #region Fields and Autoproperties

        [HideInInspector] public List<BranchHierarchyData> branches;
        [HideInInspector] public List<FruitHierarchyData> fruits;
        [HideInInspector] public List<LeafHierarchyData> leaves;

        #endregion

        /// <inheritdoc />
        public override int Count =>
            (branches?.Count ?? 0) + (fruits?.Count ?? 0) + (leaves?.Count ?? 0) + (trunks?.Count ?? 0);

        /// <inheritdoc />
        protected override ResponsiveSettingsType SettingsType => ResponsiveSettingsType.Branch;

        /// <inheritdoc />
        public override void DeleteHierarchyChain(int hierarchyID, bool rebuildState = true)
        {
            var branchData = _treeSpeciesEditorSelection.branch.selection.selected;

            if (byParentID.ContainsKey(hierarchyID))
            {
                var children = byParentID[hierarchyID];

                foreach (var child in children)
                {
                    DeleteHierarchyChain(child.hierarchyID, false);
                }
            }

            branchData.branch.shapes.DeleteAllShapeChainsInHierarchy(hierarchyID, false);

            var hierarchy = byID[hierarchyID];

            byID.Remove(hierarchyID);

            RemoveHierarchies(hierarchy);

            if (rebuildState)
            {
                Rebuild();
            }
        }

        /// <inheritdoc />
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

            foreach (var hierarchy in leaves)
            {
                yield return hierarchy;
            }

            foreach (var hierarchy in fruits)
            {
                yield return hierarchy;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<HierarchyData> GetHierarchies(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Trunk:
                    return trunks;
                case TreeComponentType.Branch:
                    return branches;
                case TreeComponentType.Leaf:
                    return leaves;
                case TreeComponentType.Fruit:
                    return fruits;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <inheritdoc />
        public override void UpdateHierarchyParent(
            HierarchyData hierarchy,
            HierarchyData parent,
            bool rebuildState = true)
        {
            var branchData = _treeSpeciesEditorSelection.branch.selection.selected;

            branchData.branch.shapes.DeleteAllShapeChainsInHierarchy(hierarchy.hierarchyID);

            hierarchy.parentHierarchyID = parent.hierarchyID;

            if (rebuildState)
            {
                Rebuild();
            }
        }

        public void CopyHierarchiesTo(BranchHierarchies bh)
        {
            if (bh == this)
            {
                return;
            }

            var barkMat = bh.trunks.Select(b => b.geometry.barkMaterial).FirstOrDefault(b => b != null);

            if (barkMat == null)
            {
                barkMat = bh.branches.Select(b => b.geometry.barkMaterial).FirstOrDefault(b => b != null);
            }

            //var breakMat = bh.trunks.Select(b => b.limb.breakMaterial).FirstOrDefault(b => b != null);

            /*if (breakMat == null)
            {
                breakMat = bh.branches.Select(b => b.limb.breakMaterial).FirstOrDefault(b => b != null);
            }*/

            var leafMat = bh.leaves.Select(b => b.geometry.leafMaterial).FirstOrDefault(b => b != null);

            var fruit = bh.fruits.Select(b => b.geometry.prefab).FirstOrDefault(b => b != null);

            bh.branches.Clear();
            bh.fruits.Clear();
            bh.leaves.Clear();
            bh.trunks.Clear();

            bh.idGenerator.SetNextID(0);

            for (var i = 0; i < trunks.Count; i++)
            {
                var model = trunks[i];

                bh.trunks.Add(new TrunkHierarchyData(model.hierarchyID, SettingsType));

                bh.trunks[i].CopyGenerationSettings(model);
                bh.trunks[i].geometry.barkMaterial = barkMat;

                //bh.trunks[i].limb.breakMaterial = breakMat;
            }

            for (var i = 0; i < branches.Count; i++)
            {
                var model = branches[i];

                bh.branches.Add(
                    new BranchHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                bh.branches[i].CopyGenerationSettings(model);
                bh.branches[i].geometry.barkMaterial = barkMat;

                //bh.branches[i].limb.breakMaterial = breakMat;
            }

            for (var i = 0; i < leaves.Count; i++)
            {
                var model = leaves[i];

                bh.leaves.Add(
                    new LeafHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                bh.leaves[i].CopyGenerationSettings(model);
                bh.leaves[i].geometry.leafMaterial = leafMat;
            }

            for (var i = 0; i < fruits.Count; i++)
            {
                var model = fruits[i];

                bh.fruits.Add(
                    new FruitHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                bh.fruits[i].CopyGenerationSettings(model);
                bh.fruits[i].geometry.prefab = fruit;
            }

            bh.Rebuild();
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

        /// <inheritdoc />
        protected override HierarchyData AddHierarchy(int id, int parentID, TreeComponentType type)
        {
            HierarchyData newHierarchy;
            switch (type)
            {
                case TreeComponentType.Branch:
                    newHierarchy = new BranchHierarchyData(id, parentID, SettingsType);
                    branches.Add((BranchHierarchyData)newHierarchy);
                    break;
                case TreeComponentType.Leaf:
                    newHierarchy = new LeafHierarchyData(id, parentID, SettingsType);
                    leaves.Add((LeafHierarchyData)newHierarchy);
                    break;
                case TreeComponentType.Fruit:
                    newHierarchy = new FruitHierarchyData(id, parentID, SettingsType);
                    fruits.Add((FruitHierarchyData)newHierarchy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return newHierarchy;
        }

        /// <inheritdoc />
        protected override void ClearInternal()
        {
            trunks.Clear();
            branches.Clear();
            leaves.Clear();
            fruits.Clear();
        }

        /// <inheritdoc />
        protected override void DistributionSettingsChanged()
        {
            BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        /// <inheritdoc />
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
                case TreeComponentType.Leaf:
                    leaves.Remove(hierarchy as LeafHierarchyData);
                    break;
                case TreeComponentType.Fruit:
                    fruits.Remove(hierarchy as FruitHierarchyData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hierarchy.type), hierarchy.type, null);
            }
        }
    }
}
