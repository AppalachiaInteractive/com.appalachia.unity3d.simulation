using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public abstract class TreeMaterial : ResponsiveSettings
    {
        static TreeMaterial()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<DefaultShaderResource>().IsAvailableThen( i => _defaultShaderResource = i);
        }

        protected TreeMaterial(int materialID, ResponsiveSettingsType settingsType) : base(settingsType)
        {
            _materialID = materialID;
        }

        #region Static Fields and Autoproperties

        protected static DefaultShaderResource _defaultShaderResource;

        #endregion

        #region Fields and Autoproperties

        [SerializeField]
        [VerticalGroup("RIGHT", Order = 10)]
        [ReadOnly, LabelWidth(110), HideInInspector, PropertyOrder(-1000)]
        private int _materialID;

        #endregion

        public abstract MaterialContext MaterialContext { get; }

        public bool isInputMaterial => !isOutputMaterial;

        public bool isOutputMaterial =>
            (MaterialContext == MaterialContext.AtlasOutputMaterial) ||
            (MaterialContext == MaterialContext.TiledOutputMaterial);

        public int materialID => _materialID;
    }
}
