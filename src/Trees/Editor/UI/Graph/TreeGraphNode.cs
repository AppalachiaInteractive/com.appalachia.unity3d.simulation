using System;
using System.Collections.Generic;
using System.Diagnostics;
using Appalachia.Simulation.Trees.Hierarchy;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Graph
{
    public class TreeGraphNode : IEquatable<TreeGraphNode>
    {
        [DebuggerStepThrough] public bool Equals(TreeGraphNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(data, other.data);
        }

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((TreeGraphNode) obj);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            return (data != null ? data.GetHashCode() : 0);
        }

        [DebuggerStepThrough] public static bool operator ==(TreeGraphNode left, TreeGraphNode right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator !=(TreeGraphNode left, TreeGraphNode right)
        {
            return !Equals(left, right);
        }

        public List<TreeGraphNode> children;

        public HierarchyData data;

        public int depth;

        public List<float> maxXRelativeToRoot;

        //private class LayeredTreeInfo
        //{
        public List<float> minXRelativeToRoot;

        public TreeGraphNode parent;

        public Vector2 position;

        public bool branchNode => children.Count > 0;

        public int degree => children.Count;

        public bool leafNode => children.Count == 0;

        public int level => depth + 1;

        public bool root => parent == null;

        public float minXRelativeToBoundingBox { get; set; }

        public float minXRelativeToParent { get; set; }

        public float subtreeWidth { get; set; }

        public float xRelativeToLeftSibling { get; set; }

        public int GetSize()
        {
            if (leafNode)
            {
                return 1;
            }

            var childSize = 0;

            foreach (var child in children)
            {
                childSize += child.GetSize();
            }

            return childSize + 1;
        }

        public int GetLevelCount()
        {
            if (leafNode)
            {
                return 1;
            }

            var childLevels = 0;

            foreach (var child in children)
            {
                var c = GetLevelCount();

                if (c > childLevels)
                {
                    childLevels = c;
                }
            }

            return childLevels + 1;
        }

        public void Recursive(Action<TreeGraphNode> action, bool depthFirst)
        {
            if (!depthFirst)
            {
                action(this);
            }

            foreach (var child in children)
            {
                child.Recursive(action, depthFirst);
            }

            if (depthFirst)
            {
                action(this);
            }
        }

        
        public Rect GetRect(TreeGraphSettings settings, Vector2 offset)
        {
            return new Rect(
                position.x + offset.x,
                position.y + offset.y,
                settings.nodeWidth,
                settings.nodeHeight
            );
        }

        public TreeGraphNode(HierarchyData data, TreeGraphNode parent)
        {
            this.data = data;
            this.parent = parent;
            children = new List<TreeGraphNode>();
            minXRelativeToRoot = new List<float>();
            maxXRelativeToRoot = new List<float>();

            if (this.parent != null)
            {
                parent.children.Add(this);
            }
        }

        /*
            /// <summary>
            /// Initializes a new instance of the GraphLayoutInfo class.
            /// </summary>
            public LayeredTreeInfo(double subTreeWidth, ITreeNode tn)
            {
                SubTreeWidth = subTreeWidth;
                pxLeftPosRelativeToParent = 0;
                pxFromTop = 0;
                ign = tn;
            }
        }*/
    }
}
