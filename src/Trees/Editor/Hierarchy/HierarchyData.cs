using System;
using System.Diagnostics;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Curves;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Interfaces;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public abstract class HierarchyData : IIconProvider, IEquatable<HierarchyData>, IResponsive
    {
        [DebuggerStepThrough] public bool Equals(HierarchyData other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (hierarchyID == other.hierarchyID) && (parentHierarchyID == other.parentHierarchyID);
        }
        
        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((HierarchyData) obj);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            unchecked
            {
                return (hierarchyID * 397) ^ parentHierarchyID;
            }
        }

        public void UpdateSettingsType(ResponsiveSettingsType t)
        {
            this.HandleResponsiveUpdate(t);
        }

        [DebuggerStepThrough] public static bool operator ==(HierarchyData left, HierarchyData right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator !=(HierarchyData left, HierarchyData right)
        {
            return !Equals(left, right);
        }

        [SerializeField, HideInInspector] private string _externalHash;

        private bool showAge => settingsType != ResponsiveSettingsType.Log;
        
        [PropertyOrder(-900)]
        //[FoldoutGroup("General")]
        //[BoxGroup("General", true)]
        [TreeProperty]
        [ShowIf(nameof(showAge))]
        [OnValueChanged(nameof(HierarchyDataChanged))]
        [PropertySpace]
        public TreeAgeTypeFlags ageEligibility;


        [TreeHeader, PropertyOrder(-1)]
        //[FoldoutGroup("Distribution", false)]
        [TabGroup("Distribution", Paddingless = true)]
        public DistributionSettings distribution;
        
        [HideInInspector] public int hierarchyID;
        [HideInInspector] public bool hidden;
        [HideInInspector] public int parentHierarchyID;
        [HideInInspector] public int hierarchyDepth;

        [PropertyOrder(-1000), TreeHeader]
        //[FoldoutGroup("General", false)]
        //[BoxGroup("General", true)]
        [OnValueChanged(nameof(HierarchyDataChanged), true)]
        public ExternalSeed seed;

        public bool CanChangeParent => IsRoot || IsBranch || IsLeaf || IsFruit || IsKnot || IsFungus;

        public bool IsBranch => type == TreeComponentType.Branch;

        public bool IsFruit => type == TreeComponentType.Fruit;
        
        public bool IsFungus => type == TreeComponentType.Fungus;

        public bool IsKnot => type == TreeComponentType.Knot;

        public bool IsLeaf => type == TreeComponentType.Leaf;

        public bool IsRoot => type == TreeComponentType.Root;

        public bool IsTrunk => type == TreeComponentType.Trunk;


        public virtual bool IsFrond => false;

        public bool HasSpline => IsTrunk || IsRoot || IsBranch;

        public bool SupportsBranchChild => IsTrunk || IsBranch;

        public bool SupportsChildren =>
            SupportsRootChild || SupportsBranchChild || SupportsLeafChild || SupportsFruitChild ||
            SupportsKnotChild;

        public bool SupportsFruitChild => (IsTrunk || IsBranch) && ((settingsType == ResponsiveSettingsType.Tree) || (settingsType == ResponsiveSettingsType.Branch));
        
        public bool SupportsFungusChild => (IsTrunk || IsRoot) && (settingsType == ResponsiveSettingsType.Tree);

        public bool SupportsKnotChild => (IsTrunk || IsRoot) && (settingsType == ResponsiveSettingsType.Tree);

        public bool SupportsLeafChild => (IsTrunk || IsBranch) && ((settingsType == ResponsiveSettingsType.Tree) || (settingsType == ResponsiveSettingsType.Branch)) ;

        public bool SupportsRootChild => (IsTrunk && (settingsType == ResponsiveSettingsType.Tree));

        public abstract TreeComponentType type { get; }

        public bool SupportsChildType(TreeComponentType t)
        {
            switch (t)
            {
                case TreeComponentType.Root:
                    return SupportsRootChild;
                case TreeComponentType.Trunk:
                    return false;
                case TreeComponentType.Branch:
                    return SupportsBranchChild;
                case TreeComponentType.Leaf:
                    return SupportsLeafChild;
                case TreeComponentType.Fruit:
                    return SupportsFruitChild;
                case TreeComponentType.Knot:
                    return SupportsKnotChild;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
        
        protected abstract Object[] GetExternalObjects();

        public bool HasExternalChanges()
        {
            var objects = GetExternalObjects();

            if ((objects == null) || (objects.Length == 0))
            {
                return false;
            }

            var hash = UnityEditorInternal.InternalEditorUtility
                .CalculateHashForObjectsAndDependencies(objects);

            if (hash != _externalHash)
            {
                _externalHash = hash;
                return true;
            }

            return false;
        }

        public bool IsEligibleForAge(AgeType age)
        {
            if (age == AgeType.None) return true;

            if (ageEligibility < 0) ageEligibility = TreeAgeTypeFlags.All;
            if (ageEligibility > TreeAgeTypeFlags.All) ageEligibility = TreeAgeTypeFlags.All;
            
            if (ageEligibility == TreeAgeTypeFlags.All) return true;

            var check = TreeAgeTypeFlags.None;

            switch (age)
            { 
                case AgeType.Sapling:
                    check = TreeAgeTypeFlags.Sapling;
                    break;
                case AgeType.Young:
                    check = TreeAgeTypeFlags.Young;
                    break;
                case AgeType.Adult:
                    check = TreeAgeTypeFlags.Adult;
                    break;
                case AgeType.Mature:
                    check = TreeAgeTypeFlags.Mature;
                    break;
                case AgeType.Spirit:
                    check = TreeAgeTypeFlags.Spirit;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(age), age, null);
            }

            var result = ageEligibility.HasFlag(check);

            return result;
        }

        protected HierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type)
        {
            this.hierarchyID = hierarchyID;
            this.parentHierarchyID = parentHierarchyID;
            distribution = new DistributionSettings(type);
            hidden = false;
            ageEligibility = TreeAgeTypeFlags.All;
        }

        protected HierarchyData(int hierarchyID, int parentHierarchyID, TreeEditor.TreeGroup group)
        {
            this.hierarchyID = hierarchyID;
            this.parentHierarchyID = parentHierarchyID;
            seed = new ExternalSeed(0, Mathf.Clamp(group.seed, 0, BaseSeed.HIGH_ELEMENT));

            distribution = new DistributionSettings(ResponsiveSettingsType.Tree);
            distribution.distributionFrequency.SetValue(group.distributionFrequency);
            distribution.distributionCurve.SetValue(group.distributionCurve);

            distribution.clusterCount.SetValue(group.distributionMode.ToClusterCount(group.distributionNodes));
            distribution.radial.SetValue(group.distributionMode.ToRadial());
            distribution.vertical.SetValue(group.distributionMode.ToVertical());
            distribution.radialStepOffset.SetValue(group.distributionMode.ToStepOffset());
            distribution.radialStepJitter.SetValue(0.1f);
            distribution.distributionAngleOffsetJitter.SetValue(0.1f);
            distribution.distributionSpin.SetValue(new floatCurve(group.distributionTwirl));
            distribution.growthAngle.SetValue(new floatCurve(group.distributionPitch,
                group.distributionPitchCurve));
            distribution.distributionScale.SetValue(new floatCurve(group.distributionScale,
                group.distributionScaleCurve));

            hidden = !group.visible;
            ageEligibility = TreeAgeTypeFlags.All;
        }

        public TreeIcon GetIcon(bool enabled)
        {
            switch (type)
            {
                case TreeComponentType.Root:
                    return enabled ? TreeIcons.root : TreeIcons.disabledRoot;
                
                case TreeComponentType.Trunk:
                    return enabled ? TreeIcons.trunk : TreeIcons.disabledTrunk;
                
                case TreeComponentType.Branch:
                    var b = this as BranchHierarchyData;

                    switch (b.geometry.geometryMode)
                    {
                        case BranchGeometryMode.Branch:
                            return enabled ? TreeIcons.branch : TreeIcons.disabledBranch;
                        
                        case BranchGeometryMode.BranchFrond:
                            return enabled ? TreeIcons.branchfrond : TreeIcons.disabledBranchfrond;
                        
                        case BranchGeometryMode.Frond:
                            return enabled ? TreeIcons.frond : TreeIcons.disabledFrond;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                case TreeComponentType.Leaf:
                    return enabled ? TreeIcons.leaf : TreeIcons.disabledLeaf;
                
                case TreeComponentType.Fruit:
                    return enabled ? TreeIcons.fruit : TreeIcons.disabledFruit;
                
                case TreeComponentType.Knot:
                    return enabled ? TreeIcons.knot : TreeIcons.disabledKnot;
                
                case TreeComponentType.Fungus:
                    return enabled ? TreeIcons.fungus : TreeIcons.disabledFungus;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HierarchyDataChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
            }
            else if (settingsType == ResponsiveSettingsType.Branch)
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
            }
            else
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
            }

            if (ageEligibility > TreeAgeTypeFlags.All)
            {
                ageEligibility = TreeAgeTypeFlags.All;
            }
            else if (ageEligibility < 0)
            {
                ageEligibility = TreeAgeTypeFlags.All;
            }
        }

        public void CopyGenerationSettings(HierarchyData model)
        {
            distribution = model.distribution.Clone();
            hidden = model.hidden;
            CopyInternalGenerationSettings(model);
        }

        protected abstract void CopyInternalGenerationSettings(HierarchyData model);

        public ResponsiveSettingsType settingsType => distribution.settingsType;

        /*
        public abstract void ToggleCheckboxes(bool enabled);*/

        public abstract string GetSortKey();
    }
}