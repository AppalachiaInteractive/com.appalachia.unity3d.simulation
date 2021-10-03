#region

using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Comparers;
using Appalachia.Simulation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical
{
    public class PhysicsMaterials : MetadataLookupBase<PhysicsMaterials, PhysicMaterialWrapper>
    {
        [FoldoutGroup("Misc")]
        public Material physicsVisualizationMaterial;
        

        [Button]
        public void SearchAll()
        {
            var list = all;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null) continue;
                
                if (list[i].CanSearch)
                {
                    list[i].Search();
                }
            }
        }

        private Dictionary<PhysicMaterial, PhysicMaterialWrapper> _lookup;
        
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
                _lookup = all.Where(a => a != null).ToDictionary(k => k.material, i => i, ObjectComparer<PhysicMaterial>.Instance);
            }

            if (_lookup.ContainsKey(m))
            {
                return _lookup[m];
            }

            return null;
        }
    }
}