namespace Appalachia.Simulation.Trees.Generation.Texturing.Specifications
{
    public enum TextureMap
    {
        None = 0,

        /// <summary>
        ///     Albedo represents diffuse light color, and is completely void of directional light,
        ///     ambient occlusion and specular reflections.
        ///     The alpha channel represents the transparency of the surface.
        /// </summary>
        Albedo = 10,

        /// <summary>
        ///     Albedo detail represents fine grain differences in lighting color.
        /// </summary>
        AlbedoDetail = 12,

        /// <summary>
        ///     Represents where albedo details, normal bumps, or displacement bumps
        ///     should be applied.
        /// </summary>
        DetailMask = 19,

        /// <summary>
        ///     Tangent normals represent surface normal directions at each pixel.
        ///     Red, Green and Blue channels of the image represent the normal directions.
        /// </summary>
        Normal_TS = 20,

        /// <summary>
        ///     Normal Bump represents high frequency surface detail similar to a bump map.
        ///     Tangent normals represent surface normal directions at each pixel.
        ///     Also known as a detail normal map.
        /// </summary>
        NormalBump_TS = 25,

        /// <summary>
        ///     Object normals represent surface normal directions at each pixel with a gradient
        ///     representing the pixel's location in object space, otherwise known as local space.
        ///     Red, Green and Blue channels of the image represent the normal directions and
        ///     physical location of the pixel in the XYZ axes
        /// </summary>
        Normal_OS = 30,

        /// <summary>
        ///     Normal Bump represents high frequency surface detail similar to a bump map.
        ///     Object normals represent surface normal directions at each pixel with a gradient
        ///     representing the pixel's location in object space, otherwise known as local space.
        /// </summary>
        NormalBump_OS = 35,

        /// <summary>
        ///     Derivative Maps are a recent method of bump mapping that does not use tangent spaces.
        ///     This has numerous advantages over traditional normal map based workflows.
        /// </summary>
        Derivative = 40,

        /// <summary>
        ///     Displacement represents the encoded height differences of a high resolution model,
        ///     baked into a texture map.
        /// </summary>
        Displacement = 50,

        /// <summary>
        ///     Bump represents height differences in a surface.
        ///     Bump maps generally encode finer / higher frequency detail, and should be used to enhance
        ///     the look of high resolution models.
        /// </summary>
        Bump = 51,

        /// <summary>
        ///     AO represents diffuse light occlusion and is to be multiplied with the diffuse albedo
        ///     in the shader. This map comprises 100% real-world shadows and is fully separated
        ///     from the diffuse albedo.
        /// </summary>
        AmbientOcclusion = 60,

        /// <summary>
        ///     Cavity represent specular light occlusion and is used to weaken the specular reflection
        ///     in tightly occluded cavities and steep areas.
        /// </summary>
        Cavity = 61,

        /// <summary>
        ///     Metallicity represents how “metal-like” the surface is. When a surface is more metallic,
        ///     it reflects the environment more and its albedo colour becomes less visible.
        /// </summary>
        Metallicity = 70,

        /// <summary>
        ///     Specular represents base reflectivity (also called F0) when viewing the surface
        ///     head on, and is primarily used for real-time applications.
        /// </summary>
        Specularity = 80,

        /// <summary>
        ///     Gloss represents how smooth the surface is, and dictates the sharpness and intensity of
        ///     specular reflections. It is matched to a GGX BRDF.
        /// </summary>
        Glossiness = 90,

        /// <summary>
        ///     Roughness represents how rough the surface is, dictating the spread and intensity of
        ///     specular reflections, and is the inverse of Gloss/Smoothness. It is matched to a GGX BRDF.
        /// </summary>
        Roughness = 91,

        /// <summary>
        ///     Opacity represents the transparency of a surface.
        /// </summary>
        Opacity = 100,

        /// <summary>
        ///     Transmission represents the amount and intensity of light scattered through an object
        ///     when lit from behind.
        /// </summary>
        Transmission = 110,

        /// <summary>
        ///     Concavity represents how concave the surface is.
        /// </summary>
        Concavity = 120,

        /// <summary>
        ///     Convexity represents how convex the surface is.
        /// </summary>
        Convexity = 121,

        /// <summary>
        ///     Single channel curvature is a monochrome texture that contains the curvature data &
        ///     represents it using grayscale values.
        ///     Anything below 50% grey is concavity information.
        ///     Anything above 50% grey is convexity information.
        /// </summary>
        CurvatureSingleChannel = 122,

        /// <summary>
        ///     Dual channel curvature is an RGB texture that contains curvature data within the
        ///     red and green channels of the image.
        ///     The red channel contains concavity information.
        ///     The green channel contains convexity information.
        /// </summary>
        CurvatureDualChannel = 123,

        /// <summary>
        ///     A common optimized mapping.
        ///     The red channel contains metallicity information.
        ///     The green channel contains ambient occlusion information.
        ///     The blue channel contains displacement information.
        ///     The alpha channel contains glossiness information.
        /// </summary>
        MAOHS = 200,

        /// <summary>
        ///     A common optimized mapping.
        ///     The red channel contains tangent space normal X+ information.
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains ambient occlusion information.
        ///     The alpha channel contains glossiness information.
        /// </summary>
        NormSAO = 210,

        /// <summary>
        ///     A common optimized mapping.
        ///     The red channel contains glossiness information.
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains ambient occlusion information.
        ///     The alpha channel contains tangent space normal X+ information.
        /// </summary>
        NormSAOPacked = 211,

        /// <summary>
        ///     A common optimized mapping.
        ///     The RGB channels contain albedo information.
        ///     The alpha channel contains glossiness information.
        /// </summary>
        AlbedoGlossiness = 220,

