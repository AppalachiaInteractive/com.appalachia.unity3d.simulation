using System;
using System.Collections;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape.Collections
{

    [Serializable]
    public class LogShapes : ShapeCollection<LogShapes>
    {
        public List<TrunkShapeData> trunkShapes;
        public List<BranchShapeData> branchShapes;

        public LogShapes()
        {
            trunkShapes = new List<TrunkShapeData>();
            branchShapes = new List<BranchShapeData>();
        }

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
        }
        

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

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        protected override IEnumerable<ShapeData> GetShapesInternal(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Trunk:
                    return trunkShapes;
                case TreeComponentType.Branch:
                    return branchShapes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape.type), shape.type, null);
            }
        }

        public override void CopyPropertiesToClone(LogShapes clone)
        {
            clone.branchShapes = branchShapes;
            clone.trunkShapes = trunkShapes;
        }

        public override IList this[int i]
        {
            get
            {
                if ((i < 0) || (i > 1))
                {
                    throw new IndexOutOfRangeException("Range must be between 0 and 3");
                }

                if (i == 0) return trunkShapes;
                return branchShapes;
            }
        }

        protected override void ClearInternal()
        {
            trunkShapes.Clear();
            branchShapes.Clear();
        }
    }
}