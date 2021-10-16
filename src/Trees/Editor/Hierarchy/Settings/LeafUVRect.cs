using System;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class LeafUVRect
    {
        public UVRect rect = new UVRect();

        [PropertyRange(0f, 1f)] public float probability = 1.0f;

        public override string ToString()
        {
            return 
            $"{100f*probability:F0}% | " + 
            $"cen ({rect.center.x:F1},{rect.center.y:F1}) | " +
            $"{rect.rotation:F0}° | " +
            $"sz ({rect.size.x:F1},{rect.size.y:F1})";
        }

        public LeafUVRect Clone()
        {
            return new LeafUVRect() {rect = rect.Clone(), probability = probability};
        }
    }
    
}
