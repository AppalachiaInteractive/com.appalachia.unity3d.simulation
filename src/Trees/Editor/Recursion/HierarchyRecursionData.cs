using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Recursion
{
    [NonSerializable]
    public class HierarchyRecursionData : AppalachiaSimpleBase
    {
        public HierarchyRecursionData(HierarchyData hierarchy, HierarchyData parentHierarchy)
        {
            this.hierarchy = hierarchy;
            this.parentHierarchy = parentHierarchy;
            type = hierarchy.type;
        }

        #region Fields and Autoproperties

        public HierarchyData hierarchy;
        public HierarchyData parentHierarchy;
        public TreeComponentType type;

        #endregion
    }
}
