namespace Appalachia.Simulation.Trees.Hierarchy.Options
{
    public interface ICloneable<T>
    {
        T Clone();
    }

    public interface IElementCloner<T>
    {
        T CloneElement(T model);
    }
}
