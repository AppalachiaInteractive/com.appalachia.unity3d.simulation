using System.Collections.Generic;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Graph
{
    public struct TreeGraphConnection
    {
        public TreeGraphNode Parent { get; }

        public TreeGraphNode Child { get; }

        public List<Vector2> Points { get; }

        public TreeGraphConnection(TreeGraphNode parent, TreeGraphNode child, List<Vector2> points)
        {
            Child = child;
            Parent = parent;
            Points = points;
        }
    }
}
