using System;
using System.Diagnostics;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class LeafUVRect : AppalachiaSimpleBase
    {
        public UVRect rect = new UVRect();

        [PropertyRange(0f, 1f)] public float probability = 1.0f;

        [DebuggerStepThrough] public override string ToString()
        {
            return ZString.Format("{0:F0}% | ",             100f * probability) +
                   ZString.Format("cen ({0:F1},{1:F1}) | ", rect.center.x, rect.center.y) +
                   ZString.Format("{0:F0}° | ",             rect.rotation) +
                   ZString.Format("sz ({0:F1},{1:F1})",     rect.size.x, rect.size.y);
        }

        public LeafUVRect Clone()
        {
            return new LeafUVRect() {rect = rect.Clone(), probability = probability};
        }
    }
    
}
