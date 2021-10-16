using System;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    [Serializable]
    public struct BillboardPlane
    {
        public float effectiveScale;
        public TreeVertex v0;
        public TreeVertex v1;
        public TreeVertex v2;
        public TreeVertex v3;

        public void Reset()
        {
            effectiveScale = 0f;
            /*v0.Return();
            v1.Return();
            v2.Return();
            v3.Return();*/
            v0 = default;
            v1 = default;
            v2 = default;
            v3 = default;
        }

        public TreeVertex this[int i]
        {
            get
            {
                if (i == 0) return v0;
                if (i == 1) return v1;
                if (i == 2) return v2;
                if (i == 3) return v3;
                
                return default;
            }
            
            set
            {
                if (i == 0) v0 = value;
                if (i == 1) v1 = value;
                if (i == 2) v2 = value;
                if (i == 3) v3 = value;
                
            }
        }
    }
}
