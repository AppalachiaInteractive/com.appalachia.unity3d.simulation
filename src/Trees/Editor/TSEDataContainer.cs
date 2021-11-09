using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core.Serialization;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Utilities;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees
{
    public abstract class TSEDataContainer : ResponsiveAppalachiaObject<TSEDataContainer>
    {
        [HideInInspector] 
        public bool initialized;

        public bool canInitialize =>
            !initialized &&
            (initializationSettings != null) &&
            !string.IsNullOrWhiteSpace(initializationSettings.name) &&
            (initializationSettings.name.Length > 2) &&
            (!initializationSettings.convertTreeData || (initializationSettings.original != null));
        
        [HideInInspector] public DataState dataState;
        
        [HideInInspector] public bool graphDirty;


        [PropertyOrder(-100)]
        [HideIf(nameof(initialized))]
        [InfoBox("Initialize it!")]
        [HideLabel]
        [Title("Initialization", "Initialize the tree or branch before proceeding.")]
        public InitializationSettings initializationSettings;

        [FormerlySerializedAs("individualIDs")] [HideInInspector] 
        public IDIncrementer primaryIDs = new IDIncrementer(false);

        [HideInInspector]
        public TreePrefabCollection hierarchyPrefabs;

        [HideInInspector] public TreeAssetSubfolders subfolders;
        
        [HideInInspector] public BuildState buildState = BuildState.Default;

        public enum DataState
        {
            Normal = 0,
            Dirty = 10,
            PendingSave = 20
        }

        private BuildProgressTracker _progressTracker;

        public BuildProgressTracker progressTracker
        {
            get
            {
                if (_progressTracker == null)
                {
                    _progressTracker = new BuildProgressTracker();
                }

                return _progressTracker;
            }
        }

        public abstract void SetDirtyStates();

        public void Save(bool generateImpostors = false)
        {
            using (var progress = new ProgressDisplay(3))
            {
                try
                {
                    progress.Do(SetDirtyStates, "Setting states...");
                    progress.Do(AssetDatabaseManager.SaveAssets, "Saving assets...");
                    progress.Do(() => SaveAllAssets(generateImpostors), "Persisting changes...");
                    dataState = DataState.Normal;
                }
                catch (Exception ex)
                {
                    AppaLog.Error(ex);
                    dataState = DataState.PendingSave;
                }
            }
        }
        
        protected abstract void SaveAllAssets(bool saveImpostors);
        
        public abstract ResponsiveSettingsType settingsType { get; }

        private const string _PRF_PFX = nameof(TSEDataContainer) + ".";
        private static readonly ProfilerMarker _PRF_OnEnable = new ProfilerMarker(_PRF_PFX + nameof(OnEnable));
        private void OnEnable()
        {
            using (_PRF_OnEnable.Auto())
            {
                UpdateSettingsType(settingsType);
            }
        }

        private static readonly ProfilerMarker _PRF_UpdateSettingsType = new ProfilerMarker(_PRF_PFX + nameof(UpdateSettingsType));
        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
            using (_PRF_UpdateSettingsType.Auto())
            {
                this.HandleResponsiveUpdate(t);
            }
        }

        public abstract void RebuildStructures();

        public abstract NameBasis GetNameBasis();

        public override string ToString()
        {
            var basis = GetNameBasis();

            if (basis == null)
            {
                return name;
            }
            
            return GetNameBasis().friendlyName;
        }

        public abstract void BuildFull();
        public abstract void BuildForceFull();
        public abstract void BuildDefault();

        public abstract void SettingsChanged(SettingsUpdateTarget target);

        public abstract void CopySettingsFrom(TSEDataContainer tsd);
        
        public abstract void CopyHierarchiesFrom(TSEDataContainer tsd);
    }
}
