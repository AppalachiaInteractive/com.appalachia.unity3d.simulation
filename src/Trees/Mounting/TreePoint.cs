using System;
using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Mounting
{
    [Serializable]
    public class TreePoint : AppalachiaSimpleBase
    {
        public Matrix4x4 point;

        public TreePointType pointType;
    }
}
