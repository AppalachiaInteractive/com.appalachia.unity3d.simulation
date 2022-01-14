using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class BestEffort_InputMaterialShader : IInputMaterialShader
    {
        #region Fields and Autoproperties

        private InputTextureProfile[] _profiles =
        {
            InputTextureProfileFactory.Get(
                TextureMap.Albedo,
                "ALBEDO",
                "ALBEDOATLAS",
                "ALBEDOMAP",
                "ALBEDORGBOPACITYA",
                "ALBEDOTEX",
                "BASECOLOR",
                "BC",
                "BASECOLORMAP",
                "BASECOLORMAP0",
                "BASETEX",
                "BASETEXTURE",
                "DIFFUSE",
                "MAINSAMPLE",
                "MAINTEX",
                "MAINTEXTURE",
                "MAINTEX",
                "UTEXTURESETSALBEDO",
                "PLANTALBEDO",
                "RGBTEX",
                "SVBRDFDIFFUSECOLORMAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.AlbedoDisplacement,
                "ALBEDOH",
                "ALBEDOHEIGHT",
                "BCH",
                "BASECOLORHEIGHT"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.AmbientOcclusion,
                "AO",
                "AMBIENT",
                "AMBIENTOCCLUSION",
                "AMBIENTOCCLUSIONA",
                "AMBIENTOCCLUSIONG",
                "AMBIENTOCCLUSIONMAP",
                "UTEXTURESETSOCCLUSION",
                "OCCLUSION",
                "OCCLUSIONMAP",
                "OCLUSSION",
                "SSAOSHADOWMAP",
                "SHADOWOFFSET",
                "SHADOWTEX"
            ),
            InputTextureProfileFactory.Get(TextureMap.Cavity, "CAVITY", "CAVITYMAP", "AODETAIL"),
            InputTextureProfileFactory.Get(
                TextureMap.Concavity,
                "CONCAVITY",
                "CONCAVITYMAP",
                "CONCAVE",
                "CONCAVEMAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Convexity,
                "CONVEXITY",
                "CONVEXITYMAP",
                "CONVEX",
                "CONVEXMAP"
            ),
            InputTextureProfileFactory.Get(TextureMap.CurvatureDualChannel, "CURVE", "CURVATURE"),
            InputTextureProfileFactory.Get(
                TextureMap.Displacement,
                "DISPLACEMENT",
                "DISPLACEMENTTEX",
                "HEIGHT",
                "HEIGHTMAP",
                "HEIGHTMAP0",
                "PARALLAXMAP",
                "PARALLAX",
                "UHEIGHTMAP",
                "UTEXTURESETSDISPLACEMENT",
                "SVBRDFHEIGHTMAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Emission,
                "EMISION",
                "EMISSION",
                "EMISSIONMAP",
                "EMISSIVE",
                "EMISSIVITY",
                "EMISIVITY",
                "EMISSIVECOLORMAP",
                "GLOWTEX"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Glossiness,
                "GLOSS",
                "GLOSSMAP",
                "SMOOTHNESS",
                "GLOSSINESS"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.MAOHS,
                "MAOHS",
                "AMBIENTOCCLUSIONGSMOOTHNESSA",
                "AMBIENTOCCLUSIONGSMOTHNESSA",
                "METALICRAMBIENTOCCLUSIONGSMOOTHNESSA",
                "SURFACETEX",
                "METALICRAOGSMOTHNESSA",
                "METALICRAMBIENTOCCLUSIONGSMOOTHNESSA",
                "PLANTSURFACE",
                "RAOGEDGES",
                "MASKMAP",
                "MASKMAP0",
                "MASKTEX",
                "MASK",
                "MAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Metallicity,
                "METALLIC",
                "METALIC",
                "METALLICMAP",
                "METALICMAP",
                "METALLICTEX",
                "METALICTEX",
                "METALLICITY",
                "METALICITY"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.MetallicityGlossiness,
                "METALICSMOOTHNESS",
                "METALLICSMOOTHNESS",
                "METALLICITYSMOOTHNESS",
                "METALLICITYGLOSS",
                "METALLICITYGLOSSINESS",
                "METALLICGLOSS",
                "METALLICGLOSSMAP",
                "METALLICSMOOTH"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Normal_TS,
                "NORMAL",
                "NORMAL0",
                "NORMALATLAS",
                "NORMALMAP",
                "NORMALTEX",
                "NORMALTEXTURE",
                "NORMALS",
                "BASENORMAL",
                "BUMP",
                "BUMPMAP",
                "BUMPTEX",
                "BASEBUMP",
                "BMPMAP",
                "BENTNORMALMAP",
                "BENTNORMALMAP0",
                "TANGENTMAP",
                "UTEXTURESETSNORMAL",
                "PLANTNORMAL",
                "SVBRDFNORMALMAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Normal_OS,
                "BENTNORMALMAPOS",
                "BENTNORMALMAPOS0",
                "TANGENTMAPOS",
                "NORMALMAPOS"
            ),
            InputTextureProfileFactory.Get(TextureMap.NormSAOPacked, "NORMALSAO", "NORMSAO", "NORMSAO"),
            InputTextureProfileFactory.Get(
                TextureMap.Opacity,
                "OPACITYMASK",
                "OPACITYTEX",
                "TRANSPARENCYMAP",
                "SVBRDFALPHAMAP",
                "ALPHATEX",
                "ALPHATEXTURE"
            ),
            InputTextureProfileFactory.Get(TextureMap.Roughness, "ROUGHNESS", "ROUGH", "RGH"),
            InputTextureProfileFactory.Get(
                TextureMap.Specularity,
                "SPECMAP",
                "SPECULAR",
                "SPECULARCOLORMAP",
                "REFLECTIONTEX",
                "REFLECTIVECOLOR",
                "SVBRDFSPECULARCOLORMAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.SpecularityGlossiness,
                "UTEXTURESETSSPECSMOOTH",
                "SPECULARRGBSMOOTHNESA",
                "SPECULARRGBSMOOTHNESSA",
                "SPECULARRGBSMOTHNESSA",
                "SPECULARSMOOTHNESS",
                "SPECGLOSSMAP"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Transmission,
                "TRANSLUCENCYMAP",
                "TRANSMITTANCECOLORMAP",
                "TRANSMISSIONTHICKNESSMAP",
                "THICKNESSMAP",
                "THICKNESSMAP0",
                "SUBSURFACEMASKMAP",
                "SUBSURFACEMASKMAP0",
                "SUBSURFACETEX"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_Albedo,
                "ALBEDOTEX3",
                "MOSS",
                "COVERALBEDO",
                "COVERALBEDORGB",
                "COVERAGEALBEDO",
                "SNOWALBEDO",
                "SNOWALBEDORGB",
                "SNOWCOVER",
                "SNOWDIFF"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_Displacement,
                "COVERHEIGHTG",
                "SNOWHEIGHTG"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_Normal_TS,
                "NORMALTEX3",
                "COVERNORMAL",
                "COVERNORMALRGB",
                "COVERAGENORMAL",
                "MOSSNORMAL",
                "SNOWCOVERNORMAL",
                "SNOWNORMAL",
                "SNOWNORMALRGB"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_MAOHS,
                "SURFACETEX3",
                "COVERMETALICRAMBIENTOCCLUSIONGSMOTHNESSA",
                "COVERMETALICRAMBIENTOCCLUSIONGSMOOTHNESSA",
                "SNOWCOVERMETALICRAOGSMOTHNESSA",
                "SNOWMETALICRAOGSMOTHNESSA",
                "SNOWMETALICRAMBIENTOCCLUSIONGSMOTHNESSA"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_AmbientOcclusion,
                "SNOWAMBIENTOCCLUSIONG",
                "SNOWCOVERAMBIENTOCCLUSIONG"
            ),
            InputTextureProfileFactory.Get(TextureMap.Variant_Specularity, "SNOWSPECULAR"),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_SpecularityGlossiness,
                "SNOWCOVERSPECULARRGBSMOTHNESSA",
                "SNOWSPECULARRGBSMOTHNESSA",
                "SNOWSPECULARRGBSMOOTHNESSA"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.DetailMap,
                "MICRODETAIL",
                "DETAIL",
                "DETAILTEXDETAILMAP",
                "DETAILMAP0"
            ),
            InputTextureProfileFactory.Get(TextureMap.DetailMapPacked, "DETAILMAPALBEDORNYGNXA"),
            InputTextureProfileFactory.Get(
                TextureMap.AlbedoDetail,
                "DETAILALBEDO",
                "DETAILALBEDOMAP",
                "MASKYMIXALBEDO"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.DetailMask,
                "DETAILMASK",
                "DETAILMASKMAP",
                "MASKYMIXMASK"
            ),
            InputTextureProfileFactory.Get(
                TextureMap.Bump,
                "DETAILNORMAL",
                "DETAILNORMALMAP",
                "MASKYMIXBUMPMAP",
                "MICRODETAILNORMAL"
            )
        };

        #endregion

        #region IInputMaterialShader Members

        public int Priority { get; } = 100000;

        public bool AllowInsignificantPropertyNameDifferences { get; } = true;

        public bool CanProvideProfiles(Shader shader)
        {
            return true;
        }

        public bool FailOnMissingTexture(TextureMap texture)
        {
            switch (texture)
            {
                case TextureMap.Albedo:
                    return true;
            }

            return false;
        }

        public IEnumerable<InputTextureProfile> GetInputProfiles(Material m)
        {
            return _profiles;
        }

        #endregion
    }
}
