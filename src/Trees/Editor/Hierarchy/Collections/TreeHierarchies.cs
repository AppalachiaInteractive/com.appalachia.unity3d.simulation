using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.Selections.State;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Collections
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class TreeHierarchies : HierarchyCollection<TreeHierarchies>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static TreeHierarchies()
        {
            TreeSpeciesEditorSelection.InstanceAvailable += i => _treeSpeciesEditorSelection = i;
        }

        public TreeHierarchies()
        {
            roots = new List<RootHierarchyData>();
            branches = new List<BranchHierarchyData>();
            leaves = new List<LeafHierarchyData>();
            fruits = new List<FruitHierarchyData>();
            knots = new List<KnotHierarchyData>();
            fungi = new List<FungusHierarchyData>();
        }

        #region Static Fields and Autoproperties

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        #region Fields and Autoproperties

        [HideInInspector] public List<BranchHierarchyData> branches;
        [HideInInspector] public List<FruitHierarchyData> fruits;
        [HideInInspector] public List<FungusHierarchyData> fungi;
        [HideInInspector] public List<KnotHierarchyData> knots;
        [HideInInspector] public List<LeafHierarchyData> leaves;
        [HideInInspector] public List<RootHierarchyData> roots;

        #endregion

        public override int Count =>
            (branches?.Count ?? 0) +
            (fruits?.Count ?? 0) +
            (fungi?.Count ?? 0) +
            (knots?.Count ?? 0) +
            (leaves?.Count ?? 0) +
            (roots?.Count ?? 0) +
            (trunks?.Count ?? 0);

        protected override ResponsiveSettingsType SettingsType => ResponsiveSettingsType.Tree;

        public override void DeleteHierarchyChain(int hierarchyID, bool rebuildState = true)
        {
            var individuals = _treeSpeciesEditorSelection.tree.selection.selected.individuals;

            if (byParentID.ContainsKey(hierarchyID))
            {
                var children = byParentID[hierarchyID];

                foreach (var child in children)
                {
                    DeleteHierarchyChain(child.hierarchyID, false);
                }
            }

            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    foreach (var stage in age.stages)
                    {
                        stage.shapes.DeleteAllShapeChainsInHierarchy(hierarchyID, false);
                    }
                }
            }

            var hierarchy = byID[hierarchyID];

            byID.Remove(hierarchyID);

            RemoveHierarchies(hierarchy);

            if (rebuildState)
            {
                Rebuild();
            }
        }

        public override IEnumerator<HierarchyData> GetEnumerator()
        {
            foreach (var hierarchy in trunks)
            {
                yield return hierarchy;
            }

            foreach (var hierarchy in roots)
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

            foreach (var hierarchy in fungi)
            {
                yield return hierarchy;
            }

            foreach (var hierarchy in knots)
            {
                yield return hierarchy;
            }
        }

        public override IEnumerable<HierarchyData> GetHierarchies(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Root:
                    return roots;
                case TreeComponentType.Trunk:
                    return trunks;
                case TreeComponentType.Branch:
                    return branches;
                case TreeComponentType.Leaf:
                    return leaves;
                case TreeComponentType.Fruit:
                    return fruits;
                case TreeComponentType.Knot:
                    return knots;
                case TreeComponentType.Fungus:
                    return fungi;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public override void UpdateHierarchyParent(
            HierarchyData hierarchy,
            HierarchyData parent,
            bool rebuildState = true)
        {
            foreach (var individual in _treeSpeciesEditorSelection.tree.selection.selected.individuals)
            {
                foreach (var age in individual.ages)
                {
                    foreach (var stage in age.stages)
                    {
                        stage.shapes.DeleteAllShapeChainsInHierarchy(hierarchy.hierarchyID);
                    }
                }
            }

            hierarchy.parentHierarchyID = parent.hierarchyID;

            if (rebuildState)
            {
                Rebuild();
            }
        }

        public void CopyHierarchiesTo(TreeHierarchies th)
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

            //var breakMat = th.trunks.Select(b => b.limb.breakMaterial).FirstOrDefault(b => b != null);

            /*if (breakMat == null)
            {
                breakMat = th.branches.Select(b => b.limb.breakMaterial).FirstOrDefault(b => b != null);
            }*/

            var leafMat = th.leaves.Select(b => b.geometry.leafMaterial).FirstOrDefault(b => b != null);

            var fruit = th.fruits.Select(b => b.geometry.prefab).FirstOrDefault(b => b != null);
            var knot = th.knots.Select(b => b.geometry.prefab).FirstOrDefault(b => b != null);
            var mushroom = th.fungi.Select(b => b.geometry.prefab).FirstOrDefault(b => b != null);

            th.trunks.Clear();
            th.branches.Clear();
            th.roots.Clear();
            th.fruits.Clear();
            th.leaves.Clear();
            th.knots.Clear();
            th.fungi.Clear();

            th.idGenerator.SetNextID(0);

            for (var i = 0; i < trunks.Count; i++)
            {
                var model = trunks[i];

                th.trunks.Add(new TrunkHierarchyData(model.hierarchyID, SettingsType));

                th.trunks[i].CopyGenerationSettings(model);
                th.trunks[i].geometry.barkMaterial = barkMat;

                //th.trunks[i].limb.breakMaterial = breakMat;
            }

            for (var i = 0; i < roots.Count; i++)
            {
                var model = roots[i];

                th.roots.Add(new RootHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType));

                th.roots[i].CopyGenerationSettings(model);
                th.roots[i].geometry.barkMaterial = barkMat;

                //th.roots[i].limb.breakMaterial = breakMat;
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

            for (var i = 0; i < leaves.Count; i++)
            {
                var model = leaves[i];

                th.leaves.Add(
                    new LeafHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                th.leaves[i].CopyGenerationSettings(model);
                th.leaves[i].geometry.leafMaterial = leafMat;
            }

            for (var i = 0; i < fruits.Count; i++)
            {
                var model = fruits[i];

                th.fruits.Add(
                    new FruitHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                th.fruits[i].CopyGenerationSettings(model);
                th.fruits[i].geometry.prefab = fruit;
            }

            for (var i = 0; i < knots.Count; i++)
            {
                var model = knots[i];

                th.knots.Add(new KnotHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType));

                th.knots[i].CopyGenerationSettings(model);
                th.knots[i].geometry.prefab = knot;
            }

            for (var i = 0; i < fungi.Count; i++)
            {
                var model = fungi[i];

                th.fungi.Add(
                    new FungusHierarchyData(model.hierarchyID, model.parentHierarchyID, SettingsType)
                );

                th.fungi[i].CopyGenerationSettings(model);
                th.fungi[i].geometry.prefab = mushroom;
            }

            th.Rebuild();
        }

        protected override HierarchyData AddHierarchy(int id, int parentID, TreeComponentType type)
        {
            HierarchyData newHierarchy;
            switch (type)
            {
                case TreeComponentType.Root:
                    newHierarchy = new RootHierarchyData(id, parentID, SettingsType);
                    roots.Add((RootHierarchyData)newHierarchy);
                    break;
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
                case TreeComponentType.Knot:
                    newHierarchy = new KnotHierarchyData(id, parentID, SettingsType);
                    knots.Add((KnotHierarchyData)newHierarchy);
                    break;
                case TreeComponentType.Fungus:
                    newHierarchy = new FungusHierarchyData(id, parentID, SettingsType);
                    fungi.Add((FungusHierarchyData)newHierarchy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return newHierarchy;
        }

        protected override void ClearInternal()
        {
            trunks.Clear();
            roots.Clear();
            branches.Clear();
            leaves.Clear();
            fruits.Clear();
            knots.Clear();
            fungi.Clear();
        }

        protected override void DistributionSettingsChanged()
        {
            TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        protected override void RemoveHierarchies(HierarchyData hierarchy)
        {
            switch (hierarchy.type)
            {
                case TreeComponentType.Root:
                    roots.Remove(hierarchy as RootHierarchyData);
                    break;
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
                case TreeComponentType.Knot:
                    knots.Remove(hierarchy as KnotHierarchyData);
                    break;
                case TreeComponentType.Fungus:
                    fungi.Remove(hierarchy as FungusHierarchyData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hierarchy.type), hierarchy.type, null);
            }
        }
    }
}
