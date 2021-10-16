using System;
using System.Collections;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Collections;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public sealed class TreeBranch : TypeBasedSettings<TreeBranch>, IBranch
    {
        [PropertySpace]
        [TitleGroup("Branch Information", Alignment = TitleAlignments.Centered)]
        [LabelText("Branch Name")]
        [PropertyOrder(0), LabelWidth(100), InlineProperty, HideReferenceObjectPicker]
        public NameBasis nameBasis;

        [PropertyOrder(1), InlineProperty, HideLabel]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public ExternalSeed seed = new ExternalSeed(0, 0);

        public ExternalSeed Seed
        {
            get { return seed; }
            set { seed = value; }
        }

        [HideInInspector]
        public BranchHierarchies hierarchies;
      
        public static TreeBranch Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("branch");
            var instance = LoadOrCreateNew(folder, assetName);
            
            instance.nameBasis = nameBasis;
            instance.hierarchies = new BranchHierarchies();
            instance.shapes = new BranchShapes();
            instance.output = new LODGenerationOutput(0);
            
            return instance;
        }
        
        public IEnumerator<HierarchyData> GetEnumerator()
        {
            return hierarchies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<BranchHierarchyData> Branches => hierarchies.branches;

        public List<TrunkHierarchyData> Trunks => hierarchies.trunks;

        public List<LeafHierarchyData> Leaves => hierarchies.leaves;

        public List<FruitHierarchyData> Fruits => hierarchies.fruits;
        
        public BranchShapes shapes;

        public LODGenerationOutput output;
        
        public TreeIcon GetMenuIcon()
        {
            return TreeIcons.branch2;
        }
        private void DistributionSettingsChanged()
        {
            BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }
    }
}