using System.Collections.Generic;
using System.Diagnostics;

namespace Appalachia.Simulation.Trees.Extensions
{
    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        [DebuggerStepThrough] public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        [DebuggerStepThrough] public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}