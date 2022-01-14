using System;
using System.Linq;
using Appalachia.Simulation.Trees.Build.Execution;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    [Serializable]
    public sealed class OutputShaderSelection
    {
        public delegate void shaderEventDelegate();

        public event shaderEventDelegate OnShaderChanged;

        public OutputShaderSelection(IOutputMaterialShader materialShader)
        {
            _shaderKey = materialShader.Name;
            _materialShader = materialShader;
            _shader = materialShader.Shader;
        }

        #region Fields and Autoproperties

        [PropertyRange(1, 5)] public int lodCoverage = 1;
        private IOutputMaterialShader _materialShader;
        private OutputMaterialShaderSelector _selector;
        [SerializeField] private Shader _shader;

        [SerializeField, HideInInspector]
        private string _shaderKey;

        #endregion

        public IOutputMaterialShader materialShader
        {
            get
            {
                if (_materialShader == null)
                {
                    if (_shader != null)
                    {
                        _shaderKey = _shader.name;
                        _materialShader = TreeShaderFactory.GetByName(_shaderKey);
                    }
                    else
                    {
                        _materialShader = TreeShaderFactory.GetByName(_shaderKey);
                    }
                }

                return _materialShader;
            }
        }

        public Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = materialShader.Shader;
                }

                return _shader;
            }
        }

        public string shaderKey => _shaderKey;

        [Button]
        private void Change()
        {
            using (BUILD_TIME.OUT_MAT.Change.Auto())
            {
                if (_selector == null)
                {
                    _selector = new OutputMaterialShaderSelector();
                    _selector.SelectionConfirmed += ms =>
                    {
                        using (BUILD_TIME.OUT_MAT.UpdateShader.Auto())
                        {
                            var m = ms.FirstOrDefault();

                            _materialShader = m;
                            _shader = _materialShader?.Shader;
                            _shaderKey = _materialShader?.Name;
                        }

                        OnShaderChanged?.Invoke();
                    };
                }

                _selector.Show(_materialShader);
            }
        }
    }
}
