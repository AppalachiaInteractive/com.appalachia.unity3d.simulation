using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class Internal_Plant_InputMaterialShader : IInputMaterialShader
    {
        #region Fields and Autoproperties

        private InputTextureProfile[] _plant =
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo,    "_AlbedoTex",  "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex",  "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOHS,     "_SurfaceTex", "_MetallicGlossMap"),
        };

        #endregion

        #region IInputMaterialShader Members

        public int Priority { get; } = 0;

        public bool AllowInsignificantPropertyNameDifferences { get; } = false;

        public bool CanProvideProfiles(Shader shader)
        {
            return shader.name.ToLower().Contains("appalachia/plant-individual");
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
            if (m.shader.name == "appalachia/plant-individual")
            {
                return _plant;
            }

            return null;
        }

        #endregion
    }
}
