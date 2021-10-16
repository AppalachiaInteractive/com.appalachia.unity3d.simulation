using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class TrunkShapeData : BarkShapeData
    {
        public override TreeComponentType type => TreeComponentType.Trunk;

        protected override ShapeData GetNew()
        {
            return new TrunkShapeData(shapeID, hierarchyID);
        }

        public TrunkShapeData(int shapeID, int hierarchyID) : base(shapeID, hierarchyID, -1)
        {
        }
    }
}