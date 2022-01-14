using System;
using System.Diagnostics;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials
{
    [Serializable]
    public class UVScale : ICloneable<UVScale>
    {
        public UVScale(int x = 1, int y = 1)
        {
            this.x = x;
            this.y = y;
        }

        #region Static Fields and Autoproperties

        private static float[] valuesX;
        private static float[] valuesY;

        #endregion

        #region Fields and Autoproperties

        //[PropertyRange(1, 5)]
        [HorizontalGroup(.5f), LabelWidth(20)]
        [ValueDropdown(nameof(GetValuesX))]
        public float x;

        //[PropertyRange(1, 5)]
        [HorizontalGroup(.5f), LabelWidth(20)]
        [ValueDropdown(nameof(GetValuesY))]
        public float y;

        #endregion

        [DebuggerStepThrough]
        public static implicit operator Vector2(UVScale uv)
        {
            return new Vector2(uv.x, uv.y);
        }

        private float[] GetValuesX()
        {
            if (valuesX == null)
            {
                valuesX = new[] { 1f, 2f, 3f, 4f, 5f };
            }

            return valuesX;
        }

        private float[] GetValuesY()
        {
            if (valuesY == null)
            {
                valuesY = new[] { .25f, .5f, .75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f, 5f };
            }

            return valuesY;
        }

        #region ICloneable<UVScale> Members

        public UVScale Clone()
        {
            return new UVScale { x = x, y = y, };
        }

        #endregion
    }
}
