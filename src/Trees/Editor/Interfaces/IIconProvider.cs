using Appalachia.Simulation.Trees.Icons;

namespace Appalachia.Simulation.Trees.Interfaces
{
    public interface IIconProvider
    {
        TreeIcon GetIcon(bool enabled);
    }
}