        /// <summary>
        ///     An optimized mapping.
        ///     The RGB channels contain albedo information.
        ///     The alpha channel contains roughness information.
        /// </summary>
        AlbedoRoughness = 221,

        /// <summary>
        ///     A common optimized mapping.
        ///     The red channel contains albedo information.
        ///     The alpha channel contains glossiness information.
        /// </summary>
        MetallicityGlossiness = 230,

        /// <summary>
        ///     An optimized mapping.
        ///     The red channel contains albedo information.
        ///     The alpha channel contains roughness information.
        /// </summary>
        MetallicityRoughness = 231,

        /// <summary>
        ///     A common optimized mapping.
        ///     The RGB channels contain specularity information.
        ///     The alpha channel contains glossiness information.
        /// </summary>
        SpecularityGlossiness = 240,

        /// <summary>
        ///     An optimized mapping.
        ///     The RGB channels contain specularity information.
        ///     The alpha channel contains roughness information.
        /// </summary>
        SpecularityRoughness = 241,

        /// <summary>
        ///     An optimized mapping.
        ///     The RGB channels contain specularity information.
        ///     The alpha channel contains displacement information.
        /// </summary>
        SpecularityDisplacement = 242,

        /// <summary>
        ///     An optimized mapping.
        ///     The RGB channels contain albedo information.
        ///     The alpha channel contains displacement information.
        /// </summary>
        AlbedoDisplacement = 250,

        /// <summary>
        ///     An optimized mapping.
        ///     The RGB channels of the image represent the tangent space normal directions.
        ///     The alpha channel contains displacement information.
        /// </summary>
        NormalTSDisplacement = 260,

        /// <summary>
        ///     An optimized mapping.
        ///     The RGB channels of the image represent the object space normal directions.
        ///     The alpha channel contains displacement information.
        /// </summary>
        NormalOSDisplacement = 270,

        /// <summary>
        ///     An optimized mapping (CTI Tree Shaders).
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains specularity information.
        ///     The alpha channel contains tangent space normal X+ information.
        /// </summary>
        NormalTSSpecularityMap = 280,

        /// <summary>
        ///     An optimized mapping (CTI Tree Shaders).
        ///     The red channel contains specularity information.
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains ambient occlusion information.
        ///     The alpha channel contains tangent space normal X+ information.
        /// </summary>
        NormalTSSpecularityOcclusionMap = 281,

        /// <summary>
        ///     An optimized mapping.
        ///     The red channel contains metallicity information.
        ///     The green channel contains ambient occlusion information.
        ///     The blue channel contains translucency information.
        ///     The alpha channel contains glossiness information.
        /// </summary>
        MAOTS = 290,

        /// <summary>
        ///     Emission represents how much the surface glows or self-illuminates.
        ///     The RGB channels of the image represent the emission color and intensity.
        /// </summary>
        Emission = 300,

        /// <summary>
        ///     An optimized mapping.
        ///     The red channel contains tangent space normal X+ information.
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains tangent space normal Z+ information.
        ///     The alpha channel represents an albedo multiplier.
        /// </summary>
        DetailMap = 310,

        /// <summary>
        ///     An optimized mapping.
        ///     The red channel represents an albedo multiplier.
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains tangent space normal Z+ information.
        ///     The alpha channel contains tangent space normal X+ information.
        /// </summary>
        DetailMapPacked = 311,

        /// <summary>
        ///     An map used to augment shadow information. The red channel represents shadow casting strength..
        /// </summary>
        ShadowMap = 320,

        /// <summary>
        ///     A variant of the surface albedo information.
        /// </summary>
        Variant_Albedo = 400,

        /// <summary>
        ///     A variant of the surface albedo glossiness information.
        /// </summary>
        Variant_AlbedoGlossiness = 401,

        /// <summary>
        ///     A variant of the surface tangent-space normal information.
        /// </summary>
        Variant_Normal_TS = 410,

        /// <summary>
        ///     A variant of the surface displacement information.
        /// </summary>
        Variant_Displacement = 420,

        /// <summary>
        ///     A variant of the surface ambient occlusion information.
        /// </summary>
        Variant_AmbientOcclusion = 430,

        /// <summary>
        ///     A variant of the surface metallicity information.
        /// </summary>
        Variant_Metallicity = 440,

        /// <summary>
        ///     A variant of the surface glossiness information.
        /// </summary>
        Variant_Glossiness = 450,

        /// <summary>
        ///     A variant of the surface roughness information.
        /// </summary>
        Variant_Roughness = 460,

        /// <summary>
        ///     A variant of the surface opacity information.
        /// </summary>
        Variant_Opacity = 470,

        /// <summary>
        ///     A variant of the surface metallicity, ambient occlusion, displacement, and
        ///     glossiness information.
        /// </summary>
        Variant_MAOHS = 475,

        /// <summary>
        ///     A variant of the surface specularity (RGB).
        /// </summary>
        Variant_Specularity = 480,

        /// <summary>
        ///     A variant of the surface specularity (RGB) and smoothness (A).
        /// </summary>
        Variant_SpecularityGlossiness = 490,

        /// <summary>
        ///     An optimized mapping (CTI Tree Shaders).
        ///     The red channel contains specularity information.
        ///     The green channel contains tangent space normal Y+ information.
        ///     The blue channel contains ambient occlusion information.
        ///     The alpha channel contains tangent space normal X+ information.
        /// </summary>
        Variant_NormalTSSpecularityOcclusionMap = 491,

        /// <summary>
        ///     A user defined mapping.
        /// </summary>
        Custom = 500
    }
}
