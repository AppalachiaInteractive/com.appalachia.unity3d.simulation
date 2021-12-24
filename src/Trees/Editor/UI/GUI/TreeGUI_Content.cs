using System.Collections.Generic;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI
{
    public static partial class TreeGUI
    {
        public static class Content
        {
            private static readonly GUIContent _empty = new GUIContent();

            private static Dictionary<string, GUIContent> _labels =
                new Dictionary<string, GUIContent>();

            private static Dictionary<string, Dictionary<string, GUIContent>> _labelsAndTooltips =
                new Dictionary<string, Dictionary<string, GUIContent>>();

            private static Dictionary<string, GUIContent> _tooltips =
                new Dictionary<string, GUIContent>();

            public static GUIContent Label(string label)
            {
                if (string.IsNullOrWhiteSpace(label))
                {
                    return _empty;
                }

                if (_labels == null)
                {
                    _labels = new Dictionary<string, GUIContent>();
                }

                if (!_labels.ContainsKey(label))
                {
                    _labels.Add(label, new GUIContent(label));
                }

                return _labels[label];
            }

            public static GUIContent Tooltip(string tooltip)
            {
                if (string.IsNullOrWhiteSpace(tooltip))
                {
                    return _empty;
                }

                if (_tooltips == null)
                {
                    _tooltips = new Dictionary<string, GUIContent>();
                }

                if (!_tooltips.ContainsKey(tooltip))
                {
                    _tooltips.Add(tooltip, new GUIContent(string.Empty, tooltip));
                }

                return _tooltips[tooltip];
            }

            public static GUIContent LabelAndTooltip(string label, string tooltip)
            {
                if (string.IsNullOrWhiteSpace(label) && string.IsNullOrWhiteSpace(tooltip))
                {
                    return _empty;
                }

                if (string.IsNullOrWhiteSpace(tooltip))
                {
                    return Label(label);
                }

                if (string.IsNullOrWhiteSpace(label))
                {
                    return Tooltip(tooltip);
                }

                if (_labelsAndTooltips == null)
                {
                    _labelsAndTooltips = new Dictionary<string, Dictionary<string, GUIContent>>();
                }

                if (!_labelsAndTooltips.ContainsKey(label))
                {
                    _labelsAndTooltips.Add(label, new Dictionary<string, GUIContent>());
                }

                if (!_labelsAndTooltips[label].ContainsKey(tooltip))
                {
                    _labelsAndTooltips[label].Add(tooltip, new GUIContent(label, tooltip));
                }

                return _labelsAndTooltips[label][tooltip];
            }
        }
    }
}
