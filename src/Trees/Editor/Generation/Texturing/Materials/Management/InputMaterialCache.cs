#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management
{
    [Serializable]
    public class InputMaterialCache
    {
        
        
        private Dictionary<Material, int> _atlasInputIDsByMaterial;

        private Dictionary<Material, int> _atlasMaterialIndexLookup;

        private Dictionary<Material, int> _tiledInputIDsByMaterial;

        private Dictionary<Material, int> _tiledMaterialIndexLookup;

        private Dictionary<int, InputMaterial> _materialsByID;
        
        [SerializeField]
        [TabGroup("Atlas", Paddingless = true)]
        [InlineProperty, PropertyOrder(-800)]
        [ListDrawerSettings(
            Expanded = true,
            DraggableItems = false,
            IsReadOnly = true,
            HideAddButton = true,
            HideRemoveButton = true,
            NumberOfItemsPerPage = 1,
            ShowPaging = true
        )]
        [ShowIf(nameof(_showAtlasInputMaterials))]
        private List<AtlasInputMaterial> _atlasInputMaterials;

        [SerializeField]
        [TabGroup("Tiled", Paddingless = true)]
        [InlineProperty, PropertyOrder(-700)]
        [ListDrawerSettings(
            Expanded = true,
            DraggableItems = false,
            IsReadOnly = true,
            HideAddButton = true,
            HideRemoveButton = true,
            NumberOfItemsPerPage = 1,
            ShowPaging = true
        )]
        [ShowIf(nameof(_showTiledInputMaterials))]
        private List<TiledInputMaterial> _tiledInputMaterials;

        [TabGroup("Default Materials", Paddingless = true)]
        [InlineProperty, HideLabel]
        public DefaultMaterialSettings defaultMaterials;

        public InputMaterialCache()
        {
            _atlasInputIDsByMaterial = new Dictionary<Material, int>();
            _atlasMaterialIndexLookup = new Dictionary<Material, int>();
            _tiledInputIDsByMaterial = new Dictionary<Material, int>();
            _tiledMaterialIndexLookup = new Dictionary<Material, int>();
            _atlasInputMaterials = new List<AtlasInputMaterial>();
            _tiledInputMaterials = new List<TiledInputMaterial>();
            defaultMaterials = new DefaultMaterialSettings();
            _materialsByID = new Dictionary<int, InputMaterial>();
        }

        private bool _showAtlasInputMaterials => _atlasInputMaterials.Count > 0;
        private bool _showTiledInputMaterials => _tiledInputMaterials.Count > 0;

        public Dictionary<Material, int> tiledInputIDsByMaterial
        {
            get
            {
                RebuildCache();
                return _tiledInputIDsByMaterial;
            }
        }

        public Dictionary<Material, int> tiledMaterialIndexLookup
        {
            get
            {
                RebuildCache();
                return _tiledMaterialIndexLookup;
            }
        }

        public Dictionary<Material, int> atlasInputIDsByMaterial
        {
            get
            {
                RebuildCache();
                return _atlasInputIDsByMaterial;
            }
        }

        public Dictionary<Material, int> atlasMaterialIndexLookup
        {
            get
            {
                RebuildCache();
                return _atlasMaterialIndexLookup;
            }
        }

        public Dictionary<int, InputMaterial> materialsByID
        {
            get
            {
                RebuildCache();
                return _materialsByID;
            }
        }

        public IReadOnlyList<AtlasInputMaterial> atlasInputMaterials => _atlasInputMaterials;

        public IReadOnlyList<TiledInputMaterial> tiledInputMaterials => _tiledInputMaterials;

        public void RebuildCache()
        {
            using (BUILD_TIME.TREE_MAT_CACHE.RebuildCache.Auto())
            {
                if (_atlasInputIDsByMaterial == null)
                {
                    _atlasInputIDsByMaterial = new Dictionary<Material, int>();
                }

                if (_atlasInputIDsByMaterial.Count != _atlasInputMaterials.Count)
                {
                    _atlasInputIDsByMaterial.Clear();

                    for (var i = 0; i < _atlasInputMaterials.Count; i++)
                    {
                        var original = _atlasInputMaterials[i];

                        if (_atlasInputIDsByMaterial.ContainsKey(original.material))
                        {
                            _atlasInputMaterials.RemoveAt(i);
                            continue;
                        }
                        
                        _atlasInputIDsByMaterial.Add(original.material, original.materialID);
                    }
                }

                if (_tiledInputIDsByMaterial == null)
                {
                    _tiledInputIDsByMaterial = new Dictionary<Material, int>();
                }

                if (_tiledInputIDsByMaterial.Count != _tiledInputMaterials.Count)
                {
                    _tiledInputIDsByMaterial.Clear();

                    for (var i = _tiledInputMaterials.Count - 1; i >= 0; i--)
                    {
                        var original = _tiledInputMaterials[i];

                        if (_tiledInputIDsByMaterial.ContainsKey(original.material))
                        {
                            _tiledInputMaterials.RemoveAt(i);
                            continue;
                        }
                        
                        _tiledInputIDsByMaterial.Add(original.material, original.materialID);
                    }
                }

                if (_atlasMaterialIndexLookup == null)
                {
                    _atlasMaterialIndexLookup = new Dictionary<Material, int>();
                }

                if (_atlasMaterialIndexLookup.Count != _atlasInputMaterials.Count)
                {
                    _atlasMaterialIndexLookup.Clear();
                    for (var i = _atlasInputMaterials.Count - 1; i >= 0; i--)
                    {
                        var original = _atlasInputMaterials[i];

                        _atlasMaterialIndexLookup.Add(original.material, i);
                    }
                }

                if (_tiledMaterialIndexLookup == null)
                {
                    _tiledMaterialIndexLookup = new Dictionary<Material, int>();
                }

                if (_tiledMaterialIndexLookup.Count != _tiledInputMaterials.Count)
                {
                    _tiledMaterialIndexLookup.Clear();
                    for (var i = 0; i < _tiledInputMaterials.Count; i++)
                    {
                        var original = _tiledInputMaterials[i];
                        _tiledMaterialIndexLookup.Add(original.material, i);
                    }
                }

                if (_materialsByID == null)
                {
                    _materialsByID = new Dictionary<int, InputMaterial>();
                }

                if (_materialsByID.Count != (tiledInputMaterials.Count + atlasInputMaterials.Count))
                {
                    _materialsByID.Clear();

                    for (var i = 0; i < atlasInputMaterials.Count; i++)
                    {
                        var original = atlasInputMaterials[i];
                        _materialsByID.Add(original.materialID, original);
                    }

                    for (var i = 0; i < tiledInputMaterials.Count; i++)
                    {
                        var original = tiledInputMaterials[i];
                        _materialsByID.Add(original.materialID, original);
                    }
                }
            }
        }

        public void RemoveUnneededAtlasMaterial(int i)
        {
            _atlasInputMaterials.RemoveAt(i);
            RebuildCache();
        }

        public void RemoveUnneededTiledMaterial(int i)
        {
            _tiledInputMaterials.RemoveAt(i);
            RebuildCache();
        }

        public void AddAtlasMaterial(AtlasInputMaterial material)
        {
            _atlasInputMaterials.Add(material);
            RebuildCache();
        }

        public void AddTiledMaterial(TiledInputMaterial material)
        {
            _tiledInputMaterials.Add(material);
            RebuildCache();
        }

        [Button]
        public void Reset()
        {
            ClearCache();

            _atlasInputMaterials.Clear();
            _tiledInputMaterials.Clear();
        }

        public void ClearCache()
        {
            _materialsByID.Clear();
            _atlasMaterialIndexLookup.Clear();
            _tiledMaterialIndexLookup.Clear();
            _atlasInputIDsByMaterial.Clear();
            _tiledInputIDsByMaterial.Clear();
        }

        public void Update(ITree species, TreePrefabCollection prefabs, IDIncrementer ids)
        {
            using (BUILD_TIME.TREE_MAT_CACHE.Update.Auto())
            {
                try
                {
                    ClearCache();
                    RebuildCache();

                    UpdateDefaultMaterials(species);
                    AddTrunkTileMaterials(species, ids, ResponsiveSettingsType.Tree);
                    AddRootTileMaterials(species, ids, ResponsiveSettingsType.Tree);
                    AddBranchTileMaterials(species, ids, ResponsiveSettingsType.Tree);
                    AddTrunkAtlasMaterials(species, ids, ResponsiveSettingsType.Tree);
                    AddRootAtlasMaterials(species, ids, ResponsiveSettingsType.Tree);
                    AddBranchAtlasMaterials(species, ids, ResponsiveSettingsType.Tree);
                    AddLeafMaterials(species, prefabs, ids, ResponsiveSettingsType.Tree);
                    AddFruitMaterials(species, prefabs, ids, ResponsiveSettingsType.Tree);
                    AddKnotMaterials(species, prefabs, ids, ResponsiveSettingsType.Tree);
                    AddFungiMaterials(species, prefabs, ids, ResponsiveSettingsType.Tree);

                    RemoveUnnecessaryMaterials(species, prefabs, ResponsiveSettingsType.Tree);

                    foreach (var atlasInputMaterial in _atlasInputMaterials)
                    {
                        atlasInputMaterial.UpdateTextures();
                    }

                    foreach (var tiledInputMaterial in _tiledInputMaterials)
                    {
                        tiledInputMaterial.UpdateTextures();
                    }
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ex);
                    throw;
                }
            }
        }

        public void Update(IBranch branch, TreePrefabCollection prefabs, IDIncrementer ids)
        {
            using (BUILD_TIME.TREE_MAT_CACHE.Update.Auto())
            {
                try
                {
                    ClearCache();
                    RebuildCache();

                    UpdateDefaultMaterials(branch);
                    AddTrunkTileMaterials(branch, ids, ResponsiveSettingsType.Branch);
                    AddBranchTileMaterials(branch, ids, ResponsiveSettingsType.Branch);
                    AddTrunkAtlasMaterials(branch, ids, ResponsiveSettingsType.Branch);
                    AddBranchAtlasMaterials(branch, ids, ResponsiveSettingsType.Branch);
                    AddLeafMaterials(branch, prefabs, ids, ResponsiveSettingsType.Branch);
                    AddFruitMaterials(branch, prefabs, ids, ResponsiveSettingsType.Branch);

                    RemoveUnnecessaryMaterials(branch, prefabs, ResponsiveSettingsType.Branch);

                    foreach (var atlasInputMaterial in _atlasInputMaterials)
                    {
                        atlasInputMaterial.UpdateTextures();
                    }

                    foreach (var tiledInputMaterial in _tiledInputMaterials)
                    {
                        tiledInputMaterial.UpdateTextures();
                    }
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ex);
                    throw;
                }
            }
        }

        private void UpdateDefaultMaterials(IEnumerable<HierarchyData> hierarchies)
        {
            foreach (var hierarchy in hierarchies)
            {
                var barkHierarchy = hierarchy as BarkHierarchyData;

                if (barkHierarchy != null)
                {
                    if ((barkHierarchy.geometry.barkMaterial == null) &&
                        (defaultMaterials != null) &&
                        (defaultMaterials.materialBark != null))
                    {
                        barkHierarchy.geometry.barkMaterial = defaultMaterials.materialBark;
                    }

                    /*if ((barkHierarchy.limb.breakMaterial == null) &&
                        (defaultMaterials != null) &&
                        (defaultMaterials.materialBreak != null))
                    {
                        barkHierarchy.limb.breakMaterial = defaultMaterials.materialBreak;
                    }*/

                    if ((barkHierarchy.geometry.frond != null) &&
                        (barkHierarchy.geometry.frond.frondMaterial == null) &&
                        (defaultMaterials != null) &&
                        (defaultMaterials.materialFrond != null))
                    {
                        barkHierarchy.geometry.frond.frondMaterial = defaultMaterials.materialFrond;
                    }
                }

                var leafHierarchy = hierarchy as LeafHierarchyData;

                if (leafHierarchy != null)
                {
                    if ((leafHierarchy.geometry.leafMaterial == null) &&
                        (defaultMaterials != null) &&
                        (defaultMaterials.materialLeaf != null))
                    {
                        leafHierarchy.geometry.leafMaterial = defaultMaterials.materialLeaf;
                    }
                }
            }
        }

        private void AddTrunkTileMaterials(ITrunkProvider provider, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Trunks)
            {
                if (hierarchy.geometry.barkMaterial != null)
                {
                    if (!_tiledInputIDsByMaterial.ContainsKey(hierarchy.geometry.barkMaterial))
                    {
                        var newInputMaterial = new TiledInputMaterial(
                            ids.GetNextIdAndIncrement(),
                            hierarchy.geometry.barkMaterial,
                            type
                        );

                        AddTiledMaterial(newInputMaterial);
                    }
                }

                /*if (hierarchy.limb.breakMaterial != null)
                {
                    if (!_tiledInputIDsByMaterial.ContainsKey(hierarchy.limb.breakMaterial))
                    {
                        var newInputMaterial = new TiledInputMaterial(
                            ids.GetNextIdAndIncrement(),
                            hierarchy.limb.breakMaterial,
                            type
                        );

                        newInputMaterial.SetEligibilityAsBreak(true);
                        AddTiledMaterial(newInputMaterial);
                    }
                    else
                    {
                        _tiledInputMaterials[_tiledMaterialIndexLookup[hierarchy.limb.breakMaterial]]
                            .SetEligibilityAsBreak(true);
                    }
                }*/
            }
        }

        private void AddBranchTileMaterials(IBranchProvider provider, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Branches)
            {
                if (hierarchy.geometry.barkMaterial != null)
                {
                    if (!_tiledInputIDsByMaterial.ContainsKey(hierarchy.geometry.barkMaterial))
                    {
                        var newInputMaterial = new TiledInputMaterial(
                            ids.GetNextIdAndIncrement(),
                            hierarchy.geometry.barkMaterial,
                            type
                        );

                        AddTiledMaterial(newInputMaterial);
                    }
                }

                /*if (hierarchy.limb.breakMaterial != null)
                {
                    if (!_tiledInputIDsByMaterial.ContainsKey(hierarchy.limb.breakMaterial))
                    {
                        var newInputMaterial = new TiledInputMaterial(
                            ids.GetNextIdAndIncrement(),
                            hierarchy.limb.breakMaterial,
                            type
                        );

                        newInputMaterial.SetEligibilityAsBreak(true);
                        AddTiledMaterial(newInputMaterial);
                    }
                    else
                    {
                        _tiledInputMaterials[_tiledMaterialIndexLookup[hierarchy.limb.breakMaterial]]
                            .SetEligibilityAsBreak(true);
                    }
                }*/
            }
        }

        private void AddRootTileMaterials(IRootProvider provider, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Roots)
            {
                if (hierarchy.geometry.barkMaterial != null)
                {
                    if (!_tiledInputIDsByMaterial.ContainsKey(hierarchy.geometry.barkMaterial))
                    {
                        var newInputMaterial = new TiledInputMaterial(
                            ids.GetNextIdAndIncrement(),
                            hierarchy.geometry.barkMaterial,
                            type
                        );

                        AddTiledMaterial(newInputMaterial);
                    }
                }

                /*if (hierarchy.limb.breakMaterial != null)
                {
                    if (!_tiledInputIDsByMaterial.ContainsKey(hierarchy.limb.breakMaterial))
                    {
                        var newInputMaterial = new TiledInputMaterial(
                            ids.GetNextIdAndIncrement(),
                            hierarchy.limb.breakMaterial,
                            type
                        );

                        newInputMaterial.SetEligibilityAsBreak(true);
                        AddTiledMaterial(newInputMaterial);
                    }
                    else
                    {
                        _tiledInputMaterials[_tiledMaterialIndexLookup[hierarchy.limb.breakMaterial]]
                            .SetEligibilityAsBreak(true);
                    }
                }*/
            }
        }

        private void AddTrunkAtlasMaterials(ITrunkProvider species, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in species.Trunks)
            {
                AddBarkAtlasMaterial(hierarchy, ids, type);
            }
        }

        private void AddBranchAtlasMaterials(IBranchProvider species, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in species.Branches)
            {
                AddBarkAtlasMaterial(hierarchy, ids, type);
            }
        }

        private void AddRootAtlasMaterials(IRootProvider species, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in species.Roots)
            {
                AddBarkAtlasMaterial(hierarchy, ids, type);
            }
        }

        private void AddBarkAtlasMaterial(BarkHierarchyData hierarchy, IDIncrementer ids, ResponsiveSettingsType type)
        {
            if (hierarchy.geometry.frond.frondMaterial != null)
            {
                if (!_atlasInputIDsByMaterial.ContainsKey(hierarchy.geometry.frond.frondMaterial))
                {
                    var newMaterial = new AtlasInputMaterial(
                        ids.GetNextIdAndIncrement(),
                        hierarchy.geometry.frond.frondMaterial,
                        type
                    );

                    newMaterial.SetEligibilityAsFrond(true);
                    AddAtlasMaterial(newMaterial);
                }
                else
                {
                    _atlasInputMaterials[_atlasMaterialIndexLookup[hierarchy.geometry.frond.frondMaterial]]
                        .SetEligibilityAsFrond(true);
                }
            }
        }

        private void AddLeafMaterials(ILeafProvider provider, TreePrefabCollection prefabs, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Leaves)
            {
                if (hierarchy.geometry.geometryMode == LeafGeometryMode.Mesh)
                {
                    var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Leaf);

                    foreach (var material in prefab.GetMaterials())
                    {
                        if (!_atlasInputIDsByMaterial.ContainsKey(material))
                        {
                            var newMaterial = new AtlasInputMaterial(ids.GetNextIdAndIncrement(), material, type);

                            newMaterial.SetEligibilityAsLeaf(true);
                            AddAtlasMaterial(newMaterial);
                        }
                        else
                        {
                            _atlasInputMaterials[_atlasMaterialIndexLookup[material]].SetEligibilityAsLeaf(true);
                        }
                    }
                }
                else
                {
                    if (hierarchy.geometry.leafMaterial != null)
                    {
                        if (!_atlasInputIDsByMaterial.ContainsKey(hierarchy.geometry.leafMaterial))
                        {
                            var newMaterial = new AtlasInputMaterial(
                                ids.GetNextIdAndIncrement(),
                                hierarchy.geometry.leafMaterial,
                                type
                            );

                            newMaterial.SetEligibilityAsLeaf(true);
                            AddAtlasMaterial(newMaterial);
                        }
                        else
                        {
                            _atlasInputMaterials[_atlasMaterialIndexLookup[hierarchy.geometry.leafMaterial]]
                                .SetEligibilityAsLeaf(true);
                        }
                    }
                }
            }
        }

        private void AddFruitMaterials(IFruitProvider provider, TreePrefabCollection prefabs, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Fruits)
            {
                if (hierarchy.geometry.prefab == null)
                {
                    continue;
                }

                var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Fruit);
                foreach (var material in prefab.GetMaterials())
                {
                    if (!_atlasInputIDsByMaterial.ContainsKey(material))
                    {
                        var newMaterial = new AtlasInputMaterial(ids.GetNextIdAndIncrement(), material, type);

                        AddAtlasMaterial(newMaterial);
                    }
                }
            }
        }

        private void AddKnotMaterials(IKnotProvider provider, TreePrefabCollection prefabs, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Knots)
            {
                if (hierarchy.geometry.prefab == null)
                {
                    continue;
                }

                var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Knot);
                foreach (var material in prefab.GetMaterials())
                {
                    if (!_atlasInputIDsByMaterial.ContainsKey(material))
                    {
                        var newMaterial = new AtlasInputMaterial(ids.GetNextIdAndIncrement(), material, type);

                        AddAtlasMaterial(newMaterial);
                    }
                }
            }
        }

        private void AddFungiMaterials(IFungusProvider provider, TreePrefabCollection prefabs, IDIncrementer ids, ResponsiveSettingsType type)
        {
            foreach (var hierarchy in provider.Fungi)
            {
                if (hierarchy.geometry.prefab == null)
                {
                    continue;
                }

                var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Fungus);
                foreach (var material in prefab.GetMaterials())
                {
                    if (!_atlasInputIDsByMaterial.ContainsKey(material))
                    {
                        var mID = ids.GetNextIdAndIncrement();

                        var newMaterial = new AtlasInputMaterial(mID, material, type);

                        AddAtlasMaterial(newMaterial);
                    }
                }
            }
        }

        private void RemoveUnnecessaryMaterials(IEnumerable<HierarchyData> hierarchies, TreePrefabCollection prefabs, ResponsiveSettingsType type)
        {
            using (BUILD_TIME.TREE_MAT_CACHE.RemoveUnnecessary.Auto())
            {
                for (var i = _atlasInputMaterials.Count - 1; i >= 0; i--)
                {
                    var matched = false;

                    foreach (var hierarchy in hierarchies)
                    {
                        if (hierarchy.IsLeaf)
                        {
                            var h = hierarchy as LeafHierarchyData;
                            if ((h.geometry.geometryMode != LeafGeometryMode.Mesh) &&
                                (h.geometry.leafMaterial == _atlasInputMaterials[i].material))
                            {
                                matched = true;
                                break;
                            }
                        }
                        else if (hierarchy.HasSpline)
                        {
                            var h = hierarchy as BarkHierarchyData;

                            /*if (h.limb.breakMaterial == _atlasInputMaterials[i].material)
                            {
                                matched = true;
                                break;
                            }*/

                            if (hierarchy.IsFrond)
                            {
                                if (h.geometry.frond.frondMaterial == _atlasInputMaterials[i].material)
                                {
                                    matched = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!matched)
                    {
                        if (!prefabs.ContainsMaterial(_atlasInputMaterials[i].material))
                        {
                            RemoveUnneededAtlasMaterial(i);
                        }
                    }
                }

                for (var i = _tiledInputMaterials.Count - 1; i >= 0; i--)
                {
                    var matched = false;

                    foreach (var hierarchy in hierarchies)
                    {
                        if (hierarchy.HasSpline)
                        {
                            var h = hierarchy as BarkHierarchyData;
                            if (h.geometry.barkMaterial == _tiledInputMaterials[i].material)
                            {
                                matched = true;
                                break;
                            }
                        }
                    }

                    if (!matched)
                    {
                        if (!prefabs.ContainsMaterial(_tiledInputMaterials[i].material))
                        {
                            RemoveUnneededTiledMaterial(i);
                        }
                    }
                }
            }
        }

        public InputMaterial GetInputMaterialData(Material material, bool atlas)
        {
            IReadOnlyList<InputMaterial> materials = atlasInputMaterials;
            var lookup = atlasMaterialIndexLookup;

            if (!atlas)
            {
                materials = tiledInputMaterials;
                lookup = tiledMaterialIndexLookup;
            }

            if ((lookup.Count == 0) || (material == null) || !lookup.ContainsKey(material))
            {
                return null;
            }

            var index = lookup[material];

            return materials[index];
        }

        public InputMaterial GetInputMaterialData(Material material, TreeMaterialUsage usage)
        {
            switch (usage)
            {
                case TreeMaterialUsage.Bark:
                case TreeMaterialUsage.SplineBreak:
                    return GetInputMaterialData(material, false);
                case TreeMaterialUsage.Billboard:
                case TreeMaterialUsage.Prefab:
                case TreeMaterialUsage.LeafPlane:
                case TreeMaterialUsage.Frond:
                    return GetInputMaterialData(material, true);
                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
            }
        }

        public int GetMaterialIDByMaterial(Material m, bool atlas)
        {
            return atlas ? atlasInputIDsByMaterial[m] : tiledInputIDsByMaterial[m];
        }

        public void SetDefaultMaterials(DefaultMaterialSettingsPublic settings)
        {
            using (BUILD_TIME.TREE_MAT_COLL.SetDefaultMaterials.Auto())
            {
                if (settings == null)
                {
                    return;
                }

                if (defaultMaterials == null)
                {
                    defaultMaterials = new DefaultMaterialSettings();
                }

                if (settings.breaks != null)
                {
                    var material = GetInputMaterialData(settings.breaks, TreeMaterialUsage.SplineBreak);

                    if (material != null)
                    {
                        defaultMaterials.materialBreak = material.material;
                    }
                }

                if (settings.branches != null)
                {
                    var material = GetInputMaterialData(settings.branches, TreeMaterialUsage.Bark);

                    if (material != null)
                    {
                        defaultMaterials.materialBark = material.material;
                    }
                }

                if (settings.fronds != null)
                {
                    var material = GetInputMaterialData(settings.fronds, TreeMaterialUsage.Frond);

                    if (material != null)
                    {
                        defaultMaterials.materialFrond = material.material;
                    }
                }

                if (settings.leaves != null)
                {
                    var material = GetInputMaterialData(settings.leaves, TreeMaterialUsage.LeafPlane);

                    if (material != null)
                    {
                        defaultMaterials.materialLeaf = material.material;
                    }
                }
            }
        }

        public void UpdateDefaultMaterials()
        {
            using (BUILD_TIME.TREE_MAT_COLL.UpdateDefaultMaterials.Auto())
            {
                if (defaultMaterials == null)
                {
                    defaultMaterials = new DefaultMaterialSettings();
                }

                if (!IsMaterialValidAsDefault(defaultMaterials.materialBark, false))
                {
                    defaultMaterials.materialBark = tiledInputMaterials
                        .FirstOrDefault(m => (m != null) && (m.material != null) && m.eligibleAsBranch)
                        ?.material;
                }

                if (!IsMaterialValidAsDefault(defaultMaterials.materialBreak, false))
                {
                    defaultMaterials.materialBreak = atlasInputMaterials
                        .FirstOrDefault(m => (m != null) && (m.material != null) && m.eligibleAsBreak)
                        ?.material;
                }

                if (!IsMaterialValidAsDefault(defaultMaterials.materialFrond, true))
                {
                    defaultMaterials.materialFrond = atlasInputMaterials
                        .FirstOrDefault(m => (m != null) && (m.material != null) && m.eligibleAsFrond)
                        ?.material;
                }

                if (!IsMaterialValidAsDefault(defaultMaterials.materialLeaf, true))
                {
                    defaultMaterials.materialLeaf = atlasInputMaterials
                        .FirstOrDefault(m => (m != null) && (m.material != null) && m.eligibleAsLeaf)
                        ?.material;
                }
            }
        }

        private bool IsMaterialValidAsDefault(Material m, bool atlas)
        {
            if (m == null)
            {
                return false;
            }

            if (m.primaryTexture() == null)
            {
                return false;
            }

            if (!(atlas ? atlasInputIDsByMaterial : tiledInputIDsByMaterial).ContainsKey(m))
            {
                return false;
            }

            return true;
        }

        public InputMaterial GetByMaterialID(int materialID)
        {
            return materialsByID[materialID];
        }

        
        public string CalculateHash()
        {
            using (BUILD_TIME.TREE_MAT_COLL.CalculateHash.Auto())
            {
                var materials = tiledInputMaterials.Select(tm => tm.material)
                    .Concat(atlasInputMaterials.Select(tm => tm.material));

                var builder = new StringBuilder();

                foreach (var material in materials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    builder.Append(material.name);
                    builder.Append(material.shader.name);
                    builder.Append(material.GetInstanceID());

                    foreach (var prop in material.GetTexturePropertyNames())
                    {
                        var tex = material.GetTexture(prop) as Texture2D;
                        if (tex == null)
                        {
                            continue;
                        }

                        var path = AssetDatabaseManager.GetAssetPath(tex);

                        if (path.StartsWith("Assets"))
                        {
                            path = path.Replace("Assets", Application.dataPath);
                        }

                        if (path.StartsWith("Assets"))
                        {
                            path = path.Replace("Assets", Application.dataPath);
                        }

                        builder.Append(tex.name);
                        builder.Append(tex.GetInstanceID());
                        builder.Append(AppaFile.GetLastWriteTime(path).ToString("s"));
                        builder.Append(AppaFile.ReadAllText(ZString.Format("{0}.meta", path)));
                    }
                }

                return builder.ToString().GetHashCode().ToString();
            }
        }
    }
}
