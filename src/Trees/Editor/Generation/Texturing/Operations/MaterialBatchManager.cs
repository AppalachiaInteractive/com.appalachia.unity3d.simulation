using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Strings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    public static class MaterialBatchManager
    {
        public static void ActivateProcessingTextureImporterSettings(
            this IEnumerable<InputMaterial> inputMaterials,
            TextureSize maxTextureSize)
        {
            using (BUILD_TIME.MAT_BAT_MGR.ActivateProcessingTextureImporterSettings.Auto())
            {
                MaterialProcess(
                    inputMaterials,
                    texture => texture.ActivateProcessingTextureImporterSettings(maxTextureSize)
                );
            }
        }

        public static void RestoreOriginalTextureImporterSettings(this IEnumerable<InputMaterial> inputMaterials)
        {
            using (BUILD_TIME.MAT_BAT_MGR.RestoreOriginalTextureImporterSettings.Auto())
            {
                MaterialProcess(inputMaterials, texture => texture.RestoreOriginalTextureImporterSettings());
            }
        }

        private static void MaterialProcess(IEnumerable<InputMaterial> materials, Func<InputTexture, string> action)
        {
            var reimportPaths = new List<string>();

            foreach (var inputMaterial in materials)
            {
                foreach (var inputTexture in inputMaterial.textures.inputTextures)
                {
                    reimportPaths.Add(action(inputTexture));
                }
            }

            try
            {
                AssetDatabaseManager.StartAssetEditing();
                foreach (var reimportPath in reimportPaths)
                {
                    if (reimportPath != null)
                    {
                        AssetDatabaseManager.ImportAsset(reimportPath);
                    }
                }
            }
            finally
            {
                AssetDatabaseManager.StopAssetEditing();
            }
        }

        public static List<string> SaveTextures(
            this Dictionary<OutputMaterial, List<OutputTexture>> textureSets,
            TreeAssetSubfolders subfolders,
            int lodCount,
            TreeAssetSubfolderType folder = TreeAssetSubfolderType.Textures)
        {
            using (BUILD_TIME.MAT_BAT_MGR.SaveTextures.Auto())
            {
                var filePaths = new List<string>();

                using (BUILD_TIME.MAT_BAT_MGR.WriteAllTextures.Auto())
                {
                    foreach (var set in textureSets)
                    {
                        var material = set.Key;

                        material.EnsureCreated(lodCount);

                        var textures = set.Value;

                        if (textures == null)
                        {
                            continue;
                        }

                        foreach (var texture in textures)
                        {
                            var fileName = ZString.Format(
                                "{0}{1}",
                                material.First().asset.name,
                                texture.profile.fileNameSuffix
                            );

                            texture.texture.name = fileName;

                            var targetSavePath = subfolders.GetFilePathByType(
                                folder,
                                ZString.Format("{0}.png", fileName)
                            );

                            filePaths.Add(targetSavePath);

                            using (BUILD_TIME.MAT_BAT_MGR.WriteTexture.Auto())
                            {
                                var absolutePath = targetSavePath;

                                if (absolutePath.StartsWith("Assets"))
                                {
                                    absolutePath = targetSavePath.Replace("Assets", Application.dataPath);
                                }

                                AppaFile.WriteAllBytes(absolutePath, texture.texture.EncodeToPNG());
                            }
                        }
                    }
                }

                return filePaths;
            }
        }

        public static void ReimportTextures(
            this Dictionary<OutputMaterial, List<OutputTexture>> textureSets,
            TreeAssetSubfolders subfolders,
            TextureSize textureSize,
            QualityMode mode,
            List<string> filePaths,
            int lodCount,
            TreeAssetSubfolderType folder = TreeAssetSubfolderType.Materials)
        {
            using (BUILD_TIME.MAT_BAT_MGR.ReimportTextures.Auto())
            {
                using (BUILD_TIME.MAT_BAT_MGR.AssetDatabaseRefresh.Auto())
                {
                    AssetDatabaseManager.Refresh();
                }


                var index = -1;

                using (BUILD_TIME.MAT_BAT_MGR.ApplyAllSettings.Auto())
                {
                    foreach (var set in textureSets)
                    {
                        if (set.Value == null)
                        {
                            continue;
                        }

                        var material = set.Key;

                        material.EnsureCreated(lodCount);

                        if (mode == QualityMode.Working) continue;

                        var textures = set.Value;

                        foreach (var texture in textures)
                        {
                            index += 1;

                            var path = filePaths[index];

                            using (BUILD_TIME.MAT_BAT_MGR.ApplySettings.Auto())
                            {
                                texture.profile.settings.Apply(path, textureSize, mode);
                            }
                        }
                    }
                }

                
                var materialPaths = new List<(OutputMaterial mat, List<string> paths)>();

                using (BUILD_TIME.MAT_BAT_MGR.Import.Auto())
                {
                    try
                    {

                        AssetDatabaseManager.StartAssetEditing();

                        using (BUILD_TIME.MAT_BAT_MGR.ImportMaterials.Auto())
                        {
                            foreach (var outputMaterial in textureSets)
                            {
                                SaveMaterials(outputMaterial.Key, subfolders, folder);
                                
                                if (outputMaterial.Value == null)
                                {
                                    continue;
                                }
                          
                                var elementMaterial = outputMaterial.Key;
                                var elementPaths = new List<string>();

                                foreach (var element in outputMaterial.Key)
                                {
                                    var asset = element.asset;

                                    string path;
                                    string existingPath;

                                    using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsGetPath.Auto())
                                    {
                                        path = subfolders.GetFilePathByType(
                                            folder,
                                            ZString.Format("{0}.mat", asset.name)
                                        );

                                        existingPath = AssetDatabaseManager.GetAssetPath(asset);
                                    }

                                    if (string.IsNullOrWhiteSpace(existingPath))
                                    {
                                        using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsCreate.Auto())
                                        {
                                            AssetDatabaseManager.CreateAsset(asset, path);
                                        }
                                    }
                                    else if (existingPath != path)
                                    {
                                        using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsMove.Auto())
                                        {
                                            AssetDatabaseManager.MoveAsset(existingPath, path);
                                        }
                                    }
                                    
                                    elementPaths.Add(path);
                                }

                                (OutputMaterial mat, List<string> paths) item;
                                item.mat = elementMaterial;
                                
                                item.paths = elementPaths;

                                materialPaths.Add(item);

                            }
                        }

                        using (BUILD_TIME.MAT_BAT_MGR.ImportTextures.Auto())
                        {
                            foreach (string path in filePaths)
                            {
                                AssetDatabaseManager.ImportAsset(path);
                            }
                        }
                    }
                    finally
                    {
                        AssetDatabaseManager.StopAssetEditing();
                    }
                }

                index = -1;

                using (BUILD_TIME.MAT_BAT_MGR.ReloadAll.Auto())
                {
                    foreach (var set in textureSets)
                    {
                        var textures = set.Value;

                        if (textures == null) continue;

                        foreach (var texture in textures)
                        {
                            index += 1;

                            var path = filePaths[index];

                            using (BUILD_TIME.MAT_BAT_MGR.Reload.Auto())
                            {
                                var temp = texture.texture;

                                texture.Reload(path);

                                Object.DestroyImmediate(temp);
                            }
                        }
                    }

                    foreach (var (mat, paths) in materialPaths)
                    {
                        var count = 0;
                        foreach (var element in mat)
                        {
                            element.SetMaterial(
                                paths[count],
                                (mat.MaterialContext == MaterialContext.AtlasOutputMaterial) ||
                                (mat.MaterialContext == MaterialContext.BranchOutputMaterial)
                            );
                            count += 1;
                        }
                    }
                }
            }
        }

        public static List<string> SaveTextures(
            this List<OutputTexture> textures,
            OutputMaterial material,
            TreeAssetSubfolders subfolders,
            int lodCount,
            TreeAssetSubfolderType folder = TreeAssetSubfolderType.Textures)
        {
            using (BUILD_TIME.MAT_BAT_MGR.SaveTextures.Auto())
            {
                var filePaths = new List<string>();

                using (BUILD_TIME.MAT_BAT_MGR.WriteAllTextures.Auto())
                {
                    material.EnsureCreated(lodCount);

                    if (textures == null)
                    {
                        return filePaths;
                    }

                    foreach (var texture in textures)
                    {
                        var fileName = ZString.Format(
                            "{0}{1}",
                            material.First().asset.name,
                            texture.profile.fileNameSuffix
                        );

                        texture.texture.name = fileName;

                        var targetSavePath = subfolders.GetFilePathByType(
                            folder,
                            ZString.Format("{0}.png", fileName)
                        );

                        filePaths.Add(targetSavePath);

                        using (BUILD_TIME.MAT_BAT_MGR.WriteTexture.Auto())
                        {
                            var absolutePath = targetSavePath;

                            if (absolutePath.StartsWith("Assets"))
                            {
                                absolutePath = targetSavePath.Replace("Assets", Application.dataPath);
                            }

                           AppaFile.WriteAllBytes(absolutePath, texture.texture.EncodeToPNG());
                        }
                    }
                }


                return filePaths;
            }
        }

        public static void SaveMaterials(
            OutputMaterial material,
            TreeAssetSubfolders subfolders,
            TreeAssetSubfolderType folder = TreeAssetSubfolderType.Materials)
        {
            using (BUILD_TIME.MAT_BAT_MGR.ImportMaterials.Auto())
            {
                if (material == null)
                {
                    return;
                }

                foreach (var element in material)
                {
                    var mat = element.asset;

                    string path;
                    string existingPath;

                    using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsGetPath.Auto())
                    {
                        path = subfolders.GetFilePathByType(
                            folder,
                            ZString.Format("{0}.mat", mat.name)
                        );

                        existingPath = AssetDatabaseManager.GetAssetPath(mat);
                    }

                    if (string.IsNullOrWhiteSpace(existingPath))
                    {
                        using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsCreate.Auto())
                        {
                            AssetDatabaseManager.CreateAsset(mat, path);
                            element.SetMaterial(path, (material.MaterialContext == MaterialContext.AtlasOutputMaterial) ||
                                (material.MaterialContext == MaterialContext.BranchOutputMaterial));
                        }
                    }
                    else if (existingPath != path)
                    {
                        using (BUILD_TIME.MAT_BAT_MGR.ImportMaterialsMove.Auto())
                        {
                            AssetDatabaseManager.MoveAsset(existingPath, path);

                            element.SetMaterial(
                                path,
                                (material.MaterialContext == MaterialContext.AtlasOutputMaterial) ||
                                (material.MaterialContext == MaterialContext.BranchOutputMaterial)
                            );
                        }
                    }
                }

                
            }
        }

        public static void ReimportTextures(
            this List<OutputTexture> textures,
            OutputMaterial material,
            TreeAssetSubfolders subfolders,
            TextureSize textureSize,
            QualityMode mode,
            List<string> filePaths,
            TreeAssetSubfolderType folder = TreeAssetSubfolderType.Materials)
        {
            using (BUILD_TIME.MAT_BAT_MGR.ReimportTextures.Auto())
            {
                using (BUILD_TIME.MAT_BAT_MGR.AssetDatabaseRefresh.Auto())
                {
                    AssetDatabaseManager.Refresh();
                }

                var index = -1;

                using (BUILD_TIME.MAT_BAT_MGR.ApplyAllSettings.Auto())
                {
                    foreach (var texture in textures)
                    {
                        index += 1;

                        var path = filePaths[index];

                        using (BUILD_TIME.MAT_BAT_MGR.ApplySettings.Auto())
                        {
                            texture.profile.settings.Apply(path, textureSize, mode);
                        }
                    }
                }

                using (BUILD_TIME.MAT_BAT_MGR.Import.Auto())
                {
                    try
                    {
                        AssetDatabaseManager.StartAssetEditing();

                        SaveMaterials(material, subfolders, folder);

                        using (BUILD_TIME.MAT_BAT_MGR.ImportTextures.Auto())
                        {
                            foreach (string path in filePaths)
                            {
                                AssetDatabaseManager.ImportAsset(path);
                            }
                        }
                    }
                    finally
                    {
                        AssetDatabaseManager.StopAssetEditing();
                    }
                }

                index = -1;

                using (BUILD_TIME.MAT_BAT_MGR.ReloadAll.Auto())
                {
                    foreach (var texture in textures)
                    {
                        index += 1;

                        var path = filePaths[index];

                        using (BUILD_TIME.MAT_BAT_MGR.Reload.Auto())
                        {
                            var temp = texture.texture;

                            texture.Reload(path);

                            Object.DestroyImmediate(temp);
                        }
                    }
                }
            }
        }

    }
}