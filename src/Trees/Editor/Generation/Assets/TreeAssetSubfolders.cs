using System;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Core.Serialization;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    [Title("Subfolders", TitleAlignment = TitleAlignments.Centered)]
    public class TreeAssetSubfolders : ResponsiveNestedAppalachiaObject<TreeAssetSubfolders>
    {
        #region Constants and Static Readonly

        private const string _DATA_FOLDER = "Data";
        private const string _IMPOSTORS_FOLDER = "Impostors";
        private const string _MATERIALS_FOLDER = "Materials";
        private const string _MESHES_FOLDER = "Meshes";
        private const string _PREFABS_FOLDER = "Prefabs";
        private const string _SNAPSHOTS_FOLDER = "Snapshots";
        private const string _TEXTURES_FOLDER = "Textures";

        private const string DEFAULT_NAME = "subfolders";

        #endregion

        private TreeAssetSubfolders()
        {
        }

        #region Fields and Autoproperties

        public bool lockFolders;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string data;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string materials;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string meshes;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string prefabs;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string impostors;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string textures;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string snapshots;

        [FolderPath, DisableIf(nameof(lockFolders))]
        public string main;

        public TSEDataContainer parent;
        public NameBasis nameBasis;

        #endregion

        /// <inheritdoc />
        protected override string DefaultName => DEFAULT_NAME;

        /// <inheritdoc />
        public override void InitializeFromParent(AppalachiaObject p)
        {
            parent = p as TSEDataContainer;
            Update();
        }

        /// <inheritdoc />
        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
        }

        public void CreateFolders()
        {
            if (!lockFolders)
            {
                Update();
            }

            ResetEmptyPaths();

            AppaDirectory.CreateDirectory(data);
            AppaDirectory.CreateDirectory(textures);
            AppaDirectory.CreateDirectory(materials);
            AppaDirectory.CreateDirectory(meshes);
            AppaDirectory.CreateDirectory(prefabs);
        }

        public void CreateImpostorFolder()
        {
            if (!lockFolders)
            {
                Update();
            }

            ResetEmptyPaths();
            AppaDirectory.CreateDirectory(impostors);
        }

        public void CreateSnapshotFolder()
        {
            if (!lockFolders)
            {
                Update();
            }

            ResetEmptyPaths();
            AppaDirectory.CreateDirectory(snapshots);
        }

        public string GetFilePathByType(TreeAssetSubfolderType type, string filename)
        {
            switch (type)
            {
                case TreeAssetSubfolderType.Data:
                {
                    return AppaPath.Combine(data, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Textures:
                {
                    return AppaPath.Combine(textures, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Snapshots:
                {
                    return AppaPath.Combine(snapshots, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Materials:
                {
                    return AppaPath.Combine(materials, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Meshes:
                {
                    return AppaPath.Combine(meshes, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Prefabs:
                {
                    return AppaPath.Combine(prefabs, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Impostors:
                {
                    return AppaPath.Combine(impostors, filename).Replace("\\", "/");
                }
                case TreeAssetSubfolderType.Main:
                {
                    return AppaPath.Combine(prefabs, filename).Replace("\\", "/");
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public string GetFolderPathByType(TreeAssetSubfolderType type)
        {
            switch (type)
            {
                case TreeAssetSubfolderType.Data:
                {
                    return data;
                }
                case TreeAssetSubfolderType.Textures:
                {
                    return textures;
                }
                case TreeAssetSubfolderType.Materials:
                {
                    return materials;
                }
                case TreeAssetSubfolderType.Meshes:
                {
                    return meshes;
                }
                case TreeAssetSubfolderType.Prefabs:
                {
                    return prefabs;
                }
                case TreeAssetSubfolderType.Impostors:
                {
                    return impostors;
                }
                case TreeAssetSubfolderType.Snapshots:
                {
                    return snapshots;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void ResetEmptyPaths()
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                data = AppaPath.Combine(main, _DATA_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(textures))
            {
                textures = AppaPath.Combine(main, _TEXTURES_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(materials))
            {
                materials = AppaPath.Combine(main, _MATERIALS_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(meshes))
            {
                meshes = AppaPath.Combine(main, _MESHES_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(prefabs))
            {
                prefabs = AppaPath.Combine(main, _PREFABS_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(impostors))
            {
                impostors = AppaPath.Combine(main, _IMPOSTORS_FOLDER);
            }

            if (string.IsNullOrWhiteSpace(snapshots))
            {
                snapshots = AppaPath.Combine(main, _SNAPSHOTS_FOLDER);
            }
        }

        [Button]
        public void Update()
        {
            var directory = parent.DirectoryPath;
            main = AppaPath.Combine(directory, nameBasis.nameBasis);
            data = AppaPath.Combine(main,      _DATA_FOLDER);
            textures = AppaPath.Combine(main,  _TEXTURES_FOLDER);
            materials = AppaPath.Combine(main, _MATERIALS_FOLDER);
            meshes = AppaPath.Combine(main,    _MESHES_FOLDER);
            prefabs = AppaPath.Combine(main,   _PREFABS_FOLDER);
            impostors = AppaPath.Combine(main, _IMPOSTORS_FOLDER);
            snapshots = AppaPath.Combine(main, _SNAPSHOTS_FOLDER);
        }
    }
}
