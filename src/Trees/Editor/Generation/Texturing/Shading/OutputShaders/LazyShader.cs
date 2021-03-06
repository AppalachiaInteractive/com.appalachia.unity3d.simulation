using System;
using System.Diagnostics;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    [Serializable]
    public sealed class LazyShader
    {
        public LazyShader(string shaderName)
        {
            _shaderName = shaderName;
        }

        public LazyShader(Shader shader)
        {
            _shaderName = shader.name;
            _shader = shader;
        }

        #region Fields and Autoproperties

        [SerializeField] private Shader _shader;

        [SerializeField] private string _shaderName;

        #endregion

        public Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Shader.Find(_shaderName);
                }

                return _shader;
            }
        }

        [DebuggerStepThrough]
        public static implicit operator Shader(LazyShader s)
        {
            return s?.shader;
        }
    }
}
