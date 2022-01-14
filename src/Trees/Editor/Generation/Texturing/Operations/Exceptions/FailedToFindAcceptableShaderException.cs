using System;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations.Exceptions
{
    public class FailedToFindAcceptableShaderException : Exception
    {
        public FailedToFindAcceptableShaderException(Material m) : base(
            ZString.Format(
                "Failed to find an input shader that could handle shader [{0}] from material [{1}]",
                m.shader.name,
                m?.name
            )
        )
        {
            this.m = m;
        }

        #region Fields and Autoproperties

        public Material m;

        #endregion
    }
}
