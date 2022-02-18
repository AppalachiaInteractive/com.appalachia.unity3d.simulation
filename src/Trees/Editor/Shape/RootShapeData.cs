using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class RootShapeData : BarkShapeData
    {
        public RootShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(
            shapeID,
            hierarchyID,
            parentShapeID
        )
        {
        }

        /// <inheritdoc />
        public override TreeComponentType type => TreeComponentType.Root;

        /// <inheritdoc />
        protected override void Clone(ShapeData shapeData)
        {
            var s = shapeData as RootShapeData;
            s.spline = spline;
            s.capRange = capRange;
            s.breakOffset = breakOffset;
        }

        /// <inheritdoc />
        protected override ShapeData GetNew()
        {
            return new RootShapeData(shapeID, hierarchyID, parentShapeID);
        }
    }
}
