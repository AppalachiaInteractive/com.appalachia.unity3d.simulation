using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class GenericRecursionData : ShapeRecursionData<ShapeData, HierarchyData>
    {
        public GenericRecursionData(ShapeData shape,
                                    HierarchyData hierarchy,
                                    int shapeIndex, 
                                    int shapeCount,
                                    HierarchyData parentHierarchy,
                                    ShapeData parentShape) :
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
        }
    }
}