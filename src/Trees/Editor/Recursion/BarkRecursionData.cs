using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Shape;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class BarkRecursionData : ShapeRecursionData<BarkShapeData, BarkHierarchyData>
    {
        public BarkHierarchyData barkParentHierarchy;
        public BarkShapeData barkParentShape;
        public SplineData parentSpline;

        public SplineData spline;

        public BarkRecursionData(BarkShapeData shape,
                                 BarkHierarchyData hierarchy,
                                 int shapeIndex, 
                                 int shapeCount,
                                 HierarchyData parentHierarchy,
                                 ShapeData parentShape) :
            base(shape, hierarchy, shapeIndex, shapeCount, parentHierarchy, parentShape)
        {
            spline = shape.spline;
            barkParentHierarchy = parentHierarchy as BarkHierarchyData;
            barkParentShape = parentShape as BarkShapeData;
            parentSpline = barkParentShape?.spline;
        }
    }
}