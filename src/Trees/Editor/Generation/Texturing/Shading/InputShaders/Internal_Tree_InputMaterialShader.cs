using System.Collections.Generic;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class Internal_Tree_InputMaterialShader : IInputMaterialShader
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static Internal_Tree_InputMaterialShader()
        {
            GSR.InstanceAvailable += i => _GSR = i;
        }

        #region Static Fields and Autoproperties

        private static GSR _GSR;

        #endregion

        #region Fields and Autoproperties

        private InputTextureProfile[] _bark =
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo,            "_AlbedoTex",  "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS,         "_NormalTex",  "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOHS,             "_SurfaceTex", "_MetallicGlossMap"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Albedo,    "_AlbedoTex3", "_MainTex3"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Normal_TS, "_NormalTex3", "_BumpMap3"),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_MAOHS,
                "_SurfaceTex3",
                "_MetallicGlossMap3"
            ),
        };

        private InputTextureProfile[] _leaf =
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo,    "_AlbedoTex",  "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex",  "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOTS,     "_SurfaceTex", "_MetallicGlossMap"),
        };

        #endregion

        #region IInputMaterialShader Members

        public int Priority { get; } = 0;

        public bool AllowInsignificantPropertyNameDifferences { get; } = false;

        public bool CanProvideProfiles(Shader shader)
        {
            return _GSR.barkShaders.Contains(shader) || _GSR.leafShaders.Contains(shader);
        }

        public bool FailOnMissingTexture(TextureMap texture)
        {
            switch (texture)
            {
                case TextureMap.Albedo:
                case TextureMap.Normal_TS:
                    return true;
            }

            return false;
        }

        public IEnumerable<InputTextureProfile> GetInputProfiles(Material m)
        {
            if (_GSR.barkShaders.Contains(m.shader))
            {
                return _bark;
            }

            return _leaf;
        }

        #endregion
    }
}
