using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects
{
    [Serializable]
    public class LeafUVRectSet
    {
        public Material material;
        
        public List<LeafUVRect> uvRects = new List<LeafUVRect>();
    }
}
