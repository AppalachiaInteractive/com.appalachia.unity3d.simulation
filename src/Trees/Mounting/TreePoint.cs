using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Mounting
{
    [Serializable]
    public class TreePoint
    {
        public Matrix4x4 point;

        public TreePointType pointType;
    }
}