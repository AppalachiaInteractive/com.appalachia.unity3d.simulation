#region

using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Extensions;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Geometry
{
    public static class TextureHullGenerator
    {
        #region Static Fields and Autoproperties

        private static Dictionary<Material, Texture2D>
            primaryTextures = new Dictionary<Material, Texture2D>();

        private static Dictionary<Texture2D, TextureHullData> _textureHullData =
            new Dictionary<Texture2D, TextureHullData>();

        #endregion

        public static TextureHullData GetTextureHullData(this Material mat)
        {
            if (mat == null)
            {
                return null;
            }

            Texture2D tex;

            using (BUILD_TIME.TEX_HULL_GEN.LoadAndCacheTexture.Auto())
            {
                if (!primaryTextures.ContainsKey(mat))
                {
                    primaryTextures.Add(mat, mat.primaryTexture());
                }

                tex = primaryTextures[mat];
            }

            return GetTextureHullData(tex);
        }

        public static TextureHullData GetTextureHullData(this Texture2D tex)
        {
            using (BUILD_TIME.TEX_HULL_GEN.GetTextureHullData.Auto())
            {
                if (!tex)
                {
                    return null;
                }

                if (_textureHullData == null)
                {
                    _textureHullData = new Dictionary<Texture2D, TextureHullData>();
                }

                if (_textureHullData.ContainsKey(tex))
                {
                    return _textureHullData[tex];
                }

                var hull = new TextureHullData();
                _textureHullData.Add(tex, hull);

                hull.rawHull = new[]
                {
                    new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1)
                };

                //hull.textureHull = GetShapePoints(tex, 4);

                if (hull.textureHull == null)
                {
                    hull.textureHull = hull.rawHull;
                }

                var tmp = hull.textureHull[1];
                hull.textureHull[1] = hull.textureHull[3];
                hull.textureHull[3] = tmp;

                var firstRowPopulatedFromBottom = -1;
                var firstRowPopulatedXValues = new List<int>();

                using (BUILD_TIME.TEX_HULL_GEN.IteratePixels.Auto())
                {
                    for (var y = 0; y < tex.height; y++) // bottom to top
                    {
                        var pixels = tex.GetPixels(0, y, tex.width, 1);

                        for (var x = 0; x < tex.width; x++) // left to right
                        {
                            if (x == 0)
                            {
                                continue;
                            }

                            if ((firstRowPopulatedFromBottom > -1) && (y != firstRowPopulatedFromBottom))
                            {
                                continue;
                            }

                            if ((firstRowPopulatedFromBottom == -1) && (pixels[x].a > .01f))
                            {
                                firstRowPopulatedFromBottom = y;
                            }

                            if ((y == firstRowPopulatedFromBottom) && (pixels[x].a > .01f))
                            {
                                firstRowPopulatedXValues.Add(x);
                            }
                        }
                    }
                }

                using (BUILD_TIME.TEX_HULL_GEN.AggregatePixelData.Auto())
                {
                    hull.baseHeightOffset0To1 = firstRowPopulatedFromBottom / (float)tex.height;
                    var columnSum = (float)firstRowPopulatedXValues.Sum();
                    var columnMin = firstRowPopulatedXValues.Min();
                    var columnMax = firstRowPopulatedXValues.Max();
                    var meanColumn = columnSum / firstRowPopulatedXValues.Count;

                    hull.baseWidth = (columnMax - columnMin) / (float)tex.width;
                    hull.baseWidthCenter0To1 = meanColumn / tex.width;
                }

                return hull;
            }
        }

        private static Vector2[] GetShapePoints(Texture2D tex, int outlinePoints)
        {
            using (BUILD_TIME.TEX_HULL_GEN.GetShapePoints.Auto())
            {
                // create a 2d texture for calculations
                var testRect = new Rect(0, 0, tex.width, tex.height);

                Vector2[][] paths;

                using (BUILD_TIME.TEX_HULL_GEN.GenerateOutline.Auto())
                {
                    paths = tex.GenerateOutline(testRect, 0.15f, 254, false);
                }

                var sum = 0;
                for (var i = 0; i < paths.Length; i++)
                {
                    sum += paths[i].Length;
                }

                var shapePoints = new Vector2[sum];
                var index = 0;
                for (var i = 0; i < paths.Length; i++)
                {
                    for (var j = 0; j < paths[i].Length; j++)
                    {
                        shapePoints[index] = paths[i][j] + new Vector2(tex.width * 0.5f, tex.height * 0.5f);
                        shapePoints[index] = Vector2.Scale(
                            shapePoints[index],
                            new Vector2(1.0f / tex.width, 1.0f / tex.height)
                        );
                        index++;
                    }
                }

                using (BUILD_TIME.TEX_HULL_GEN.ConvexHull.Auto())
                {
                    // make it convex hull
                    shapePoints = shapePoints.ConvexHull();
                }

                using (BUILD_TIME.TEX_HULL_GEN.ReduceVertices.Auto())
                {
                    // reduce vertices
                    shapePoints = shapePoints.ReduceVertices(outlinePoints);
                }

                using (BUILD_TIME.TEX_HULL_GEN.ClampHull.Auto())
                {
                    // clamp to box (needs a cut algorithm)
                    for (var i = 0; i < shapePoints.Length; i++)
                    {
                        shapePoints[i].x = Mathf.Clamp01(shapePoints[i].x);
                        shapePoints[i].y = Mathf.Clamp01(shapePoints[i].y);
                    }
                }

                using (BUILD_TIME.TEX_HULL_GEN.ConvexHull.Auto())
                {
                    // make it convex hull gain to clean edges
                    shapePoints = shapePoints.ConvexHull();
                }

                // invert Y
                for (var i = 0; i < shapePoints.Length; i++)
                {
                    shapePoints[i] = new Vector2(shapePoints[i].x, 1 - shapePoints[i].y);
                }

                return shapePoints;
            }
        }
    }
}
