using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class TrunkRecursionData : ShapeRecursionData<TrunkShapeData, TrunkHierarchyData>
    {
        public TrunkRecursionData(TrunkShapeData shape,
                                  TrunkHierarchyData hierarchy,
                                  int shapeIndex, 
                                  int shapeCount,
                                  HierarchyData parentHierarchy,
                                  ShapeData parentShape) : 
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
        }
    }
}