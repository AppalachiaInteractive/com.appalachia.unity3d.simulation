/*using System;
using Appalachia.Simulation.Trees.Building;
using Appalachia.Simulation.Trees.Metadata;
using Appalachia.Simulation.Trees.Metadata.Build;
using Appalachia.Simulation.Trees.Metadata.Icons;
using Appalachia.Simulation.Trees.Metadata.Settings;
using Appalachia.Simulation.Trees.Runtime.Editing;

using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees
{
    public static class TreeSpeciesBuildToolbar
    {
        public static void DrawBuildToolbar(TreeDataContainer tree, float buttonHeight, float previewWidth)
        {
            TreeGUI.Button.EnableDisable(
                !tree.progressTracker.buildActive,
                TreeIcons.bomb,
                TreeIcons.disabledBomb,
                "Nuclear rebuild",
                () =>
                {
                    tree.RecordUndo(TreeEditMode.BuildTrees);
                    TreeBuildRequestManager.ForceFull();
                },
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight) //.MaxWidth(buttonWidth/4f)
            );

            TreeGUI.Button.EnableDisable(
                !tree.progressTracker.buildActive,
                TreeIcons.hammer,
                TreeIcons.disabledHammer,
                "Full rebuild",
                () =>
                {
                    tree.RecordUndo(TreeEditMode.BuildTrees);
                    TreeBuildRequestManager.Full();
                },
                TreeGUI.Styles.ButtonMid,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );
            
            TreeGUI.Button.EnableDisable(
                !tree.progressTracker.buildActive ,
                TreeIcons.repair,
                TreeIcons.disabledRepair,
                "Default rebuild",
                () =>
                {
                    tree.RecordUndo(TreeEditMode.BuildTrees);
                    TreeBuildRequestManager.Default();
                },
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            TreeGUI.Button.EnableDisable(
                tree.buildState == BuildState.Disabled,
                TreeIcons.play,
                TreeIcons.disabledPlay,
                $"Enable auto-building",
                () => { tree.buildState = BuildState.Default; },
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            TreeGUI.Button.EnableDisable(
                tree.buildState != BuildState.Disabled,
                TreeIcons.pause,
                TreeIcons.disabledPause,
                $"Disable auto-building",
                () => { tree.buildState = BuildState.Disabled; },
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            TreeGUI.Button.Toolbar(
                true,
                tree.settings.qualityMode == QualityMode.Finalized,
                TreeIcons.preview,
                TreeIcons.disabledPreview,
                $"Enable final quality",
                () => { tree.settings.qualityMode = QualityMode.Finalized; },
                TreeGUI.Styles.ButtonLeftSelected,
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );

            TreeGUI.Button.Toolbar(
                true,
                tree.settings.qualityMode == QualityMode.Preview,
                TreeIcons.preview2,
                TreeIcons.disabledPreview2,
                $"Enable preview quality",
                () => { tree.settings.qualityMode = QualityMode.Preview; },
                TreeGUI.Styles.ButtonMidSelected,
                TreeGUI.Styles.ButtonMid,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );

            TreeGUI.Button.Toolbar(
                true,
                tree.settings.qualityMode == QualityMode.Working,
                TreeIcons.preview3,
                TreeIcons.disabledPreview3,
                $"Enable working quality",
                () => { tree.settings.qualityMode = QualityMode.Working; },
                TreeGUI.Styles.ButtonRightSelected,
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );

            TreeGUI.Button.EnableDisable(
                tree.dataState == TSEDataContainer.DataState.PendingSave,
                TreeIcons.save,
                TreeIcons.disabledSave,
                $"Save assets",
                tree.Save,
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );
        }

        public static void DrawBuildProgress(TreeDataContainer tree, float buttonHeight)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.height = buttonHeight;

            var width = Mathf.Clamp01(tree.progressTracker.buildProgress);

            if (width == 0 && tree.progressTracker.buildMessage == null)
            {  
                TreeGUI.Draw.Solid(rect, GetProgressBarConfig(tree, buttonHeight).BackgroundColor);
            }
            else
            {  SirenixEditorFields.ProgressBarField(
                    rect,
                    GUIContent.none,
                    width,
                    0f,
                    1f,
                    GetProgressBarConfig(tree, buttonHeight),
                    tree.progressTracker.buildMessage
                );
            }
          
        }

        private static ProgressBarConfig _progressBarActiveConfig;
        private static ProgressBarConfig _progressBarDirtyConfig;
        private static ProgressBarConfig _progressBarFailedConfig;
        private static ProgressBarConfig _progressBarCompleteConfig;

        private static ProgressBarConfig GetProgressBarConfig(TreeDataContainer tree, float buttonHeight)
        {
            if (_progressBarActiveConfig.Equals(default(ProgressBarConfig)))
            {
                _progressBarActiveConfig = new ProgressBarConfig(
                    (int) buttonHeight,
                    TreeGUI.Colors.BurntOrange,
                    TreeGUI.Colors.LightBeige,
                    true,
                    TextAlignment.Center
                );
                
                _progressBarDirtyConfig = new ProgressBarConfig(
                    (int) buttonHeight,
                    TreeGUI.Colors.LightBeige,
                    TreeGUI.Colors.BurntOrange,
                    true,
                    TextAlignment.Center
                );
                
                _progressBarFailedConfig = new ProgressBarConfig(
                    (int) buttonHeight,
                    TreeGUI.Colors.LightBeige,
                    TreeGUI.Colors.DarkRed,
                    true,
                    TextAlignment.Center
                );
                _progressBarCompleteConfig = new ProgressBarConfig(
                    (int) buttonHeight,
                    TreeGUI.Colors.LightBeige,
                    TreeGUI.Colors.DarkGreen,
                    true,
                    TextAlignment.Center
                );
            }

            switch (tree.progressTracker.buildResult)
            {
                case BuildResult.Normal:
                    return _progressBarDirtyConfig;
                
                case BuildResult.Success:
                    if (tree.dataState == TSEDataContainer.DataState.PendingSave)
                    {
                        return _progressBarDirtyConfig;
                    }
                    
                    return _progressBarCompleteConfig;
                case BuildResult.InProgress:
                    return _progressBarActiveConfig;
                case BuildResult.Error:
                    return _progressBarFailedConfig;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}*/