using Appalachia.Simulation.Trees.Core.Seeds;

namespace Appalachia.Simulation.Trees.Definition.Interfaces
{
    public interface ITree : IBranch, IRootProvider, IFungusProvider, IKnotProvider
    {
        ExternalDualSeed Seed { get; set; }
    }
}