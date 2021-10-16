using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Core.Metadata.POI;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Interfaces;
using Appalachia.Simulation.Trees.Settings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public sealed class TreeIndividual : TypeBasedSettings<TreeIndividual>, IMenuItemProvider
    {
        [SerializeField] private List<TreeAge> _ages;

        private Dictionary<AgeType, TreeAge> _ageLookupInternal;
        
        private Dictionary<AgeType, TreeAge> _ageLookup
        {
            get
            {
                if (_ageLookupInternal == null)
                {
                    _ageLookupInternal = new Dictionary<AgeType, TreeAge>();

                }
                                    
                if (_ages == null)
                {
                    _ages = new List<TreeAge>();
                }

                if (_ages.Count != _ageLookupInternal.Count)
                {
                    _ageLookupInternal.Clear();

                    foreach (var age in _ages)
                    {
                        _ageLookupInternal.Add(age.ageType, age);
                    }
                }
                
                return _ageLookupInternal;
            }
        }

        public bool active;
        
        public int individualID;
        
        public InternalSeed seed;

        public TreeAge this[AgeType type] => _ageLookup[type];

        public IReadOnlyList<TreeAge> ages => _ages;
        
        public int ageCount => _ages.Count;
        
        public bool hasAdult => _ageLookup.ContainsKey(AgeType.Adult);
        
        public bool hasAny => _ageLookup.Count > 0;
        public bool hasMature => _ageLookup.ContainsKey(AgeType.Mature);
        public bool hasSapling => _ageLookup.ContainsKey(AgeType.Sapling);
        public bool hasSpirit => _ageLookup.ContainsKey(AgeType.Spirit);
        public bool hasYoung => _ageLookup.ContainsKey(AgeType.Young);
   
        public BuildRequestLevel GetRequestLevel(BuildState buildState)
        {
            BuildRequestLevel rl = BuildRequestLevel.None;
            
            foreach (var age in ages)
            {
                if (age.buildRequest == null)
                {
                    age.buildRequest = new AgeBuildRequests();
                }
                
                if (age.active || (buildState > BuildState.Full))
                {
                    rl = rl.Max(age.GetRequestLevel(buildState));
                    
                    if (rl == BuildRequestLevel.InitialPass)
                    {
                        return rl;
                    }                        
                }
            }

            return rl;
        }
        
        public bool HasType(AgeType type) => _ageLookup.ContainsKey(type);

        public TreeAge AddAge(string folder, NameBasis nameBasis, AgeType type, TreeAsset asset)
        {
            var age = TreeAge.Create(folder, nameBasis, individualID, type, asset);

            if (_ageLookupInternal == null)
            {
                _ageLookupInternal = new Dictionary<AgeType, TreeAge>();
            }
            
            _ageLookupInternal.Add(age.ageType, age);
            _ages.Add(age);
            _ages.Sort(new Comparison<TreeAge>((a, b) => a.ageType.CompareTo(b.ageType) ));
            return age;
        }

        public void RemoveAge(AgeType type)
        {
            var age = _ageLookupInternal[type];

            var variants = age.Variants.ToArray();


            for (var i = 0; i < variants.Length; i++)
            {
                age.RemoveVariant(variants[i].stageType);
            }
            
            age.RemoveNormal();
            _ageLookupInternal.Remove(age.ageType);
            _ages.Remove(age);
        }

        public static TreeIndividual Create(string folder, NameBasis nameBasis, int individualID, TreeAsset asset)
        {
            var assetName = nameBasis.FileNameIndividualSO(individualID);
            var instance = LoadOrCreateNew(folder, assetName);
            
            instance.individualID = individualID;

            var mature = TreeAge.Create(folder, nameBasis, instance.individualID, AgeType.Mature, asset);

            instance._ages = new List<TreeAge>();
            
            instance._ageLookupInternal = new Dictionary<AgeType, TreeAge>();
            
            instance._ageLookupInternal.Add(mature.ageType, mature);
            instance._ages.Add(mature);

            return instance;
        }

        public string GetMenuString()
        {
            return GetMenuString(individualID);
        }
        
        public static string GetMenuString(int individualID)
        {
            return $"{individualID:00}";
        }


        /*public OdinMenuItem GetMenuItem(OdinMenuTree tree)
        {
            var item = new OdinMenuItem(tree, GetMenuString(), this) {Icon = GetIcon(true).icon};

            return item;
        }*/

        public TreeIcon GetIcon(bool enabled)
        {
            return enabled ? TreeIcons.newTree : TreeIcons.disabledNewTree;
        }

        public void UpdateMetadata(TreeSpecies species)
        {
            foreach (var age in ages)
            {
                age.individualID = individualID;

                age.normalStage.ageType = age.ageType;
                age.normalStage.individualID = individualID;

                foreach (var stage in age.stages)
                {
                    stage.ageType = age.ageType;
                    stage.individualID = individualID;

                    if (stage.runtimeMetadata == null)
                    {
                        stage.runtimeMetadata =
                            TreeRuntimeInstanceMetadata.LoadOrCreateNew(stage.DirectoryPath, $"runtime_{stage.name}");
                        
                        EditorUtility.SetDirty(stage.runtimeMetadata);
                    }

                    if (stage.runtimeMetadata.age != stage.ageType)
                    {
                        stage.runtimeMetadata.age = stage.ageType;
                        EditorUtility.SetDirty(stage.runtimeMetadata);
                    }

                    if (stage.runtimeMetadata.stage != stage.stageType)
                    {
                        stage.runtimeMetadata.stage = stage.stageType;
                        EditorUtility.SetDirty(stage.runtimeMetadata);
                    }

                    if (Math.Abs(stage.runtimeMetadata.rootDepth - species.hierarchies.verticalOffset) > float.Epsilon)
                    {
                        stage.runtimeMetadata.rootDepth = species.hierarchies.verticalOffset;
                        EditorUtility.SetDirty(stage.runtimeMetadata);
                    }

                    if (stage.runtimeMetadata.speciesName != species.nameBasis.safeName)
                    {
                        stage.runtimeMetadata.speciesName = species.nameBasis.safeName;
                        EditorUtility.SetDirty(stage.runtimeMetadata);
                    }

                    if (stage.runtimeMetadata.individualID != individualID)
                    {
                        stage.runtimeMetadata.individualID = individualID;
                        EditorUtility.SetDirty(stage.runtimeMetadata);
                    }

                    if (stage.runtimeMetadata.pointsOfInterest == null)
                    {
                        stage.runtimeMetadata.pointsOfInterest = new List<RuntimePointOfInterest>();
                        EditorUtility.SetDirty(stage.runtimeMetadata);                 
                    }
                }
            }
        }

        public bool StageRequiresRuntimeUpdate(
            TreeDataContainer tree,
            TreeStage stage)
        {
            var runtime = stage.asset.prefab.GetComponent<TreeRuntimeInstance>();
                    
            if (runtime == null)
            {
                Debug.LogWarning("Prefab update required: Missing TreeRuntimeInstance.");
                return true;
            }

            if (runtime.metadata == null)
            {
                Debug.LogWarning("Prefab update required: Missing tree runtime metadata.");
                return true;
            }

            if (runtime.metadata != stage.runtimeMetadata)
            {
                Debug.LogWarning("Prefab update required: Missing tree runtime metadata.");
                return true;
            }
            
            return false;
        }
        
        public bool RuntimePointsRequireUpdate(
            TreeStage stage,
            GameObject gameObject)
        {
            var points = stage.runtimeMetadata.pointsOfInterest;

            var childCount = gameObject.transform.childCount;

            if (childCount != points.Count)
            {
                Debug.LogWarning("Prefab update required: Wrong point of interest count.");
                return true;
            }
            
            if (gameObject.name != "POINTS")
            {
                Debug.LogWarning("Prefab update required: Wrong POI name.");
                return true;
            }

            return false;
        }
        
        public void ApplyStageRuntime(
            TreeDataContainer tree,
            TreeStage stage,
            GameObject prefab,
            GameObject points)
        {
            var globals = TreeGlobalSettings.instance;
            
            points.layer = TreeGlobalSettings.instance.interactionLayer.layer;
            points.name = "POINTS";
            
            var runtime = prefab.GetComponent<TreeRuntimeInstance>();

            if (runtime == null)
            {
                runtime = prefab.AddComponent<TreeRuntimeInstance>();
            }

            runtime.metadata = stage.runtimeMetadata;

            runtime.enabled = true;

            runtime.metadata.pointsOfInterest.Clear();

            if (tree.runtimeSpeciesMetadata == null)
            {
                tree.CreateRuntimeMetadata();
            }

            runtime.speciesMetadata = tree.runtimeSpeciesMetadata;

            var spheres = points.GetComponents<SphereCollider>();
            for (var i = spheres.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(spheres[i]);
            }

            var requiresCut = stage.CanBeCut && _ageLookup[stage.ageType].RequiresCut;

            if (requiresCut)
            {
                foreach (var trunk in stage.shapes.trunkShapes)
                {
                    var trunkLength = SplineModeler.GetApproximateLength(trunk.spline);

                    var verticalOffset = tree.species.hierarchies.verticalOffset;
                    
                    var cutTime = (verticalOffset + tree.settings.variants.trunkCutHeight) / trunkLength;

                    
                    var trunkRadius = SplineModeler.GetRadiusWithCollarAtTime(
                        tree.species.hierarchies,
                        trunk,
                        tree.species.hierarchies.GetHierarchyData(trunk.hierarchyID) as BarkHierarchyData, 
                        cutTime
                    );

                    var trunkCenter = 
                        trunk.effectiveMatrix.MultiplyPoint(SplineModeler.GetPositionAtTime(trunk.spline, cutTime));

                    var sphereCollider = points.AddComponent<SphereCollider>();
                    sphereCollider.center = trunkCenter;
                    sphereCollider.enabled = true;
                    sphereCollider.isTrigger = true;
                    sphereCollider.sharedMaterial = null;
                    sphereCollider.radius = trunkRadius+globals.trunkCutColliderRadiusAdditive;

                    var poi = new RuntimePointOfInterest()
                    {
                        layer = TreeGlobalSettings.instance.interactionLayer,
                        radius = sphereCollider.radius,
                        position = trunkCenter,
                        sphereCollider = sphereCollider,
                        zoneType = RuntimePointOfInterestType.CutZone
                    };

                    runtime.metadata.pointsOfInterest.Add(poi);
                }
            }
            
        }
    }
}