using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class RenderTextureExtensions
    {
        public static RenderTexture GetTemporary(RenderTexture renderTexture)
        {
            return RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, renderTexture.depth,
                renderTexture.format);
        }
    }
}