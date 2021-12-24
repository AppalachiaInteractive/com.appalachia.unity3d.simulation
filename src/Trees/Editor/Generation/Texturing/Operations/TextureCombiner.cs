#region

using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Texturing.Atlassing;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Utility.Strings;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    [CallStaticConstructorInEditor]
    public static class TextureCombiner
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static TextureCombiner()
        {
            GSR.InstanceAvailable += i => _GSR = i;
        }

        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        private static GSR _GSR;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(TextureCombiner));
                }

                return _context;
            }
        }

        public static List<OutputTexture> DrawAtlasTextures(
            IReadOnlyList<AtlasInputMaterial> inputMaterials,
            AtlasOutputMaterial output,
            Vector2 textureSize,
            bool debugCombinationOutputs)
        {
            using (BUILD_TIME.TEX_COMBR.DrawAtlasTextures.Auto())
            {
                if ((inputMaterials.Count == 0) || inputMaterials.All(m => m.textures.count == 0))
                {
                    return null;
                }

                try
                {
                    var atlas = new TextureAtlas();

                    foreach (var inputMaterial in inputMaterials)
                    {
                        var nodeSize = inputMaterial.textures.size;
                        var node = new TextureAtlasNode(nodeSize, textureSize)
                        {
                            materialID = inputMaterial.materialID
                        };

                        atlas.nodes.Add(node);
                    }

                    //atlas.Pack(textureSize);
                    atlas.Pack((int)textureSize.x, (int)textureSize.y);
                    atlas.PopulateAtlasDataToMaterials(inputMaterials);
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ex);
                    throw;
                }

                var textures = DrawTextures(
                    textureSize,
                    output,
                    output.GetOutputTextureProfiles(true),
                    inputMaterials,
                    debugCombinationOutputs,
                    true
                );

                output.textureSet.Set(textures);

                return textures;
            }
        }

        public static List<OutputTexture> DrawTiledTextures(
            TiledInputMaterial inputMaterial,
            TiledOutputMaterial output,
            Vector2 textureSize,
            bool tiledMaterialsKeepOriginalSize,
            bool debugCombinationOutputs)
        {
            using (BUILD_TIME.TEX_COMBR.DrawTiledTextures.Auto())
            {
                if (inputMaterial.textures.count == 0)
                {
                    return null;
                }

                if (tiledMaterialsKeepOriginalSize)
                {
                    textureSize = inputMaterial.textures.size;
                }

                var textures = DrawTextures(
                    textureSize,
                    output,
                    output.GetOutputTextureProfiles(false),
                    new[] { inputMaterial },
                    debugCombinationOutputs,
                    false
                );

                output.textureSet.Set(textures);

                return textures;
            }
        }

        private static void DrawTexturePass(
            int pass,
            Material combinerMaterial,
            OutputTextureProfile outputTextureProfile,
            Dictionary<TextureMapChannel, InputTextureUsageElement> inputMaterialChannelLookup,
            Vector2 size)
        {
            using (BUILD_TIME.TEX_COMBR.DrawTexturePass.Auto())
            {
                var rect = new Rect(0, 0, size.x, size.y);
                var any = false;

                for (var i = 0; i < 4; i++)
                {
                    var tex = Properties.Texture(i);
                    var chan = Properties.Channel(i);
                    var ivn = Properties.Invert(i);
                    var pack = Properties.PackStyle(i);
                    var mode = Properties.Mode(i);
                    var val = Properties.FixedValue(i);

                    if (inputMaterialChannelLookup.ContainsKey(outputTextureProfile[i]))
                    {
                        any = true;
                        var inputChannel = inputMaterialChannelLookup[outputTextureProfile[i]];

                        combinerMaterial.SetTexture(tex, inputChannel.value);
                        combinerMaterial.SetFloat(chan, inputChannel.channel);
                        combinerMaterial.SetFloat(ivn,  inputChannel.invert);
                        combinerMaterial.SetFloat(pack, inputChannel.packing);
                        combinerMaterial.SetFloat(mode, inputChannel.textureMode);
                        combinerMaterial.SetFloat(val,  -1.0f);

                        rect = inputChannel.rect;
                    }
                    else if (outputTextureProfile[i] == TextureMapChannel.SetToOne)
                    {
                        combinerMaterial.SetFloat(val, 1.0f);
                    }
                    else if (outputTextureProfile[i] == TextureMapChannel.SetToPointFive)
                    {
                        combinerMaterial.SetFloat(val, 0.5f);
                    }
                    else if (outputTextureProfile[i] == TextureMapChannel.SetToZero)
                    {
                        combinerMaterial.SetFloat(val, 0.0f);
                    }
                    else
                    {
                        combinerMaterial.SetTexture(tex, null);
                        combinerMaterial.SetFloat(chan, i);
                        combinerMaterial.SetFloat(ivn,  0.0f);
                        combinerMaterial.SetFloat(pack, 0.0f);
                        combinerMaterial.SetFloat(mode, 0.0f);

                        var fixedValue = 0.0f;

                        switch (outputTextureProfile[i])
                        {
                            case TextureMapChannel.SetToPointFive:

                            case TextureMapChannel.Normal_TS_X:
                            case TextureMapChannel.NormalBump_TS_X:
                            case TextureMapChannel.Variant_Normal_TS_X:

                            case TextureMapChannel.Normal_TS_Y:
                            case TextureMapChannel.NormalBump_TS_Y:
                            case TextureMapChannel.Variant_Normal_TS_Y:

                                fixedValue = 0.5f;
                                break;

                            case TextureMapChannel.Normal_TS_Z:
                            case TextureMapChannel.NormalBump_TS_Z:
                            case TextureMapChannel.Variant_Normal_TS_Z:

                                fixedValue = 1.0f;
                                break;

                            case TextureMapChannel.SetToOne:
                            case TextureMapChannel.AmbientOcclusion:
                            case TextureMapChannel.Variant_AmbientOcclusion:
                            case TextureMapChannel.Roughness:
                            case TextureMapChannel.Variant_Roughness:
                            case TextureMapChannel.Opacity:
                            case TextureMapChannel.Variant_Opacity:
                            case TextureMapChannel.AlbedoDetail_Opacity:
                                //case TextureMapChannel.Transmission:
                                //case TextureMapChannel.Variant_Transmission:
                                fixedValue = 1.0f;
                                break;
                        }

                        combinerMaterial.SetFloat(val, fixedValue);
                    }
                }

                if (!any)
                {
                    return;
                }

                if ((outputTextureProfile.map == TextureMap.Albedo) ||
                    (outputTextureProfile.map == TextureMap.AlbedoDetail) ||
                    (outputTextureProfile.map == TextureMap.AlbedoDisplacement) ||
                    (outputTextureProfile.map == TextureMap.AlbedoGlossiness) ||
                    (outputTextureProfile.map == TextureMap.AlbedoRoughness) ||
                    (outputTextureProfile.map == TextureMap.Variant_Albedo) ||
                    (outputTextureProfile.map == TextureMap.Variant_AlbedoGlossiness))
                {
                    var inputChannel = inputMaterialChannelLookup[outputTextureProfile[0]];

                    combinerMaterial.SetFloat(Properties.ColorStyle(), (float)inputChannel.source.colorStyle);
                    combinerMaterial.SetFloat(Properties.ColorBoost(), inputChannel.source.colorBoost);
                }
                else
                {
                    combinerMaterial.SetFloat(Properties.ColorStyle(), 0.0f);
                    combinerMaterial.SetFloat(Properties.ColorBoost(), 1.0f);
                }

                combinerMaterial.SetPass(pass);

                var x = rect.x;
                var y = rect.y;
                var xMax = rect.x + rect.width;
                var yMax = rect.y + rect.height;

                GL.LoadPixelMatrix(0, size.x, size.y, 0);

                GL.Begin(GL.QUADS);

                GL.TexCoord(Vector3.zero);
                GL.Vertex3(x, y, 0.0f);

                GL.TexCoord(new Vector3(1, 0, 0));
                GL.Vertex3(xMax, y, 0.0f);

                GL.TexCoord(new Vector3(1, 1, 0));
                GL.Vertex3(xMax, yMax, 0.0f);

                GL.TexCoord(new Vector3(0, 1, 0));
                GL.Vertex3(x, yMax, 0.0f);

                GL.End();
            }
        }

        private static List<OutputTexture> DrawTextures(
            Vector2 textureSize,
            OutputMaterial output,
            IEnumerable<OutputTextureProfile> profiles,
            IEnumerable<InputMaterial> inputMaterials,
            bool debugCombinationOutputs,
            bool atlas)
        {
            using (BUILD_TIME.TEX_COMBR.DrawTextures.Auto())
            {
                RenderTexture rt = null;
                var storedRenderState = new StoredRenderState();

                try
                {
                    rt = RenderTexture.GetTemporary(
                        (int)textureSize.x,
                        (int)textureSize.y,
                        0,
                        RenderTextureFormat.ARGB32,
                        RenderTextureReadWrite.Linear
                    );

                    RenderTexture.active = rt;

                    var inputMaterialChannelLookupCollection =
                        new List<Dictionary<TextureMapChannel, InputTextureUsageElement>>();

                    foreach (var inputMaterial in inputMaterials)
                    {
                        var usageData = GetChannelData(textureSize, inputMaterial);

                        inputMaterialChannelLookupCollection.Add(usageData);
                    }

                    var combinerMats = new List<Tuple<string, Material>>();

                    output.textureSet.Clear();
                    var outputTextures = new List<OutputTexture>();

                    foreach (var outputProfile in profiles)
                    {
                        var combinerMaterial = new Material(_GSR.textureCombiner);

                        var anyChannelsPresent = false;

                        for (var i = 0; i < 4; i++)
                        {
                            anyChannelsPresent = anyChannelsPresent |
                                                 inputMaterialChannelLookupCollection.Any(
                                                     ud => ud.ContainsKey(outputProfile[i])
                                                 );
                        }

                        if (!anyChannelsPresent)
                        {
                            continue;
                        }

                        var newTextureName = ZString.Format(
                            "{0}_{1}",
                            output.TexturePrefix,
                            outputProfile.fileNameSuffix
                        );

                        var newTexture = outputProfile.settings.Create(textureSize, newTextureName);

                        var outputTexture = new OutputTexture(outputProfile, newTexture);
                        outputTextures.Add(outputTexture);

                        //GL.sRGBWrite = false;
                        GL.sRGBWrite = outputProfile.settings.sRGB;

                        GL.Clear(true, true, outputProfile.color);

                        for (var i = 0; i < inputMaterialChannelLookupCollection.Count; i++)
                        {
                            var inputMaterialChannelLookup = inputMaterialChannelLookupCollection[i];

                            DrawTexturePass(
                                i,
                                combinerMaterial,
                                outputProfile,
                                inputMaterialChannelLookup,
                                textureSize
                            );
                        }

                        var flipperMaterial = new Material(_GSR.textureFlipper);

                        GL.sRGBWrite = false;

                        var flip = RenderTexture.GetTemporary(
                            (int)textureSize.x,
                            (int)textureSize.y,
                            0,
                            RenderTextureFormat.ARGB32,
                            RenderTextureReadWrite.Linear
                        );

                        Graphics.Blit(rt, flip, flipperMaterial);

                        RenderTexture.active = flip;

                        try
                        {
                            using (BUILD_TIME.TEX_COMBR.ReadRenderTexture.Auto())
                            {
                                var rect = new Rect(0, 0, textureSize.x, textureSize.y);

                                newTexture.ReadPixels(rect, 0, 0);
                                newTexture.Apply(true);
                            }
                        }
                        finally
                        {
                            RenderTexture.ReleaseTemporary(flip);
                            RenderTexture.active = rt;
                        }

                        /*
                        using (BUILD_TIME.TEX_COMBR.ReadRenderTexture.Auto())
                        {
                            
                            var rect = new Rect(0, 0, textureSize, textureSize);
                            
                            var tempTexture = new Texture2D(
                                textureSize,
                                textureSize,
                                newTexture.graphicsFormat,
                                TextureCreationFlags.None
                            );

                             tempTexture.ReadPixels(rect, 0, 0);
                            tempTexture.Apply(true);

                            Color32[] newPixels = null;

                            using (BUILD_TIME.TEX_COMBR.FlipRenderTexture.Auto())
                            {
                                var pixels = tempTexture.GetPixels32();
                                newPixels = new Color32[pixels.Length];

                                for (var y = 0; y < tempTexture.height; y++) // bottom to top
                                for (var x = 0; x < tempTexture.width; x++) // left to right
                                {
                                    var index = (y * tempTexture.width) + x;
                                    var targetIndex = ((tempTexture.height - y - 1) * tempTexture.width) + x;

                                    newPixels[targetIndex] = pixels[index];
                                }
                            }

                            using (BUILD_TIME.TEX_COMBR.SetFlippedPixels.Auto())
                            {
                                newTexture.SetPixels32(newPixels);

                                newTexture.Apply(true);
                                Object.DestroyImmediate(tempTexture);
                            }
                        }
                        */

                        if (debugCombinationOutputs)
                        {
                            var path = AssetDatabaseManager.GetAssetPath(
                                output.GetMaterialElementByIndex(0).asset
                            );

                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                combinerMats.Add(new Tuple<string, Material>(path, combinerMaterial));
                            }
                        }
                    }

                    if (debugCombinationOutputs)
                    {
                        using (BUILD_TIME.TEX_COMBR.SaveCombiners.Auto())
                        {
                            try
                            {
                                AssetDatabaseManager.StartAssetEditing();

                                foreach (var (path, asset) in combinerMats)
                                {
                                    AssetDatabaseManager.CreateAsset(
                                        asset,
                                        AssetDatabaseManager.GenerateUniqueAssetPath(path)
                                    );
                                }
                            }
                            finally
                            {
                                AssetDatabaseManager.StopAssetEditing();
                            }
                        }
                    }

                    storedRenderState.Restore();
                    return outputTextures;
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ex);

                    if (rt != null)
                    {
                        RenderTexture.ReleaseTemporary(rt);
                        Object.DestroyImmediate(rt);
                    }

                    storedRenderState.Restore();

                    throw;
                }
            }
        }

        private static Dictionary<TextureMapChannel, InputTextureUsageElement> GetChannelData(
            Vector2 outputTextureSize,
            InputMaterial inputMaterial)
        {
            var existingInputChannels = new Dictionary<TextureMapChannel, InputTextureUsageElement>();
            var materialSize = inputMaterial.textures.size;

            var rect = inputMaterial.GetRect(materialSize, outputTextureSize);

            foreach (var inputTexture in inputMaterial.textures.inputTextures)
            {
                var inputProfile = inputTexture.profile;

                if (inputProfile.red != TextureMapChannel.None)
                {
                    if (existingInputChannels.ContainsKey(inputProfile.red.mapChannel))
                    {
                        throw new NotSupportedException("Bad channel!");
                    }

                    existingInputChannels.Add(
                        inputProfile.red.mapChannel,
                        inputTexture.GetUsageData(TextureChannel.R, rect)
                    );
                }

                if (inputProfile.green != TextureMapChannel.None)
                {
                    if (existingInputChannels.ContainsKey(inputProfile.green.mapChannel))
                    {
                        throw new NotSupportedException("Bad channel!");
                    }

                    existingInputChannels.Add(
                        inputProfile.green.mapChannel,
                        inputTexture.GetUsageData(TextureChannel.G, rect)
                    );
                }

                if (inputProfile.blue != TextureMapChannel.None)
                {
                    if (existingInputChannels.ContainsKey(inputProfile.blue.mapChannel))
                    {
                        throw new NotSupportedException("Bad channel!");
                    }

                    existingInputChannels.Add(
                        inputProfile.blue.mapChannel,
                        inputTexture.GetUsageData(TextureChannel.B, rect)
                    );
                }

                if (inputProfile.alpha != TextureMapChannel.None)
                {
                    if (existingInputChannels.ContainsKey(inputProfile.alpha.mapChannel))
                    {
                        throw new NotSupportedException("Bad channel!");
                    }

                    existingInputChannels.Add(
                        inputProfile.alpha.mapChannel,
                        inputTexture.GetUsageData(TextureChannel.A, rect)
                    );
                }
            }

            foreach (var channel in existingInputChannels)
            {
                channel.Value.source = inputMaterial;
            }

            return existingInputChannels;
        }

        #region Nested type: Properties

        public static class Properties
        {
            #region Constants and Static Readonly

            private const string TEX_CHANNEL_FORMAT = "_{0}TexChannel";
            private const string TEX_COLOR_BOOST = "_ColorBoost";
            private const string TEX_COLOR_STYLE = "_ColorStyle";
            private const string TEX_FIXEDVALUE_FORMAT = "_{0}TexFixedValue";
            private const string TEX_FORMAT = "_{0}Tex";
            private const string TEX_INVERT_FORMAT = "_{0}TexInvert";
            private const string TEX_MODE_FORMAT = "_{0}TexMode";
            private const string TEX_PACKSTYLE_FORMAT = "_{0}TexPackStyle";

            #endregion

            public static string Channel(int channel)
            {
                return ZString.Format(TEX_CHANNEL_FORMAT, (TextureChannel)channel);
            }

            public static string ColorBoost()
            {
                return TEX_COLOR_BOOST;
            }

            public static string ColorStyle()
            {
                return TEX_COLOR_STYLE;
            }

            public static string FixedValue(int channel)
            {
                return ZString.Format(TEX_FIXEDVALUE_FORMAT, (TextureChannel)channel);
            }

            public static string Invert(int channel)
            {
                return ZString.Format(TEX_INVERT_FORMAT, (TextureChannel)channel);
            }

            public static string Mode(int channel)
            {
                return ZString.Format(TEX_MODE_FORMAT, (TextureChannel)channel);
            }

            public static string PackStyle(int channel)
            {
                return ZString.Format(TEX_PACKSTYLE_FORMAT, (TextureChannel)channel);
            }

            public static string Texture(int channel)
            {
                return ZString.Format(TEX_FORMAT, (TextureChannel)channel);
            }
        }

        #endregion
    }
}
