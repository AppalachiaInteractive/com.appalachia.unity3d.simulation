using System;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Input
{
    [Serializable]
    public class InputTextureProfile : AppalachiaSimpleBase
    {
        public InputTextureProfile(TextureMap map)
        {
            _map = map;
        }

        #region Fields and Autoproperties

        private TextureMap _map;

        [HideInInspector] public string[] propertyNames;

        [TitleGroup(
            "$map",
            Subtitle = "Invert | Channel | Packing",
            Alignment = TitleAlignments.Split,
            HorizontalLine = false,
            BoldTitle = false
        )]
        [InlineProperty, LabelWidth(20), LabelText("R"), EnableIf(nameof(_isEnabled))]
        public InputTextureProfileChannel red;

        [InlineProperty, LabelWidth(20), LabelText("G"), EnableIf(nameof(_isEnabled))]
        public InputTextureProfileChannel green;

        [InlineProperty, LabelWidth(20), LabelText("B"), EnableIf(nameof(_isEnabled))]
        public InputTextureProfileChannel blue;

        [InlineProperty, LabelWidth(20), LabelText("A"), EnableIf(nameof(_isEnabled))]
        public InputTextureProfileChannel alpha;

        #endregion

        public TextureMap map => _map;

        private bool _isEnabled => _map == TextureMap.Custom;

        public InputTextureProfileChannel this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return red;
                    case 1:
                        return green;
                    case 2:
                        return blue;
                    case 3:
                        return alpha;
                    default:
                        throw new IndexOutOfRangeException("0 to 3 allowed.");
                }
            }
        }

        [Button(Style = ButtonStyle.Box), ShowIf(nameof(_isEnabled))]
        public void InitializeProfile(TextureMap m)
        {
            var i = InputTextureProfileFactory.Get(m, null);
            red = i.red;
            green = i.green;
            blue = i.blue;
            alpha = i.alpha;
        }
    }
}
