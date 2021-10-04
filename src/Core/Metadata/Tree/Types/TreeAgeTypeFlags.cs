using System;

namespace Appalachia.Simulation.Core.Metadata.Tree.Types
{
    [Flags]
    public enum TreeAgeTypeFlags
    {
        All = Mature | Sapling | Young | Adult | Spirit,
        None = 0,

        // ReSharper disable once ShiftExpressionRealShiftCountIsZero
        Mature = 1 << 0,
        Sapling = 1 << 1,
        Young = 1 << 2,
        Adult = 1 << 3,
        Spirit = 1 << 4
    }
}
