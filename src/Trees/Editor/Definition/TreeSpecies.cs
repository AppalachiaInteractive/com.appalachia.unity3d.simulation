using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Core.Metadata.Wood;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Collections;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public sealed class TreeSpecies : TypeBasedSettings<TreeSpecies>, ITree
    {
        #region Fields and Autoproperties

        [PropertySpace]
        [PropertyOrder(1), InlineProperty, HideLabel]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public ExternalDualSeed seed;

        [FormerlySerializedAs("name")]
        [TitleGroup("Species Information", Alignment = TitleAlignments.Centered)]
        [PropertyOrder(0)]
        public NameBasis nameBasis;

        [HideInInspector] public TreeHierarchies hierarchies;

        public WoodSimulationData woodData;

        #endregion

        [PropertyTooltip("How deep into the ground to start tree generation.")]
        [PropertyRange(0f, 3f), PropertyOrder(2), ShowInInspector]
        [OnValueChanged(nameof(DistributionSettingsChanged)), LabelWidth(100)]
        public float verticalOffset
        {
            get => hierarchies.verticalOffset;
            set => hierarchies.verticalOffset = value;
        }

        public static TreeSpecies Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("species");
            var instance = TreeSpecies.LoadOrCreateNew(folder, assetName);

            instance.nameBasis = nameBasis;
            instance.hierarchies = new TreeHierarchies();

            return instance;
        }

        public static TreeSpecies Create(TreeEditor.TreeData data, string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("species");
            var instance = TreeSpecies.LoadOrCreateNew(folder, assetName);

            instance.nameBasis = nameBasis;
            instance.seed = new ExternalDualSeed(0, 0, Mathf.Clamp(data.root.seed, 0, BaseSeed.HIGH_ELEMENT));

            var idLookup = new Dictionary<int, int>();

            instance.hierarchies.trunks = data.branchGroups
                                              .Where(bg => bg.parentGroupID == data.root.uniqueID)
                                              .Select(
                                                   bg =>
                                                   {
                                                       var newID = instance.hierarchies.idGenerator
                                                          .GetNextIdAndIncrement();

                                                       idLookup.Add(bg.uniqueID, newID);

                                                       return new TrunkHierarchyData(
                                                           newID,
                                                           bg,
                                                           data.root,
                                                           bg.materialBranch,
                                                           bg.materialBreak,
                                                           bg.materialFrond
                                                       );
                                                   }
                                               )
                                              .ToList();

            var trunkIDs = instance.hierarchies.trunks.Select(t => t.hierarchyID).ToList();

            var parentIDs = data.branchGroups.Select(t => t.parentGroupID)
                                .Concat(data.leafGroups.Select(l => l.parentGroupID))
                                .ToList();

            var allBranches = data.branchGroups.Where(b => !trunkIDs.Contains(b.uniqueID)).ToArray();

            var rootGroups = new List<TreeEditor.TreeGroupBranch>();
            var branchGroups = new List<TreeEditor.TreeGroupBranch>();

            foreach (var branch in allBranches)
            {
                if (idLookup.ContainsKey(branch.uniqueID))
                {
                    continue;
                }

                if (parentIDs.Contains(branch.parentGroupID))
                {
                    branchGroups.Add(branch);
                    continue;
                }

                var hit = false;
                for (var i = .25f; i < 1f; i++)
                {
                    var dist = branch.distributionCurve.Evaluate(i);

                    if (dist > .01f)
                    {
                        branchGroups.Add(branch);
                        hit = true;
                        break;
                    }
                }

                if (hit)
                {
                    continue;
                }

                var below0 = 0;
                var tested = 0;
                foreach (var nodeID in branch.nodeIDs)
                {
                    var node = data.GetNode(nodeID);
                    var mesh = data.mesh;
                    var vertices = mesh.vertices;

                    for (var i = node.vertStart; i < node.vertEnd; i++)
                    {
                        tested += 1;

                        var vertex = vertices[i];

                        if (vertex.y < 0)
                        {
                            below0 += 1;
                        }
                    }
                }

                var ratio = below0 / (float) tested;

                if (ratio < .1f)
                {
                    branchGroups.Add(branch);
                    continue;
                }

                rootGroups.Add(branch);
            }

            instance.hierarchies.roots = rootGroups.Select(
                                                        rg =>
                                                        {
                                                            var newID = instance.hierarchies.idGenerator
                                                               .GetNextIdAndIncrement();

                                                            idLookup.Add(rg.uniqueID, newID);

                                                            return new RootHierarchyData(
                                                                newID,
                                                                idLookup[rg.parentGroupID],
                                                                rg,
                                                                rg.materialBranch,
                                                                rg.materialBreak,
                                                                rg.materialFrond
                                                            );
                                                        }
                                                    )
                                                   .ToList();

            instance.hierarchies.branches = branchGroups.Select(
                                                             bg =>
                                                             {
                                                                 var newID = instance.hierarchies.idGenerator
                                                                    .GetNextIdAndIncrement();

                                                                 idLookup.Add(bg.uniqueID, newID);

                                                                 return new BranchHierarchyData(
                                                                     newID,
                                                                     idLookup[bg.parentGroupID],
                                                                     bg,
                                                                     bg.materialBranch,
                                                                     bg.materialBreak,
                                                                     bg.materialFrond
                                                                 );
                                                             }
                                                         )
                                                        .ToList();

            instance.hierarchies.leaves = data.leafGroups.Select(
                                                   lg =>
                                                   {
                                                       if (idLookup.ContainsKey(lg.parentGroupID))
                                                       {
                                                           var newID = instance.hierarchies.idGenerator
                                                              .GetNextIdAndIncrement();

                                                           idLookup.Add(lg.uniqueID, newID);
                                                           return new LeafHierarchyData(
                                                               newID,
                                                               idLookup[lg.parentGroupID],
                                                               lg,
                                                               lg.materialLeaf,
                                                               lg.instanceMesh
                                                           );
                                                       }

                                                       return null;
                                                   }
                                               )
                                              .Where(lg => lg != null)
                                              .ToList();

            return instance;
        }

        private void DistributionSettingsChanged()
        {
            TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        #region ITree Members

        public ExternalDualSeed Seed
        {
            get => seed;
            set => seed = value;
        }

        public List<BranchHierarchyData> Branches => hierarchies.branches;

        public List<TrunkHierarchyData> Trunks => hierarchies.trunks;

        public List<LeafHierarchyData> Leaves => hierarchies.leaves;

        public List<FruitHierarchyData> Fruits => hierarchies.fruits;

        public List<RootHierarchyData> Roots => hierarchies.roots;

        public List<FungusHierarchyData> Fungi => hierarchies.fungi;

        public List<KnotHierarchyData> Knots => hierarchies.knots;

        public IEnumerator<HierarchyData> GetEnumerator()
        {
            return hierarchies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
