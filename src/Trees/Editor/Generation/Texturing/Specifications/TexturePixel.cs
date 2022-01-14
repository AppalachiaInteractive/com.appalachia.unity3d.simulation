using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Specifications
{
    public class TexturePixel : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        public Color color;
        public int height;
        public float heightTime;

        public int width;
        public float widthTime;
        public int x;
        public int y;

        #endregion

        public bool IsBlack => (r < .001f) && (g < .001f) && (b < .001f);

        public bool IsTransparent => a < .001f;
        public bool IsWhite => (r > .999f) && (g > .999f) && (b > .999f);
        public float a => color.a;
        public float b => color.b;
        public float g => color.g;

        public float r => color.r;
    }
}
