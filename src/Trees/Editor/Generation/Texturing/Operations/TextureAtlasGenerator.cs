/*using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Materials;
using Appalachia.Simulation.Trees.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Debug = System.Diagnostics.Debug;

namespace Appalachia.Simulation.Trees.Texturing
{
    public static class TextureGenerationManager
    {
        public static void Generate()
        {
            
        }
    }
    
    public static class TextureAtlasGenerator
    {
        public static FinalTreeTextureCollection GenerateTextureAtlas(
            TreeMaterialCollection materials,
            FinalTreeTextureCollection textureCollection,
            TextureAtlas atlas,
            TextureSettingsCollection.DefaultTextureSettings textureSettings,
            string treeName,
            bool variant = false)
        {
            var renderTextureState = new PreAtlasRenderState();
            RenderTexture targetTexture = null;

            try
            {
                CacheOriginalTextureImporters(atlas, materials);

                if (textureCollection == null)
                {
                    textureCollection = new FinalTreeTextureCollection(
                        textureSettings[0].Create(
                            atlas.dimensions.width,
                            atlas.dimensions.height,
                            treeName
                        ),
                        textureSettings[1].Create(
                            atlas.dimensions.width,
                            atlas.dimensions.height,
                            treeName
                        ),
                        textureSettings[2].Create(
                            atlas.dimensions.width,
                            atlas.dimensions.height,
                            treeName
                        )
                    );
                }

                if (!textureCollection.HasAlbedo ||
                    textureCollection.albedo.width != atlas.dimensions.width ||
                    textureCollection.albedo.height != atlas.dimensions.height)
                {
                    textureCollection.albedo = textureSettings[0].Create(
                        atlas.dimensions.width,
                        atlas.dimensions.height,
                        treeName
                    );
                }

                if (!textureCollection.HasNormal ||
                    textureCollection.normal.width != atlas.dimensions.width ||
                    textureCollection.normal.height != atlas.dimensions.height)
                {
                    textureCollection.normal = textureSettings[1].Create(
                        atlas.dimensions.width,
                        atlas.dimensions.height,
                        treeName
                    );
                }

                if (!textureCollection.HasSurface ||
                    textureCollection.surface.width != atlas.dimensions.width ||
                    textureCollection.surface.height != atlas.dimensions.height)
                {
                    textureCollection.surface = textureSettings[2].Create(
                        atlas.dimensions.width,
                        atlas.dimensions.height,
                        treeName
                    );
                }

                foreach (var texture in textureCollection)
                {
                    texture.EnsureReadable();
                }

                targetTexture = RenderTexture.GetTemporary(
                    atlas.dimensions.width,
                    atlas.dimensions.height,
                    0,
                    SystemInfo.GetGraphicsFormat(DefaultFormat.LDR)
                );

                RenderTexture.active = targetTexture;

                for (var textureIndex = 0;
                    textureIndex < TextureSettingsCollection.blit.Length;
                    textureIndex++)
                {
                    var blitSetting = TextureSettingsCollection.blit[textureIndex];
                    var texture = textureCollection[textureIndex];


                    DrawTextures(
                        atlas,
                        texture,
                        blitSetting,
                        node => variant
                            ? materials.GetByMaterialID(node.materialID)
                                .variantTextures[textureIndex]
                            : materials.GetByMaterialID(node.materialID).textures[textureIndex],
                        targetTexture
                    );
                }
                
                if (renderTextureState != null)
                {
                    renderTextureState.Restore();
                }
                if (targetTexture != null)
                {
                    RenderTexture.ReleaseTemporary(targetTexture);
                }

                RestoreOriginalTextureImporterSettings(atlas, materials);
            }
            catch (Exception ex)
            {
                UnityEngine.AppaLog.Error(ex);
                
                if (renderTextureState != null)
                {
                    renderTextureState.Restore();
                }
                if (targetTexture != null)
                {
                    RenderTexture.ReleaseTemporary(targetTexture);
                }

                RestoreOriginalTextureImporterSettings(atlas, materials);
                
                throw;
            }

            return textureCollection;
        }


        public static void PrepareAndPackAtlas(
            TextureAtlas atlas,
            List<TreeMaterial> materials,
            TreeMaterialCollection materialCollection,
            TextureSettings settings)
        {
            foreach (var material in materials)
            {
                var scale = new Vector2(material.proportionalArea, material.proportionalArea);

                scale.x *= ((int) settings.textureSize / (float) material.textures.width);
                scale.y *= ((int) settings.textureSize / (float) material.textures.height);

                var uvTiling = material.textures.scale;

                if (!material.tileV)
                {
                    uvTiling = new Vector2(1, 1); // ignore if not tiling
                }

                atlas.AddMaterial(material, scale, uvTiling);
            }

            var textureSize = (int)(settings.textureSize);

            atlas.Pack(
                textureSize,
                textureSize,
                settings.padding,
                true
            );

            atlas.PopulateAtlasDataToMaterials(materialCollection);
        }

        private static void DrawTextures(
            TextureAtlas atlas,
            Texture2D texture,
            TextureSettingsCollection.TextureBlitSettings settings,
            Func<TextureAtlas.TextureNode, Texture2D> nodeTexture,
            RenderTexture renderTexture)
        {
            var textureCombinerMaterial =
                EditorGUIUtility.LoadRequired(
                    "Inspectors/TreeCreator/TreeTextureCombinerMaterial.mat"
                ) as Material;

            RenderTexture.active = renderTexture;
            GL.LoadPixelMatrix(0, atlas.dimensions.width, 0, atlas.dimensions.height);

            textureCombinerMaterial.SetVector(
                "_TexSize",
                new Vector4(atlas.dimensions.width, atlas.dimensions.height, 0, 0)
            );

            GL.sRGBWrite = false;
            GL.Clear(settings.clearDepth, settings.clearColor, settings.defaultColor);

            for (var i = 0; i < atlas.nodes.Count; i++)
            {
                var node = atlas.nodes[i];
                var nodeRect = node.packedRect;
                
                DrawTexture(nodeRect, nodeTexture(node), textureCombinerMaterial);
            }

            texture.ReadPixels(
                new Rect(0, 0, atlas.dimensions.width, atlas.dimensions.height),
                0,
                0
            );
            
            texture.Apply(true);
        }

        private static void DrawTexture(Rect rect, Texture texture, Material material)
        {
            material.SetColor("_Color", Color.white);
            material.SetTexture("_RGBSource", texture);
            material.SetTexture("_AlphaSource", texture);
            material.SetPass(1);
            
            GL.Begin(GL.QUADS);
            GL.TexCoord(new Vector3(0, 0, 0));
            GL.Vertex3(rect.x, rect.y, 0.0f);

            GL.TexCoord(new Vector3(1, 0, 0));
            GL.Vertex3(rect.x + rect.width, rect.y, 0.0f);

            GL.TexCoord(new Vector3(1, 1, 0));
            GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0.0f);

            GL.TexCoord(new Vector3(0, 1, 0));
            GL.Vertex3(rect.x, rect.y + rect.height, 0.0f);
            GL.End();
        }


    
}*/