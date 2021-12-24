using Appalachia.Simulation.Trees.Core;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.UI.Graph
{
    public class TreeGraphSettings : SingletonAppalachiaTreeObject<TreeGraphSettings>
    {
        [BoxGroup("Graph")]
        public float graphSize = 250f;
        [BoxGroup("Graph")]
        public float horizontalBufferPixels = 20f;
        [BoxGroup("Graph")]
        public float horizontalSubtreeBufferPixels = 20f;
        [BoxGroup("Graph")]
        public float verticalBufferPixels = 20f;
        [BoxGroup("Graph")]
        public VerticalJustification justification;
        
        [BoxGroup("Node")]
        public float nodeHeight = 48f;
        [BoxGroup("Node")]
        public float nodeWidth = 60f;
        [BoxGroup("Node")]
        public float nodeIconSize = 36f;
        [BoxGroup("Node")]
        public float nodeIconPaddingX = 1f;
        [BoxGroup("Node")]
        public float nodeIconPaddingY = 1f;
        [BoxGroup("Node")]
        public float nodePadding = 24f;

        [BoxGroup("Visibility")]
        public float visibleIconPadding = 2f;
        [BoxGroup("Visibility")]
        public float visibleIconSize = 18f;

        
        [BoxGroup("Shape Count")]
        public float countPadding = 4f;
        [BoxGroup("Shape Count")]
        public int countSize = 13;
        
        
        [BoxGroup("Stats")]
        public float statsWindowWidth = 84f;
        [BoxGroup("Stats")]
        public float statsWidth = 80f;
        [BoxGroup("Stats")]
        public float statsHeight = 50f;
        [BoxGroup("Stats")]
        public float statsPaddingX = 4f;
        [BoxGroup("Stats")]
        public float statsPaddingY = 4f;

        
        [BoxGroup("Scrolling")]
        public float scrollSpeed = 5f;
        [BoxGroup("Scrolling")]
        public float fastScrollSpeed = 12f;
    }
}
