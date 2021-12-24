using Appalachia.Core.Objects.Root;

namespace Appalachia.Simulation.Trees.Core
{
    public abstract class SingletonAppalachiaTreeObject<T> : SingletonAppalachiaObject<T>
        where T : SingletonAppalachiaObject<T>
    {
    }
}
