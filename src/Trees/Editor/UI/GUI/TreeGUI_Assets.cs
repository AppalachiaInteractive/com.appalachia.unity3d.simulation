using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI
{
    public static partial class TreeGUI
    {
        public static class Assets
        {
            public static T CreateAndSave<T>()
                where T : ScriptableObject
            {
                var instance = (T) ScriptableObject.CreateInstance(typeof(T));
                var script = MonoScript.FromScriptableObject(instance);
                var scriptPath = AssetDatabaseManager.GetAssetPath(script);
                var scriptFolder = AppaPath.GetDirectoryName(scriptPath);
                var dataFolder = AppaPath.Combine(scriptFolder, "_data");

                if (!AppaDirectory.Exists(dataFolder))
                {
                    AppaDirectory.CreateDirectory(dataFolder);
                }

                var assetPath = AppaPath.Combine(
                    dataFolder,
                    ZString.Format("{0}_{1:yyyyMMdd-hhmmssfff}.asset", typeof(T).Name, DateTime.Now)
                );

                return CreateAndSave(assetPath, instance);
            }

            public static T CreateAndSaveInFolder<T>(string folder, string name)
                where T : ScriptableObject
            {
                var instance = (T) ScriptableObject.CreateInstance(typeof(T));

                if (!AppaDirectory.Exists(folder))
                {
                    AppaDirectory.CreateDirectory(folder);
                }

                var assetPath = AppaPath.Combine(folder, ZString.Format("{0}.asset", name));

                return CreateAndSave(assetPath, instance);
            }

            public static T CreateAndSave<T>(string assetPath)
                where T : ScriptableObject
            {
                var instance = (T) ScriptableObject.CreateInstance(typeof(T));
                return CreateAndSave(assetPath, instance);
            }

            public static T CreateAndSave<T>(string assetPath, T instance)
                where T : ScriptableObject
            {
                var path = AssetDatabaseManager.GenerateUniqueAssetPath(assetPath);

                AssetDatabaseManager.CreateAsset(instance, path);

                return instance;
            }
        }
    }
}
