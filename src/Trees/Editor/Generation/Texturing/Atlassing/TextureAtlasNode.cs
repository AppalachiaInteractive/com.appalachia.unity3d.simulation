using System;
using System.Diagnostics;
using Appalachia.Core.Objects.Root;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Atlassing
{
    [Serializable]
    public class TextureAtlasNode : AppalachiaSimpleBase
    {
        public TextureAtlasNode(Vector2 size, Vector2 outputSize)
        {
            sourceRect.width = size.x;
            sourceRect.height = size.y;
            
            scale = new Vector2(outputSize.x/size.x, outputSize.y/size.y);
        }
        
        public int materialID;
        
        [FormerlySerializedAs("packedRect")] public Rect atlasPackedRect = new Rect(0, 0, 0, 0);

        public Vector2 scale = new Vector2(1.0f, 1.0f);

        public Rect sourceRect = new Rect(0, 0, 0, 0);
        
        [FormerlySerializedAs("uvRect")] public Rect atlasUVRect = new Rect(0, 0, 0, 0);

        public static bool Overlap(TextureAtlasNode a, TextureAtlasNode b)
        {
            return (!((a.atlasPackedRect.x > (b.atlasPackedRect.x + b.atlasPackedRect.width)) ||
                ((a.atlasPackedRect.x + a.atlasPackedRect.width) < b.atlasPackedRect.x) ||
                (a.atlasPackedRect.y > (b.atlasPackedRect.y + b.atlasPackedRect.height)) ||
                ((a.atlasPackedRect.y + a.atlasPackedRect.height) < b.atlasPackedRect.y)));
        }

        [DebuggerStepThrough] public int CompareTo(TextureAtlasNode b)
        {
            return -atlasPackedRect.height.CompareTo(b.atlasPackedRect.height);
        }
    }
}
