using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Recursion;
using Appalachia.Utility.Constants;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Hierarchy.Collections
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public abstract class HierarchyCollection<T> : IEnumerable<HierarchyData>, ISerializationCallbackReceiver, IHierarchyReadWrite, IResponsive
        where T : HierarchyCollection<T>
    {
        static HierarchyCollection()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<TreePresets>().IsAvailableThen( i => _treePresets = i);
        }

        private static TreePresets _treePresets;
            
        [NonSerialized] public Dictionary<int, HierarchyData> byID;
        [NonSerialized] public Dictionary<int, List<HierarchyData>> byParentID;
        [HideInInspector] public IDIncrementer idGenerator = new IDIncrementer(true);
        
        [HideInInspector] public List<TrunkHierarchyData> trunks;

        [HideInInspector] public int hierarchyDepth;
        [HideInInspector] public int splineHierarchyDepth;
        
        [FormerlySerializedAs("groundOffset")]
        [PropertyTooltip("How deep into the ground to start tree generation.")]
        [PropertyRange(0f, 3f), PropertyOrder(10)]
        [OnValueChanged(nameof(DistributionSettingsChanged)), LabelWidth(100)]
        public float verticalOffset = 1f;
        
        protected HierarchyCollection()
        {
            InitializeSelf();
        }
        
        public abstract int Count { get; }

        public void Clear()
        {
            ClearSelf();
            ClearInternal();
        }

        private void ClearSelf()
        {
            byID.Clear();
            byParentID.Clear();
        }
        
        private void InitializeSelf()
        {
            if (idGenerator == null)
            {
                idGenerator = new IDIncrementer(true);
            }
            
            if (byID == null)
            {
                byID = new Dictionary<int, HierarchyData>();
            }

            if (byParentID == null)
            {
                byParentID = new Dictionary<int, List<HierarchyData>>();
            }

            if (trunks == null)
            {
                trunks = new List<TrunkHierarchyData>();
            }
        }

        protected abstract void ClearInternal();
        
        public void Rebuild()
        {
            InitializeSelf();
            ClearSelf();

            foreach (var hierarchyData in this)
            {
                byID.Add(hierarchyData.hierarchyID, hierarchyData);

                if (hierarchyData.parentHierarchyID < 0) continue;

                if (!byParentID.ContainsKey(hierarchyData.parentHierarchyID))
                {
                    byParentID.Add(hierarchyData.parentHierarchyID,
                        new List<HierarchyData>());
                }

                byParentID[hierarchyData.parentHierarchyID].Add(hierarchyData);
            }
            
            SetHierarchyDepths();

            if (byID.Keys.Count > 0)
            {
                idGenerator.SetNextID(byID.Keys.Max() + 1);
            }
        }

        public void OnBeforeSerialize()
        {
            using var scope = APPASERIALIZE.OnBeforeSerialize();
        }

        public void OnAfterDeserialize()
        {
            using var scope = APPASERIALIZE.OnAfterDeserialize();
            Rebuild();
            
        }

        public abstract IEnumerator<HierarchyData> GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RecurseHierarchies(Action<HierarchyData> action)
        {
            RecurseHierarchies(null, action);
        }

        public void RecurseHierarchiesWithData(Action<HierarchyRecursionData> action)
        {
            RecurseHierarchiesWithData(null, action);
        }

        public bool IsAncestor(HierarchyData ancestor, HierarchyData descendent)
        {
            return IsAncestor(ancestor, descendent, false);
        }

        public void RecurseHierarchiesWithData(
            HierarchyRecursionData current, Action<HierarchyRecursionData> action)
        {
            if (current == null)
            {
                foreach (var trunk in trunks)
                {
                    var newData = new HierarchyRecursionData(trunk, null);
                    RecurseHierarchiesWithData(newData, action);
                }
            }
            else
            {
                action(current);

                if (!byParentID.ContainsKey(current.hierarchy.hierarchyID))
                {
                    return;
                }

                foreach (var childHierarchy in byParentID[current.hierarchy.hierarchyID])
                {
                    var newData = new HierarchyRecursionData(childHierarchy, current.hierarchy);

                    RecurseHierarchiesWithData(newData, action);
                }
            }
        }
        
        public void RecurseHierarchies(HierarchyData hierarchy, Action<HierarchyData> action)
        {
            if (hierarchy == null)
            {
                foreach (var trunk in trunks)
                {
                    RecurseHierarchies(trunk, action);
                }
            }
            else
            {
                action(hierarchy);

                if (!byParentID.ContainsKey(hierarchy.hierarchyID))
                {
                    return;
                }

                foreach (var childHierarchy in byParentID[hierarchy.hierarchyID])
                {
                    RecurseHierarchies(childHierarchy, action);
                }
            }
        }
        
        public void IterateChildrenHierarchies(HierarchyData hierarchy, Action<HierarchyData> action)
        {
            if (hierarchy == null)
            {
                return;
            }

            if (!byParentID.ContainsKey(hierarchy.hierarchyID))
            {
                return;
            }

            foreach (var childHierarchy in byParentID[hierarchy.hierarchyID])
            {
                action(childHierarchy);
            }
        }
        
        public bool IsAncestor(HierarchyData ancestor, HierarchyData descendent, bool isAncestor)
        {
            if (isAncestor)
            {
                return true;
            }

            if (descendent.parentHierarchyID == ancestor.hierarchyID)
            {
                return true;
            }

            IterateChildrenHierarchies(ancestor, child => { isAncestor = IsAncestor(child, descendent, isAncestor); });

            return isAncestor;
        }
        
        public void SetHierarchyDepths()
        {
            hierarchyDepth = 0;
            splineHierarchyDepth = 0;

            foreach (var trunk in trunks)
            {
                trunk.hierarchyDepth = 0;
            }

            RecurseHierarchiesWithData(
                data =>
                {
                    if (data.parentHierarchy != null)
                    {
                        data.hierarchy.hierarchyDepth = data.parentHierarchy.hierarchyDepth + 1;

                        hierarchyDepth = Mathf.Max(
                            hierarchyDepth,
                            data.hierarchy.hierarchyDepth
                        );

                        if (data.hierarchy.HasSpline)
                        {
                            splineHierarchyDepth = Mathf.Max(
                                splineHierarchyDepth,
                                data.hierarchy.hierarchyDepth
                            );
                        }
                    }
                }
            );
        }

        public IEnumerable<HierarchyData> GetHierarchies()
        {
            return this;
        }

        public abstract IEnumerable<HierarchyData> GetHierarchies(TreeComponentType type);
        public int GetHierarchyCount(TreeComponentType type)
        {
            return GetHierarchies(type).Count();
        }

        public IEnumerable<HierarchyData> GetHierarchiesByParent(int parentHierarchyID)
        {
            if (!byParentID.ContainsKey(parentHierarchyID))
            {
                return null;
            }
            
            return byParentID[parentHierarchyID];
        }

        public HierarchyData CreateHierarchy(
            TreeComponentType type,
            HierarchyData parent,
            InputMaterialCache materials,
            bool rebuildState = true)
        {
            var nextID = idGenerator.GetNextIdAndIncrement();
            HierarchyData newHierarchy = AddHierarchy(nextID, parent.hierarchyID, type);

            switch (type)
            {
                case TreeComponentType.Root:
                case TreeComponentType.Trunk:
                case TreeComponentType.Branch:
                    var barkH = newHierarchy as BarkHierarchyData;

                    
                    barkH.geometry.barkMaterial = materials?.defaultMaterials.materialBark;

                    //barkH.limb.breakMaterial = materials?.defaultMaterials.materialBreak;

                    barkH.geometry.frond.frondMaterial = materials?.defaultMaterials.materialFrond;

                    switch (type)
                    {
                        case TreeComponentType.Trunk:
                            barkH.distribution = _treePresets.trunk_distribution.Clone();
                            barkH.geometry = _treePresets.trunk_branch.Clone();
                            barkH.curvature = _treePresets.trunk_curvature.Clone();
                            barkH.limb = _treePresets.trunk_limb.Clone();
                            barkH.geometry.relativeToParentAllowed = false;
                            barkH.geometry.relativeToParent = false;
                            break;
                        
                        case TreeComponentType.Branch:
                            barkH.distribution = _treePresets.branch_distribution.Clone();
                            barkH.geometry = _treePresets.branch_branch.Clone();
                            barkH.curvature = _treePresets.branch_curvature.Clone();
                            barkH.limb = _treePresets.branch_limb.Clone();
                        {
                            var ch = barkH as CollaredBarkHierarchyData;
                            ch.collar = _treePresets.branch_collar.Clone();
                            barkH.geometry.relativeToParentAllowed = true;
                            barkH.geometry.relativeToParent = true;
                        }
                            break;
                        
                        case TreeComponentType.Root:
                            barkH.distribution = _treePresets.root_distribution.Clone();
                            barkH.geometry = _treePresets.root_branch.Clone();
                            barkH.curvature = _treePresets.root_curvature.Clone();
                            barkH.limb = _treePresets.root_limb.Clone();
                            barkH.geometry.relativeToParentAllowed = true;
                            barkH.geometry.relativeToParent = false;

                        {
                            var ch = barkH as CollaredBarkHierarchyData;
                            ch.collar = _treePresets.root_collar.Clone();
                        }
                            break;
                    }


                    break;
                case TreeComponentType.Leaf:
                {
                    {
                        var h = newHierarchy as LeafHierarchyData;

                        h.geometry.leafMaterial = materials?.defaultMaterials.materialLeaf;
                        h.distribution = _treePresets.leaf_distribution.Clone();
                    }
                }
                    break;
                case TreeComponentType.Fruit:
                {
                    var h = newHierarchy as FruitHierarchyData;
                    h.distribution = _treePresets.fruit_distribution.Clone();
                }
                    break;
                case TreeComponentType.Knot:
                {
                    var h = newHierarchy as KnotHierarchyData;
                    h.distribution = _treePresets.knot_distribution.Clone();
                }
                    break;
                case TreeComponentType.Fungus:
                {
                    var h = newHierarchy as FungusHierarchyData;
                    h.distribution = _treePresets.fungus_distribution.Clone();
                }
                    break;
            }

            if (rebuildState)
            {
                Rebuild();
            }

            return newHierarchy;
        }

        public abstract void UpdateHierarchyParent(HierarchyData hierarchy, HierarchyData parent, bool rebuildState = true);
        
        public abstract void DeleteHierarchyChain(int hierarchyID, bool rebuildState = true);

        protected abstract HierarchyData AddHierarchy(int id, int parentID, TreeComponentType type);

        protected abstract void RemoveHierarchies(HierarchyData hierarchy);
        
        public HierarchyData CreateTrunkHierarchy(
            InputMaterialCache materials, 
            bool rebuildState = true)
        {
            var nextID = idGenerator.GetNextIdAndIncrement();
            var newHierarchy = new TrunkHierarchyData(nextID, SettingsType);

            newHierarchy.distribution = _treePresets.trunk_distribution.Clone();
            newHierarchy.geometry = _treePresets.trunk_branch.Clone();
            newHierarchy.curvature = _treePresets.trunk_curvature.Clone();
            newHierarchy.limb = _treePresets.trunk_limb.Clone();
            newHierarchy.geometry.relativeToParentAllowed = false;
            newHierarchy.geometry.relativeToParent = false;
            
            trunks.Add(newHierarchy);
            
            byID.Add(newHierarchy.hierarchyID, newHierarchy);
            byParentID.Add(newHierarchy.hierarchyID, new List<HierarchyData>());

            newHierarchy.geometry.barkMaterial = materials?.defaultMaterials.materialBark;

            //newHierarchy.limb.breakMaterial = materials?.defaultMaterials.materialBreak;

            newHierarchy.geometry.frond.frondMaterial = materials?.defaultMaterials.materialFrond;

            if (rebuildState)
            {
                Rebuild();
            }
            
            return newHierarchy;
        }

        public HierarchyData GetHierarchyData(ShapeData shape)
        {
            if (shape == null)
            {
                throw new NotSupportedException("Missing shape!");
            }

            if (shape.hierarchyID == -1)
            {
                throw new NotSupportedException(
                    ZString.Format("Bad hierarchy for shape {0}.", shape.shapeID)
                );
            }

            return GetHierarchyData(shape.hierarchyID);
        }

        public float GetVerticalOffset()
        {
            return verticalOffset;
        }
        
        public bool DoesHierarchyExist(int hierarchyID)
        {
            return byID.ContainsKey(hierarchyID);
        }
        
        public HierarchyData GetHierarchyData(int hierarchyID)
        {
            if (hierarchyID == -1)
            {
                return null;
            }

            return byID[hierarchyID];
        }

        protected abstract ResponsiveSettingsType SettingsType { get; }
        
        protected abstract void DistributionSettingsChanged();
        
        /*public void ToggleCheckboxes(bool enabled)
        {
            foreach (var hierarchy in this)
            {
                hierarchy.ToggleCheckboxes(enabled);
            }
        }*/

       
        public void UpdateSettingsType(ResponsiveSettingsType t)
        {
            this.HandleResponsiveUpdate(t);
        }
    }
}
