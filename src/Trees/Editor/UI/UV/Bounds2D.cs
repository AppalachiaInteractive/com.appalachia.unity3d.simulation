using System;
using System.Diagnostics;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    /// <summary>
    ///     Internal bounds class.
    /// </summary>
    [Serializable]
    internal sealed class Bounds2D
    {
        [SerializeField] public Vector2 center = Vector2.zero;
        [SerializeField] private Vector2 m_Size = Vector2.zero;

        public Bounds2D()
        {
        }

        public Bounds2D(Vector2 center, Vector2 size)
        {
            this.center = center;
            this.size = size;
        }

        public Vector2 size
        {
            get => m_Size;

            set => m_Size = value;
        }

        [DebuggerStepThrough] public override string ToString()
        {
            return "[cen: " + center + " size: " + size + "]";
        }
    }
}
