/*
using System;
using System.Linq;
using Appalachia.Simulation.Trees.Metadata.Build;
using Appalachia.Simulation.Trees.Metadata.ResponsiveUI;
using Appalachia.Simulation.Trees.Metadata.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Metadata.Texturing.Output;
using Appalachia.Simulation.Trees.Metadata.Texturing.Shading;
using Appalachia.Simulation.Trees.Metadata.Texturing.Shading.OutputShaders;
using Appalachia.Simulation.Trees.Runtime.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Metadata.Texturing.Materials.Output
{
    [Serializable]
    public abstract class DynamicShaderOutputMaterial : OutputMaterial
    {
        [SerializeField, HideInInspector] private string _outputMaterialKey;
        
        private IOutputMaterialShader _materialShader;

        public IOutputMaterialShader materialShader
        {
            get
            {
                if (_materialShader == null && !string.IsNullOrEmpty(_outputMaterialKey))
                {
                    _materialShader = TreeShaderFactory.GetByName(_outputMaterialKey);
                }
                
                if (_materialShader == null && _shaders != null)
                {
                    _materialShader = TreeShaderFactory.GetByName(_shaders.name);
                }

                _outputMaterialKey = _materialShader?.Name;
                return _materialShader;
            }
        }

        private OutputMaterialShaderSelector _selector;
        
        [VerticalGroup("MAT/RIGHT"), PropertyOrder(-499), LabelWidth(110)]
        public Material prototypeMaterial;

        protected DynamicShaderOutputMaterial(int materialID, ResponsiveSettingsType settingsType) : base(materialID, settingsType)
        {
            _textureSet = new OutputTextureSet();
        }

        protected abstract Shader defaultShader { get; }

        public override Shader GetShader(int lod)
        {
            if (_shaders == null)
            {
                if (materialShader == null)
                {
                    _shaders = defaultShader;
                }
                else
                {
                    _shaders = materialShader.Shader;
                }
            }

            return _shaders;
        }

        protected override Material CreateMaterialInternal(int lod)
        {
            using (BUILD_TIME.OUT_MAT.CreateMaterialInternal.Auto())
            {
                if (GetShader(lod) == null)
                {
                    if (materialShader == null)
                    {
                        throw new NotSupportedException("Need to set material shader!");
                    }

                    _shaders = materialShader.Shader;
                }

                if (GetShader(lod) == null)
                {
                    throw new NotSupportedException("Need to set shader!");
                }

                if (_material == null)
                {
                    _material = new Material(GetShader(TODO));
                }

                material.shader = GetShader(TODO);

                return _material;
            }
        }

        public void UpdateShader(IOutputMaterialShader s)
        {
            using (BUILD_TIME.OUT_MAT.UpdateShader.Auto())
            {
                _materialShader = s;
                _shaders = _materialShader?.Shader;
                _outputMaterialKey = _materialShader?.Name;
            }
        }
        
        public override void FinalizeMaterial()
        {
            using (BUILD_TIME.OUT_MAT.UpdateMaterial.Auto())
            {
                material.shader = GetShader(TODO);
                
                foreach (var texture in textureSet.outputTextures)
                {
                    texture.AssignToMaterial(material);
                }

                materialShader.FinalizeSettings(material, MaterialContext == MaterialContext.AtlasOutputMaterial);
            }
        }
        

        [HorizontalGroup("A", Order = -101)]
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
                        UpdateShader(ms.FirstOrDefault());
                        FinalizeMaterial();
                    };
                }

                _selector.Show(materialShader);
            }
        }
    }
}
*/
