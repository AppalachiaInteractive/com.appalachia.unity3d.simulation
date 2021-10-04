namespace Appalachia.Simulation.Core.Metadata.Tree.Types
{
    public enum AgeType
    {
        None = 0,
        Sapling    = 1 << 1,
        Young      = 1 << 2,
        Adult      = 1 << 3,
        Mature     = 1 << 4,
        Spirit     = 1 << 5
    }
}