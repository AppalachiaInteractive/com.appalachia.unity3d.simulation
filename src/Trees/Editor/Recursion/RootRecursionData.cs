using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class RootRecursionData : ShapeRecursionData<RootShapeData, RootHierarchyData>
    {
        public RootRecursionData(RootShapeData shape,
                                 RootHierarchyData hierarchy,
                                 int shapeIndex, 
                                 int shapeCount,
                                 HierarchyData parentHierarchy,
                                 ShapeData parentShape) :
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
        }
    }
}