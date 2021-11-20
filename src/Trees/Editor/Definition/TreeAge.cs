using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.AmbientOcclusion;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Interfaces;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Spatial.Octree;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public class TreeAge : TypeBasedSettings<TreeAge>, IMenuItemProvider
    {
        #region Fields and Autoproperties

        public AgeBuildRequests buildRequest;

        public AgeType ageType;

        public bool active;

        public BoundsOctree<AmbientOcclusionSamplePoint> samplePoints;

        public int individualID;

        public IntegrationAsset integrationAsset;

        public TreeStage normalStage;

        private Dictionary<StageType, TreeStage> _variantLookupInternal;
        [SerializeField] private List<TreeStage> _variants;

        #endregion

        public bool HasAnyVariant => _variantLookup.Count > 0;
        public bool HasDeadFelledRottedVariant => _variantLookup.ContainsKey(StageType.DeadFelledRotted);
        public bool HasDeadFelledVariant => _variantLookup.ContainsKey(StageType.DeadFelled);

        public bool HasDeadVariant => _variantLookup.ContainsKey(StageType.Dead);
        public bool HasFelledBareRottedVariant => _variantLookup.ContainsKey(StageType.FelledBareRotted);
        public bool HasFelledBareVariant => _variantLookup.ContainsKey(StageType.FelledBare);
        public bool HasFelledVariant => _variantLookup.ContainsKey(StageType.Felled);
        public bool HasStumpRottedVariant => _variantLookup.ContainsKey(StageType.StumpRotted);
        public bool HasStumpVariant => _variantLookup.ContainsKey(StageType.Stump);

        public bool RequiresCut =>
            HasFelledVariant ||
            HasDeadFelledVariant ||
            HasDeadFelledRottedVariant ||
            HasFelledBareVariant ||
            HasFelledBareRottedVariant;

        public IEnumerable<TreeStage> stages
        {
            get
            {
                yield return normalStage;
                foreach (var variant in _variants)
                {
                    yield return variant;
                }
            }
        }

        public IEnumerable<TreeStage> Variants => _variants;
        public int StageCount => VariantCount + (normalStage == null ? 0 : 1);

        public int VariantCount => _variants.Count;

        private Dictionary<StageType, TreeStage> _variantLookup
        {
            get
            {
                if (_variantLookupInternal == null)
                {
                    _variantLookupInternal = new Dictionary<StageType, TreeStage>();
                }

                if (_variants == null)
                {
                    _variants = new List<TreeStage>();
                }

                if (_variants.Count != _variantLookupInternal.Count)
                {
                    _variantLookupInternal.Clear();

                    foreach (var stage in _variants)
                    {
                        _variantLookupInternal.Add(stage.stageType, stage);
                    }
                }

                return _variantLookupInternal;
            }
        }

        public TreeStage this[StageType type] =>
            type == StageType.Normal ? normalStage : _variantLookup[type];

        public static TreeAge Create(
            string folder,
            NameBasis nameBasis,
            int individualID,
            AgeType ageType,
            TreeAsset asset)
        {
            var assetName = nameBasis.FileNameAgeSO(individualID, ageType);
            var instance = LoadOrCreateNew<TreeAge>(folder, assetName);

            instance.individualID = individualID;
            instance.ageType = ageType;

            instance.normalStage = TreeStage.Create(
                folder,
                nameBasis,
                individualID,
                ageType,
                StageType.Normal,
                asset
            );

            instance._variants = new List<TreeStage>();
            instance._variantLookupInternal = new Dictionary<StageType, TreeStage>();
            instance.buildRequest = new AgeBuildRequests();

            return instance;
        }

        public static string GetMenuString(AgeType age)
        {
            return age.ToString();
        }

        public TreeStage AddVariant(string folder, NameBasis nameBasis, StageType stage, TreeAsset asset)
        {
            var variant = TreeStage.Create(folder, nameBasis, individualID, ageType, stage, asset);

            _variantLookupInternal.Add(stage, variant);
            _variants.Add(variant);
            _variants.Sort((a, b) => a.stageType.CompareTo(b.stageType));
            return variant;
        }

        public bool CheckAllStageSettings(Predicate<StageBuildRequests> check)
        {
            foreach (var setting in stages)
            {
                if (!check(setting.buildRequest))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckAnyStageSetting(Predicate<StageBuildRequests> check)
        {
            foreach (var setting in stages)
            {
                if (check(setting.buildRequest))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            return buildRequest.GetBuildCosts(level);
        }

        public OdinMenuItem GetMenuItem(OdinMenuTree tree)
        {
            var item = new OdinMenuItem(tree, GetMenuString(), this) {Icon = GetIcon(true).icon};

            return item;
        }

        public string GetMenuString()
        {
            return GetMenuString(ageType);
        }

        public BuildRequestLevel GetRequestLevel(BuildState buildState)
        {
            var rl = buildRequest.requestLevel;

            if (rl == BuildRequestLevel.InitialPass)
            {
                return rl;
            }

            foreach (var stage in stages)
            {
                if (stage.buildRequest == null)
                {
                    stage.buildRequest = new StageBuildRequests();
                }

                if (stage.active || (buildState > BuildState.Full))
                {
                    rl = rl.Max(stage.GetRequestLevel(buildState));

                    if (rl == BuildRequestLevel.InitialPass)
                    {
                        return rl;
                    }
                }
            }

            return rl;
        }

        public bool HasType(StageType type)
        {
            if (type == StageType.Normal)
            {
                return true;
            }

            return _variantLookup.ContainsKey(type);
        }

        public void PushStageGenerationSetting(Action<StageBuildRequests> action)
        {
            foreach (var setting in stages)
            {
                action(setting.buildRequest);
            }
        }

        public void RemoveNormal()
        {
            normalStage = null;
        }

        public void RemoveVariant(StageType stage)
        {
            var variant = _variantLookup[stage];
            _variants.Remove(variant);
            _variantLookupInternal.Remove(stage);
        }

        public bool ShouldRebuildDistribution(BuildRequestLevel level)
        {
            return buildRequest.distribution == level;
        }

        public bool ShouldRebuildGeometry(BuildRequestLevel level)
        {
            var buildGeometry = stages.Any(s => s.ShouldRebuildGeometry(level));

            return buildGeometry;
        }

        #region IMenuItemProvider Members

        public TreeIcon GetIcon(bool enabled)
        {
            switch (ageType)
            {
                case AgeType.Mature:
                    return enabled ? TreeIcons.age50 : TreeIcons.disabledAge50;
                case AgeType.Sapling:
                    return enabled ? TreeIcons.age10 : TreeIcons.disabledAge10;
                case AgeType.Young:
                    return enabled ? TreeIcons.age20 : TreeIcons.disabledAge20;
                case AgeType.Adult:
                    return enabled ? TreeIcons.age35 : TreeIcons.disabledAge35;
                case AgeType.Spirit:
                    return enabled ? TreeIcons.spirit : TreeIcons.disabledSpirit;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
