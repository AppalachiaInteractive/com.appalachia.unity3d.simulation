using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Root;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Output
{
    [Serializable]
    public class OutputTextureSet : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        [ListDrawerSettings(
            Expanded = true,
            IsReadOnly = true,
            DraggableItems = false,
            HideAddButton = true,
            HideRemoveButton = true,
            ShowIndexLabels = false,
            ShowPaging = true,
            NumberOfItemsPerPage = 1
        )]
        [SerializeField]
        private List<OutputTexture> _outputTextures = new List<OutputTexture>();

        #endregion

        public IReadOnlyList<OutputTexture> outputTextures => _outputTextures;

        public void Clear()
        {
            if (_outputTextures == null)
            {
                _outputTextures = new List<OutputTexture>();
            }

            _outputTextures.Clear();
        }

        public void Set(IEnumerable<OutputTexture> textures)
        {
            Clear();

            foreach (var texture in textures)
            {
                _outputTextures.Add(texture);
            }
        }

        public void UpdateAlphaTestReferenceValue(float value)
        {
            foreach (var outputTexture in outputTextures)
            {
                outputTexture.UpdateAlphaTestReferenceValue(value);
            }
        }
    }
}
