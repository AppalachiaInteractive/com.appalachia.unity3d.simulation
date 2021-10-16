using System;
using System.Linq;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.GUI;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI
{
    public static class TreeEditorGenerationToggleMenuManager
    {
        public static void DrawAgeToolbar(
            TreeDataContainer tree,
            TreeIndividual set,
            Func<AgeType, bool> isEnabled,
            string trueTooltipVerb,
            string falseTooltipVerb,
            Action<AgeType> trueAction,
            Action<AgeType> falseAction,
            float minWidth = 32f,
            float height = 32f)
        {
            DrawAgeToolbar(
                tree,
                set,
                isEnabled,
                (age) => set.HasType(age),
                trueTooltipVerb,
                falseTooltipVerb,
                trueAction,
                falseAction,
                minWidth,
                height);
        }
        

        public static void DrawAgeToolbar(
            TreeDataContainer tree,
            TreeIndividual set,
            Func<AgeType, bool> isEnabled,
            Func<AgeType, bool> contextCondition,
            string trueTooltipVerb,
            string falseTooltipVerb,
            Action<AgeType> trueAction,
            Action<AgeType> falseAction,
            float minWidth = 32f,
            float height = 32f)
        {
            using (TreeGUI.Layout.Vertical())
            {
                TreeGUI.Draw.Title(
                    $"{trueTooltipVerb} or {falseTooltipVerb.ToLower()} ages", string.Empty
                );

                using (TreeGUI.Layout.Horizontal())
                {
                    if (set == null)
                    {
                        var disabledButtonAction = new Action<AgeType, GUIStyle>(
                            (ageType, style) =>
                            {
                                GUILayout.Button(
                                    GetIcon(ageType, false).Get(),
                                    style,
                                    TreeGUI.Layout.Options.Height(height).MinWidth(minWidth)
                                        .ExpandWidth()
                                );
                            }
                        );

                        using (TreeGUI.Enabled.Never())
                        {
                            disabledButtonAction(AgeType.Sapling, TreeGUI.Styles.ButtonLeft);
                            disabledButtonAction(AgeType.Young, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(AgeType.Adult, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(AgeType.Mature, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(AgeType.Spirit, TreeGUI.Styles.ButtonRight);
                        }
                    }
                    else
                    {
                        var buttonAction = new Action<AgeType, GUIStyle>(
                            (ageType, style) =>
                            {
                                TreeGUI.Button.ContextEnableDisable(
                                    isEnabled(ageType),
                                    contextCondition(ageType),
                                    GetIcon(ageType, true),
                                    GetIcon(ageType, false),
                                    $"{trueTooltipVerb} {ageType.ToString().ToLower()}",
                                    $"{falseTooltipVerb} {ageType.ToString().ToLower()}",
                                    () => trueAction(ageType),
                                    () => falseAction(ageType),
                                    style,
                                    TreeGUI.Layout.Options.Height(height).MinWidth(minWidth)
                                        .ExpandWidth()
                                );
                            }
                        );

                        buttonAction(AgeType.Sapling, TreeGUI.Styles.ButtonLeft);
                        buttonAction(AgeType.Young, TreeGUI.Styles.ButtonMid);
                        buttonAction(AgeType.Adult, TreeGUI.Styles.ButtonMid);
                        buttonAction(AgeType.Mature, TreeGUI.Styles.ButtonMid);
                        buttonAction(AgeType.Spirit, TreeGUI.Styles.ButtonRight);
                    }
                }
            }
        }

        public static void DrawStageToolbar(
            TreeDataContainer tree,
            TreeAge age,
            Func<StageType, bool> isEnabled,
            string trueTooltipVerb,
            string falseTooltipVerb,
            Action<StageType> trueAction,
            Action<StageType> falseAction,
            float minWidth = 32f,
            float height = 32f)
        {
            DrawStageToolbar(
                tree, 
                age,
                isEnabled,
                (stage) => age.HasType(stage),
                trueTooltipVerb,
                falseTooltipVerb,
                trueAction,
                falseAction,
                minWidth,
                height
            );
        }
        
        public static void DrawStageToolbar(
            TreeDataContainer tree,
            TreeAge age,
            Func<StageType, bool> isEnabled,
            Func<StageType, bool> contextCondition,
            string trueTooltipVerb,
            string falseTooltipVerb,
            Action<StageType> trueAction,
            Action<StageType> falseAction,
            float minWidth = 32f,
            float height = 32f)
        {
            using (TreeGUI.Layout.Vertical())
            {
                TreeGUI.Draw.Title(
                    $"{trueTooltipVerb} or {falseTooltipVerb.ToLower()} variants", string.Empty
                );
                
                if (age == null)
                {
                    var disabledButtonAction = new Action<StageType, GUIStyle>(
                        (stageType, style) =>
                        {
                            GUILayout.Button(
                                GetIcon(stageType, false).Get(),
                                style,
                                TreeGUI.Layout.Options.Height(height).MinWidth(minWidth)
                                    .ExpandWidth()
                            );
                        }
                    );

                    using (TreeGUI.Enabled.Never())
                    {
                        using (TreeGUI.Layout.Horizontal())
                        {
                            disabledButtonAction(StageType.Stump, TreeGUI.Styles.ButtonLeft);
                            disabledButtonAction(StageType.Felled, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(StageType.FelledBare, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(StageType.FelledBareRotted, TreeGUI.Styles.ButtonRight);
                        }

                        using (TreeGUI.Layout.Horizontal())
                        {
                            
                            disabledButtonAction(StageType.StumpRotted, TreeGUI.Styles.ButtonLeft);
                            disabledButtonAction(StageType.Dead, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(StageType.DeadFelled, TreeGUI.Styles.ButtonMid);
                            disabledButtonAction(StageType.DeadFelledRotted, TreeGUI.Styles.ButtonRight);
                        }
                    }
                }
                else
                {
                    var buttonAction = new Action<StageType, GUIStyle>(
                        (stageType, style) =>
                        {
                            var friendly = new string(
                                stageType.ToString().SelectMany(
                                    ac => char.IsUpper(ac) ? new[] {' ', ac} : new[] {ac}
                                ).ToArray()
                            );

                            TreeGUI.Button.ContextEnableDisable(
                                isEnabled(stageType),
                                contextCondition(stageType),
                                GetIcon(stageType, true),
                                GetIcon(stageType, false),
                                $"{trueTooltipVerb}{friendly} variant",
                                $"{falseTooltipVerb}{friendly} variant",
                                () => trueAction(stageType),
                                () => falseAction(stageType),
                                style,
                                TreeGUI.Layout.Options.Height(height).MinWidth(minWidth)
                                    .ExpandWidth()
                            );
                        }
                    );
                    
                    using (TreeGUI.Layout.Horizontal())
                    {
                        buttonAction(StageType.Stump, TreeGUI.Styles.ButtonLeft);
                        buttonAction(StageType.Felled, TreeGUI.Styles.ButtonMid);
                        buttonAction(StageType.FelledBare, TreeGUI.Styles.ButtonMid);
                        buttonAction(StageType.FelledBareRotted, TreeGUI.Styles.ButtonRight);
                    }

                    using (TreeGUI.Layout.Horizontal())
                    {
                        buttonAction(StageType.StumpRotted, TreeGUI.Styles.ButtonLeft);
                        buttonAction(StageType.Dead, TreeGUI.Styles.ButtonMid);
                        buttonAction(StageType.DeadFelled, TreeGUI.Styles.ButtonMid);
                        buttonAction(StageType.DeadFelledRotted, TreeGUI.Styles.ButtonRight);
                    }
                }
            }
        }


        public static void DrawEditorAgeToolbar(
            TreeDataContainer tree,
            TreeIndividual set,
            float height)
        {
            var commonAction = new Action(
                () =>
                {
                    tree.RebuildStructures();
                    tree.graphDirty = true;
                    tree.dataState = TSEDataContainer.DataState.Dirty;
                    TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
                }
            );

            DrawAgeToolbar(
                tree,
                set,
                age => !set.HasType(age) || (set.HasType(age) && (set.ageCount > 1)),
                "Remove",
                "Add",
                (age) =>
                {                   
                    set.RemoveAge(age);
                    commonAction();
                },
                (age) =>
                {
                    var asset = TreeAsset.Create(tree.subfolders.data, tree.species.nameBasis, set.individualID, age, StageType.Normal);
                    
                    set.AddAge(tree.subfolders.data, tree.species.nameBasis, age, asset);
                    
                    commonAction();
                },
                32f,
                height
            );
        }

        public static void DrawEditorStageToolbar(
            TreeDataContainer tree,
            TreeIndividual set,
            TreeAge age,
            float height)
        {
            var commonAction = new Action(
                () =>
                {
                    tree.RebuildStructures();
                    tree.graphDirty = true;
                    tree.dataState = TSEDataContainer.DataState.Dirty;
                    TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
                }
            );

            DrawStageToolbar(
                tree,
                age,
                stage => true,
                "Remove",
                "Add",
                (stage) =>
                {
                    age.RemoveVariant(stage);
                    commonAction();
                },
                (stage) =>
                {
                    var asset = TreeAsset.Create(
                        tree.subfolders.data,
                        tree.species.nameBasis,
                        age.individualID,
                        age.ageType,
                        stage
                    );
                    age.AddVariant(tree.subfolders.data, tree.species.nameBasis, stage, asset);
                    commonAction();
                },
                32f,
                height
            );
        }

        private static TreeIcon GetIcon(AgeType age, bool enabled)
        {
            switch (age)
            {
                case AgeType.Sapling:
                    return enabled ? TreeIcons.age10 : TreeIcons.disabledAge10;

                case AgeType.Young:
                    return enabled ? TreeIcons.age20 : TreeIcons.disabledAge20;

                case AgeType.Adult:
                    return enabled ? TreeIcons.age35 : TreeIcons.disabledAge35;

                case AgeType.Mature:
                    return enabled ? TreeIcons.age50 : TreeIcons.disabledAge50;

                case AgeType.Spirit:
                    return enabled ? TreeIcons.spirit : TreeIcons.disabledSpirit;

                default:
                    throw new ArgumentOutOfRangeException(nameof(age), age, null);
            }
        }

        private static TreeIcon GetIcon(StageType age, bool enabled)
        {
            switch (age)
            {
                case StageType.Normal:
                    return enabled ? TreeIcons.newTree : TreeIcons.disabledNewTree;

                case StageType.Stump:
                    return enabled ? TreeIcons.stump : TreeIcons.disabledStump;

                case StageType.StumpRotted:
                    return enabled ? TreeIcons.stumpRotted : TreeIcons.disabledStumpRotted;

                case StageType.Felled:
                    return enabled ? TreeIcons.felled : TreeIcons.disabledFelled;

                case StageType.FelledBare:
                    return enabled ? TreeIcons.felledBare : TreeIcons.disabledFelledBare;

                case StageType.FelledBareRotted:
                    return enabled ? TreeIcons.felledBareRotted : TreeIcons.disabledFelledBareRotted;

                case StageType.Dead:
                    return enabled ? TreeIcons.dead : TreeIcons.disabledDead;

                case StageType.DeadFelled:
                    return enabled ? TreeIcons.deadFelled : TreeIcons.disabledDeadFelled;

                case StageType.DeadFelledRotted:
                    return enabled ? TreeIcons.deadFelledRotted : TreeIcons.disabledDeadFelledRotted;

                default:
                    throw new ArgumentOutOfRangeException(nameof(age), age, null);
            }
        }

    }
}