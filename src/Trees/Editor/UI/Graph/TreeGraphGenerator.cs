using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Graph
{
    public class TreeGraphLayoutResult
    {
        public float height;
        public List<float> layerHeights = new List<float>();
    }

    public static class TreeGraphGenerator
    {
        public static TreeGraphLayoutResult LayoutTree(
            TreeGraphSettings settings,
            TreeGraphNode root)
        {
            var result = new TreeGraphLayoutResult();

            LayoutTree(result, settings, root, 0);

            DetermineFinalPositions(result, settings, root, 0, 0f, root.minXRelativeToBoundingBox);

            return result;
        }

        private static void LayoutTree(
            TreeGraphLayoutResult result,
            TreeGraphSettings settings,
            TreeGraphNode node,
            int row)
        {
            if (node.children.Count == 0)
            {
                LayoutLeafNode(settings, node);
            }
            else
            {
                LayoutInteriorNode(result, settings, node, row);
            }

            UpdateLayerHeight(result, settings, row);
        }

        private static void LayoutLeafNode(TreeGraphSettings settings, TreeGraphNode node)
        {
            node.subtreeWidth = settings.nodeWidth;
            node.minXRelativeToParent = 0f;
            node.position.y = 0f;
            node.minXRelativeToRoot.Add(0f);
            node.maxXRelativeToRoot.Add(settings.nodeWidth);
        }

        private static void LayoutInteriorNode(
            TreeGraphLayoutResult result,
            TreeGraphSettings settings,
            TreeGraphNode node,
            int row)
        {
            LayoutAllOurChildren(result, settings, row, node.children);

            // This width doesn't account for the parent node's width...
            node.subtreeWidth = CalculateWidthFromInterChildDistances(settings, node);
            node.minXRelativeToParent = 0f;
            node.position.y = 0f;

            // ...so that this centering may place the parent node negatively while the "width" is the width of
            // all the child nodes.
            CenterOverChildren(settings, node);
            DetermineParentRelativePositionsOfChildren(settings, node);
            CalculateBoundaryLists(settings, node);
        }

        private static void LayoutAllOurChildren(
            TreeGraphLayoutResult result,
            TreeGraphSettings settings,
            int row,
            List<TreeGraphNode> children)
        {
            var xsRelativeToBoundingBox = new List<float>();
            var responsibleNodeIndices = new List<int>();
            for (var childIndex = 0; childIndex < children.Count; childIndex++)
            {
                var tn = children[childIndex];
                LayoutTree(result, settings, tn, row + 1);
                RepositionSubtree(
                    settings,
                    childIndex,
                    children,
                    xsRelativeToBoundingBox,
                    responsibleNodeIndices
                );
            }
        }

        private static void CenterOverChildren(TreeGraphSettings settings, TreeGraphNode node)
        {
            // We should be centered between  the connection points of our children...
            var firstChild = node.children.First();
            var pxLeftChild = firstChild.minXRelativeToBoundingBox + (settings.nodeWidth / 2f);

            var lastChild = node.children.Last();
            var pxRightChild = lastChild.minXRelativeToBoundingBox + (settings.nodeWidth / 2f);

            node.minXRelativeToBoundingBox =
                ((pxLeftChild + pxRightChild) - settings.nodeWidth) / 2f;

            // If the root node was wider than the subtree, then we'll have a negative position for it.  We need
            // to readjust things so that the left of the root node represents the left of the bounding box and
            // the child distances to the Bounding box need to be adjusted accordingly.
            if (node.minXRelativeToBoundingBox < 0f)
            {
                foreach (var child in node.children)
                {
                    child.minXRelativeToBoundingBox -= node.minXRelativeToBoundingBox;
                }

                node.minXRelativeToBoundingBox = 0f;
            }
        }

        private static void DetermineParentRelativePositionsOfChildren(
            TreeGraphSettings settings,
            TreeGraphNode node)
        {
            foreach (var child in node.children)
            {
                child.minXRelativeToParent = child.minXRelativeToBoundingBox -
                    node.minXRelativeToBoundingBox;
            }
        }

        private static float CalculateWidthFromInterChildDistances(
            TreeGraphSettings settings,
            TreeGraphNode node)
        {
            var currentMinXRelativeToBoundingBox = node.children.First().minXRelativeToBoundingBox;
            var width = 0.0f;

            // If a subtree extends deeper than it's left neighbors then at that lower level it could potentially extend beyond those neighbors
            // on the left.  We have to check for this and make adjustements after the loop if it occurred.
            var pxUndercut = 0.0f;

            foreach (var child in node.children)
            {
                currentMinXRelativeToBoundingBox += child.xRelativeToLeftSibling;

                if (child.minXRelativeToBoundingBox > currentMinXRelativeToBoundingBox)
                {
                    pxUndercut = Mathf.Max(
                        pxUndercut,
                        child.minXRelativeToBoundingBox - currentMinXRelativeToBoundingBox
                    );
                }

                // pxWidth might already be wider than the current node's subtree if earlier nodes "undercut" on the
                // right hand side so we have to take the Max here...
                width = Mathf.Max(
                    width,
                    (currentMinXRelativeToBoundingBox + child.subtreeWidth) -
                    child.minXRelativeToBoundingBox
                );

                // After this next statement, the BoundingBox we're relative to is the one of our parent's subtree rather than
                // our own subtree (with the exception of undercut considerations)
                child.minXRelativeToBoundingBox = currentMinXRelativeToBoundingBox;
            }

            if (pxUndercut > 0.0f)
            {
                foreach (var child in node.children)
                {
                    child.minXRelativeToBoundingBox += pxUndercut;
                }

                width += pxUndercut;
            }

            // We are never narrower than our root node's width which we haven't taken into account yet so
            // we do that here.
            return Mathf.Max(settings.nodeWidth, width);
        }

        private static void CalculateBoundaryLists(TreeGraphSettings settings, TreeGraphNode node)
        {
            node.minXRelativeToRoot.Add(0.0f);
            node.maxXRelativeToRoot.Add(settings.nodeWidth);

            DetermineBoundary(settings, node.children, true /* fLeft */, node.minXRelativeToRoot);

            DetermineBoundary(
                settings,
                node.children.ToArray().Reverse(),
                false /* fLeft */,
                node.maxXRelativeToRoot
            );
        }

        private static void DetermineBoundary(
            TreeGraphSettings settings,
            IEnumerable<TreeGraphNode> children,
            bool leftBoundary,
            List<float> boundaries)
        {
            var cLayersDeep = 1;

            foreach (var child in children)
            {
                List<float> currentPositions;
                if (leftBoundary)
                {
                    currentPositions = child.minXRelativeToRoot;
                }
                else
                {
                    currentPositions = child.maxXRelativeToRoot;
                }

                if (currentPositions.Count >= boundaries.Count)
                {
                    using (IEnumerator<float> enPosCur = currentPositions.GetEnumerator())
                    {
                        for (var i = 0; i < (cLayersDeep - 1); i++)
                        {
                            enPosCur.MoveNext();
                        }

                        while (enPosCur.MoveNext())
                        {
                            boundaries.Add(enPosCur.Current + child.minXRelativeToParent);
                            cLayersDeep++;
                        }
                    }
                }
            }
        }

        private static void ApportionSlop(
            TreeGraphSettings settings,
            int siblingIndex,
            int responsibleIndex,
            List<TreeGraphNode> siblings)
        {
            var pxSlop = siblings[siblingIndex].xRelativeToLeftSibling - settings.nodeWidth -
                settings.horizontalBufferPixels;

            if (pxSlop > 0f)
            {
                for (var i = responsibleIndex + 1; i < siblingIndex; i++)
                {
                    siblings[i].xRelativeToLeftSibling += (pxSlop * (i - responsibleIndex)) /
                        (siblingIndex - responsibleIndex);
                }

                siblings[siblingIndex].xRelativeToLeftSibling -=
                    ((siblingIndex - responsibleIndex - 1f) * pxSlop) /
                    (siblingIndex - responsibleIndex);
            }
        }

        private static void RepositionSubtree(
            TreeGraphSettings settings,
            int siblingIndex,
            List<TreeGraphNode> siblings,
            List<float> xsRelativeToBoundingBox,
            List<int> responsibleNodeIndices)
        {
            int responsibleNodeIndex;
            var sibling = siblings[siblingIndex];

            if (siblingIndex == 0)
            {
                // No shifting but we still have to prepare the initial version of the
                // left hand skeleton list
                foreach (var pxRelativeToRoot in sibling.maxXRelativeToRoot)
                {
                    xsRelativeToBoundingBox.Add(
                        pxRelativeToRoot + sibling.minXRelativeToBoundingBox
                    );
                    responsibleNodeIndices.Add(0);
                }

                return;
            }

            int row;
            var horizontalBufferPixels = settings.horizontalBufferPixels;

            var newXRelativeToBoundingBox = CalculateNewPositions(
                settings,
                sibling,
                xsRelativeToBoundingBox,
                responsibleNodeIndices,
                out responsibleNodeIndex,
                out row
            );

            if (row != 0)
            {
                horizontalBufferPixels = settings.horizontalSubtreeBufferPixels;
            }

            sibling.xRelativeToLeftSibling =
                (newXRelativeToBoundingBox - xsRelativeToBoundingBox.First()) + settings.nodeWidth +
                horizontalBufferPixels;

            var cLevels = Mathf.Min(sibling.maxXRelativeToRoot.Count, xsRelativeToBoundingBox.Count);

            for (var i = 0; i < cLevels; i++)
            {
                xsRelativeToBoundingBox[i] = sibling.maxXRelativeToRoot[i] +
                    newXRelativeToBoundingBox + horizontalBufferPixels;
                responsibleNodeIndices[i] = siblingIndex;
            }

            for (var i = xsRelativeToBoundingBox.Count; i < sibling.maxXRelativeToRoot.Count; i++)
            {
                xsRelativeToBoundingBox.Add(
                    sibling.maxXRelativeToRoot[i] + newXRelativeToBoundingBox +
                    horizontalBufferPixels
                );
                responsibleNodeIndices.Add(siblingIndex);
            }

            ApportionSlop(settings, siblingIndex, responsibleNodeIndex, siblings);
        }

        private static float CalculateNewPositions(
            TreeGraphSettings settings,
            TreeGraphNode node,
            List<float> xsRelativeToBoundingBox,
            List<int> responsibleNodeIndices,
            out int responsibleNodeIndex,
            out int row)
        {
            var rows = Mathf.Min(node.minXRelativeToRoot.Count, xsRelativeToBoundingBox.Count);
            var pxRootPosRightmost = 0.0f;
            row = 0;

            using (IEnumerator<float> enRight = node.minXRelativeToRoot.GetEnumerator(),
                                      enLeft = xsRelativeToBoundingBox.GetEnumerator())
            using (IEnumerator<int> enResponsible = responsibleNodeIndices.GetEnumerator())
            {
                responsibleNodeIndex = -1;

                enRight.MoveNext();
                enLeft.MoveNext();
                enResponsible.MoveNext();
                for (var currentRow = 0; currentRow < rows; currentRow++)
                {
                    var pxLeftBorderFromBB = enLeft.Current;
                    var pxRightBorderFromRoot = enRight.Current;
                    float pxRightRootBasedOnThisLevel;
                    var itnResponsibleCur = enResponsible.Current;

                    enLeft.MoveNext();
                    enRight.MoveNext();
                    enResponsible.MoveNext();

                    pxRightRootBasedOnThisLevel = pxLeftBorderFromBB - pxRightBorderFromRoot;
                    if (pxRightRootBasedOnThisLevel > pxRootPosRightmost)
                    {
                        row = currentRow;
                        pxRootPosRightmost = pxRightRootBasedOnThisLevel;
                        responsibleNodeIndex = itnResponsibleCur;
                    }
                }
            }

            return pxRootPosRightmost;
        }

        private static void UpdateLayerHeight(
            TreeGraphLayoutResult result,
            TreeGraphSettings settings,
            int row)
        {
            while (result.layerHeights.Count <= row)
            {
                result.layerHeights.Add(0.0f);
            }

            result.layerHeights[row] = Mathf.Max(settings.nodeHeight, result.layerHeights[row]);
        }

        private static float Justify(TreeGraphSettings settings, float height, float pxRowHeight)
        {
            var dRet = 0.0f;

            switch (settings.justification)
            {
                case VerticalJustification.top:
                    break;

                case VerticalJustification.center:
                    dRet = (pxRowHeight - height) / 2f;
                    break;

                case VerticalJustification.bottom:
                    dRet = pxRowHeight - height;
                    break;
            }

            return dRet;
        }

        private static void DetermineFinalPositions(
            TreeGraphLayoutResult result,
            TreeGraphSettings settings,
            TreeGraphNode node,
            int row,
            float currentY,
            float currentX)
        {
            var pxRowHeight = result.layerHeights[row];
            float pxBottom;
            Vector2 originPoint;

            node.position.y = currentY + Justify(settings, settings.nodeHeight, pxRowHeight);
            pxBottom = node.position.y + settings.nodeHeight;
            if (pxBottom > result.height)
            {
                result.height = pxBottom;
            }

            node.position.x = node.minXRelativeToParent + currentX;
            originPoint = new Vector2(
                node.position.x + (settings.nodeWidth / 2f),
                node.position.y + settings.nodeHeight
            );

            row++;

            foreach (var child in node.children)
            {
                DetermineFinalPositions(
                    result,
                    settings,
                    child,
                    row,
                    currentY + pxRowHeight + settings.verticalBufferPixels,
                    node.position.x
                );
            }
        }
    }
}
