using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Simulation.Trees.Extensions;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Icons
{
    public class TreeIcon
    {
        private string _fieldName;
        private string _assetPath;
        private string _assetName;
        private Texture2D _icon;

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

                    var nameVariants = new List<string>()
                    {
                        _fieldName, _fieldName.ToLowerInvariant(), _fieldName.ToUpperInvariant()
                    };

                    var appBuilder = new StringBuilder();

                    char last = default(char);
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
                    nameVariants.Add(
                        appBuilder.ToString().Replace("-", string.Empty).ToLowerInvariant()
                    );
                    nameVariants.Add(appBuilder.ToString().Replace('-', '_'));
                    nameVariants.Add(appBuilder.ToString().Replace('-', '_').ToLowerInvariant());

                    foreach (var nameVariant in nameVariants)
                    {
                        var found = AssetDatabaseManager.FindAssets($"icon_{nameVariant} t:Texture2D")
                            .Where(
                                fa => (nameVariant.Contains("small") && fa.Contains("small")) ||
                                    (!nameVariant.Contains("small") && !fa.Contains("small"))
                            )
                            .Select(AssetDatabaseManager.GUIDToAssetPath)
                            .OrderBy(fa => fa.Length)
                            .ToArray();

                        if (found.Length > 0)
                        {
                            _assetPath = found[0];
                            _assetName = AppaPath.GetFileNameWithoutExtension(found[0]);
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

        public void SetFieldName(string fieldName)
        {
            _fieldName = fieldName;
        }

        private class GUIContentCache
        {
            public GUIContent blankContent;
            public GUIContent labelledContent;

            public Dictionary<string, GUIContent> contentByTip =
                new Dictionary<string, GUIContent>();

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
                    var name = icon._assetName
                        .ToLowerInvariant()
                        .Replace("_", " ")
                        .Replace("_", " ")
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

        private GUIContentCache cache = new GUIContentCache();

        
        public GUIContent Get()
        {
            if (cache == null) cache = new GUIContentCache();

            return cache.Get(this);
        }

        public GUIContent Get(string tooltip)
        {
            if (cache == null) cache = new GUIContentCache();

            return cache.Get(this, tooltip);
        }
        
        
        public GUIContent GetDefault()
        {
            if (cache == null) cache = new GUIContentCache();

            return cache.GetDefault(this);
        }

        

        public void Draw(Rect rect, float drawSize)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            
            rect = AlignCenter(rect,  drawSize, drawSize);
            
            Draw(rect);
        }

        public void Draw(Rect rect)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            rect.x = (int) rect.x;
            rect.y = (int) rect.y;
            rect.width = (int) rect.width;
            rect.height = (int) rect.height;
            GUI.DrawTexture(rect, icon);
        }

        private static Rect AlignCenter(Rect rect, float width, float height)
        {
            rect.x = (float) ((rect.x + (rect.width * 0.5)) - (width * 0.5));
            rect.y = (float) ((rect.y + (rect.height * 0.5)) - (height * 0.5));
            rect.width = width;
            rect.height = height;
            return rect;
        }
        
    }
}