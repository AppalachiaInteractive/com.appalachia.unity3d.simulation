using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Recursion
{
    public class HierarchyRecursionData : AppalachiaSimpleBase
    {
        public HierarchyData hierarchy;
        public HierarchyData parentHierarchy;
        public TreeComponentType type;

        public HierarchyRecursionData(
            HierarchyData hierarchy,
            HierarchyData parentHierarchy)
        {
            this.hierarchy = hierarchy;
            this.parentHierarchy = parentHierarchy;
            type = hierarchy.type;
        }
    }
}
