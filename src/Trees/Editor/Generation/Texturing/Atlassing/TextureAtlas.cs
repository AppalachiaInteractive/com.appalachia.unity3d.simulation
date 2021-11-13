#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Utility.Logging;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Atlassing
{
    [Serializable]
    public class TextureAtlas
    {
        public List<TextureAtlasNode> nodes = new List<TextureAtlasNode>();

        private Vector2 _size;
        public Vector2 size => _size;

        [DebuggerStepThrough] public override int GetHashCode()
        {
            var hash = 0;
            for (var i = 0; i < nodes.Count; i++)
            {
                hash ^= nodes[i].atlasUVRect.GetHashCode();
            }

            return hash;
        }

        public void Pack(Vector2 textureSize)
        {
            using (BUILD_TIME.TEX_ATLAS.Pack.Auto())
            {
                _size = textureSize;

                var maximumHeight = _size.y;

                foreach (var node in nodes)
                {
                    node.atlasPackedRect.x = 0;
                    node.atlasPackedRect.y = 0;
                    node.atlasPackedRect.width = Mathf.Round(node.sourceRect.width * node.scale.x);
                    node.atlasPackedRect.height = Mathf.Min(
                        maximumHeight,
                        Mathf.Round(node.sourceRect.height * node.scale.y)
                    );
                }

                nodes.Sort((a, b) => a.CompareTo(b));

                var width = 0;
                var height = 0;

                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    var valid = false;

                    for (var x = 0; x < width; x++)
                    {
                        node.atlasPackedRect.x = x;
                        node.atlasPackedRect.y = 0;
                        valid = true;

                        for (var y = 0; y <= height; y++)
                        {
                            valid = true;
                            node.atlasPackedRect.y = y;

                            for (var j = 0; j < i; j++)
                            {
                                var node2 = nodes[j];

                                if (TextureAtlasNode.Overlap(node, node2))
                                {
                                    valid = false;

                                    y = (int) (node2.atlasPackedRect.y + node2.atlasPackedRect.height);

                                    break;
                                }
                            }

                            if (valid)
                            {
                                break;
                            }
                        }

                        if (valid)
                        {
                            break;
                        }
                    }

                    if (!valid)
                    {
                        node.atlasPackedRect.x = width;
                        node.atlasPackedRect.y = 0;
                    }

                    width = Mathf.Max(width, (int) (node.atlasPackedRect.x + node.atlasPackedRect.width));
                    height = Mathf.Max(height, (int) (node.atlasPackedRect.y + node.atlasPackedRect.height));
                }

                var scaleU = _size.x / width;
                var scaleV = _size.y / height;

                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    node.atlasPackedRect.x *= scaleU;
                    node.atlasPackedRect.y *= scaleV;
                    node.atlasPackedRect.width *= scaleU;
                    node.atlasPackedRect.height *= scaleV;

                    if (node.atlasPackedRect.width < 1)
                    {
                        node.atlasPackedRect.width = 1;
                    }

                    if (node.atlasPackedRect.height < 1)
                    {
                        node.atlasPackedRect.height = 1;
                    }

                    node.atlasPackedRect.x = Mathf.Round(node.atlasPackedRect.x);
                    node.atlasPackedRect.y = Mathf.Round(node.atlasPackedRect.y);
                    node.atlasPackedRect.width = Mathf.Round(node.atlasPackedRect.width);
                    node.atlasPackedRect.height = Mathf.Round(node.atlasPackedRect.height);
                }

                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    node.atlasUVRect.x = node.atlasPackedRect.x / _size.x;
                    node.atlasUVRect.y = node.atlasPackedRect.y / _size.y;
                    node.atlasUVRect.width = node.atlasPackedRect.width / _size.x;
                    node.atlasUVRect.height = node.atlasPackedRect.height / _size.y;
                }
            }
        }
        
        public void Pack(int targetWidth, int targetHeight, int padding = 8)
        {
            _size = new Vector2(targetWidth, targetHeight);
            //
            // Very simple packing.. top->bottom left->right
            // Uses fixed height packing to ensure, that textures can tile vertically..
            // Allows scale factor of individual textures..
            // Works best with pow2 textures..
            //

            if ((padding % 2) != 0)
            {
               AppaLog.Warn("Padding not an even number");
                padding += 1;
            }


            // Maximal height of a node, to ensure V tiling is possible
            int maxHeight = targetHeight;

            // Set corrected size according to padding and scale..
            for (int i = 0; i < nodes.Count; i++)
            {
                TextureAtlasNode node = nodes[i];

                node.atlasPackedRect.x = 0;
                node.atlasPackedRect.y = 0;
                node.atlasPackedRect.width = Mathf.Round(node.sourceRect.width * node.scale.x);
                node.atlasPackedRect.height = Mathf.Min(maxHeight, Mathf.Round(node.sourceRect.height * node.scale.y));
            }

            nodes.Sort(delegate(TextureAtlasNode a, TextureAtlasNode b) { return a.CompareTo(b); });

            int interiorw = 0;
            int interiorh = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                TextureAtlasNode node = nodes[i];
                bool good = false;

                // left - right first
                for (int x = 0; x < interiorw; x++)
                {
                    node.atlasPackedRect.x = x;
                    node.atlasPackedRect.y = 0;
                    good = true;

                    // top - bottom
                    for (int y = 0; y <= interiorh; y++)
                    {
                        good = true;
                        node.atlasPackedRect.y = y;

                        for (int j = 0; j < i; j++)
                        {
                            TextureAtlasNode node2 = nodes[j];
                            if (TextureAtlasNode.Overlap(node, node2))
                            {
                                good = false;
                                // No point in searching for free place in top - bottom if node2.tileV is true, so exit loop 'top - bottom'
                                
                                y = (int)(node2.atlasPackedRect.y + node2.atlasPackedRect.height);
                                break;
                            }
                        }

                        if (good) break;
                    }
                    if (good) break;
                }

                if (!good)
                {
                    // no good position inside, so push onto the right hand side at the top
                    node.atlasPackedRect.x = interiorw;
                    node.atlasPackedRect.y = 0;
                }

                interiorw = Mathf.Max(interiorw, (int)(node.atlasPackedRect.x + node.atlasPackedRect.width));
                interiorh = Mathf.Max(interiorh, (int)(node.atlasPackedRect.y + node.atlasPackedRect.height));
            }

            //
            // Scale to fit
            //
            float scaleU = targetWidth / ((float)interiorw);
            float scaleV = targetHeight / ((float)interiorh);

            for (int i = 0; i < nodes.Count; i++)
            {
                TextureAtlasNode node = nodes[i];

                // packed rect
                node.atlasPackedRect.x *= scaleU;
                node.atlasPackedRect.y *= scaleV;
                node.atlasPackedRect.width *= scaleU;
                node.atlasPackedRect.height *= scaleV;


                node.atlasPackedRect.x += padding / 2f;
                node.atlasPackedRect.y += padding / 2f;
                node.atlasPackedRect.width -= padding;
                node.atlasPackedRect.height -= padding;
                

                if (node.atlasPackedRect.width < 1) node.atlasPackedRect.width = 1;
                if (node.atlasPackedRect.height < 1) node.atlasPackedRect.height = 1;

                // round to clean pixel values
                node.atlasPackedRect.x = Mathf.Round(node.atlasPackedRect.x);// - 0.5f;// +0.5f;
                node.atlasPackedRect.y = Mathf.Round(node.atlasPackedRect.y);// - 0.5f;// +0.5f;
                node.atlasPackedRect.width = Mathf.Round(node.atlasPackedRect.width);// - 0.5f;// +1.0f;// +0.5f;
                node.atlasPackedRect.height = Mathf.Round(node.atlasPackedRect.height);// - 0.5f;// +1.0f;// +0.5f;

                // uv rect
                node.atlasUVRect.x = node.atlasPackedRect.x / targetWidth;
                node.atlasUVRect.y = node.atlasPackedRect.y / targetHeight;
                node.atlasUVRect.width = node.atlasPackedRect.width / targetWidth;
                node.atlasUVRect.height = node.atlasPackedRect.height / targetHeight;
            }
        }

        public void PopulateAtlasDataToMaterials(IReadOnlyList<AtlasInputMaterial> materials)
        {
            foreach (var node in nodes)
            {
                var mat = materials.FirstOrDefault(m => m.materialID == node.materialID);

                if (mat != null)
                {
                    mat.atlasUVRect = node.atlasUVRect;
                    mat.atlasPackedUVRect = node.atlasPackedRect;
                }
            }
        }
    }
}
