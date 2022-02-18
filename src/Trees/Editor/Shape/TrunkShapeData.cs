using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class TrunkShapeData : BarkShapeData
    {
        public TrunkShapeData(int shapeID, int hierarchyID) : base(shapeID, hierarchyID, -1)
        {
        }

        /// <inheritdoc />
        public override TreeComponentType type => TreeComponentType.Trunk;

        /// <inheritdoc />
        protected override ShapeData GetNew()
        {
            return new TrunkShapeData(shapeID, hierarchyID);
        }
    }
}
