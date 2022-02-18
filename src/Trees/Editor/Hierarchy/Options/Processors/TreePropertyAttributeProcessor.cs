using System;
using System.Collections.Generic;
using System.Reflection;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Curves;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Utility.Reflection.Extensions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Processors
{
    [ResolverPriority(500)]
    public class TreePropertyAttributeProcessor : OdinAttributeProcessor
    {
        /// <inheritdoc />
        public override void ProcessChildMemberAttributes(
            InspectorProperty parentProperty,
            MemberInfo member,
            List<Attribute> attributes)
        {
            if ((member.Name != "accessor") && (member.Name != "value") && (member.Name != "curve"))
            {
                return;
            }

            var thisType = member.GetReturnType();

            if (typeof(structCurve).IsAssignableFrom(thisType) && (member.Name == "accessor"))
            {
                return;
            }

            var parentType = parentProperty.Info.TypeOfValue;
            var grandparentType = parentProperty.ParentType;

            var validParent = typeof(TreeProperty).IsAssignableFrom(parentType);
            var validGrandParent = typeof(TreeProperty).IsAssignableFrom(grandparentType);

            if (!validParent && !validGrandParent)
            {
                return;
            }

            var atties = validParent
                ? parentProperty.Info.GetMemberInfo()?.GetCustomAttributes()
                : parentProperty.Parent?.Parent?.Info?.GetMemberInfo()?.GetCustomAttributes();

            foreach (var attribute in atties)
            {
                if (ShouldPushToChildren(attribute))
                {
                    if (attribute is TreeCurveAttribute)
                    {
                        if (member.Name == "curve")
                        {
                            attributes.Add(attribute);
                        }
                        else if ((thisType == typeof(AnimationCurve)) && (member.Name == "accessor"))
                        {
                            attributes.Add(attribute);
                        }
                    }
                    else
                    {
                        if (member.Name != "curve")
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            if (property.Info.TypeOfValue == null)
            {
                return;
            }

            var match = typeof(TreeProperty).IsAssignableFrom(property.Info.TypeOfValue);

            if (match)
            {
                for (var i = attributes.Count - 1; i >= 0; i--)
                {
                    if (ShouldPushToChildren(attributes[i]))
                    {
                        attributes.Remove(attributes[i]);
                    }
                }
            }
        }

        private static bool ShouldPushToChildren(Attribute a)
        {
            if (a is OnValueChangedAttribute)
            {
                return true;
            }

            if (a is PropertyRangeAttribute)
            {
                return true;
            }

            if (a is RangeAttribute)
            {
                return true;
            }

            if (a is MinValueAttribute)
            {
                return true;
            }

            if (a is MinAttribute)
            {
                return true;
            }

            if (a is MaxValueAttribute)
            {
                return true;
            }

            if (a is MinMaxSliderAttribute)
            {
                return true;
            }

            if (a is ToggleLeftAttribute)
            {
                return true;
            }

            if (a is TreeCurveAttribute)
            {
                return true;
            }

            return false;
        }
    }
}
