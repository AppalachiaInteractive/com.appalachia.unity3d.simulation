#region

using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.GUI;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.UI.Model
{
    public class ModelSelectionDrawer : OdinValueDrawer<ModelSelection>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var selection = ValueEntry.SmartValue;

            var model = Property.Tree.WeakTargets[0] as TreeModel;

            if (selection.container == null)
            {
                if (model == null)
                {
                    SirenixEditorGUI.ErrorMessageBox("An error occurred.  The tree model container needs to be set.");
                    return;
                }

                var name = model.name.Replace("Tree Model - ", string.Empty);

                var datas = AssetDatabaseManager.FindAssets("t: TreeDataContainer");

                if ((datas == null) || (datas.Length == 0))
                {
                    SirenixEditorGUI.ErrorMessageBox("An error occurred.  The tree data container can not be found.");
                    return;
                }

                foreach (var data in datas)
                {
                    var path = AssetDatabaseManager.GUIDToAssetPath(data);
                    var tree = AssetDatabaseManager.LoadAssetAtPath<TreeDataContainer>(path);

                    if (tree.species == null)
                    {
                        continue;
                    }

                    if (tree.species.nameBasis.nameBasis == name)
                    {
                        model._containerSO = tree.GetSerializable();
                        selection.container = tree;
                        break;
                    }
                }
            }

            if ((selection.container == null) || (model._containerSO == null))
            {
                SirenixEditorGUI.ErrorMessageBox("An error occurred.  The tree model container needs to be set.  It was not found during a search");
                return;
            }

            SirenixEditorFields.RangeIntField(
                TreeGUI.Content.Label("Individual"),
                selection.individualSelection,
                0,
                selection.maxIndividual,
                TreeGUI.Layout.Options.None
            );

            var container = selection.container as TreeDataContainer;

            var individual = container?.individuals?[selection.individualSelection];

            TreeGUI.Draw.Title("Show model information", string.Empty);

            var commonAction = new Action(
                () =>
                {
                    EditorSceneManager.MarkSceneDirty(model.gameObject.scene); 
                    EditorApplication.QueuePlayerLoopUpdate();
                    SceneView.RepaintAll();
                    
                });

            using (TreeGUI.Layout.Horizontal())
            {
                TreeGUI.Button.Context(
                    model.visible,
                    TreeIcons.visible,
                    TreeIcons.disabledVisible,
                    "Hide model",
                    "Show model",
                    () =>
                    {
                        model.visible = false;
                        commonAction();
                    },
                    () =>
                    {
                        model.visible = true;
                        commonAction();
                    },
                    TreeGUI.Styles.ButtonLeft,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(32f)
                );

                var tree = model._containerSO as TreeDataContainer;

                TreeGUI.Button.Context(
                    tree.drawGizmos,
                    TreeIcons.scene,
                    TreeIcons.disabledScene,
                    "Stop drawing gizmos",
                    "Start drawing gizmos",
                    () =>
                    {
                        tree._drawGizmos = false;
                        commonAction();
                    },
                    () =>
                    {
                        tree._drawGizmos = true;
                        commonAction();
                    },
                    TreeGUI.Styles.ButtonMid,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(32f)
                );
                
                TreeGUI.Button.Context(
                    model.autoUpdateSceneView,
                    TreeIcons.unlocked,
                    TreeIcons.locked,
                    "Lock scene view to prevent automatic camera changes.",
                    "Unlock scene view to allow automatic camera changes.",
                    () =>
                    {
                        model.autoUpdateSceneView = !model.autoUpdateSceneView;
                        commonAction();
                    },
                    () =>
                    {
                        model.autoUpdateSceneView = !model.autoUpdateSceneView;
                        commonAction();
                    },
                    TreeGUI.Styles.ButtonRight,
                    TreeGUI.Layout.Options /*.MinWidth(width)*/
                        .MaxHeight(32f)
                );
            }

            TreeEditorGenerationToggleMenuManager.DrawAgeToolbar(
                container, 
                individual,
                a => individual.HasType(a),
                a => selection.ageSelection == a,
                "Hide",
                "Show",
                value =>
                {
                    selection.ageSelection = value;
                    commonAction();
                },
                value =>
                {
                    selection.ageSelection = value;
                    commonAction();
                },
                32f,
                64f
            );

            var age = individual?[selection.ageSelection];

            TreeEditorGenerationToggleMenuManager.DrawStageToolbar(
                container,
                age,
                s => age.HasType(s),
                s => selection.stageSelection == s,
                "Hide",
                "Show",
                value =>
                {
                    selection.stageSelection = StageType.Normal;
                    commonAction();
                },
                value =>
                {
                    selection.stageSelection = value;
                    commonAction();
                },
                32f,
                64f
            );
        }
    }
}
