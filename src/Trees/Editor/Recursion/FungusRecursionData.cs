using System;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;

namespace Appalachia.Simulation.Trees.Recursion
{
    [Serializable]
    public class FungusRecursionData : ShapeRecursionData<FungusShapeData, FungusHierarchyData>
    {
        public FungusRecursionData(FungusShapeData shape,
                                   FungusHierarchyData hierarchy,
                                   int shapeIndex, 
                                   int shapeCount,
                                   HierarchyData parentHierarchy,
                                   ShapeData parentShape) :
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
        }
    }
}
