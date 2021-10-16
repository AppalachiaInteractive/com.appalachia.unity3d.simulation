using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Attributes
{
    public class TreeCurveAttributeDrawer : OdinAttributeDrawer<TreeCurveAttribute, AnimationCurve>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            AnimationCurve tempCurve = ValueEntry.SmartValue;
            
            var range = new Rect(
                Attribute.MinTime,
                Attribute.MinValue,
                Attribute.MaxTime - Attribute.MinTime,
                Attribute.MaxValue - Attribute.MinValue
            );

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.CurveField(tempCurve, Color.green, range);
            
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = tempCurve;

                var parentValue = Property.ParentValueProperty;

                var wsValue = parentValue.ValueEntry.WeakSmartValue;

                if (wsValue is ResponsiveSettings r)
                {
                    if (r.settingsType == ResponsiveSettingsType.Tree)
                    {
                        TreeBuildRequestManager.SettingsChanged(Attribute.target);
                    }
                    else if (r.settingsType == ResponsiveSettingsType.Log)
                    {
                        LogBuildRequestManager.SettingsChanged(Attribute.target);
                    }
                    else
                    {
                        BranchBuildRequestManager.SettingsChanged(Attribute.target);
                    }
                }
            }
        }
    }
}
