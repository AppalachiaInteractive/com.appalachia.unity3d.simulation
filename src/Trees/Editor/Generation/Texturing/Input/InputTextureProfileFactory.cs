using System;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Input
{
    public static class InputTextureProfileFactory
    {
        public static InputTextureProfile Get(TextureMap map, params string[] propertyNames)
        {
            InputTextureProfile p;
            
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
                case TextureMap.CurvatureSingleChannel:
                    p = SingleChannelCurvature;
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
                case TextureMap.Variant_AlbedoGlossiness:
                    p = Variant_AlbedoGlossiness;
                    break;
                case TextureMap.Variant_NormalTSSpecularityOcclusionMap:
                    p = Variant_NormalTSSpecularityOcclusionMap;
                    break;
                case TextureMap.Variant_MAOHS:
                    p = Variant_MAOHS;
                    break;
                case TextureMap.ShadowMap:
                    p = ShadowMap;
                    break;
                case TextureMap.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(map), map, null);
            }

            p.propertyNames = propertyNames;
            return p;
        }

        public static InputTextureProfile AlbedoRGB =>
            new InputTextureProfile(TextureMap.Albedo)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_B
                }
            };
        
        public static InputTextureProfile Albedo =>
            new InputTextureProfile(TextureMap.Albedo)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Opacity
                }
            };
        
        public static InputTextureProfile AlbedoDetail =>
            new InputTextureProfile(TextureMap.AlbedoDetail)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AlbedoDetail_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AlbedoDetail_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AlbedoDetail_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AlbedoDetail_Opacity
                }
            };
        
        
        public static InputTextureProfile DetailMask =>
            new InputTextureProfile(TextureMap.DetailMask)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.DetailMask
                }
            };

        public static InputTextureProfile Normal_TS =>
            new InputTextureProfile(TextureMap.Normal_TS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Z
                }
            };
        
        public static InputTextureProfile NormalBump_TS =>
            new InputTextureProfile(TextureMap.NormalBump_TS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_Z
                }
            };
        
        public static InputTextureProfile Normal_OS =>
            new InputTextureProfile(TextureMap.Normal_OS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_Z
                }
            };

        public static InputTextureProfile NormalBump_OS =>
            new InputTextureProfile(TextureMap.NormalBump_OS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_OS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_OS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_OS_Z
                }
            };

        public static InputTextureProfile Derivative =>
            new InputTextureProfile(TextureMap.Derivative)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Derivative_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Derivative_Y
                }
            };

        public static InputTextureProfile Displacement =>
            new InputTextureProfile(TextureMap.Displacement)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                }
            };
        
        public static InputTextureProfile Bump =>
            new InputTextureProfile(TextureMap.Bump)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Bump
                }
            };
        
        public static InputTextureProfile AmbientOcclusion =>
            new InputTextureProfile(TextureMap.AmbientOcclusion)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AmbientOcclusion
                }
            };
        
        public static InputTextureProfile Cavity =>
            new InputTextureProfile(TextureMap.Cavity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Cavity
                }
            };
        
        public static InputTextureProfile Metallicity =>
            new InputTextureProfile(TextureMap.Metallicity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Metallicity
                }
            };
        
        public static InputTextureProfile Specularity =>
            new InputTextureProfile(TextureMap.Specularity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_B
                },
            };
        
        public static InputTextureProfile Glossiness =>
            new InputTextureProfile(TextureMap.Glossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };
        
        public static InputTextureProfile Roughness =>
            new InputTextureProfile(TextureMap.Roughness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Roughness
                }
            };
        
        public static InputTextureProfile Opacity =>
            new InputTextureProfile(TextureMap.Opacity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Opacity
                }
            };
        
        public static InputTextureProfile Transmission =>
            new InputTextureProfile(TextureMap.Transmission)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Transmission
                }
            };
        
        public static InputTextureProfile Concavity =>
            new InputTextureProfile(TextureMap.Concavity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Concavity
                }
            };
        
        public static InputTextureProfile Convexity =>
            new InputTextureProfile(TextureMap.Convexity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Convexity
                }
            };

        
        public static InputTextureProfile SingleChannelCurvature =>
            new InputTextureProfile(TextureMap.CurvatureSingleChannel)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Concavity,
                    packing = TextureChannelPacking.PackedInLowerHalf
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Convexity,
                    packing = TextureChannelPacking.PackedInTopHalf
                }
            };
        
        public static InputTextureProfile DualChannelCurvature =>
            new InputTextureProfile(TextureMap.CurvatureDualChannel)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Concavity,
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Convexity
                }
            };

        public static InputTextureProfile NormSAO =>
            new InputTextureProfile(TextureMap.NormSAO)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AmbientOcclusion
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };
        
        public static InputTextureProfile NormSAOPacked =>
            new InputTextureProfile(TextureMap.NormSAOPacked)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AmbientOcclusion
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_X
                }
            };
        
        public static InputTextureProfile MAOHS =>
            new InputTextureProfile(TextureMap.MAOHS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Metallicity
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AmbientOcclusion
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };
        
        public static InputTextureProfile AlbedoGlossiness =>
            new InputTextureProfile(TextureMap.AlbedoGlossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };
        
        public static InputTextureProfile AlbedoRoughness =>
            new InputTextureProfile(TextureMap.AlbedoRoughness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Roughness
                }
            };
        public static InputTextureProfile MetallicityGlossiness =>
            new InputTextureProfile(TextureMap.MetallicityGlossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Metallicity
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };
        
        public static InputTextureProfile MetallicityRoughness =>
            new InputTextureProfile(TextureMap.MetallicityRoughness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Metallicity
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Roughness
                }
            };
        public static InputTextureProfile SpecularityGlossiness =>
            new InputTextureProfile(TextureMap.SpecularityGlossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };
        
        public static InputTextureProfile SpecularityRoughness =>
            new InputTextureProfile(TextureMap.SpecularityRoughness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Roughness
                }
            };
        
        public static InputTextureProfile SpecularityDisplacement =>
            new InputTextureProfile(TextureMap.SpecularityDisplacement)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                }
            };

        public static InputTextureProfile AlbedoDisplacement =>
            new InputTextureProfile(TextureMap.AlbedoDisplacement)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Albedo_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                }
            };
            
        public static InputTextureProfile NormalTSDisplacement =>
            new InputTextureProfile(TextureMap.NormalTSDisplacement)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Z
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                }
            };
            
        public static InputTextureProfile NormalOSDisplacement =>
            new InputTextureProfile(TextureMap.NormalOSDisplacement)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_Z
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                }
            };
        
        public static InputTextureProfile Emission =>
            new InputTextureProfile(TextureMap.Emission)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_OS_Z
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Displacement
                }
            };
        
        public static InputTextureProfile DetailMap =>
            new InputTextureProfile(TextureMap.DetailMap)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_Z
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AlbedoDetail_BW
                }
            };
        
        public static InputTextureProfile DetailMapPacked =>
            new InputTextureProfile(TextureMap.DetailMapPacked)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AlbedoDetail_BW
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_Z
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.NormalBump_TS_X
                }
            };
        
        public static InputTextureProfile Custom =>
            new InputTextureProfile(TextureMap.Custom)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.None
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.None
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.None
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.None
                }
            };
        
          public static InputTextureProfile Variant_Albedo =>
            new InputTextureProfile(TextureMap.Variant_Albedo)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Albedo_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Opacity
                }
            };
          
        public static InputTextureProfile Variant_Normal_TS =>
            new InputTextureProfile(TextureMap.Variant_Normal_TS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Normal_TS_X
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Normal_TS_Z
                }
            };


        public static InputTextureProfile Variant_Displacement =>
            new InputTextureProfile(TextureMap.Variant_Displacement)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Displacement
                }
            };

        
        public static InputTextureProfile Variant_AmbientOcclusion =>
            new InputTextureProfile(TextureMap.Variant_AmbientOcclusion)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_AmbientOcclusion
                }
            };

        
        public static InputTextureProfile Variant_Metallicity =>
            new InputTextureProfile(TextureMap.Variant_Metallicity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Metallicity
                }
            };
        
        
        public static InputTextureProfile Variant_Specularity =>
            new InputTextureProfile(TextureMap.Variant_Specularity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_B
                },
            };
        
        public static InputTextureProfile Variant_SpecularityGlossiness =>
            new InputTextureProfile(TextureMap.Variant_SpecularityGlossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Glossiness
                }
            };

        public static InputTextureProfile Variant_Glossiness =>
            new InputTextureProfile(TextureMap.Variant_Glossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Glossiness
                }
            };
        
        public static InputTextureProfile Variant_Roughness =>
            new InputTextureProfile(TextureMap.Variant_Roughness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Roughness
                }
            };
        
        public static InputTextureProfile Variant_Opacity =>
            new InputTextureProfile(TextureMap.Variant_Opacity)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Opacity
                }
            };
        
        public static InputTextureProfile Variant_MAOHS =>
            new InputTextureProfile(TextureMap.Variant_MAOHS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Metallicity
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_AmbientOcclusion
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Displacement
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Glossiness
                }
            };

        public static InputTextureProfile ShadowMap =>
            new InputTextureProfile(TextureMap.ShadowMap)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Shadow
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.SetToZero
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.SetToZero
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.SetToOne
                }
            };

        public static InputTextureProfile NormalTSSpecularityMap =>
            new InputTextureProfile(TextureMap.NormalTSSpecularityMap)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.SetToZero
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_Strength
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_X
                }
            };       
        
        public static InputTextureProfile NormalTSSpecularityOcclusionMap =>
            new InputTextureProfile(TextureMap.NormalTSSpecularityOcclusionMap)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Specularity_Strength
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AmbientOcclusion
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Normal_TS_X
                }
            };       
        
        public static InputTextureProfile MAOTS =>
            new InputTextureProfile(TextureMap.MAOTS)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Metallicity
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.AmbientOcclusion
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Transmission
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Glossiness
                }
            };        
        
        public static InputTextureProfile Variant_AlbedoGlossiness =>
            new InputTextureProfile(TextureMap.Variant_AlbedoGlossiness)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Albedo_R
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Albedo_G
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Albedo_B
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Glossiness
                }
            };

        public static InputTextureProfile Variant_NormalTSSpecularityOcclusionMap =>
            new InputTextureProfile(TextureMap.Variant_NormalTSSpecularityOcclusionMap)
            {
                red = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Specularity_Strength
                },
                green = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Normal_TS_Y
                },
                blue = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_AmbientOcclusion
                },
                alpha = new InputTextureProfileChannel()
                {
                    mapChannel = TextureMapChannel.Variant_Normal_TS_X
                }
            };
    }
}
