using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class BranchShapeData : BarkShapeData
    {
        public override TreeComponentType type => TreeComponentType.Branch;

        protected override ShapeData GetNew()
        {
            return new BranchShapeData(shapeID, hierarchyID, parentShapeID);
        }

        protected override void Clone(ShapeData shapeData)
        {
            var s = shapeData as BranchShapeData;
            s.spline = spline;
            s.capRange = capRange;
            s.breakOffset = breakOffset;
        }

        public BranchShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(shapeID, hierarchyID, parentShapeID)
        {
        }
    }
}