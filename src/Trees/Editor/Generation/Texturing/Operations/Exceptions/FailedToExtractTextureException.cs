using System;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations.Exceptions
{
    public class FailedToExtractTextureException : Exception
    {
        public FailedToExtractTextureException(Material m, TextureMap map)
        : base($"Failed to extract map [{map}] from material [{m?.name}]")
        {
            this.m = m;
            this.map = map;
        }
        
        public Material m;
        public TextureMap map;
    }
}
