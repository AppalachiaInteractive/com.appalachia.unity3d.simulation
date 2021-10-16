using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Definition.Interfaces
{
    public interface IHierarchyWrite
    {
        void SetHierarchyDepths();

        HierarchyData CreateTrunkHierarchy(InputMaterialCache materials, bool rebuildState = true);

        HierarchyData CreateHierarchy(TreeComponentType type, HierarchyData parent, InputMaterialCache materials, bool rebuildState = true);
        
        void UpdateHierarchyParent(HierarchyData hierarchy, HierarchyData parent, bool rebuildState = true);

        void DeleteHierarchyChain(int hierarchyID, bool rebuildState = true);
    }
}