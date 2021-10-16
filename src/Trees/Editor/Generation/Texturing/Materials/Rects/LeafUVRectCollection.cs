using System.Collections.Generic;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects
{
    public class LeafUVRectCollection : SelfSavingSingletonScriptableObject<LeafUVRectCollection>
    {
        public List<LeafUVRectSet> leafUVRectSets = new List<LeafUVRectSet>();
        
        private Dictionary<Material, LeafUVRectSet> _lookup = new Dictionary<Material, LeafUVRectSet>();

        private Dictionary<Material, LeafUVRectSet> lookup
        {
            get
            {
                if (_lookup == null)
                {
                    _lookup = new Dictionary<Material, LeafUVRectSet>();
                }

                foreach (var materialSet in _lookup)
                {
                    
                    if ((materialSet.Key != null) && 
                        (materialSet.Value != null) &&
                        !leafUVRectSets.Contains(materialSet.Value))
                    {
                        materialSet.Value.material = materialSet.Key;
                        
                        leafUVRectSets.Add(materialSet.Value);
                    }
                }

                for (var index = leafUVRectSets.Count - 1; index >= 0; index--)
                {
                    var set = leafUVRectSets[index];

                    if (set == null)
                    {
                        leafUVRectSets.RemoveAt(index);
                        continue;
                    }

                    if (_lookup.ContainsKey(set.material))
                    {
                        continue;
                    }
                    
                    _lookup.Add(set.material, set);
                }

                return _lookup;
            }   
        }

        public List<LeafUVRect> defaultRects = new List<LeafUVRect>(); 
        public List<LeafUVRect> Get(Material m)
        {
            if (m == null)
            {
                if (defaultRects == null)
                {
                    defaultRects = new List<LeafUVRect>(); 
                }

                if (defaultRects.Count != 1)
                {
                    defaultRects.Clear();
                    var r = new LeafUVRect();
                    r.rect.Reset();

                    defaultRects.Add(r);
                }
                
                return defaultRects;
            }
            
            if (!lookup.ContainsKey(m))
            {
                lookup.Add(m, new LeafUVRectSet());
            }

            var set = lookup[m];

            if (set.uvRects == null)
            {
                set.uvRects = new List<LeafUVRect>();
            }
            if (set.uvRects.Count == 0)
            {
                var rect = new LeafUVRect();
                rect.rect.Reset();
                set.uvRects.Add(rect);
            }

            return set.uvRects;
        }
    }
}
