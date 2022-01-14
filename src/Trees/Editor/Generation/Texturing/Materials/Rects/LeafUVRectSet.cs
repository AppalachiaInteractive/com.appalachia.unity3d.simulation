using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects
{
    [Serializable]
    public class LeafUVRectSet : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        public Material material;

        public List<LeafUVRect> uvRects = new List<LeafUVRect>();

        #endregion
    }
}
