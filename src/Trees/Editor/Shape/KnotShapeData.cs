using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class KnotShapeData : ShapeData
    {
        public override TreeComponentType type => TreeComponentType.Knot;


        protected override ShapeData GetNew()
        {
            return new KnotShapeData(shapeID, hierarchyID, parentShapeID);
        }

        protected override void Clone(ShapeData shapeData)
        {
        }

        public KnotShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(shapeID, hierarchyID, parentShapeID)
        {
            
        }
    }
}