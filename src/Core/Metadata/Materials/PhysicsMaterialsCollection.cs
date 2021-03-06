#region

using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Comparisons;
using Appalachia.Core.Objects.Scriptables;
using Appalachia.Simulation.Core.Metadata.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core.Metadata.Materials
{
    public class PhysicsMaterialsCollection : AppalachiaMetadataCollection<PhysicsMaterialsCollection, PhysicMaterialWrapper, AppaList_PhysicMaterialWrapper>
    {
        
        
        [FoldoutGroup("Misc")] public Material physicsVisualizationMaterial;

        private Dictionary<PhysicMaterial, PhysicMaterialWrapper> _lookup;

#if UNITY_EDITOR
        [Button]
        public void SearchAll()
        {
            var list = all;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    continue;
                }

                if (list[i].CanSearch)
                {
                    list[i].Search();
                }
            }
        }
#endif

        public PhysicMaterialWrapper Lookup(PhysicMaterial m)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<PhysicMaterial, PhysicMaterialWrapper>();
            }

            if (m == null)
            {
                return null;
            }

            if (_lookup.Count == 0)
            {
                _lookup = all.Where(a => a != null)
                             .ToDictionary(
                                  k => k.material,
                                  i => i,
                                  ObjectComparer<PhysicMaterial>.Instance
                              );
            }

            if (_lookup.TryGetValue(m, out var result)) return result;

            return null;
        }
    }
}
