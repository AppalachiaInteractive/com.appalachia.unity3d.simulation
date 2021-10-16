using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class BranchRecursionData : ShapeRecursionData<BranchShapeData, BranchHierarchyData>
    {
        public BranchRecursionData(BranchShapeData shape,
                                   BranchHierarchyData hierarchy,
                                   int shapeIndex, 
                                   int shapeCount,
                                   HierarchyData parentHierarchy,
                                   ShapeData parentShape) :
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
        }
    }
}