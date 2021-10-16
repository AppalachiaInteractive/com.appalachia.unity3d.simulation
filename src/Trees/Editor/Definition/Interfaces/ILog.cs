using System.Collections.Generic;
using Appalachia.Simulation.Trees.Hierarchy;

namespace Appalachia.Simulation.Trees.Definition.Interfaces
{
    public interface ILog : IBranchProvider, ITrunkProvider, IEnumerable<HierarchyData>
    {
    }
}