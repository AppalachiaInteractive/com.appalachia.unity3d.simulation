using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations.Exceptions;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders;
using UnityEngine;
using UnityEngine.Rendering;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    public static class TextureExtractor
    {
        #region Static Fields and Autoproperties

        private static Dictionary<string, HashSet<string>> _shaderTexturePropertyNames =
            new Dictionary<string, HashSet<string>>();

        #endregion

        public static InputTextureSet GetInputTextureSet(Material m)
        {
            using (BUILD_TIME.TEX_EXTRC.GetInputTextureSet.Auto())
            {
                var inputShader = GetShader(m);

                if (_shaderTexturePropertyNames == null)
                {
                    _shaderTexturePropertyNames = new Dictionary<string, HashSet<string>>();
                }

                if (!_shaderTexturePropertyNames.ContainsKey(m.shader.name))
                {
                    _shaderTexturePropertyNames.Add(m.shader.name, new HashSet<string>());

                    for (var i = 0; i < m.shader.GetPropertyCount(); i++)
                    {
                        var propertyType = m.shader.GetPropertyType(i);

                        if (propertyType != ShaderPropertyType.Texture)
                        {
                            continue;
                        }

                        var propertyString = m.shader.GetPropertyName(i);
                        _shaderTexturePropertyNames[m.shader.name].Add(propertyString);
                    }
                }

                var propertyNames = _shaderTexturePropertyNames[m.shader.name];

                var set = new InputTextureSet();

                var profiles = inputShader.GetInputProfiles(m);

                foreach (var profile in profiles)
                {
                    var found = false;

                    foreach (var propertyName in profile.propertyNames)
                    {
                        var prop = propertyName;

                        if (propertyNames.Contains(prop))
                        {
                            var tex = m.GetTexture(prop) as Texture2D;

                            if (tex == null)
                            {
                                continue;
                            }

                            found = true;

                            var texture = new InputTexture { profile = profile, texture = tex };

                            set.inputTextures.Add(texture);

                            break;
                        }
                    }

                    if (!found)
                    {
                        if (inputShader.FailOnMissingTexture(profile.map))
                        {
                            throw new FailedToExtractTextureException(m, profile.map);
                        }
                    }
                }

                return set;
            }
        }

        public static IInputMaterialShader GetShader(Material m)
        {
            var shaders = TreeShaderFactory.GetInputMaterialShaders();

            foreach (var shader in shaders)
            {
                if (shader.CanProvideProfiles(m.shader))
                {
                    return shader;
                }
            }

            throw new FailedToFindAcceptableShaderException(m);
        }
    }
}
