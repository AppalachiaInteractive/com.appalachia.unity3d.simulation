using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Recursion
{
    public abstract class ShapeRecursionData<TShape, THierarchy> 
        where TShape : ShapeData
        where THierarchy : HierarchyData
    {
        public THierarchy hierarchy;
        public HierarchyData parentHierarchy;
        public ShapeData parentShape;

        public TShape shape;
        public int shapeCount;
        public int shapeIndex;
        public TreeComponentType type;

        protected ShapeRecursionData(
            TShape shape, 
            THierarchy hierarchy, 
            int shapeIndex, 
            int shapeCount,
            HierarchyData parentHierarchy,
            ShapeData parentShape)
        {
            this.shape = shape;
            this.hierarchy = hierarchy;
            this.shapeIndex = shapeIndex;
            this.shapeCount = shapeCount;
            type = shape.type;
            this.parentHierarchy = parentHierarchy;
            this.parentShape = parentShape;
        }

        public override string ToString()
        {
            return
                $"ShapeID: {shape.shapeID} | {shape.type} | HierarchyID: {hierarchy.hierarchyID}";
        }
    }
}