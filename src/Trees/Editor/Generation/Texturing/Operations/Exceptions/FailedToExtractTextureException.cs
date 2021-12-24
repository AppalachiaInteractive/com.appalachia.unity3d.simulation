using System;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations.Exceptions
{
    public class FailedToExtractTextureException : Exception
    {
        public FailedToExtractTextureException(Material m, TextureMap map) : base(
            ZString.Format("Failed to extract map [{0}] from material [{1}]", map, m?.name)
        )
        {
            this.m = m;
            this.map = map;
        }
        
        public Material m;
        public TextureMap map;
    }
}
