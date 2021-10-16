using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class LeafRecursionData : ShapeRecursionData<LeafShapeData, LeafHierarchyData>
    {
        public BarkHierarchyData barkParentHierarchy;
        public BarkShapeData barkParentShape;
        public SplineData parentSpline;

        public LeafRecursionData(LeafShapeData shape,
                                 LeafHierarchyData hierarchy,
                                 int shapeIndex, 
                                 int shapeCount,
                                 BarkHierarchyData parentHierarchy,
                                 BarkShapeData parentShape) : 
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
            barkParentHierarchy = parentHierarchy;
            barkParentShape = parentShape;
            parentSpline = barkParentShape?.spline;
        }
    }
}