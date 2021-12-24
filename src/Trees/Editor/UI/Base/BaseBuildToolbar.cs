using System;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Settings;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedParameter.Global

namespace Appalachia.Simulation.Trees.UI.Base
{
    public abstract class BaseBuildToolbar<T>
        where T: TSEDataContainer
    {
        protected abstract QualityMode GetQuality(T data);
        
        protected abstract void SetQuality(T data, QualityMode mode);

        protected abstract void Full();
        
        protected abstract void Default();

        protected abstract void TextureOnly();
        
        protected abstract bool SupportsColliderOnly { get; }

        protected abstract void CollidersOnly();

        protected abstract bool SupportsImpostorOnly { get; }
        
        protected abstract void ImpostorsOnly();
        
        protected abstract void ForceFull();

        public void DrawBuildToolbar(T data, float buttonHeight, float previewWidth)
        {
            using (TreeGUI.Layout.Vertical())
            {
                using (TreeGUI.Layout.Horizontal())
                {
                    DrawBuildMainToolbar(data, buttonHeight, previewWidth);
                }
                
                using (TreeGUI.Layout.Horizontal())
                {
                    DrawBuildSubToolbar(data, buttonHeight, previewWidth);
                }
            }
        }
        
        public void DrawBuildMainToolbar(T data, float buttonHeight, float previewWidth)
        {
            TreeGUI.Button.EnableDisable(
                !data.progressTracker.buildActive,
                TreeIcons.bomb,
                TreeIcons.disabledBomb,
                "Nuclear rebuild",
                () =>
                {
                    data.RecordUndo(TreeEditMode.BuildTrees);
                    ForceFull();
                },
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight) //.MaxWidth(buttonWidth/4f)
            );

            TreeGUI.Button.EnableDisable(
                !data.progressTracker.buildActive,
                TreeIcons.hammer,
                TreeIcons.disabledHammer,
                "Full rebuild",
                () =>
                {
                    data.RecordUndo(TreeEditMode.BuildTrees);
                    Full();
                },
                TreeGUI.Styles.ButtonMid,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );
            
            TreeGUI.Button.EnableDisable(
                !data.progressTracker.buildActive ,
                TreeIcons.repair,
                TreeIcons.disabledRepair,
                "Default rebuild",
                () =>
                {
                    data.RecordUndo(TreeEditMode.BuildTrees);
                    Default();
                },
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            TreeGUI.Button.EnableDisable(
                !data.progressTracker.buildActive && data is TreeDataContainer,
                TreeIcons.paint,
                TreeIcons.disabledPaint,
                "Texture rebuild",
                () =>
                {
                    data.RecordUndo(TreeEditMode.BuildTrees);
                    TextureOnly();
                },
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            if (SupportsColliderOnly)
            {
                TreeGUI.Button.EnableDisable(
                    !data.progressTracker.buildActive && data is TreeDataContainer,
                    TreeIcons.collision,
                    TreeIcons.disabledCollision,
                    "Collider rebuild",
                    () =>
                    {
                        data.RecordUndo(TreeEditMode.BuildTrees);
                        CollidersOnly();
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options.MaxHeight(buttonHeight)
                );
            }

            if (SupportsImpostorOnly)
            {
                TreeGUI.Button.EnableDisable(
                    !data.progressTracker.buildActive && data is TreeDataContainer,
                    TreeIcons.impostor,
                    TreeIcons.disabledImpostor,
                    "Impostor rebuild",
                    () =>
                    {
                        data.RecordUndo(TreeEditMode.BuildTrees);
                        ImpostorsOnly();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options.MaxHeight(buttonHeight)
                );
            }

            TreeGUI.Button.EnableDisable(
                data.progressTracker.buildActive ,
                TreeIcons.x,
                TreeIcons.disabledX,
                "Cancel",
                () =>
                {
                    data.buildState = BuildState.Cancelled;
                    data.progressTracker.CompleteBuildBatch(false);
                },
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );
        }
        
        public void DrawBuildSubToolbar(T data, float buttonHeight, float previewWidth)
        {
            TreeGUI.Button.EnableDisable(
                (data.buildState == BuildState.Disabled) && (data.buildState != BuildState.Cancelled),
                TreeIcons.play,
                TreeIcons.disabledPlay,
                $"Enable auto-building",
                () => { data.buildState = BuildState.Default; },
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            TreeGUI.Button.EnableDisable(
                (data.buildState != BuildState.Disabled) && (data.buildState != BuildState.Cancelled),
                TreeIcons.pause,
                TreeIcons.disabledPause,
                $"Disable auto-building",
                () => { data.buildState = BuildState.Disabled; },
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight)
            );

            var qualityMode = GetQuality(data);
            
            TreeGUI.Button.Toolbar(
                qualityMode != QualityMode.Finalized,
                qualityMode == QualityMode.Finalized,
                TreeIcons.preview,
                TreeIcons.disabledPreview,
                $"Enable final quality",
                () => { SetQuality(data, QualityMode.Finalized); },
                TreeGUI.Styles.ButtonLeftSelected,
                TreeGUI.Styles.ButtonLeft,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );

            TreeGUI.Button.Toolbar(
                qualityMode != QualityMode.Preview,
                qualityMode == QualityMode.Preview,
                TreeIcons.preview2,
                TreeIcons.disabledPreview2,
                $"Enable preview quality",
                () => { SetQuality(data, QualityMode.Preview); },
                TreeGUI.Styles.ButtonMidSelected,
                TreeGUI.Styles.ButtonMid,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );

            TreeGUI.Button.Toolbar(
                qualityMode != QualityMode.Working,
                qualityMode == QualityMode.Working,
                TreeIcons.preview3,
                TreeIcons.disabledPreview3,
                $"Enable working quality",
                () => { SetQuality(data, QualityMode.Working); },
                TreeGUI.Styles.ButtonRightSelected,
                TreeGUI.Styles.ButtonRight,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );

            TreeGUI.Button.EnableDisable(
                data.dataState == TSEDataContainer.DataState.PendingSave,
                TreeIcons.save,
                TreeIcons.disabledSave,
                $"Save assets",
                () => data.Save(true),
                TreeGUI.Styles.Button,
                TreeGUI.Layout.Options.MaxHeight(buttonHeight).MaxWidth(previewWidth)
            );
        }

        public void DrawBuildProgress(T tree, float buttonHeight)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.height = buttonHeight;

            var width = Mathf.Clamp01(tree.progressTracker.buildProgress);

            if ((width == 0) && (tree.progressTracker.buildMessage == null))
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

        protected ProgressBarConfig _progressBarActiveConfig;
        protected ProgressBarConfig _progressBarDirtyConfig;
        protected ProgressBarConfig _progressBarFailedConfig;
        protected ProgressBarConfig _progressBarCompleteConfig;

        protected ProgressBarConfig GetProgressBarConfig(T tree, float buttonHeight)
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
}
        