using System;
using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    public class DefaultMaterialSettings : AppalachiaSimpleBase
    {
        public Material materialBark;
        public Material materialBreak;
        public Material materialFrond;
        public Material materialLeaf;
    }
}