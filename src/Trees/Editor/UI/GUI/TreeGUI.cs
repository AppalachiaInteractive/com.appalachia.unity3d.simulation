using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Extensions;
using Appalachia.Editing.Core.State;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Utility.Reflection.Extensions;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.UI.GUI
{
    public static partial class TreeGUI
    {
        public static void Repaint()
        {
            InternalEditorUtility.RepaintAllViews();
        }

        public static class Enabled
        {
            public static IDisposable Always()
            {
                return new GUIToken(true);
            }

            public static IDisposable Never()
            {
                return new GUIToken(false);
            }

            public static IDisposable If(bool value)
            {
                return new GUIToken(value);
            }

            public static IDisposable IfNotNull(object value)
            {
                return new GUIToken(value != null);
            }

            public static IDisposable IfNull(object value)
            {
                return new GUIToken(value == null);
            }

            private class GUIToken : IDisposable
            {
                public readonly bool original;

                public GUIToken(bool shouldEnable)
                {
                    original = UnityEngine.GUI.enabled;
                    UnityEngine.GUI.enabled = shouldEnable;
                }

                public void Dispose()
                {
                    UnityEngine.GUI.enabled = original;
                }
            }
        }

        public static class Layout
        {
            public static GUIToken Vertical(bool bordered = true, params GUILayoutOption[] args)
            {
                return new GUIToken(false, bordered, args);
            }

            public static GUIToken Horizontal(bool bordered = true, params GUILayoutOption[] args)
            {
                return new GUIToken(true, bordered, args);
            }

            public static GUIToken Vertical(params GUILayoutOption[] args)
            {
                return new GUIToken(false, true, args);
            }

            public static GUIToken Horizontal(params GUILayoutOption[] args)
            {
                return new GUIToken(true, true, args);
            }

            public class GUIToken : IDisposable
            {
                private readonly bool bordered;
                private readonly bool horizontal;
                public Rect rect;

                public GUIToken(bool horizontal, bool bordered, params GUILayoutOption[] args)
                {
                    this.horizontal = horizontal;
                    this.bordered = bordered;

                    if (this.horizontal)
                    {
                        rect = EditorGUILayout.BeginHorizontal(args);
                    }
                    else
                    {
                        rect = EditorGUILayout.BeginVertical(args);
                    }
                }

                public void Dispose()
                {
                    if (horizontal)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.EndVertical();
                    }

                    if (bordered)
                    {
                        Draw.Borders(rect, 1);
                    }
                }
            }

            public static class Options
            {
                public static readonly GUILayoutOption[] None = new GUILayoutOption[0];

                private static readonly Dictionary<OptionsInstance, GUILayoutOption[]> GUILayoutOptionsCache =
                    new Dictionary<OptionsInstance, GUILayoutOption[]>();

                private static readonly OptionsInstance[] OptionsInstanceCache = new OptionsInstance[30];

                private static int CurrentCacheIndex;

                static Options()
                {
                    OptionsInstanceCache[0] = new OptionsInstance();
                    for (var index = 1; index < 30; ++index)
                    {
                        OptionsInstanceCache[index] = new OptionsInstance();
                        OptionsInstanceCache[index].Parent = OptionsInstanceCache[index - 1];
                    }
                }

                public static OptionsInstance Width(float width)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.Width, width);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance Height(float height)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.Height, height);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance MaxHeight(float height)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.MaxHeight, height);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance MaxWidth(float width)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.MaxWidth, width);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance MinWidth(float width)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.MinWidth, width);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance MinHeight(float height)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.MinHeight, height);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance ExpandHeight(bool expand = true)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.ExpandHeight, expand);
                    return layoutOptionsInstance;
                }

                public static OptionsInstance ExpandWidth(bool expand = true)
                {
                    CurrentCacheIndex = 0;
                    var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                    layoutOptionsInstance.SetValue(GUILayoutOptionType.ExpandWidth, expand);
                    return layoutOptionsInstance;
                }

                internal enum GUILayoutOptionType
                {
                    Width,
                    Height,
                    MinWidth,
                    MaxHeight,
                    MaxWidth,
                    MinHeight,
                    ExpandHeight,
                    ExpandWidth
                }

                public sealed class OptionsInstance : IEquatable<OptionsInstance>
                {
                    internal GUILayoutOptionType GUILayoutOptionType;
                    internal OptionsInstance Parent;
                    private float value;

                    internal OptionsInstance()
                    {
                    }

                    public bool Equals(OptionsInstance other)
                    {
                        var loi1 = this;
                        OptionsInstance loi2;
                        for (loi2 = other;
                            (loi1 != null) && (loi2 != null);
                            loi2 = loi2.Parent)
                        {
                            if ((loi1.GUILayoutOptionType !=
                                    loi2.GUILayoutOptionType) ||
                                (Math.Abs(loi1.value - (double) loi2.value) > float.Epsilon))
                            {
                                return false;
                            }

                            loi1 = loi1.Parent;
                        }

                        return (loi2 == null) && (loi1 == null);
                    }

                    private GUILayoutOption[] GetCachedOptions()
                    {
                        GUILayoutOption[] guiLayoutOptionArray;
                        if (!GUILayoutOptionsCache.TryGetValue(this, out guiLayoutOptionArray))
                        {
                            guiLayoutOptionArray = GUILayoutOptionsCache[Clone()] = CreateOptionsArary();
                        }

                        return guiLayoutOptionArray;
                    }

                    public static implicit operator GUILayoutOption[](OptionsInstance options)
                    {
                        return options.GetCachedOptions();
                    }

                    private GUILayoutOption[] CreateOptionsArary()
                    {
                        var guiLayoutOptionList = new List<GUILayoutOption>();
                        for (var layoutOptionsInstance = this;
                            layoutOptionsInstance != null;
                            layoutOptionsInstance = layoutOptionsInstance.Parent)
                        {
                            switch (layoutOptionsInstance.GUILayoutOptionType)
                            {
                                case GUILayoutOptionType.Width:
                                    guiLayoutOptionList.Add(GUILayout.Width(layoutOptionsInstance.value));
                                    break;
                                case GUILayoutOptionType.Height:
                                    guiLayoutOptionList.Add(GUILayout.Height(layoutOptionsInstance.value));
                                    break;
                                case GUILayoutOptionType.MinWidth:
                                    guiLayoutOptionList.Add(GUILayout.MinWidth(layoutOptionsInstance.value));
                                    break;
                                case GUILayoutOptionType.MaxHeight:
                                    guiLayoutOptionList.Add(GUILayout.MaxHeight(layoutOptionsInstance.value));
                                    break;
                                case GUILayoutOptionType.MaxWidth:
                                    guiLayoutOptionList.Add(GUILayout.MaxWidth(layoutOptionsInstance.value));
                                    break;
                                case GUILayoutOptionType.MinHeight:
                                    guiLayoutOptionList.Add(GUILayout.MinHeight(layoutOptionsInstance.value));
                                    break;
                                case GUILayoutOptionType.ExpandHeight:
                                    guiLayoutOptionList.Add(
                                        GUILayout.ExpandHeight(layoutOptionsInstance.value > 0.200000002980232)
                                    );
                                    break;
                                case GUILayoutOptionType.ExpandWidth:
                                    guiLayoutOptionList.Add(
                                        GUILayout.ExpandWidth(layoutOptionsInstance.value > 0.200000002980232)
                                    );
                                    break;
                            }
                        }

                        return guiLayoutOptionList.ToArray();
                    }

                    private OptionsInstance Clone()
                    {
                        var layoutOptionsInstance1 = new OptionsInstance
                        {
                            value = value, GUILayoutOptionType = GUILayoutOptionType
                        };
                        var layoutOptionsInstance2 = layoutOptionsInstance1;
                        var parent = Parent;
                        while (parent != null)
                        {
                            layoutOptionsInstance2.Parent = new OptionsInstance
                            {
                                value = parent.value, GUILayoutOptionType = parent.GUILayoutOptionType
                            };
                            parent = parent.Parent;
                            layoutOptionsInstance2 = layoutOptionsInstance2.Parent;
                        }

                        return layoutOptionsInstance1;
                    }

                    public OptionsInstance Width(float width)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.Width, width);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance Height(float height)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.Height, height);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance MaxHeight(float height)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.MaxHeight, height);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance MaxWidth(float width)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.MaxWidth, width);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance MinHeight(float height)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.MinHeight, height);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance MinWidth(float width)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.MinWidth, width);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance ExpandHeight(bool expand = true)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.ExpandHeight, expand);
                        return layoutOptionsInstance;
                    }

                    public OptionsInstance ExpandWidth(bool expand = true)
                    {
                        var layoutOptionsInstance = OptionsInstanceCache[CurrentCacheIndex++];
                        layoutOptionsInstance.SetValue(GUILayoutOptionType.ExpandWidth, expand);
                        return layoutOptionsInstance;
                    }

                    internal void SetValue(GUILayoutOptionType type, float value)
                    {
                        GUILayoutOptionType = type;
                        this.value = value;
                    }

                    internal void SetValue(GUILayoutOptionType type, bool value)
                    {
                        GUILayoutOptionType = type;
                        this.value = value ? 1f : 0.0f;
                    }

                    public override int GetHashCode()
                    {
                        var num1 = 0;
                        var num2 = 17;
                        for (var layoutOptionsInstance = this;
                            layoutOptionsInstance != null;
                            layoutOptionsInstance = layoutOptionsInstance.Parent)
                        {
                            num2 = (num2 * 29) +
                                GUILayoutOptionType.GetHashCode() +
                                (value.GetHashCode() * 17) +
                                num1++;
                        }

                        return num2;
                    }
                }
            }
        }

        public static class Draw
        {
            private static readonly Dictionary<Type, Texture2D> typeTextureLookup = new Dictionary<Type, Texture2D>();

            public static void Solid(Rect rect, Color color, bool usePlaymodeTint = true)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return;
                }

                if (usePlaymodeTint)
                {
                    EditorGUI.DrawRect(rect, color);
                }
                else
                {
                    UIStateStacks.foregroundColor.Push(color);
                    UnityEngine.GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                    UIStateStacks.foregroundColor.Pop();
                }
            }

            public static void Borders(Rect rect, int borderWidth, bool usePlaymodeTint = true)
            {
                Borders(rect, borderWidth, borderWidth, borderWidth, borderWidth, Colors.BorderColor, usePlaymodeTint);
            }

            public static void Borders(
                Rect rect,
                int left,
                int right,
                int top,
                int bottom,
                Color color,
                bool usePlaymodeTint = true)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return;
                }

                if (left > 0)
                {
                    var rect1 = rect;
                    rect1.width = left;
                    Solid(rect1, color, usePlaymodeTint);
                }

                if (top > 0)
                {
                    var rect1 = rect;
                    rect1.height = top;
                    Solid(rect1, color, usePlaymodeTint);
                }

                if (right > 0)
                {
                    var rect1 = rect;
                    rect1.x += rect.width - right;
                    rect1.width = right;
                    Solid(rect1, color, usePlaymodeTint);
                }

                if (bottom <= 0)
                {
                    return;
                }

                var rect2 = rect;
                rect2.y += rect.height - bottom;
                rect2.height = bottom;
                Solid(rect2, color, usePlaymodeTint);
            }

            public static void SmallTitle(
                string title,
                TextAlignment textAlignment = TextAlignment.Center,
                bool horizontalLine = true)
            {
                GUIStyle style1;
                switch (textAlignment)
                {
                    case TextAlignment.Left:
                        style1 = Styles.SmallTitle;
                        break;
                    case TextAlignment.Center:
                        style1 = Styles.SmallTitleCentered;
                        break;
                    case TextAlignment.Right:
                        style1 = Styles.SmallTitleRight;
                        break;
                    default:
                        style1 = Styles.SmallTitle;
                        break;
                }

                if (textAlignment > TextAlignment.Right)
                {
                    var rect = GUILayoutUtility.GetRect(0.0f, 16f, style1, Layout.Options.ExpandWidth());
                    UnityEngine.GUI.Label(rect, title, style1);
                    if (!horizontalLine)
                    {
                        return;
                    }

                    Line.Horizontal(Colors.LightBorderColor);
                    GUILayout.Space(3f);
                }
                else
                {
                    var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
                    UnityEngine.GUI.Label(rect, title, style1);

                    if (!horizontalLine)
                    {
                        return;
                    }

                    Solid(rect.AlignBottom(1f), Colors.LightBorderColor);
                    GUILayout.Space(3f);
                }
            }

            public static void Title(
                string title,
                string subtitle = null,
                TextAlignment textAlignment = TextAlignment.Center,
                bool horizontalLine = true,
                bool boldLabel = true)
            {
                GUIStyle style1;
                GUIStyle style2;
                switch (textAlignment)
                {
                    case TextAlignment.Left:
                        style1 = boldLabel ? Styles.BoldTitle : Styles.Title;
                        style2 = Styles.Subtitle;
                        break;
                    case TextAlignment.Center:
                        style1 = boldLabel ? Styles.BoldTitleCentered : Styles.TitleCentered;
                        style2 = Styles.SubtitleCentered;
                        break;
                    case TextAlignment.Right:
                        style1 = boldLabel ? Styles.BoldTitleRight : Styles.TitleRight;
                        style2 = Styles.SubtitleRight;
                        break;
                    default:
                        style1 = boldLabel ? Styles.BoldTitle : Styles.Title;
                        style2 = Styles.SubtitleRight;
                        break;
                }

                if (textAlignment > TextAlignment.Right)
                {
                    var rect = GUILayoutUtility.GetRect(0.0f, 18f, style1, Layout.Options.ExpandWidth());
                    UnityEngine.GUI.Label(rect, title, style1);
                    rect.y += 3f;
                    UnityEngine.GUI.Label(rect, subtitle, style2);
                    if (!horizontalLine)
                    {
                        return;
                    }

                    Line.Horizontal(Colors.LightBorderColor);
                    GUILayout.Space(3f);
                }
                else
                {
                    var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
                    UnityEngine.GUI.Label(rect, title, style1);
                    if ((subtitle != null) && !subtitle.IsNullOrWhitespace())
                    {
                        rect = EditorGUI.IndentedRect(
                            GUILayoutUtility.GetRect(GUIHelper.TempContent(subtitle), style2)
                        );
                        UnityEngine.GUI.Label(rect, subtitle, style2);
                    }

                    if (!horizontalLine)
                    {
                        return;
                    }

                    Solid(rect.AlignBottom(1f), Colors.LightBorderColor);
                    GUILayout.Space(3f);
                }
            }

            public static void Preview(Rect rect, object value, GUIContent label = null)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return;
                }

                var obj = value as Object;

                if (obj == null)
                {
                    return;
                }

                var image = GetThumbnail(obj, obj.GetType());

                rect = rect.Padding(2f);

                var size = Mathf.Min(rect.width, rect.height);

                EditorGUI.DrawTextureTransparent(rect.AlignCenter(size, size), image, ScaleMode.ScaleToFit);

                if (label != null)
                {
                    rect = rect.AlignBottom(16f);
                    UnityEngine.GUI.Label(rect, label, EditorStyles.label);
                }
            }

            private static Texture2D GetThumbnail(Object obj, Type type)
            {
                Texture2D texture2D = null;
                if (obj)
                {
                    texture2D = AssetPreview.GetAssetPreview(obj);
                }

                if ((texture2D == null) && (bool) obj)
                {
                    var assetPath = AssetDatabaseManager.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        texture2D = InternalEditorUtility.GetIconForFile(assetPath);
                    }
                }

                if ((bool) obj &&
                    ((texture2D == null) || (texture2D == EditorGUIUtility.FindTexture("DefaultAsset Icon"))))
                {
                    texture2D = AssetPreview.GetMiniThumbnail(obj);
                }

                if ((texture2D == null) && (type != null))
                {
                    typeTextureLookup.TryGetValue(type, out texture2D);
                    if (texture2D == null)
                    {
                        if (!typeof(Object).IsAssignableFrom(type))
                        {
                            texture2D = EditorGUIUtility.FindTexture("cs Script Icon");
                        }
                        else
                        {
                            texture2D = AssetPreview.GetMiniTypeThumbnail(type);
                            if (texture2D == null)
                            {
                                texture2D = type.GetBaseClasses()
                                    .Select(
                                        t =>
                                        {
                                            try
                                            {
                                                return AssetPreview.GetMiniTypeThumbnail(t);
                                            }
                                            catch
                                            {
                                                return (Texture2D) null;
                                            }
                                        }
                                    )
                                    .FirstOrDefault(x => (bool) (Object) x);
                            }

                            if (texture2D == EditorGUIUtility.FindTexture("DefaultAsset Icon"))
                            {
                                texture2D = !typeof(ScriptableObject).IsAssignableFrom(type)
                                    ? EditorGUIUtility.FindTexture("cs Script Icon")
                                    : AssetPreview.GetMiniTypeThumbnail(typeof(BillboardAsset));
                            }
                        }

                        typeTextureLookup[type] = texture2D;
                    }
                }

                return texture2D;
            }

            public static class Line
            {
                public static void Horizontal(int lineWidth = 1)
                {
                    Horizontal(Colors.BorderColor, lineWidth);
                }

                public static void Horizontal(Color color, int lineWidth = 1)
                {
                    Solid(GUILayoutUtility.GetRect(lineWidth, lineWidth, Layout.Options.ExpandWidth()), color);
                }

                public static void Vertical(int lineWidth = 1)
                {
                    Vertical(Colors.BorderColor, lineWidth);
                }

                public static void Vertical(Color color, int lineWidth = 1)
                {
                    Solid(
                        GUILayoutUtility.GetRect(lineWidth, lineWidth, Layout.Options.ExpandHeight().Width(lineWidth)),
                        color
                    );
                }

            }
            public static void Space(int size = 1, bool lineAtEnd = false)
            {
                for (var i = 0; i < size; i++)
                {
                    EditorGUILayout.Space();
                }

                if (lineAtEnd)
                {
                    Line.Horizontal();
                }
            }
        }

        public static class Button
        {
            public static void Icon(
                Rect rect,
                bool condition,
                TreeIcon trueIcon,
                TreeIcon falseIcon,
                string trueTooltip,
                string falseTooltip,
                Action trueAction,
                Action falseAction,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                style = style ?? Styles.IconButton;

                var icon = condition ? trueIcon : falseIcon;
                var tooltip = condition ? trueTooltip : falseTooltip;
                var action = condition ? trueAction : falseAction;

                if (UnityEngine.GUI.Button(rect, icon.Get(tooltip), style))
                {
                    GUIHelper.RemoveFocusControl();

                    action();
                }

                if (Event.current.type == EventType.Repaint)
                {
                    var drawSize = Mathf.Min(rect.height, rect.width);

                    icon.Draw(rect, drawSize);
                }
            }

            public static void ToggleContext(
                bool condition,
                TreeIcon trueIcon,
                TreeIcon falseIcon,
                string tooltip,
                Action action,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                if (GUILayout.Button((condition ? trueIcon : falseIcon).Get(tooltip), style, args))
                {
                    action();
                }
            }

            public static void EnumContext<T>(
                T current,
                Func<T, bool> enabled,
                Func<T, TreeIcon> icon,
                Func<T, string> tooltip,
                Action<T> action,
                Func<T, GUIStyle> style,
                GUILayoutOption[] args)
                where T : Enum
            {
                using (Enabled.If(enabled(current)))
                {
                    if (GUILayout.Button(
                        icon(current)?.Get(tooltip(current)) ?? Content.Tooltip(tooltip(current)),
                        style(current),
                        args
                    ))
                    {
                        action(current);
                    }
                }
            }

            public static void Context(
                bool condition,
                TreeIcon trueIcon,
                TreeIcon falseIcon,
                string trueTooltip,
                string falseTooltip,
                Action trueAction,
                Action falseAction,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                if (GUILayout.Button(
                    (condition ? trueIcon : falseIcon).Get(condition ? trueTooltip : falseTooltip),
                    style,
                    args
                ))
                {
                    if (condition)
                    {
                        trueAction();
                    }
                    else
                    {
                        falseAction();
                    }
                }
            }

            public static void EnableDisable(
                bool enabled,
                string text,
                string tooltip,
                Action action,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                using (Enabled.If(enabled))
                {
                    if (GUILayout.Button(Content.LabelAndTooltip(text, tooltip), style, args))
                    {
                        if (enabled)
                        {
                            action();
                        }
                    }
                }
            }

            public static void ContextEnableDisable(
                bool enabled,
                bool condition,
                string trueLabel,
                string trueTooltip,
                Action trueAction,
                GUIStyle trueStyle,
                string falseLabel,
                string falseTooltip,
                Action falseAction,
                GUIStyle falseStyle,
                GUILayoutOption[] args)
            {
                using (Enabled.If(enabled))
                {
                    if (GUILayout.Button(
                        Content.LabelAndTooltip(
                            condition ? trueLabel : falseLabel,
                            condition ? trueTooltip : falseTooltip
                        ),
                        condition ? trueStyle : falseStyle,
                        args
                    ))
                    {
                        if (condition)
                        {
                            trueAction();
                        }
                        else
                        {
                            falseAction();
                        }
                    }
                }
            }

            public static void EnableDisable(
                bool enabled,
                TreeIcon trueIcon,
                TreeIcon falseIcon,
                string trueTooltip,
                Action trueAction,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                using (Enabled.If(enabled))
                {
                    if (GUILayout.Button((enabled ? trueIcon : falseIcon).Get(trueTooltip), style, args))
                    {
                        if (enabled)
                        {
                            trueAction();
                        }
                    }
                }
            }

            public static void ContextEnableDisable(
                bool enabled,
                bool condition,
                TreeIcon trueIcon,
                TreeIcon falseIcon,
                string trueTooltip,
                string falseTooltip,
                Action trueAction,
                Action falseAction,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                using (Enabled.If(enabled))
                {
                    if (GUILayout.Button(
                        (condition ? trueIcon : falseIcon).Get(condition ? trueTooltip : falseTooltip),
                        style,
                        args
                    ))
                    {
                        if (condition)
                        {
                            trueAction();
                        }
                        else
                        {
                            falseAction();
                        }
                    }
                }
            }

            public static void Standard(
                TreeIcon icon,
                string tooltip,
                Action action,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                if (GUILayout.Button(icon.Get(tooltip), style, args))
                {
                    action();
                }
            }

            public static void Standard(
                string text,
                string tooltip,
                Action action,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                if (GUILayout.Button(Content.LabelAndTooltip(text, tooltip), style, args))
                {
                    action();
                }
            }

            public static void Toolbar(
                bool enabled,
                bool selected,
                TreeIcon enabledIcon,
                TreeIcon disabledIcon,
                string tooltip,
                Action action,
                GUIStyle styleSelected,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                using (Enabled.If(enabled))
                {
                    if (GUILayout.Button(
                        (enabled ? enabledIcon : disabledIcon).Get(tooltip),
                        selected ? styleSelected : style,
                        args
                    ))
                    {
                        action();

                        GUIHelper.RemoveFocusControl();
                        GUIHelper.RequestRepaint();
                    }
                }
            }

            public static void Toolbar(
                bool enabled,
                bool selected,
                string label,
                string tooltip,
                Action action,
                GUIStyle styleSelected,
                GUIStyle style,
                GUILayoutOption[] args)
            {
                using (Enabled.If(enabled))
                {
                    if (GUILayout.Button(
                        Content.LabelAndTooltip(label, tooltip),
                        selected ? styleSelected : style,
                        args
                    ))
                    {
                        action();

                        GUIHelper.RemoveFocusControl();
                        GUIHelper.RequestRepaint();
                    }
                }
            }
        }
    }
}
