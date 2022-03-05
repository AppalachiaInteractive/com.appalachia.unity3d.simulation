using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Icons
{
    [NonSerializable]
    public class TreeIcon : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        private string _fieldName;
        private string _assetPath;
        private string _assetName;
        private Texture2D _icon;

        private GUIContentCache cache = new GUIContentCache();

        #endregion

        public Texture2D icon
        {
            get
            {
                if (_fieldName == null)
                {
                    return null;
                }

                if (_assetPath == null)
                {
                    var nameVariants = new List<string>
                    {
                        _fieldName, _fieldName.ToLowerInvariant(), _fieldName.ToUpperInvariant()
                    };

                    var appBuilder = new StringBuilder();

                    var last = default(char);
                    var first = true;
                    foreach (var character in _fieldName)
                    {
                        if (!first && char.IsUpper(character) && char.IsLower(last))
                        {
                            appBuilder.Append('-');
                        }
                        else if (!first && char.IsNumber(character) && char.IsLetter(last))
                        {
                            appBuilder.Append('-');
                        }

                        appBuilder.Append(character);
                        first = false;
                        last = character;
                    }

                    nameVariants.Add(appBuilder.ToString());
                    nameVariants.Add(appBuilder.ToString().ToLowerInvariant());
                    nameVariants.Add(appBuilder.ToString().Replace("-", string.Empty));
                    nameVariants.Add(appBuilder.ToString().Replace("-", string.Empty).ToLowerInvariant());
                    nameVariants.Add(appBuilder.ToString().Replace('-', '_'));
                    nameVariants.Add(appBuilder.ToString().Replace('-', '_').ToLowerInvariant());

                    foreach (var nameVariant in nameVariants)
                    {
                        var found = AssetDatabaseManager
                                   .FindAssets(ZString.Format("icon_{0} t:Texture2D", nameVariant))
                                   .Where(
                                        fa => (nameVariant.Contains("small") && fa.Contains("small")) ||
                                              (!nameVariant.Contains("small") && !fa.Contains("small"))
                                    )
                                   .Select(AssetDatabaseManager.GUIDToAssetPath)
                                   .OrderBy(fa => fa.EitherPath.Length)
                                   .ToArray();

                        if (found.Length > 0)
                        {
                            _assetPath = found[0].RelativePath;
                            _assetName = AppaPath.GetFileNameWithoutExtension(found[0].RelativePath);
                            break;
                        }
                    }
                }

                if (_icon == null)
                {
                    _icon = AssetDatabaseManager.LoadAssetAtPath<Texture2D>(_assetPath);
                }

                return _icon;
            }
        }

        public void Draw(Rect rect, float drawSize)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            rect = AlignCenter(rect, drawSize, drawSize);

            Draw(rect);
        }

        public void Draw(Rect rect)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            rect.x = (int)rect.x;
            rect.y = (int)rect.y;
            rect.width = (int)rect.width;
            rect.height = (int)rect.height;
            GUI.DrawTexture(rect, icon);
        }

        public GUIContent Get()
        {
            if (cache == null)
            {
                cache = new GUIContentCache();
            }

            return cache.Get(this);
        }

        public GUIContent Get(string tooltip)
        {
            if (cache == null)
            {
                cache = new GUIContentCache();
            }

            return cache.Get(this, tooltip);
        }

        public GUIContent GetDefault()
        {
            if (cache == null)
            {
                cache = new GUIContentCache();
            }

            return cache.GetDefault(this);
        }

        public void SetFieldName(string fieldName)
        {
            _fieldName = fieldName;
        }

        private static Rect AlignCenter(Rect rect, float width, float height)
        {
            rect.x = (float)((rect.x + (rect.width * 0.5)) - (width * 0.5));
            rect.y = (float)((rect.y + (rect.height * 0.5)) - (height * 0.5));
            rect.width = width;
            rect.height = height;
            return rect;
        }

        #region Nested type: GUIContentCache

        private class GUIContentCache
        {
            #region Fields and Autoproperties

            public Dictionary<string, GUIContent> contentByTip = new Dictionary<string, GUIContent>();

            public GUIContent blankContent;
            public GUIContent labelledContent;

            #endregion

            public GUIContent Get(TreeIcon icon)
            {
                if (blankContent == null)
                {
                    blankContent = new GUIContent(icon.icon);
                }

                return blankContent;
            }

            public GUIContent Get(TreeIcon icon, string tooltip)
            {
                if (string.IsNullOrWhiteSpace(tooltip))
                {
                    return Get(icon);
                }

                if (contentByTip == null)
                {
                    contentByTip = new Dictionary<string, GUIContent>();
                }

                if (!contentByTip.ContainsKey(tooltip))
                {
                    contentByTip.Add(tooltip, new GUIContent(icon.icon, tooltip));
                }

                return contentByTip[tooltip];
            }

            public GUIContent GetDefault(TreeIcon icon)
            {
                if (string.IsNullOrWhiteSpace(icon._assetName))
                {
                    return Get(icon);
                }

                if (labelledContent == null)
                {
                    var name = icon._assetName.ToLowerInvariant()
                                   .Replace("_",    " ")
                                   .Replace("_",    " ")
                                   .Replace("icon", " ")
                                   .Trim()
                                   .ToFriendly();

                    var builder = new StringBuilder();
                    var first = true;
                    foreach (var chars in name)
                    {
                        if (first)
                        {
                            builder.Append(char.ToUpperInvariant(chars));
                        }
                        else
                        {
                            builder.Append(chars);
                        }

                        first = false;
                    }

                    labelledContent = new GUIContent(icon.icon, builder.ToString());
                }

                return labelledContent;
            }
        }

        #endregion
    }
}
