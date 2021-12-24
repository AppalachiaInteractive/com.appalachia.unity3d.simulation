#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Atlassing
{
    [Serializable]
    public class TextureAtlas : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        public List<TextureAtlasNode> nodes = new List<TextureAtlasNode>();

        private Vector2 _size;

        #endregion

        public Vector2 size => _size;

        [DebuggerStepThrough]
        public override int GetHashCode()
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

                                    y = (int)(node2.atlasPackedRect.y + node2.atlasPackedRect.height);

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

                    width = Mathf.Max(width,   (int)(node.atlasPackedRect.x + node.atlasPackedRect.width));
                    height = Mathf.Max(height, (int)(node.atlasPackedRect.y + node.atlasPackedRect.height));
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
                Context.Log.Warn("Padding not an even number");
                padding += 1;
            }

            // Maximal height of a node, to ensure V tiling is possible
            var maxHeight = targetHeight;

            // Set corrected size according to padding and scale..
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                node.atlasPackedRect.x = 0;
                node.atlasPackedRect.y = 0;
                node.atlasPackedRect.width = Mathf.Round(node.sourceRect.width * node.scale.x);
                node.atlasPackedRect.height = Mathf.Min(
                    maxHeight,
                    Mathf.Round(node.sourceRect.height * node.scale.y)
                );
            }

            nodes.Sort(delegate(TextureAtlasNode a, TextureAtlasNode b) { return a.CompareTo(b); });

            var interiorw = 0;
            var interiorh = 0;

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var good = false;

                // left - right first
                for (var x = 0; x < interiorw; x++)
                {
                    node.atlasPackedRect.x = x;
                    node.atlasPackedRect.y = 0;
                    good = true;

                    // top - bottom
                    for (var y = 0; y <= interiorh; y++)
                    {
                        good = true;
                        node.atlasPackedRect.y = y;

                        for (var j = 0; j < i; j++)
                        {
                            var node2 = nodes[j];
                            if (TextureAtlasNode.Overlap(node, node2))
                            {
                                good = false;

                                // No point in searching for free place in top - bottom if node2.tileV is true, so exit loop 'top - bottom'

                                y = (int)(node2.atlasPackedRect.y + node2.atlasPackedRect.height);
                                break;
                            }
                        }

                        if (good)
                        {
                            break;
                        }
                    }

                    if (good)
                    {
                        break;
                    }
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
            var scaleU = targetWidth / (float)interiorw;
            var scaleV = targetHeight / (float)interiorh;

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                // packed rect
                node.atlasPackedRect.x *= scaleU;
                node.atlasPackedRect.y *= scaleV;
                node.atlasPackedRect.width *= scaleU;
                node.atlasPackedRect.height *= scaleV;

                node.atlasPackedRect.x += padding / 2f;
                node.atlasPackedRect.y += padding / 2f;
                node.atlasPackedRect.width -= padding;
                node.atlasPackedRect.height -= padding;

                if (node.atlasPackedRect.width < 1)
                {
                    node.atlasPackedRect.width = 1;
                }

                if (node.atlasPackedRect.height < 1)
                {
                    node.atlasPackedRect.height = 1;
                }

                // round to clean pixel values
                node.atlasPackedRect.x = Mathf.Round(node.atlasPackedRect.x); // - 0.5f;// +0.5f;
                node.atlasPackedRect.y = Mathf.Round(node.atlasPackedRect.y); // - 0.5f;// +0.5f;
                node.atlasPackedRect.width =
                    Mathf.Round(node.atlasPackedRect.width); // - 0.5f;// +1.0f;// +0.5f;
                node.atlasPackedRect.height =
                    Mathf.Round(node.atlasPackedRect.height); // - 0.5f;// +1.0f;// +0.5f;

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
