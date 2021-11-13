#if UNITY_EDITOR

#region

using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Constants;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

#endregion

namespace Appalachia.Simulation.Trees.Build.Execution
{
    public static class TreeBuildManagerMenu
    {
        static TreeBuildManagerMenu()
        {
            EditorApplication.delayCall += () => EnableBuilding(true);
        }
        
        internal static void EnableBuilding(bool build)
        {
            if (build)
            {
                EditorApplication.update += TreeBuildManager.Update;
            }
            else
            {
                EditorApplication.update -= TreeBuildManager.Update;
            }
        }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Enable Build" + SHC.CTRL_ALT_SHFT_T, true)]
        private static bool PrefabRendering_AllowUpdatesValidate()
        {
            Menu.SetChecked("Tools/Tree Tool/Trees/Enable Build", TreeBuildManager._enabled);
            return true;
        }

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Enable Build" + SHC.CTRL_ALT_SHFT_T)]
        public static void PrefabRendering_AllowUpdates()
        {
            TreeBuildManager._enabled = !TreeBuildManager._enabled;
            EnableBuilding(TreeBuildManager._enabled);
        }

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Metadata/Rebuild Individual Metadata")]
        private static void Metadata_RebuildIndividualMetadata()
        {
            var trees = AssetDatabaseManager.FindAssets<TreeDataContainer>();

            foreach (var tree in trees)
            {
                tree.CreateRuntimeMetadata();
                EditorUtility.SetDirty(tree);
                EditorUtility.SetDirty(tree.runtimeSpeciesMetadata);

                foreach (var individual in tree.individuals)
                {
                    individual.UpdateMetadata(tree.species);
                }
            }

            AssetDatabaseManager.SaveAssets();
        }
        
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Default/Working")]
        private static void Execute_AllBuilds_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Default/Preview")]
        private static void Execute_AllBuilds_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Default/Finalized")]
        private static void Execute_AllBuilds_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Full/Working")]
        private static void Execute_AllBuilds_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Full/Preview")]
        private static void Execute_AllBuilds_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Full/Finalized")]
        private static void Execute_AllBuilds_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Force Full/Working")]
        private static void Execute_AllBuilds_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Force Full/Preview")]
        private static void Execute_AllBuilds_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Force Full/Finalized")]
        private static void Execute_AllBuilds_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Textures Only/Working")]
        private static void Execute_AllBuilds_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Textures Only/Preview")]
        private static void Execute_AllBuilds_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Textures Only/Finalized")]
        private static void Execute_AllBuilds_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Colliders Only/Working")]
        private static void Execute_AllBuilds_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Colliders Only/Preview")]
        private static void Execute_AllBuilds_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Colliders Only/Finalized")]
        private static void Execute_AllBuilds_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Impostors Only/Working")]
        private static void Execute_AllBuilds_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Impostors Only/Preview")]
        private static void Execute_AllBuilds_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, string.Empty); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Build All/Impostors Only/Finalized")]
        private static void Execute_AllBuilds_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, string.Empty); }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Full/Working")]
        private static void Execute_Pine_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Full/Preview")]
        private static void Execute_Pine_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Full/Finalized")]
        private static void Execute_Pine_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Force Full/Working")]
        private static void Execute_Pine_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Force Full/Preview")]
        private static void Execute_Pine_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Force Full/Finalized")]
        private static void Execute_Pine_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Textures Only/Working")]
        private static void Execute_Pine_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Textures Only/Preview")]
        private static void Execute_Pine_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Textures Only/Finalized")]
        private static void Execute_Pine_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Colliders Only/Working")]
        private static void Execute_Pine_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Colliders Only/Preview")]
        private static void Execute_Pine_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Colliders Only/Finalized")]
        private static void Execute_Pine_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Impostors Only/Working")]
        private static void Execute_Pine_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Impostors Only/Preview")]
        private static void Execute_Pine_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, "pine"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Pine/Impostors Only/Finalized")]
        private static void Execute_Pine_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, "pine"); }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Full/Working")]
        private static void Execute_Oak_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Full/Preview")]
        private static void Execute_Oak_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Full/Finalized")]
        private static void Execute_Oak_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Force Full/Working")]
        private static void Execute_Oak_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Force Full/Preview")]
        private static void Execute_Oak_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Force Full/Finalized")]
        private static void Execute_Oak_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Textures Only/Working")]
        private static void Execute_Oak_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Textures Only/Preview")]
        private static void Execute_Oak_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Textures Only/Finalized")]
        private static void Execute_Oak_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Colliders Only/Working")]
        private static void Execute_Oak_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Colliders Only/Preview")]
        private static void Execute_Oak_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Colliders Only/Finalized")]
        private static void Execute_Oak_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Impostors Only/Working")]
        private static void Execute_Oak_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Impostors Only/Preview")]
        private static void Execute_Oak_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, "oak"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Oak/Impostors Only/Finalized")]
        private static void Execute_Oak_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, "oak"); }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Full/Working")]
        private static void Execute_Maple_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Full/Preview")]
        private static void Execute_Maple_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Full/Finalized")]
        private static void Execute_Maple_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Force Full/Working")]
        private static void Execute_Maple_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Force Full/Preview")]
        private static void Execute_Maple_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Force Full/Finalized")]
        private static void Execute_Maple_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Textures Only/Working")]
        private static void Execute_Maple_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Textures Only/Preview")]
        private static void Execute_Maple_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Textures Only/Finalized")]
        private static void Execute_Maple_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Colliders Only/Working")]
        private static void Execute_Maple_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Colliders Only/Preview")]
        private static void Execute_Maple_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Colliders Only/Finalized")]
        private static void Execute_Maple_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Impostors Only/Working")]
        private static void Execute_Maple_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Impostors Only/Preview")]
        private static void Execute_Maple_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, "maple"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Maple/Impostors Only/Finalized")]
        private static void Execute_Maple_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, "maple"); }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Full/Working")]
        private static void Execute_Birch_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Full/Preview")]
        private static void Execute_Birch_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Full/Finalized")]
        private static void Execute_Birch_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Force Full/Working")]
        private static void Execute_Birch_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Force Full/Preview")]
        private static void Execute_Birch_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Force Full/Finalized")]
        private static void Execute_Birch_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Textures Only/Working")]
        private static void Execute_Birch_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Textures Only/Preview")]
        private static void Execute_Birch_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Textures Only/Finalized")]
        private static void Execute_Birch_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Colliders Only/Working")]
        private static void Execute_Birch_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Colliders Only/Preview")]
        private static void Execute_Birch_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Colliders Only/Finalized")]
        private static void Execute_Birch_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Impostors Only/Working")]
        private static void Execute_Birch_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Impostors Only/Preview")]
        private static void Execute_Birch_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, "birch"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Birch/Impostors Only/Finalized")]
        private static void Execute_Birch_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, "birch"); }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Full/Working")]
        private static void Execute_Hickory_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Full/Preview")]
        private static void Execute_Hickory_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Full/Finalized")]
        private static void Execute_Hickory_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Force Full/Working")]
        private static void Execute_Hickory_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Force Full/Preview")]
        private static void Execute_Hickory_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Force Full/Finalized")]
        private static void Execute_Hickory_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Textures Only/Working")]
        private static void Execute_Hickory_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Textures Only/Preview")]
        private static void Execute_Hickory_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Textures Only/Finalized")]
        private static void Execute_Hickory_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Colliders Only/Working")]
        private static void Execute_Hickory_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Colliders Only/Preview")]
        private static void Execute_Hickory_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Colliders Only/Finalized")]
        private static void Execute_Hickory_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Impostors Only/Working")]
        private static void Execute_Hickory_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Impostors Only/Preview")]
        private static void Execute_Hickory_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, "hickory"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Hickory/Impostors Only/Finalized")]
        private static void Execute_Hickory_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, "hickory"); }
        
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Full/Working")]
        private static void Execute_Ash_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Full/Preview")]
        private static void Execute_Ash_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Full/Finalized")]
        private static void Execute_Ash_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Force Full/Working")]
        private static void Execute_Ash_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Force Full/Preview")]
        private static void Execute_Ash_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Force Full/Finalized")]
        private static void Execute_Ash_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Textures Only/Working")]
        private static void Execute_Ash_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Textures Only/Preview")]
        private static void Execute_Ash_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Textures Only/Finalized")]
        private static void Execute_Ash_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Colliders Only/Working")]
        private static void Execute_Ash_CollidersOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 4, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Colliders Only/Preview")]
        private static void Execute_Ash_CollidersOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 4, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Colliders Only/Finalized")]
        private static void Execute_Ash_CollidersOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 4, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Impostors Only/Working")]
        private static void Execute_Ash_ImpostorsOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 5, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Impostors Only/Preview")]
        private static void Execute_Ash_ImpostorsOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 5, "ash"); }
        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + "Tree Tool/Trees/Ash/Impostors Only/Finalized")]
        private static void Execute_Ash_ImpostorsOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 5, "ash"); }

        public static void ExecuteAllBuilds(QualityMode qualityMode, int level, string searchString)
        {
            TreeBuildManager._executing = true;
            TreeBuildManager._autobuilding = true;
            TreeBuildManager._coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(
                TreeBuildManager.ExecuteAllBuildsEnumerator(
                    qualityMode,
                    () =>
                    {
                        if (level == 0)
                        {
                            TreeBuildRequestManager.ForceFull();
                        }
                        else if (level == 1)
                        {
                            TreeBuildRequestManager.Full();
                        }
                        else if (level == 2)
                        {
                            TreeBuildRequestManager.Default();
                        }
                        else if (level == 3)
                        {
                            TreeBuildRequestManager.TextureOnly();
                        }
                        else if (level == 4)
                        {
                            TreeBuildRequestManager.CollidersOnly();
                        }
                        else if (level == 5)
                        {
                            TreeBuildRequestManager.ImpostorsOnly();
                        }
                    },
                    searchString
                )
            );
        }
    }
}

#endif