using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations.Exceptions
{
    public class FailedToFindAcceptableShaderException : Exception
    {
        public FailedToFindAcceptableShaderException(Material m) : base(
            $"Failed to find an input shader that could handle shader [{m.shader.name}] from material [{m?.name}]"
        )
        {
            this.m = m;
        }

        public Material m;
    }
}