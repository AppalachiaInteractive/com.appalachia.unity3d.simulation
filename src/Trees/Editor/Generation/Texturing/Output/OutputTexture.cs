using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Objects.Root;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Output
{
    [Serializable]
    public class OutputTexture : AppalachiaSimpleBase
    {
        public OutputTexture(
            OutputTextureProfile profile, 
            Texture2D texture)
        {
            this.profile = profile;
            _texture = texture;
        }

        [PreviewField(ObjectFieldAlignment.Center, Height = 72f), HideLabel, InlineProperty]
        [HorizontalGroup("TEX", MaxWidth = 96f)]
        [PropertySpace]
        [SerializeField]
        private Texture2D _texture;

        public Texture2D texture => _texture;

        [VerticalGroup("TEX/PANE")]
        [HideLabel, InlineProperty]
        public OutputTextureProfile profile;

        public void AssignToMaterial(Material m)
        {
            m.SetTexture(profile.propertyName, _texture);
        }

        public void UpdateAlphaTestReferenceValue(float transparency)
        {
            profile.settings.UpdateAlphaTestReferenceValue(AssetDatabaseManager.GetAssetPath(_texture), transparency);
        }

        public void Reload(string path)
        {
            var temp = _texture;
            
            _texture = AssetDatabaseManager.LoadAssetAtPath<Texture2D>(path);

            if (temp != null)
            {
                Object.DestroyImmediate(temp);
            }
        }






        /*

         *  public static void SaveToDisk(this Texture2D newTexture, string outputPath, TextureImporter modelSettings, bool sRGB)
        {
           AppaFile.WriteAllBytes(outputPath, newTexture.EncodeToPNG());

            AssetDatabaseManager.Refresh();
            
            var textureImporter = (TextureImporter)AssetImporter.GetAtPath(outputPath);
            
            var textureSettings = new TextureImporterSettings();
            modelSettings.ReadTextureSettings(textureSettings);

            textureSettings.sRGBTexture = sRGB;
            textureImporter.SetTextureSettings(textureSettings);
            textureImporter.Save
            
        
        */

        /*



        public void Save(string assetQualifier, TreeAssetSubfolders subfolders)
        {
            using (BUILD_TIME.OUT_TEX.Save.Auto())
            {
                var fileName = $"{assetQualifier}{profile.fileNameSuffix}";

                _texture.name = fileName;

                var targetSavePath = subfolders.GetFilePathByType(TreeAssetSubfolderType.Textures, $"{fileName}.png");

                var existingPath = AssetDatabaseManager.GetAssetPath(_texture);

                if (string.IsNullOrWhiteSpace(existingPath))
                {
                    using (BUILD_TIME.OUT_TEX.SaveNew.Auto())
                    {
                        var absolutePath = targetSavePath;

                        if (absolutePath.StartsWith("Assets"))
                        {
                            absolutePath = targetSavePath.Replace("Assets", Application.dataPath);
                        }

                       AppaFile.WriteAllBytes(absolutePath, _texture.EncodeToPNG());
                    }
                }
                else if (existingPath != targetSavePath)
                {
                    using (BUILD_TIME.OUT_TEX.SaveExisting.Auto())
                    {
                        profile.settings.Apply(targetSavePath);
                        AssetDatabaseManager.MoveAsset(existingPath, targetSavePath);
                    }
                }
                
                /*else if (existingPath != targetSavePath)
                {
                    using (BUILD_TIME.OUT_TEX.SaveExisting.Auto())
                    {
                        profile.settings.Apply(targetSavePath);
                        AssetDatabaseManager.MoveAsset(existingPath, targetSavePath);
                    }
            }
        }
        
        public void Reload(string assetQualifier, TreeAssetSubfolders subfolders)
        {
            using (BUILD_TIME.OUT_TEX.Reload.Auto())
            {
                var fileName = $"{assetQualifier}{profile.fileNameSuffix}";

                var targetSavePath = subfolders.GetFilePathByType(TreeAssetSubfolderType.Textures, $"{fileName}.png");

                _texture = AssetDatabaseManager.LoadAssetAtPath<Texture2D>(targetSavePath);
            }
        }


        public void Save(string assetQualifier, TreeAssetSubfolders subfolders)
        {
            using (BUILD_TIME.OUT_TEX.Save.Auto())
            {
                var fileName = $"{assetQualifier}{profile.fileNameSuffix}";

                _texture.name = fileName;

                var targetSavePath = subfolders.GetFilePathByType(TreeAssetSubfolderType.Textures, $"{fileName}.png");

                var existingPath = AssetDatabaseManager.GetAssetPath(_texture);

                if (string.IsNullOrWhiteSpace(existingPath))
                {
                    using (BUILD_TIME.OUT_TEX.SaveNew.Auto())
                    {
                        var absolutePath = targetSavePath;

                        if (absolutePath.StartsWith("Assets"))
                        {
                            absolutePath = targetSavePath.Replace("Assets", Application.dataPath);
                        }

                       AppaFile.WriteAllBytes(absolutePath, _texture.EncodeToPNG());
                    }
                }
                else if (existingPath != targetSavePath)
                {
                    using (BUILD_TIME.OUT_TEX.SaveExisting.Auto())
                    {
                        profile.settings.Apply(targetSavePath);
                        AssetDatabaseManager.MoveAsset(existingPath, targetSavePath);
                    }
                }
            }
        }
         */
    }
}