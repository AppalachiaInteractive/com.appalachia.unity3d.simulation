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
        #region Fields and Autoproperties

        [FormerlySerializedAs("name")]
        [TitleGroup("Log Information", Alignment = TitleAlignments.Centered)]
        [PropertyOrder(0)]
        public NameBasis nameBasis;

        [PropertySpace]
        [PropertyOrder(1), InlineProperty, HideLabel]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public ExternalDualSeed seed;

        [HideInInspector] public LogHierarchies hierarchies;

        #endregion

        public ExternalDualSeed Seed
        {
            get => seed;
            set => seed = value;
        }

        public static TreeLog Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("log");
            var instance = LoadOrCreateNew<TreeLog>(folder, assetName);

            instance.nameBasis = nameBasis;
            instance.hierarchies = new LogHierarchies();

            return instance;
        }

        private void DistributionSettingsChanged()
        {
            LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        #region ILog Members

        public List<BranchHierarchyData> Branches => hierarchies.branches;

        public List<TrunkHierarchyData> Trunks => hierarchies.trunks;

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
