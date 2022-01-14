using System;
using Appalachia.Core.Objects.Root;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Transmission
{
    [Serializable, ReadOnly]
    public class MaterialTransmissionValues : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        public float lastAutoTransmissionBrightness;

        public Color lastAutoTransmissionColor = Color.black;

        public Texture2D lastAutoTransmissionTexture2D;

        public Color automaticTransmissionColor = new Color(.7f, .8f, .6f, 1f);

        #endregion
    }
}
