using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Build;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Interfaces;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public class LogInstance : TypeBasedSettings<LogInstance>, IMenuItemProvider
    {
        [FoldoutGroup("Log Settings")]
        [OnValueChanged(nameof(DistributionSettingsChanged))]
        public bool constrainLength = true;
        
        [FoldoutGroup("Log Settings")]
        [PropertyRange(0.1f, 3f), OnValueChanged(nameof(DistributionSettingsChanged))]
        public float length = 1f;
        
        [FoldoutGroup("Log Settings")]
        [PropertyRange(0.1f, 2f),  OnValueChanged(nameof(DistributionSettingsChanged))]
        public float thickness = 1.0f;
        
        [FoldoutGroup("Log Settings")]
        [PropertyRange(0.0f, 0.1f),  OnValueChanged(nameof(DistributionSettingsChanged))]
        public float colliderInflation;
        
        [FoldoutGroup("Log Properties"), ReadOnly]
        public float effectiveScale;
        
        [FoldoutGroup("Log Properties"), ReadOnly]
        public float actualLength;
        
        [FoldoutGroup("Log Properties"), ReadOnly]
        public float actualDiameter;
        
        [FoldoutGroup("Log Properties"), ReadOnly]
        public Vector3 centerOfMass = Vector3.zero;
        
        [FoldoutGroup("Log Properties"), ReadOnly]
        public Vector3 center = Vector3.zero;
        
        [FoldoutGroup("Log Properties"), ReadOnly]
        public float volume;
        
        [HideInInspector] public LogAsset asset;

        [HideInInspector] public int logID;

        [HideInInspector] public LogShapes shapes;

        [HideInInspector] public bool active;
        
        [HideInInspector] public InternalSeed seed;

        [HideInInspector] public LogInstanceBuildRequest buildRequest;
        
        [HideInInspector] public List<LODGenerationOutput> lods;

        public static LogInstance Create(
            string folder, NameBasis nameBasis, int logID, LogAsset asset)
        {
            var assetName = nameBasis.FileNameLogSO(logID);
            var instance = LoadOrCreateNew(folder, assetName);
            
            instance.asset = asset;
            instance.logID = logID;

            instance.shapes = new LogShapes();
            instance.buildRequest = new LogInstanceBuildRequest();

            return instance;
        }


        public BuildRequestLevel GetRequestLevel(BuildState buildState)
        {
            var rl = buildRequest.requestLevel;

            if (rl == BuildRequestLevel.InitialPass) return rl;

            if (buildRequest == null)
            {
                buildRequest = new LogInstanceBuildRequest();
            }
            
            if (active || (buildState > BuildState.Full))
            {
                rl = rl.Max(buildRequest.requestLevel);
                
                if (rl == BuildRequestLevel.InitialPass)
                {
                    return rl;
                }
            }

            return rl;
        }


        public bool ShouldRebuildDistribution(BuildRequestLevel level)
        {
            return buildRequest.distribution == level;
        }
        
        public bool ShouldRebuildGeometry(BuildRequestLevel level)
        {
            return buildRequest.ShouldBuild(BuildCategory.HighQualityGeometry, level);
        }

        public string GetMenuString()
        {
            return GetMenuString(logID);
        }
        
        public static string GetMenuString(int logID)
        {
            return $"{logID:00}";
        }

        public TreeIcon GetIcon(bool enabled)
        {
            return enabled ? TreeIcons.knot : TreeIcons.disabledKnot; 
        }
        
        /*public OdinMenuItem GetMenuItem(OdinMenuTree tree)
        {
            var item = new OdinMenuItem(tree, GetMenuString(), this) {Icon = GetIcon(true).icon};

            return item;
        }*/
        
        private void RemoveLOD(int i)
        {
            lods.RemoveAt(i);
        }

        public void RecreateMesh(int level)
        {
            asset.levels[level].CreateMesh(asset.GetMeshName(level));
        }

        public void SetMaterials(int level, Material[] materials)
        {
            if (materials.Length == 0)
            {
                asset.levels[level].materials = new[] {DefaultMaterialResource.instance.material};
            }
            else
            {
                asset.levels[level].materials = materials;                
            }
        }

        public void ClearGeometry(LevelOfDetailSettingsCollection lodSettings)
        {
            using (BUILD_TIME.INDV_STG_GEN_CXT.ClearGeometry.Auto())
            {
                if (lods.Count > lodSettings.levels)
                {
                    for (var i = lods.Count - 1; i >= 0; i--)
                    {
                        RemoveLOD(i);
                    }
                }

                foreach (var lod in lods)
                {
                    lod.Clear();
                }
            }
        }

        public void Rebuild()
        {
            shapes.Rebuild();
        }
        
        public void Refresh(LevelOfDetailSettingsCollection lodSettings)
        {
            using (BUILD_TIME.INDV_STG_GEN_CXT.Refresh.Auto())
            {
                shapes.Rebuild();
                asset.Refresh(lodSettings);

                if (lods == null)
                {
                    lods = new List<LODGenerationOutput>();
                }
                
                var current = lods;
                lods = new List<LODGenerationOutput>();

                for (var i = 0; i < lodSettings.levels; i++)
                {
                    var match = current.FirstOrDefault(s => s.lodLevel == i);

                    if (match == null)
                    {
                        match = new LODGenerationOutput(i);
                    }

                    lods.Add(match);
                }
            }
        }
        
        
        private void DistributionSettingsChanged()
        {
            LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }
        
        public IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            return buildRequest.GetBuildCosts(level);
        }

        public bool RequiresUpdate(LogRuntimeInstance ri)
        {
            if (ri.logName != name) return true;
            if (ri.logID != logID) return true;
            //if (ri.center != center) return true;
            //if (ri.volume != volume) return true;
            //if (ri.actualDiameter != actualDiameter) return true;
            //if (ri.actualLength != actualLength) return true;
            //if (ri.effectiveScale != effectiveScale) return true;
            if (ri.centerOfMass != centerOfMass) return true;

            return false;
        }
        
        public void UpdateRuntime(LogRuntimeInstance runtime)
        {
            runtime.logName = name;
            runtime.logID = logID;
            //runtime.center = center;
            //runtime.volume = volume;
            //runtime.actualDiameter = actualDiameter;
            //runtime.actualLength = actualLength;
            //runtime.effectiveScale = effectiveScale;
            //runtime.centerOfMass = centerOfMass;
        }
    }
}