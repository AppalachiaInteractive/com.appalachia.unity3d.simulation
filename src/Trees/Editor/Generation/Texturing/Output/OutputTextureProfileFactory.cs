using System;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Output
{
    public static class OutputTextureProfileFactory
    {
        public static OutputTextureProfile Albedo =>
            new OutputTextureProfile(TextureMap.Albedo, Colors.blackInvisible)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                alpha = TextureMapChannel.Opacity,
                fileNameSuffix = "_albedo-opacity",
                settings = OutputTextureSetting.RGBA_sRGB_Transparency(),
            };

        public static OutputTextureProfile AlbedoDetail =>
            new OutputTextureProfile(TextureMap.AlbedoDetail, Colors.white)
            {
                red = TextureMapChannel.AlbedoDetail_R,
                green = TextureMapChannel.AlbedoDetail_G,
                blue = TextureMapChannel.AlbedoDetail_B,
                alpha = TextureMapChannel.AlbedoDetail_Opacity,
                fileNameSuffix = "_albedo-detail",
                settings = OutputTextureSetting.RGBA_sRGB_Transparency(),
            };

        public static OutputTextureProfile AlbedoDisplacement =>
            new OutputTextureProfile(TextureMap.AlbedoDisplacement, Colors.whiteHalfVisible)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                alpha = TextureMapChannel.Displacement,
                fileNameSuffix = "_albedo-displacement",
                settings = OutputTextureSetting.RGBA_sRGB(),
            };

        public static OutputTextureProfile AlbedoGlossiness =>
            new OutputTextureProfile(TextureMap.AlbedoGlossiness, Colors.whiteInvisible)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                alpha = TextureMapChannel.Glossiness,
                fileNameSuffix = "_albedo-glossiness",
                settings = OutputTextureSetting.RGBA_sRGB_Transparency(),
            };

        public static OutputTextureProfile AlbedoRGB =>
            new OutputTextureProfile(TextureMap.Albedo, Colors.blackInvisible)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                fileNameSuffix = "_albedo",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile AlbedoRoughness =>
            new OutputTextureProfile(TextureMap.AlbedoRoughness, Colors.white)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                alpha = TextureMapChannel.Roughness,
                fileNameSuffix = "_albedo-roughness",
                settings = OutputTextureSetting.RGBA_sRGB(),
            };

        public static OutputTextureProfile AmbientOcclusion =>
            new OutputTextureProfile(TextureMap.AmbientOcclusion, Colors.lightgrey)
            {
                red = TextureMapChannel.AmbientOcclusion,
                green = TextureMapChannel.AmbientOcclusion,
                blue = TextureMapChannel.AmbientOcclusion,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_ambient-occlusion",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Bump =>
            new OutputTextureProfile(TextureMap.Bump, Colors.grey)
            {
                red = TextureMapChannel.Bump,
                green = TextureMapChannel.Bump,
                blue = TextureMapChannel.Bump,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_bump",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Cavity =>
            new OutputTextureProfile(TextureMap.Cavity, Colors.lightgrey)
            {
                red = TextureMapChannel.Cavity,
                green = TextureMapChannel.Cavity,
                blue = TextureMapChannel.Cavity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_cavity",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Concavity =>
            new OutputTextureProfile(TextureMap.Concavity, Colors.black)
            {
                red = TextureMapChannel.Concavity,
                green = TextureMapChannel.Concavity,
                blue = TextureMapChannel.Concavity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_concavity",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Convexity =>
            new OutputTextureProfile(TextureMap.Convexity, Colors.black)
            {
                red = TextureMapChannel.Convexity,
                green = TextureMapChannel.Convexity,
                blue = TextureMapChannel.Convexity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_convexity",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Custom =>
            new OutputTextureProfile(TextureMap.Custom, Colors.white)
            {
                fileNameSuffix = "_custom", settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile CutoutAlbedo =>
            new OutputTextureProfile(TextureMap.Albedo, Colors.blackInvisible)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                alpha = TextureMapChannel.Opacity,
                fileNameSuffix = "_albedo-opacity",
                settings = OutputTextureSetting.RGBA_sRGB_Transparency(),
            };

        public static OutputTextureProfile CutoutAlbedoRGB =>
            new OutputTextureProfile(TextureMap.Albedo, Colors.blackInvisible)
            {
                red = TextureMapChannel.Albedo_R,
                green = TextureMapChannel.Albedo_G,
                blue = TextureMapChannel.Albedo_B,
                fileNameSuffix = "_albedo",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile Derivative =>
            new OutputTextureProfile(TextureMap.Derivative, Colors.black)
            {
                red = TextureMapChannel.Derivative_X,
                green = TextureMapChannel.Derivative_Y,
                blue = TextureMapChannel.SetToZero,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_derivative",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile DetailMap =>
            new OutputTextureProfile(TextureMap.DetailMap, Colors.bumpHalfVisible)
            {
                red = TextureMapChannel.NormalBump_TS_X,
                green = TextureMapChannel.NormalBump_TS_Y,
                blue = TextureMapChannel.NormalBump_TS_Z,
                alpha = TextureMapChannel.AlbedoDetail_BW,
                fileNameSuffix = "_detail",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile DetailMapPacked =>
            new OutputTextureProfile(TextureMap.DetailMapPacked, Colors.bumpPackedHalfVisible)
            {
                red = TextureMapChannel.AlbedoDetail_BW,
                green = TextureMapChannel.NormalBump_TS_Y,
                blue = TextureMapChannel.NormalBump_TS_Z,
                alpha = TextureMapChannel.NormalBump_TS_X,
                fileNameSuffix = "_detail-pack",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile DetailMask =>
            new OutputTextureProfile(TextureMap.DetailMask, Colors.black)
            {
                red = TextureMapChannel.DetailMask,
                green = TextureMapChannel.DetailMask,
                blue = TextureMapChannel.DetailMask,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_detail-mask",
                settings = OutputTextureSetting.RGBA_sRGB_Transparency(),
            };

        public static OutputTextureProfile Displacement =>
            new OutputTextureProfile(TextureMap.Displacement, Colors.grey)
            {
                red = TextureMapChannel.Displacement,
                green = TextureMapChannel.Displacement,
                blue = TextureMapChannel.Displacement,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_displacement",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile DualChannelCurvature =>
            new OutputTextureProfile(TextureMap.CurvatureDualChannel, Colors.black)
            {
                red = TextureMapChannel.Concavity,
                green = TextureMapChannel.Convexity,
                blue = TextureMapChannel.SetToZero,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_curvature",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Emission =>
            new OutputTextureProfile(TextureMap.Emission, Colors.black)
            {
                red = TextureMapChannel.Emission_R,
                green = TextureMapChannel.Emission_G,
                blue = TextureMapChannel.Emission_B,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_emission",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile Glossiness =>
            new OutputTextureProfile(TextureMap.Glossiness, Colors.black)
            {
                red = TextureMapChannel.Glossiness,
                green = TextureMapChannel.Glossiness,
                blue = TextureMapChannel.Glossiness,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_glossiness",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile MAOHS =>
            new OutputTextureProfile(TextureMap.MAOHS, Colors.map)
            {
                red = TextureMapChannel.Metallicity,
                green = TextureMapChannel.AmbientOcclusion,
                blue = TextureMapChannel.Displacement,
                alpha = TextureMapChannel.Glossiness,
                fileNameSuffix = "_m-ao-h-s",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile MAOTS =>
            new OutputTextureProfile(TextureMap.MAOTS, Colors.map)
            {
                red = TextureMapChannel.Metallicity,
                green = TextureMapChannel.AmbientOcclusion,
                blue = TextureMapChannel.Transmission,
                alpha = TextureMapChannel.Glossiness,
                fileNameSuffix = "_m-ao-t-s",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile Metallicity =>
            new OutputTextureProfile(TextureMap.Metallicity, Colors.black)
            {
                red = TextureMapChannel.Metallicity,
                green = TextureMapChannel.Metallicity,
                blue = TextureMapChannel.Metallicity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_metallicity",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile MetallicityGlossiness =>
            new OutputTextureProfile(TextureMap.MetallicityGlossiness, Colors.blackInvisible)
            {
                red = TextureMapChannel.Metallicity,
                green = TextureMapChannel.Metallicity,
                blue = TextureMapChannel.Metallicity,
                alpha = TextureMapChannel.Glossiness,
                fileNameSuffix = "_metallicity-glossiness",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile MetallicityRoughness =>
            new OutputTextureProfile(TextureMap.MetallicityRoughness, Colors.black)
            {
                red = TextureMapChannel.Metallicity,
                green = TextureMapChannel.Metallicity,
                blue = TextureMapChannel.Metallicity,
                alpha = TextureMapChannel.Roughness,
                fileNameSuffix = "_metallicity-roughness",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile Normal_OS =>
            new OutputTextureProfile(TextureMap.Normal_OS, Colors.black)
            {
                red = TextureMapChannel.Normal_OS_X,
                green = TextureMapChannel.Normal_OS_Y,
                blue = TextureMapChannel.Normal_OS_Z,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_normalOS",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Normal_TS =>
            new OutputTextureProfile(TextureMap.Normal_TS, Colors.bump)
            {
                red = TextureMapChannel.Normal_TS_X,
                green = TextureMapChannel.Normal_TS_Y,
                blue = TextureMapChannel.Normal_TS_Z,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_normal",
                settings = OutputTextureSetting.RGB_Normal(),
            };

        public static OutputTextureProfile NormalBump_OS =>
            new OutputTextureProfile(TextureMap.NormalBump_OS, Colors.black)
            {
                red = TextureMapChannel.NormalBump_OS_X,
                green = TextureMapChannel.NormalBump_OS_Y,
                blue = TextureMapChannel.NormalBump_OS_Z,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_normalBumpOS",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile NormalBump_TS =>
            new OutputTextureProfile(TextureMap.NormalBump_TS, Colors.bump)
            {
                red = TextureMapChannel.NormalBump_TS_X,
                green = TextureMapChannel.NormalBump_TS_Y,
                blue = TextureMapChannel.NormalBump_TS_Z,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_normalBump",
                settings = OutputTextureSetting.RGB_Normal(),
            };

        public static OutputTextureProfile NormalOSDisplacement =>
            new OutputTextureProfile(TextureMap.NormalOSDisplacement, Colors.blackHalfVisible)
            {
                red = TextureMapChannel.Normal_OS_X,
                green = TextureMapChannel.Normal_OS_Y,
                blue = TextureMapChannel.Normal_OS_Z,
                alpha = TextureMapChannel.Displacement,
                fileNameSuffix = "_normalOS-displacement",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile NormalTSDisplacement =>
            new OutputTextureProfile(TextureMap.NormalTSDisplacement, Colors.bumpHalfVisible)
            {
                red = TextureMapChannel.Normal_TS_X,
                green = TextureMapChannel.Normal_TS_Y,
                blue = TextureMapChannel.Normal_TS_Z,
                alpha = TextureMapChannel.Displacement,
                fileNameSuffix = "_normal-displacement",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile NormalTSSpecularityMap =>
            new OutputTextureProfile(TextureMap.NormalTSSpecularityMap, Colors.bumpSpec)
            {
                red = TextureMapChannel.SetToZero,
                green = TextureMapChannel.Normal_TS_Y,
                blue = TextureMapChannel.Specularity_Strength,
                alpha = TextureMapChannel.Normal_TS_X,
                fileNameSuffix = "_bumpSpecMap",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile NormalTSSpecularityOcclusionMap =>
            new OutputTextureProfile(TextureMap.NormalTSSpecularityOcclusionMap, Colors.bumpSpecAO)
            {
                red = TextureMapChannel.Specularity_Strength,
                green = TextureMapChannel.Normal_TS_Y,
                blue = TextureMapChannel.AmbientOcclusion,
                alpha = TextureMapChannel.Normal_TS_X,
                fileNameSuffix = "_bumpSpecAOMap",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile NormSAO =>
            new OutputTextureProfile(TextureMap.NormSAO, Colors.normsao)
            {
                red = TextureMapChannel.Normal_TS_X,
                green = TextureMapChannel.Normal_TS_Y,
                blue = TextureMapChannel.AmbientOcclusion,
                alpha = TextureMapChannel.Glossiness,
                fileNameSuffix = "_norm-s-ao",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile NormSAOPacked =>
            new OutputTextureProfile(TextureMap.NormSAOPacked, Colors.normsao_packed)
            {
                red = TextureMapChannel.Glossiness,
                green = TextureMapChannel.Normal_TS_Y,
                blue = TextureMapChannel.AmbientOcclusion,
                alpha = TextureMapChannel.Normal_TS_X,
                fileNameSuffix = "_norm-s-ao-pack",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile Opacity =>
            new OutputTextureProfile(TextureMap.Opacity, Colors.white)
            {
                red = TextureMapChannel.Opacity,
                green = TextureMapChannel.Opacity,
                blue = TextureMapChannel.Opacity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_opacity",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Roughness =>
            new OutputTextureProfile(TextureMap.Roughness, Colors.white)
            {
                red = TextureMapChannel.Roughness,
                green = TextureMapChannel.Roughness,
                blue = TextureMapChannel.Roughness,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_roughness",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile ShadowMap =>
            new OutputTextureProfile(TextureMap.ShadowMap, Colors.white)
            {
                red = TextureMapChannel.Shadow,
                green = TextureMapChannel.Shadow,
                blue = TextureMapChannel.Shadow,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_shadow",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Specularity =>
            new OutputTextureProfile(TextureMap.Specularity, Colors.black)
            {
                red = TextureMapChannel.Specularity_R,
                green = TextureMapChannel.Specularity_G,
                blue = TextureMapChannel.Specularity_B,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_specularity",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile SpecularityDisplacement =>
            new OutputTextureProfile(TextureMap.SpecularityDisplacement, Colors.blackHalfVisible)
            {
                red = TextureMapChannel.Specularity_R,
                green = TextureMapChannel.Specularity_G,
                blue = TextureMapChannel.Specularity_B,
                alpha = TextureMapChannel.Displacement,
                fileNameSuffix = "_specularity-displacement",
                settings = OutputTextureSetting.RGBA_sRGB(),
            };

        public static OutputTextureProfile SpecularityGlossiness =>
            new OutputTextureProfile(TextureMap.SpecularityGlossiness, Colors.blackInvisible)
            {
                red = TextureMapChannel.Specularity_R,
                green = TextureMapChannel.Specularity_G,
                blue = TextureMapChannel.Specularity_B,
                alpha = TextureMapChannel.Glossiness,
                fileNameSuffix = "_specularity-glossiness",
                settings = OutputTextureSetting.RGBA_sRGB(),
            };

        public static OutputTextureProfile SpecularityRoughness =>
            new OutputTextureProfile(TextureMap.SpecularityRoughness, Colors.black)
            {
                red = TextureMapChannel.Specularity_R,
                green = TextureMapChannel.Specularity_G,
                blue = TextureMapChannel.Specularity_B,
                alpha = TextureMapChannel.Roughness,
                fileNameSuffix = "_specularity-roughness",
                settings = OutputTextureSetting.RGBA_sRGB(),
            };

        public static OutputTextureProfile Transmission =>
            new OutputTextureProfile(TextureMap.Transmission, Colors.white)
            {
                red = TextureMapChannel.Transmission,
                green = TextureMapChannel.Transmission,
                blue = TextureMapChannel.Transmission,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_transmission",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_Albedo =>
            new OutputTextureProfile(TextureMap.Variant_Albedo, Colors.white)
            {
                red = TextureMapChannel.Variant_Albedo_R,
                green = TextureMapChannel.Variant_Albedo_G,
                blue = TextureMapChannel.Variant_Albedo_B,
                alpha = TextureMapChannel.Variant_Opacity,
                fileNameSuffix = "_albedo-opacity-variant",
                settings = OutputTextureSetting.RGBA_sRGB_Transparency(),
            };

        public static OutputTextureProfile Variant_AlbedoGlossiness =>
            new OutputTextureProfile(TextureMap.Variant_AlbedoGlossiness, Colors.whiteInvisible)
            {
                red = TextureMapChannel.Variant_Albedo_R,
                green = TextureMapChannel.Variant_Albedo_G,
                blue = TextureMapChannel.Variant_Albedo_B,
                alpha = TextureMapChannel.Variant_Glossiness,
                fileNameSuffix = "_albedo-glossiness-variant",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile Variant_AmbientOcclusion =>
            new OutputTextureProfile(TextureMap.Variant_AmbientOcclusion, Colors.lightgrey)
            {
                red = TextureMapChannel.Variant_AmbientOcclusion,
                green = TextureMapChannel.Variant_AmbientOcclusion,
                blue = TextureMapChannel.Variant_AmbientOcclusion,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_ambient-occlusion-variant",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_Displacement =>
            new OutputTextureProfile(TextureMap.Variant_Displacement, Colors.grey)
            {
                red = TextureMapChannel.Variant_Displacement,
                green = TextureMapChannel.Variant_Displacement,
                blue = TextureMapChannel.Variant_Displacement,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_displacement-variant",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_Glossiness =>
            new OutputTextureProfile(TextureMap.Variant_Glossiness, Colors.blackInvisible)
            {
                red = TextureMapChannel.Variant_Glossiness,
                green = TextureMapChannel.Variant_Glossiness,
                blue = TextureMapChannel.Variant_Glossiness,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_glossiness-variant",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_MAOHS =>
            new OutputTextureProfile(TextureMap.Variant_MAOHS, Colors.map)
            {
                red = TextureMapChannel.Variant_Metallicity,
                green = TextureMapChannel.Variant_AmbientOcclusion,
                blue = TextureMapChannel.Variant_Displacement,
                alpha = TextureMapChannel.Variant_Glossiness,
                fileNameSuffix = "_m-ao-h-s-variant",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile Variant_Metallicity =>
            new OutputTextureProfile(TextureMap.Variant_Metallicity, Colors.black)
            {
                red = TextureMapChannel.Variant_Metallicity,
                green = TextureMapChannel.Variant_Metallicity,
                blue = TextureMapChannel.Variant_Metallicity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_metallicity-variant",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_Normal_TS =>
            new OutputTextureProfile(TextureMap.Variant_Normal_TS, Colors.bump)
            {
                red = TextureMapChannel.Variant_Normal_TS_X,
                green = TextureMapChannel.Variant_Normal_TS_Y,
                blue = TextureMapChannel.Variant_Normal_TS_Z,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_normal-variant",
                settings = OutputTextureSetting.RGB_Normal(),
            };

        public static OutputTextureProfile Variant_NormalTSSpecularityOcclusionMap =>
            new OutputTextureProfile(TextureMap.Variant_NormalTSSpecularityOcclusionMap, Colors.bumpSpecAO)
            {
                red = TextureMapChannel.Variant_Specularity_Strength,
                green = TextureMapChannel.Variant_Normal_TS_Y,
                blue = TextureMapChannel.Variant_AmbientOcclusion,
                alpha = TextureMapChannel.Variant_Normal_TS_X,
                fileNameSuffix = "_bumpSpecAOMap-variant",
                settings = OutputTextureSetting.RGBA_Linear(),
            };

        public static OutputTextureProfile Variant_Opacity =>
            new OutputTextureProfile(TextureMap.Variant_Opacity, Colors.white)
            {
                red = TextureMapChannel.Variant_Opacity,
                green = TextureMapChannel.Variant_Opacity,
                blue = TextureMapChannel.Variant_Opacity,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_opacity-variant",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_Roughness =>
            new OutputTextureProfile(TextureMap.Variant_Roughness, Colors.white)
            {
                red = TextureMapChannel.Variant_Roughness,
                green = TextureMapChannel.Variant_Roughness,
                blue = TextureMapChannel.Variant_Roughness,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_roughness-variant",
                settings = OutputTextureSetting.RGB_Linear(),
            };

        public static OutputTextureProfile Variant_Specularity =>
            new OutputTextureProfile(TextureMap.Variant_Specularity, Colors.black)
            {
                red = TextureMapChannel.Variant_Specularity_R,
                green = TextureMapChannel.Variant_Specularity_G,
                blue = TextureMapChannel.Variant_Specularity_B,
                alpha = TextureMapChannel.SetToOne,
                fileNameSuffix = "_specularity-variant",
                settings = OutputTextureSetting.RGB_sRGB(),
            };

        public static OutputTextureProfile Variant_SpecularityGlossiness =>
            new OutputTextureProfile(TextureMap.Variant_SpecularityGlossiness, Colors.blackInvisible)
            {
                red = TextureMapChannel.Variant_Specularity_R,
                green = TextureMapChannel.Variant_Specularity_G,
                blue = TextureMapChannel.Variant_Specularity_B,
                alpha = TextureMapChannel.Variant_Glossiness,
                fileNameSuffix = "_specularity-glossiness-variant",
                settings = OutputTextureSetting.RGBA_sRGB(),
            };

        public static OutputTextureProfile Get(TextureMap map, string propertyName, bool atlas)
        {
            OutputTextureProfile p;

            switch (map)
            {
                case TextureMap.Albedo:
                    p = Albedo;
                    break;
                case TextureMap.AlbedoDetail:
                    p = AlbedoDetail;
                    break;
                case TextureMap.DetailMask:
                    p = DetailMask;
                    break;
                case TextureMap.Normal_TS:
                    p = Normal_TS;
                    break;
                case TextureMap.NormalBump_TS:
                    p = NormalBump_TS;
                    break;
                case TextureMap.Normal_OS:
                    p = Normal_OS;
                    break;
                case TextureMap.NormalBump_OS:
                    p = NormalBump_OS;
                    break;
                case TextureMap.Derivative:
                    p = Derivative;
                    break;
                case TextureMap.Displacement:
                    p = Displacement;
                    break;
                case TextureMap.Bump:
                    p = Bump;
                    break;
                case TextureMap.AmbientOcclusion:
                    p = AmbientOcclusion;
                    break;
                case TextureMap.Cavity:
                    p = Cavity;
                    break;
                case TextureMap.Metallicity:
                    p = Metallicity;
                    break;
                case TextureMap.Specularity:
                    p = Specularity;
                    break;
                case TextureMap.Glossiness:
                    p = Glossiness;
                    break;
                case TextureMap.Roughness:
                    p = Roughness;
                    break;
                case TextureMap.Opacity:
                    p = Opacity;
                    break;
                case TextureMap.Transmission:
                    p = Transmission;
                    break;
                case TextureMap.Concavity:
                    p = Concavity;
                    break;
                case TextureMap.Convexity:
                    p = Convexity;
                    break;
                case TextureMap.CurvatureDualChannel:
                    p = DualChannelCurvature;
                    break;
                case TextureMap.MAOHS:
                    p = MAOHS;
                    break;
                case TextureMap.NormSAO:
                    p = NormSAO;
                    break;
                case TextureMap.NormSAOPacked:
                    p = NormSAOPacked;
                    break;
                case TextureMap.AlbedoGlossiness:
                    p = AlbedoGlossiness;
                    break;
                case TextureMap.AlbedoRoughness:
                    p = AlbedoRoughness;
                    break;
                case TextureMap.MetallicityGlossiness:
                    p = MetallicityGlossiness;
                    break;
                case TextureMap.MetallicityRoughness:
                    p = MetallicityRoughness;
                    break;
                case TextureMap.SpecularityGlossiness:
                    p = SpecularityGlossiness;
                    break;
                case TextureMap.SpecularityRoughness:
                    p = SpecularityRoughness;
                    break;
                case TextureMap.SpecularityDisplacement:
                    p = SpecularityDisplacement;
                    break;
                case TextureMap.AlbedoDisplacement:
                    p = AlbedoDisplacement;
                    break;
                case TextureMap.NormalTSDisplacement:
                    p = NormalTSDisplacement;
                    break;
                case TextureMap.NormalOSDisplacement:
                    p = NormalOSDisplacement;
                    break;
                case TextureMap.Emission:
                    p = Emission;
                    break;
                case TextureMap.DetailMap:
                    p = DetailMap;
                    break;
                case TextureMap.DetailMapPacked:
                    p = DetailMapPacked;
                    break;
                case TextureMap.Custom:
                    p = Custom;
                    break;
                case TextureMap.Variant_Albedo:
                    p = Variant_Albedo;
                    break;
                case TextureMap.Variant_Normal_TS:
                    p = Variant_Normal_TS;
                    break;
                case TextureMap.Variant_Displacement:
                    p = Variant_Displacement;
                    break;
                case TextureMap.Variant_AmbientOcclusion:
                    p = Variant_AmbientOcclusion;
                    break;
                case TextureMap.Variant_Metallicity:
                    p = Variant_Metallicity;
                    break;
                case TextureMap.Variant_Specularity:
                    p = Variant_Specularity;
                    break;
                case TextureMap.Variant_SpecularityGlossiness:
                    p = Variant_SpecularityGlossiness;
                    break;
                case TextureMap.Variant_Glossiness:
                    p = Variant_Glossiness;
                    break;
                case TextureMap.Variant_Roughness:
                    p = Variant_Roughness;
                    break;
                case TextureMap.Variant_Opacity:
                    p = Variant_Opacity;
                    break;
                case TextureMap.NormalTSSpecularityMap:
                    p = NormalTSSpecularityMap;
                    break;
                case TextureMap.NormalTSSpecularityOcclusionMap:
                    p = NormalTSSpecularityOcclusionMap;
                    break;
                case TextureMap.MAOTS:
                    p = MAOTS;
                    break;
                case TextureMap.ShadowMap:
                    p = ShadowMap;
                    break;
                case TextureMap.Variant_AlbedoGlossiness:
                    p = Variant_AlbedoGlossiness;
                    break;
                case TextureMap.Variant_MAOHS:
                    p = Variant_MAOHS;
                    break;
                case TextureMap.Variant_NormalTSSpecularityOcclusionMap:
                    p = Variant_NormalTSSpecularityOcclusionMap;
                    break;
                case TextureMap.None:
                case TextureMap.CurvatureSingleChannel:
                default:
                    throw new ArgumentOutOfRangeException(nameof(map), map, null);
            }

            p.propertyName = propertyName;

            if (atlas)
            {
                p.settings.Clamp();
            }
            else
            {
                p.settings.Repeat();
            }

            return p;
        }

        #region Nested type: Colors

        private static class Colors
        {
            public static Color black => Color.black;
            public static Color blackHalfVisible => new Color(0f,       0f,  0f,   .5f);
            public static Color blackInvisible => new Color(0f,         0f,  0f,   .03f);
            public static Color bump => new Color(.5f,                  .5f, 1f,   1f);
            public static Color bumpHalfVisible => new Color(.5f,       .5f, 1f,   .5f);
            public static Color bumpPackedHalfVisible => new Color(.5f, .5f, 1f,   .5f);
            public static Color bumpSpec => new Color(0.05f,            .5f, .05f, .5f);
            public static Color bumpSpecAO => new Color(0.05f,          .5f, 1.0f, .5f);
            public static Color grey => Color.grey;
            public static Color lightgrey => new Color(.7f,        .7f, .7f, 1f);
            public static Color map => new Color(0f,               .7f, .5f, .05f);
            public static Color mask => new Color(0f,              .0f, 0f,  0f);
            public static Color normsao => new Color(.5f,          .5f, .7f, 0.03f);
            public static Color normsao_packed => new Color(0.03f, .5f, .7f, .5f);
            public static Color white => Color.white;
            public static Color whiteHalfVisible => new Color(1f, 1f, 1f, .5f);
            public static Color whiteInvisible => new Color(1f,   1f, 1f, .03f);
        }

        #endregion
    }
}
