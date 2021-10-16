#region

using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

#endregion

namespace Appalachia.Simulation.Trees.Build.Execution
{
    public static class BranchBuildManagerMenu
    {
        static BranchBuildManagerMenu()
        {
            EditorApplication.delayCall += () => EnableBuilding(true);
        }
        
        internal static void EnableBuilding(bool build)
        {
            if (build)
            {
                EditorApplication.update += BranchBuildManager.Update;
            }
            else
            {
                EditorApplication.update -= BranchBuildManager.Update;
            }
        }
        
        [MenuItem("Tools/Tree Tool/Branches/Enable Build" /*+ SHC.CTRL_ALT_SHFT_R*/, true)]
        private static bool PrefabRendering_AllowUpdatesValidate()
        {
            Menu.SetChecked("Tools/Tree Tool/Branches/Enable Build", BranchBuildManager._enabled);
            return true;
        }

        [MenuItem("Tools/Tree Tool/Branches/Enable Build" /*+ SHC.CTRL_ALT_SHFT_R*/)]
        public static void PrefabRendering_AllowUpdates()
        {
            BranchBuildManager._enabled = !BranchBuildManager._enabled;
            EnableBuilding(BranchBuildManager._enabled);
        }
        
        [MenuItem("Tools/Tree Tool/Branches/Build All/Default/Working")]
        private static void ExecuteAllBuilds_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Default/Preview")]
        private static void ExecuteAllBuilds_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Default/Finalized")]
        private static void ExecuteAllBuilds_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, string.Empty); }
        
        [MenuItem("Tools/Tree Tool/Branches/Build All/Full/Working")]
        private static void ExecuteAllBuilds_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Full/Preview")]
        private static void ExecuteAllBuilds_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Full/Finalized")]
        private static void ExecuteAllBuilds_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, string.Empty); }
        
        [MenuItem("Tools/Tree Tool/Branches/Build All/Force Full/Working")]
        private static void ExecuteAllBuilds_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Force Full/Preview")]
        private static void ExecuteAllBuilds_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Force Full/Finalized")]
        private static void ExecuteAllBuilds_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, string.Empty); }
        
        [MenuItem("Tools/Tree Tool/Branches/Build All/Textures Only/Working")]
        private static void ExecuteAllBuilds_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Textures Only/Preview")]
        private static void ExecuteAllBuilds_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, string.Empty); }
        [MenuItem("Tools/Tree Tool/Branches/Build All/Textures Only/Finalized")]
        private static void ExecuteAllBuilds_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, string.Empty); }
        
        [MenuItem("Tools/Tree Tool/Branches/Pine/Default/Working")]
        private static void Execute_Pine_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Default/Preview")]
        private static void Execute_Pine_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Default/Finalized")]
        private static void Execute_Pine_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Full/Working")]
        private static void Execute_Pine_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Full/Preview")]
        private static void Execute_Pine_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Full/Finalized")]
        private static void Execute_Pine_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Force Full/Working")]
        private static void Execute_Pine_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Force Full/Preview")]
        private static void Execute_Pine_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Force Full/Finalized")]
        private static void Execute_Pine_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Textures Only/Working")]
        private static void Execute_Pine_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Textures Only/Preview")]
        private static void Execute_Pine_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "pine"); }
        [MenuItem("Tools/Tree Tool/Branches/Pine/Textures Only/Finalized")]
        private static void Execute_Pine_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "pine"); }
        
        [MenuItem("Tools/Tree Tool/Branches/Oak/Default/Working")]
        private static void Execute_Oak_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Default/Preview")]
        private static void Execute_Oak_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Default/Finalized")]
        private static void Execute_Oak_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Full/Working")]
        private static void Execute_Oak_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Full/Preview")]
        private static void Execute_Oak_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Full/Finalized")]
        private static void Execute_Oak_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Force Full/Working")]
        private static void Execute_Oak_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Force Full/Preview")]
        private static void Execute_Oak_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Force Full/Finalized")]
        private static void Execute_Oak_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Textures Only/Working")]
        private static void Execute_Oak_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Textures Only/Preview")]
        private static void Execute_Oak_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "oak"); }
        [MenuItem("Tools/Tree Tool/Branches/Oak/Textures Only/Finalized")]
        private static void Execute_Oak_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "oak"); }
        
        [MenuItem("Tools/Tree Tool/Branches/Maple/Default/Working")]
        private static void Execute_Maple_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Default/Preview")]
        private static void Execute_Maple_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Default/Finalized")]
        private static void Execute_Maple_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Full/Working")]
        private static void Execute_Maple_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Full/Preview")]
        private static void Execute_Maple_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Full/Finalized")]
        private static void Execute_Maple_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Force Full/Working")]
        private static void Execute_Maple_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Force Full/Preview")]
        private static void Execute_Maple_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Force Full/Finalized")]
        private static void Execute_Maple_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Textures Only/Working")]
        private static void Execute_Maple_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Textures Only/Preview")]
        private static void Execute_Maple_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "maple"); }
        [MenuItem("Tools/Tree Tool/Branches/Maple/Textures Only/Finalized")]
        private static void Execute_Maple_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "maple"); }
        
        [MenuItem("Tools/Tree Tool/Branches/Birch/Default/Working")]
        private static void Execute_Birch_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Default/Preview")]
        private static void Execute_Birch_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Default/Finalized")]
        private static void Execute_Birch_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Full/Working")]
        private static void Execute_Birch_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Full/Preview")]
        private static void Execute_Birch_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Full/Finalized")]
        private static void Execute_Birch_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Force Full/Working")]
        private static void Execute_Birch_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Force Full/Preview")]
        private static void Execute_Birch_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Force Full/Finalized")]
        private static void Execute_Birch_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Textures Only/Working")]
        private static void Execute_Birch_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Textures Only/Preview")]
        private static void Execute_Birch_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "birch"); }
        [MenuItem("Tools/Tree Tool/Branches/Birch/Textures Only/Finalized")]
        private static void Execute_Birch_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "birch"); }
        
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Default/Working")]
        private static void Execute_Hickory_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Default/Preview")]
        private static void Execute_Hickory_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Default/Finalized")]
        private static void Execute_Hickory_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Full/Working")]
        private static void Execute_Hickory_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Full/Preview")]
        private static void Execute_Hickory_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Full/Finalized")]
        private static void Execute_Hickory_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Force Full/Working")]
        private static void Execute_Hickory_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Force Full/Preview")]
        private static void Execute_Hickory_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Force Full/Finalized")]
        private static void Execute_Hickory_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Textures Only/Working")]
        private static void Execute_Hickory_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Textures Only/Preview")]
        private static void Execute_Hickory_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "hickory"); }
        [MenuItem("Tools/Tree Tool/Branches/Hickory/Textures Only/Finalized")]
        private static void Execute_Hickory_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "hickory"); }
        
        [MenuItem("Tools/Tree Tool/Branches/Ash/Default/Working")]
        private static void Execute_Ash_Default_Working() { ExecuteAllBuilds(QualityMode.Working, 2, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Default/Preview")]
        private static void Execute_Ash_Default_Preview() { ExecuteAllBuilds(QualityMode.Preview, 2, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Default/Finalized")]
        private static void Execute_Ash_Default_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 2, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Full/Working")]
        private static void Execute_Ash_Full_Working() { ExecuteAllBuilds(QualityMode.Working, 1, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Full/Preview")]
        private static void Execute_Ash_Full_Preview() { ExecuteAllBuilds(QualityMode.Preview, 1, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Full/Finalized")]
        private static void Execute_Ash_Full_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 1, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Force Full/Working")]
        private static void Execute_Ash_ForceFull_Working() { ExecuteAllBuilds(QualityMode.Working, 0, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Force Full/Preview")]
        private static void Execute_Ash_ForceFull_Preview() { ExecuteAllBuilds(QualityMode.Preview, 0, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Force Full/Finalized")]
        private static void Execute_Ash_ForceFull_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 0, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Textures Only/Working")]
        private static void Execute_Ash_TexturesOnly_Working() { ExecuteAllBuilds(QualityMode.Working, 3, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Textures Only/Preview")]
        private static void Execute_Ash_TexturesOnly_Preview() { ExecuteAllBuilds(QualityMode.Preview, 3, "ash"); }
        [MenuItem("Tools/Tree Tool/Branches/Ash/Textures Only/Finalized")]
        private static void Execute_Ash_TexturesOnly_Finalized() { ExecuteAllBuilds(QualityMode.Finalized, 3, "ash"); }

        public static void ExecuteAllBuilds(QualityMode qualityMode, int level, string searchString)
        {
            BranchBuildManager._executing = true;
            BranchBuildManager._autobuilding = true;
            BranchBuildManager._coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(
                BranchBuildManager.ExecuteAllBuildsEnumerator(
                    qualityMode,
                    () =>
                    {
                        if (level == 0)
                        {
                            BranchBuildRequestManager.ForceFull();
                        }
                        else if (level == 1)
                        {
                            BranchBuildRequestManager.Full();
                        }
                        else if (level == 2)
                        {
                            BranchBuildRequestManager.Default();
                        }
                    },
                    searchString
                )
            );
        }

    }
}