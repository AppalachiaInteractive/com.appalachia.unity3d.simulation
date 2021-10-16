using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    /// <summary>
    ///     A class for storing and applying Vector2 masks.
    /// </summary>
    internal sealed class HandleConstraint2D
    {
        public static readonly HandleConstraint2D None = new HandleConstraint2D(1, 1);
        public int x, y;

        public HandleConstraint2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 Mask(Vector2 v)
        {
            v.x *= x;
            v.y *= y;
            return v;
        }

        public Vector2 InverseMask(Vector2 v)
        {
            v.x *= x == 1 ? 0f : 1f;
            v.y *= y == 1 ? 0f : 1f;
            return v;
        }

        public static bool operator ==(HandleConstraint2D a, HandleConstraint2D b)
        {
            return (a.x == b.x) && (a.y == b.y);
        }

        public static bool operator !=(HandleConstraint2D a, HandleConstraint2D b)
        {
            return (a.x != b.x) || (a.y != b.y);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return o is HandleConstraint2D && (((HandleConstraint2D) o).x == x) && (((HandleConstraint2D) o).y == y);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}
