/*
using System.Collections.Generic;

using UnityEngine;

namespace Appalachia.Core.Runtime.Trees
{
    public class GlobalTreeRuntimeManager : InternalMonoBehaviour
    {
        public List<TreeSpeciesPrefabData> species;
        public LinkedList<TreePrefabInstanceData> instances;

        [SerializeField] private IDIncrementer treePrefabIDGenerator = new IDIncrementer(true);
        [SerializeField] private IDIncrementer treePrefabInstanceIDGenerator = new IDIncrementer(true);
        
        private Dictionary<string, TreeSpeciesPrefabData> _speciesLookup;

        private Dictionary<string, TreeSpeciesPrefabData> speciesLookup
        {
            get
            {
                if (_speciesLookup == null)
                {
                    _speciesLookup = new Dictionary<string, TreeSpeciesPrefabData>();
                }

                if (species == null)
                {
                    species = new List<TreeSpeciesPrefabData>();
                }

                if (_speciesLookup.Count != species.Count)
                {
                    _speciesLookup.Clear();

                    for (var i = 0; i < species.Count; i++)
                    {
                        var sp = species[i];
                        _speciesLookup.Add(sp.speciesMetadata.speciesName, sp);
                    }
                }

                return _speciesLookup;
            }
        }
        
        
        private Dictionary<int, Dictionary<int, TreePrefabInstanceData>> _instanceLookup;

        private Dictionary<int, Dictionary<int, TreePrefabInstanceData>> instanceLookup
        {
            get
            {
                if (_instanceLookup == null || instances == null || _instanceLookup.Count != instances.Count)
                {
                    BuildInstanceLookups();
                }
                
                return _instanceLookup;
            }
        }
        
        private void BuildInstanceLookups()
        {
            if (instances == null)
            {
                instances = new LinkedList<TreePrefabInstanceData>();
            }

            if (_instanceLookup == null)
            {
                _instanceLookup = new Dictionary<int, Dictionary<int, TreePrefabInstanceData>>();
            }

            if (_instanceLookup.Count != instances.Count)
            {
                _instanceLookup.Clear();

                foreach(var instance in instances)
                {
                    if (!_instanceLookup.ContainsKey(instance.prefabData.treePrefabID))
                    {
                        _instanceLookup.Add(instance.prefabData.treePrefabID, new Dictionary<int, TreePrefabInstanceData>());
                    }

                    var sublookup = _instanceLookup[instance.prefabData.treePrefabID];

                    if (!sublookup.ContainsKey(instance.treePrefabInstanceID))
                    {
                        sublookup.Add(instance.treePrefabInstanceID, instance);
                    }
                }
            }
        }
    }
}
*/


