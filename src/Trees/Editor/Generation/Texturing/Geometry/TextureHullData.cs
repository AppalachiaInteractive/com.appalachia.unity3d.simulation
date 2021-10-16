using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Geometry
{
    [Serializable]
    public class TextureHullData
    {
        public float baseHeightOffset0To1;
        public float baseWidth;
        public float baseWidthCenter0To1;
        public Vector2[] rawHull;
        public Vector2[] textureHull;

        public static TextureHullData Default()
        {
            var data = new TextureHullData()
            {
                rawHull = new[]
                {
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1)
                }
            };

            data.textureHull = data.rawHull;
            data.baseWidth = 1f;
            data.baseWidthCenter0To1 = .5f;

            return data;
        }
    }
}