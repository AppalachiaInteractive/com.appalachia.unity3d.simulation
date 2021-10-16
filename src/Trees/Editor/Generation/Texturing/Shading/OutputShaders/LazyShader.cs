using System;
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

        [SerializeField] private string _shaderName;
        [SerializeField] private Shader _shader;
        
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

        public static implicit operator Shader(LazyShader s) => s?.shader;
    }
}
