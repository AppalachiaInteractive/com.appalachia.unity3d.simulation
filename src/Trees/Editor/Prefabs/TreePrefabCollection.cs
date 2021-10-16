using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Prefabs
{
    [Serializable]
    public class TreePrefabCollection : TypeBasedSettings<TreePrefabCollection>
    {
        private Dictionary<string, Dictionary<TreeComponentType, TreePrefab>> _prefabsByGUIDAndType;

        private Dictionary<string, Dictionary<TreeComponentType, TreePrefab>> prefabsByGUIDAndType
        {
            get
            {
                RebuildLookups();
                return _prefabsByGUIDAndType;
            }
        }

        private Dictionary<int, TreePrefab> _prefabsByID;

        private Dictionary<int, TreePrefab> prefabsByID
        {
            get
            {
                RebuildLookups();
                return _prefabsByID;
            }
        }

        [SerializeField]
        private IDIncrementer _prefabIDs = new IDIncrementer(true);

        [SerializeField]
        private List<TreePrefab> _prefabs;

        public IReadOnlyList<TreePrefab> prefabs => _prefabs;

        public static TreePrefabCollection Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("prefabs");
            var instance = LoadOrCreateNew(folder, assetName);
            
           instance._prefabs = new List<TreePrefab>();
           instance._prefabsByID = new Dictionary<int, TreePrefab>();
           instance._prefabsByGUIDAndType = new Dictionary<string, Dictionary<TreeComponentType, TreePrefab>>();
           
           return instance;
        }

        public int AddTreePrefab(PrefabSetup prefab, TreeComponentType type)
        {
            if (prefabsByGUIDAndType.ContainsKey(prefab.guid) && _prefabsByGUIDAndType[prefab.guid].ContainsKey(type))
            {
                return _prefabsByGUIDAndType[prefab.guid][type].prefabID;
            }

            var id = _prefabIDs.GetNextIdAndIncrement();
            var newPrefab = new TreePrefab(id, prefab, type);

            if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid))
            {
                _prefabsByGUIDAndType.Add(prefab.guid, new Dictionary<TreeComponentType, TreePrefab>());
            }

            _prefabsByID.Add(id, newPrefab);
            _prefabsByGUIDAndType[prefab.guid].Add(type, newPrefab);
            _prefabs.Add(newPrefab);

            return id;
        }

        public bool Contains(PrefabSetup prefab, TreeComponentType type)
        {
            return prefabsByGUIDAndType.ContainsKey(prefab.guid) && _prefabsByGUIDAndType[prefab.guid].ContainsKey(type);
        }

        public bool Contains(int prefabID)
        {
            return prefabsByID.ContainsKey(prefabID);
        }

        public int GetPrefabID(GameObject prefab, TreeComponentType type)
        {
            AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(prefab, out var guid, out long _);
            return prefabsByGUIDAndType[guid][type].prefabID;
        }

        public TreePrefab GetPrefab(int prefabID)
        {
            return prefabsByID[prefabID];
        }

        public TreePrefab GetPrefab(PrefabSetup prefab, TreeComponentType type)
        {
            return _prefabsByGUIDAndType[prefab.guid][type];
        }

        public void UpdatePrefabAvailability(ITree species)
        {
            foreach (var leafGroup in species.Leaves)
            {
                var prefab = leafGroup.geometry.prefab;
                if ((leafGroup.geometry.geometryMode != LeafGeometryMode.Mesh) || (prefab == null))
                {
                    continue;
                }


                var treePrefab = prefabsByGUIDAndType[prefab.guid][TreeComponentType.Leaf];

                treePrefab.SetAvailability();
            }

            foreach (var fruitGroup in species.Fruits)
            {
                var prefab = fruitGroup.geometry.prefab;
                if (prefab == null)
                {
                    continue;
                }
                

                var treePrefab = prefabsByGUIDAndType[prefab.guid][TreeComponentType.Fruit];

                treePrefab.SetAvailability();
            }

            foreach (var knotGroup in species.Knots)
            {
                var prefab = knotGroup.geometry.prefab;
                if (prefab == null)
                {
                    continue;
                }

                
                var treePrefab = prefabsByGUIDAndType[prefab.guid][TreeComponentType.Knot];

                treePrefab.SetAvailability();
            }

            foreach (var fungusGroup in species.Fungi)
            {
                var prefab = fungusGroup.geometry.prefab;
                
                if ((prefab == null) || (prefab.prefab == null))
                {
                    continue;
                }

                var treePrefab = prefabsByGUIDAndType[prefab.guid][TreeComponentType.Fungus];

                treePrefab.SetAvailability();
            }
        }

        public void UpdatePrefabAvailability(IBranch branch)
        {
            foreach (var leafGroup in branch.Leaves)
            {
                var prefab = leafGroup.geometry.prefab;
                if ((leafGroup.geometry.geometryMode != LeafGeometryMode.Mesh) || (prefab == null))
                {
                    continue;
                }


                var treePrefab = prefabsByGUIDAndType[prefab.guid][TreeComponentType.Leaf];

                treePrefab.SetAvailability();
            }

            foreach (var fruitGroup in branch.Fruits)
            {
                var prefab = fruitGroup.geometry.prefab;
                if (prefab == null)
                {
                    continue;
                }
                

                var treePrefab = prefabsByGUIDAndType[prefab.guid][TreeComponentType.Fruit];

                treePrefab.SetAvailability();
            }
        }

        
        private void RebuildLookups()
        {
            if (_prefabsByID == null)
            {
                _prefabsByID = new Dictionary<int, TreePrefab>();
            }

            if (_prefabsByID.Count != prefabs.Count)
            {
                _prefabsByID.Clear();

                for (var i = 0; i < prefabs.Count; i++)
                {
                    var original = prefabs[i];
                    _prefabsByID.Add(original.prefabID, original);
                }
            }

            if (_prefabsByGUIDAndType == null)
            {
                _prefabsByGUIDAndType = new Dictionary<string, Dictionary<TreeComponentType, TreePrefab>>();
            }

            var sum = _prefabsByGUIDAndType.Values.Sum(v => v.Count);

            if (_prefabsByID.Count != sum)
            {
                _prefabsByGUIDAndType.Clear();

                for (var i = 0; i < prefabs.Count; i++)
                {
                    var original = prefabs[i];

                    if (!_prefabsByGUIDAndType.ContainsKey(original.guid))
                    {
                        _prefabsByGUIDAndType.Add(original.guid, new Dictionary<TreeComponentType, TreePrefab>());
                    }

                    _prefabsByGUIDAndType[original.guid].Add(original.type, original);
                }
            }
        }

        public void UpdatePrefabs(ITree species)
        {
            foreach (var hierarchy in species.Leaves)
            {
                if (hierarchy.geometry.geometryMode == LeafGeometryMode.Mesh)
                {
                    var prefab = hierarchy.geometry.prefab;
                    
                    if (prefab == null) continue;

                    if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid) &&
                        !_prefabsByGUIDAndType[prefab.guid].ContainsKey(TreeComponentType.Leaf))
                    {
                        AddTreePrefab(prefab, TreeComponentType.Leaf);
                    }
                }
            }

            foreach (var hierarchy in species.Fruits)
            {
                var prefab = hierarchy.geometry.prefab;
                    
                if (prefab == null) continue;

                if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid) &&
                    !_prefabsByGUIDAndType[prefab.guid].ContainsKey(TreeComponentType.Fruit))
                {
                    AddTreePrefab(prefab, TreeComponentType.Fruit);
                }
            }

            foreach (var hierarchy in species.Knots)
            {
                var prefab = hierarchy.geometry.prefab;
                    
                if (prefab == null) continue;

                if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid) &&
                    !_prefabsByGUIDAndType[prefab.guid].ContainsKey(TreeComponentType.Knot))
                {
                    AddTreePrefab(prefab, TreeComponentType.Knot);
                }
            }

            foreach (var hierarchy in species.Fungi)
            {
                var prefab = hierarchy.geometry.prefab;
                    
                if ((prefab == null) || (prefab.prefab == null)) continue;

                if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid) ||
                    !_prefabsByGUIDAndType[prefab.guid].ContainsKey(TreeComponentType.Fungus))
                {
                    AddTreePrefab(prefab, TreeComponentType.Fungus);
                }
            }
        }


        public void UpdatePrefabs(IBranch branch)
        {
            foreach (var hierarchy in branch.Leaves)
            {
                if (hierarchy.geometry.geometryMode == LeafGeometryMode.Mesh)
                {
                    var prefab = hierarchy.geometry.prefab;
                    
                    if (prefab == null) continue;

                    if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid) &&
                        !_prefabsByGUIDAndType[prefab.guid].ContainsKey(TreeComponentType.Leaf))
                    {
                        AddTreePrefab(prefab, TreeComponentType.Leaf);
                    }
                }
            }

            foreach (var hierarchy in branch.Fruits)
            {
                var prefab = hierarchy.geometry.prefab;
                    
                if (prefab == null) continue;

                if (!_prefabsByGUIDAndType.ContainsKey(prefab.guid) &&
                    !_prefabsByGUIDAndType[prefab.guid].ContainsKey(TreeComponentType.Fruit))
                {
                    AddTreePrefab(prefab, TreeComponentType.Fruit);
                }
            }
        }
        
        public bool ContainsMaterial(Material m)
        {
            foreach (var prefab in prefabs)
            {
                var mats = prefab.GetMaterials();

                foreach (var mat in mats)
                {
                    if (m == mat)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Initialize()
        {
            if (_prefabsByGUIDAndType == null)
            {
                _prefabsByGUIDAndType = new Dictionary<string, Dictionary<TreeComponentType, TreePrefab>>();
            }

            if (_prefabsByID == null)
            {
                _prefabsByID = new Dictionary<int, TreePrefab>();
            }

            if (_prefabIDs == null)
            {
                _prefabIDs = new IDIncrementer(true);
            }

            if (_prefabs == null)
            {
                _prefabs = new List<TreePrefab>();
            }
        }
        
        public void ResetPrefabs()
        {
            Initialize();
            
            _prefabsByGUIDAndType.Clear();
            _prefabsByID.Clear();
            _prefabIDs.SetNextID(0);
            _prefabs.Clear();
            
        }
    }
}