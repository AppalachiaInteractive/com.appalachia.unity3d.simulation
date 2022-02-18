using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape.Collections
{
    [Serializable]
    public class TreeShapes : ShapeCollection<TreeShapes>
    {
        public TreeShapes()
        {
            trunkShapes = new List<TrunkShapeData>();
            rootShapes = new List<RootShapeData>();
            branchShapes = new List<BranchShapeData>();
            leafShapes = new List<LeafShapeData>();
            fruitShapes = new List<FruitShapeData>();
            knotShapes = new List<KnotShapeData>();
            fungusShapes = new List<FungusShapeData>();
        }

        #region Fields and Autoproperties

        public List<BranchShapeData> branchShapes;
        public List<FruitShapeData> fruitShapes;
        public List<FungusShapeData> fungusShapes;
        public List<KnotShapeData> knotShapes;
        public List<LeafShapeData> leafShapes;
        public List<RootShapeData> rootShapes;

        public List<TrunkShapeData> trunkShapes;

        #endregion

        /// <inheritdoc />
        public override IList this[int i]
        {
            get
            {
                if ((i < 0) || (i > 6))
                {
                    throw new IndexOutOfRangeException("Range must be between 0 and 6");
                }

                if (i == 0)
                {
                    return trunkShapes;
                }

                if (i == 1)
                {
                    return rootShapes;
                }

                if (i == 2)
                {
                    return branchShapes;
                }

                if (i == 3)
                {
                    return knotShapes;
                }

                if (i == 4)
                {
                    return leafShapes;
                }

                if (i == 5)
                {
                    return fruitShapes;
                }

                if (i == 6)
                {
                    return fungusShapes;
                }

                return fruitShapes;
            }
        }

        /// <inheritdoc />
        public override void CopyPropertiesToClone(TreeShapes clone)
        {
            clone.trunkShapes = trunkShapes.Select(t => t.Clone() as TrunkShapeData).ToList();
            clone.rootShapes = rootShapes.Select(t => t.Clone() as RootShapeData).ToList();
            clone.branchShapes = branchShapes.Select(t => t.Clone() as BranchShapeData).ToList();
            clone.leafShapes = leafShapes.Select(t => t.Clone() as LeafShapeData).ToList();
            clone.fruitShapes = fruitShapes.Select(t => t.Clone() as FruitShapeData).ToList();
            clone.knotShapes = knotShapes.Select(t => t.Clone() as KnotShapeData).ToList();
            clone.fungusShapes = fungusShapes.Select(t => t.Clone() as FungusShapeData).ToList();
        }

        /// <inheritdoc />
        public override IEnumerator<ShapeData> GetEnumerator()
        {
            foreach (var shape in trunkShapes)
            {
                yield return shape;
            }

            foreach (var shape in rootShapes)
            {
                yield return shape;
            }

            foreach (var shape in branchShapes)
            {
                yield return shape;
            }

            foreach (var shape in knotShapes)
            {
                yield return shape;
            }

            foreach (var shape in leafShapes)
            {
                yield return shape;
            }

            foreach (var shape in fungusShapes)
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
            knotShapes.Clear();
            leafShapes.Clear();
            rootShapes.Clear();
            fungusShapes.Clear();
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
                case TreeComponentType.Root:
                    var root = new RootShapeData(shapeID, hierarchyID, parentShapeID);

                    rootShapes.Add(root);
                    byID.Add(shapeID, root);

                    byHierarchyID[hierarchyID].Add(root);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(root);

                    return root;
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

                case TreeComponentType.Knot:
                    var knot = new KnotShapeData(shapeID, hierarchyID, parentShapeID);

                    knotShapes.Add(knot);
                    byID.Add(shapeID, knot);

                    byHierarchyID[hierarchyID].Add(knot);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(knot);

                    return knot;

                case TreeComponentType.Fungus:
                    var fungus = new FungusShapeData(shapeID, hierarchyID, parentShapeID);

                    fungusShapes.Add(fungus);
                    byID.Add(shapeID, fungus);

                    byHierarchyID[hierarchyID].Add(fungus);
                    byHierarchyIDAndParentID[hierarchyID][parentShapeID].Add(fungus);

                    return fungus;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<ShapeData> GetShapesInternal(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Root:
                    return rootShapes;
                case TreeComponentType.Trunk:
                    return trunkShapes;
                case TreeComponentType.Branch:
                    return branchShapes;
                case TreeComponentType.Leaf:
                    return leafShapes;
                case TreeComponentType.Fruit:
                    return fruitShapes;
                case TreeComponentType.Knot:
                    return knotShapes;
                case TreeComponentType.Fungus:
                    return fungusShapes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <inheritdoc />
        protected override void RemoveShape(ShapeData shape)
        {
            switch (shape.type)
            {
                case TreeComponentType.Root:
                    rootShapes.Remove(shape as RootShapeData);
                    break;
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
                case TreeComponentType.Knot:
                    knotShapes.Remove(shape as KnotShapeData);
                    break;
                case TreeComponentType.Fungus:
                    fungusShapes.Remove(shape as FungusShapeData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape.type), shape.type, null);
            }
        }
    }
}
