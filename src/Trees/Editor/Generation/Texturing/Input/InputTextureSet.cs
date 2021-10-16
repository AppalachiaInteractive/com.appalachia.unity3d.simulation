using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Input
{
    [Serializable]
    public class InputTextureSet
    {
        [ListDrawerSettings(DraggableItems = false, ShowPaging = true, NumberOfItemsPerPage = 1, Expanded = true,
            HideAddButton = true, HideRemoveButton =  true)]
        public List<InputTexture> inputTextures = new List<InputTexture>();

        public Vector2 size
        {
            get
            {
                if (inputTextures.Count == 0)
                {
                    return Vector2.zero;
                }

                var max = Vector2.zero;
                
                foreach (var inputTexture in inputTextures)
                {
                    if (inputTexture.texture.width > max.x)
                    {
                        max.x = inputTexture.texture.width;
                    }

                    if (inputTexture.texture.height > max.y)
                    {
                        max.y = inputTexture.texture.height;
                    }
                }

                return max;
            }
        }

        public int count => inputTextures?.Count ?? 0;
    }
}