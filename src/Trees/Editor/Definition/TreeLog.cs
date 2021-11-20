using System;
using System.Collections;
using System.Collections.Generic;
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
    public sealed class TreeLog : TypeBasedSettings<TreeLog>, ILog
    {
        [FormerlySerializedAs("name")]
        [TitleGroup("Log Information", Alignment = TitleAlignments.Centered)]
        [PropertyOrder(0)]
        public NameBasis nameBasis;

        public ExternalDualSeed Seed
        {
            get
            {
                return seed;
            }
            set
            {
                seed = value;
            }
        }

        [PropertySpace]
        [PropertyOrder(1), InlineProperty, HideLabel]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public ExternalDualSeed seed;

        [HideInInspector]
        public LogHierarchies hierarchies;

        public static TreeLog Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("log");
            var instance = LoadOrCreateNew<TreeLog>(folder, assetName);
            
            instance.nameBasis = nameBasis;
            instance.hierarchies = new LogHierarchies();
            
            return instance;
        }

        public List<BranchHierarchyData> Branches => hierarchies.branches;

        public List<TrunkHierarchyData> Trunks => hierarchies.trunks;
        
        private void DistributionSettingsChanged()
        {
            LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        public IEnumerator<HierarchyData> GetEnumerator()
        {
            return hierarchies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}



