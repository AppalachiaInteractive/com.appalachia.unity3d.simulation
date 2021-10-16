#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public abstract class OutputMaterial : TreeMaterial, IEnumerable<OutputMaterialElement>
    {
        [SerializeField]
        [ListDrawerSettings(
            HideAddButton = true,
            //HideRemoveButton = true,
            DraggableItems = false,
            NumberOfItemsPerPage = 1,
            Expanded = true
        )]
        [PropertyOrder(100)]
        private List<OutputMaterialElement> _materials;

        protected List<OutputMaterialElement> materials
        {
            get
            {
                if (_materials == null)
                {
                    _materials = new List<OutputMaterialElement>();
                }

                if (_materials.Capacity < defaultShaders.Count)
                {
                    _materials.Capacity = (defaultShaders.Count);                    
                }

                for (var i = 0; i < _materials.Count; i++)
                {
                    if (_materials[i] == null)
                    {
                        _materials[i] = new OutputMaterialElement(defaultShaders.shaders[i]);
                    }
                }

                return _materials;
            }
        }

        protected abstract OutputShaderSelectionSet defaultShaders { get; }

        [PropertyOrder(0)]
        [InlineProperty, HideLabel]
        [HideIf(nameof(_hideTextures))]
        [SerializeField]
        protected OutputTextureSet _textureSet;

        protected bool _hideTextures =>
            (_textureSet == null) || (_textureSet.outputTextures == null) || (_textureSet.outputTextures.Count == 0);

        public OutputTextureSet textureSet => _textureSet;

        protected OutputMaterial(int materialID, ResponsiveSettingsType settingsType) : base(materialID, settingsType)
        {
            _materials = new List<OutputMaterialElement>();

            for (var i = 0; i < defaultShaders.Count; i++)
            {
                var newMat = new OutputMaterialElement(defaultShaders.shaders[i]);

                _materials.Add(newMat);
            }

            _textureSet = new OutputTextureSet();
        }

        public bool HasTexture(TextureMap map)
        {
            foreach (var texture in textureSet.outputTextures)
            {
                if (texture.profile.map == map)
                {
                    return true;
                }
            }

            return false;
        }

        public void EnsureCreated(int lodsRequired)
        {
            var elements = new OutputMaterialElement[lodsRequired];

            for (var i = 0; i < materials.Count; i++)
            {
                if (materials[i].asset == null)
                {
                    var atlas = (MaterialContext == MaterialContext.AtlasOutputMaterial) || (MaterialContext == MaterialContext.ShadowCaster);

                    materials[i].SetMaterial(new Material(materials[i].selectedShader.shader), atlas);
                }
            }

            for (var i = 0; i < lodsRequired; i++)
            {
                var shaderIndex = GetMaterialIndexForLOD(i);
                
                if (shaderIndex >= (materials.Count - 1))
                {
                    elements[i] = materials[shaderIndex];
                    continue;
                }

                var shader = materials[shaderIndex];

                elements[i] = shader;
            }

            foreach (var element in elements)
            {
                if (element.asset == null)
                {
                    element.SetMaterial(new Material(element.selectedShader.shader),
                        (MaterialContext == MaterialContext.AtlasOutputMaterial) ||
                        (MaterialContext == MaterialContext.BranchOutputMaterial));
                }
                else
                {
                    element.asset.shader = element.selectedShader.shader;
                }
            }
        }

        public IEnumerable<OutputTextureProfile> GetOutputTextureProfiles(bool atlas)
        {
            var profileHash = new HashSet<OutputTextureProfile>();

            foreach (var shader in materials)
            {
                profileHash.AddRange(shader.selectedShader.materialShader.GetOutputProfiles(atlas));
            }

            return profileHash;
        }

        public int Count => materials.Count;
        public OutputMaterialElement GetMaterialElementByIndex(int index)
        {
            return materials[index];
        }

        public OutputMaterialElement GetMaterialElementForLOD(int lod)
        {
            return materials[GetMaterialIndexForLOD(lod)];
        }
        
        public int GetMaterialIndexForLOD(int lod)
        {
            var shaderIndex = 0;
            var shaderIndexCount = 0;

            for (var i = 0; i <= lod; i++)
            {
                if (shaderIndex >= (materials.Count - 1))
                {
                    return shaderIndex;
                }

                var shader = materials[shaderIndex];

                if (shaderIndexCount >= shader.selectedShader.lodCoverage)
                {
                    shaderIndex += 1;
                    shaderIndexCount = 0;
                }

                if (i == lod)
                {
                    return shaderIndex;
                }
                shaderIndexCount += 1;
            }

            return shaderIndex;
        }


        public string TexturePrefix => materials[0].asset.name;

        public IEnumerator<OutputMaterialElement> GetEnumerator()
        {
            return materials.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void FinalizeMaterial()
        {
            using (BUILD_TIME.OUT_MAT.UpdateMaterial.Auto())
            {
                foreach (var element in materials)
                {
                    element.FinalizeMaterial(MaterialContext == MaterialContext.AtlasOutputMaterial);

                    foreach (var texture in textureSet.outputTextures)
                    {
                        texture.AssignToMaterial(element.asset);
                    }
                }
            }
        }

        public bool RequiresUpdate => (materials == null) || (materials.Count == 0) || materials.Any(m => m.asset == null);
    }
}
