using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Specifications
{
    public class TexturePixel
    {
        public Color color;
        public int height;
        public float heightTime;

        public int width;
        public float widthTime;
        public int x;
        public int y;
        public float a => color.a;
        public float b => color.b;
        public float g => color.g;
        public bool IsBlack => (r < .001f) && (g < .001f) && (b < .001f);

        public bool IsTransparent => a < .001f;
        public bool IsWhite => (r > .999f) && (g > .999f) && (b > .999f);

        public float r => color.r;
    }
}