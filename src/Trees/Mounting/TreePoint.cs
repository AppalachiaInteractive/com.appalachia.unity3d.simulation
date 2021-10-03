using System;
using UnityEngine;

namespace Appalachia.Core.Trees.Mounting
{
    [Serializable]
    public class TreePoint
    {
        public Matrix4x4 point;

        public TreePointType pointType;
    }
}