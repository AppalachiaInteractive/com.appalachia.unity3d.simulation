using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Definition.Interfaces
{
    public interface IShapeWrite
    {
        void Rebuild();
        
        ShapeData CreateNewShape(TreeComponentType type, int hierarchyID, int parentShapeID);
       
        void UpdateIndividualHierarchyShapeCounts(
            HierarchyData hierarchy,
            HierarchyData parentHierarchy,
            int shapeCount,
            bool rebuildState = false);

        void DeleteShapeChain(int shapeID, bool rebuildState = true, int depth = 0);

        void DeleteManyShapeChains(IEnumerable<int> shapeIDs, bool rebuildState = true);
        
        void DeleteAllShapeChainsInHierarchy(int hierarchyID, bool rebuildState = true);
        
    }
}
