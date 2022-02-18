using System;
using System.Collections;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape.Collections
{
    [Serializable]
    public class BranchShapes : ShapeCollection<BranchShapes>
    {
        public BranchShapes()
        {
            trunkShapes = new List<TrunkShapeData>();
            branchShapes = new List<BranchShapeData>();
            leafShapes = new List<LeafShapeData>();
            fruitShapes = new List<FruitShapeData>();
        }

        #region Fields and Autoproperties

        public List<BranchShapeData> branchShapes;
        public List<FruitShapeData> fruitShapes;
        public List<LeafShapeData> leafShapes;
        public List<TrunkShapeData> trunkShapes;

        #endregion

        /// <inheritdoc />
        public override IList this[int i]
        {
            get
            {
                if ((i < 0) || (i > 3))
                {
                    throw new IndexOutOfRangeException("Range must be between 0 and 3");
                }

                if (i == 0)
                {
                    return trunkShapes;
                }

                if (i == 1)
                {
                    return branchShapes;
                }

                if (i == 2)
                {
                    return leafShapes;
                }

                if (i == 3)
                {
                    return fruitShapes;
                }

                return fruitShapes;
            }
        }

        /// <inheritdoc />
        public override void CopyPropertiesToClone(BranchShapes clone)
        {
            clone.branchShapes = branchShapes;
            clone.fruitShapes = fruitShapes;
            clone.leafShapes = leafShapes;
            clone.trunkShapes = trunkShapes;
        }

        /// <inheritdoc />
        public override IEnumerator<ShapeData> GetEnumerator()
        {
            foreach (var shape in trunkShapes)
            {
                yield return shape;
            }

            foreach (var shape in branchShapes)
            {
                yield return shape;
            }

            foreach (var shape in leafShapes)
            {
                yield return shape;
            }

            foreach (var shape in fruitShapes)
            {
                yield return shape;
            }
        }

        /// <inheritdoc />
        protected override void ClearInternal()
        {
            trunkShapes.Clear();
            branchShapes.Clear();
            fruitShapes.Clear();
            leafShapes.Clear();
        }

        /// <inheritdoc />
        protected override ShapeData CreateNewShape(
            TreeComponentType type,
            int hierarchyID,
            int shapeID,
            int parentShapeID)
        {
            switch (type)
            {
                case TreeComponentType.Trunk:
                    var trunk = new TrunkShapeData(shapeID, hierarchyID);

                    trunkShapes.Add(trunk);
                    byID.Add(shapeID, trunk);

                    byHierarchyID[hierarchyID].Add(trunk);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(trunk);

                    return trunk;

                case TreeComponentType.Branch:
                    var branch = new BranchShapeData(shapeID, hierarchyID, parentShapeID);

                    branchShapes.Add(branch);
                    byID.Add(shapeID, branch);

                    byHierarchyID[hierarchyID].Add(branch);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(branch);

                    return branch;

                case TreeComponentType.Leaf:
                    var leaf = new LeafShapeData(shapeID, hierarchyID, parentShapeID);

                    leafShapes.Add(leaf);
                    byID.Add(shapeID, leaf);

                    byHierarchyID[hierarchyID].Add(leaf);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(leaf);

                    return leaf;

                case TreeComponentType.Fruit:
                    var fruit = new FruitShapeData(shapeID, hierarchyID, parentShapeID);

                    fruitShapes.Add(fruit);
                    byID.Add(shapeID, fruit);

                    byHierarchyID[hierarchyID].Add(fruit);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(fruit);

                    return fruit;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<ShapeData> GetShapesInternal(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Trunk:
                    return trunkShapes;
                case TreeComponentType.Branch:
                    return branchShapes;
                case TreeComponentType.Leaf:
                    return leafShapes;
                case TreeComponentType.Fruit:
                    return fruitShapes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <inheritdoc />
        protected override void RemoveShape(ShapeData shape)
        {
            switch (shape.type)
            {
                case TreeComponentType.Trunk:
                    trunkShapes.Remove(shape as TrunkShapeData);
                    break;
                case TreeComponentType.Branch:
                    branchShapes.Remove(shape as BranchShapeData);
                    break;
                case TreeComponentType.Leaf:
                    leafShapes.Remove(shape as LeafShapeData);
                    break;
                case TreeComponentType.Fruit:
                    fruitShapes.Remove(shape as FruitShapeData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape.type), shape.type, null);
            }
        }
    }
}
