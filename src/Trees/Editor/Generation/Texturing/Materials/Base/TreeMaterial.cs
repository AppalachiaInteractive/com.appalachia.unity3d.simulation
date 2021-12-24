using System;
using Appalachia.Core.Attributes;
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
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static TreeMaterial()
        {
            DefaultShaderResource.InstanceAvailable += i => _defaultShaderResource = i;
        }

        protected static DefaultShaderResource _defaultShaderResource;
        
        [SerializeField]
        [VerticalGroup("RIGHT", Order = 10)]
        [ReadOnly, LabelWidth(110), HideInInspector, PropertyOrder(-1000)]
        private int _materialID;
        
        public int materialID => _materialID;
        
        public abstract MaterialContext MaterialContext { get; }

        public bool isOutputMaterial =>
            (MaterialContext == MaterialContext.AtlasOutputMaterial) ||
            (MaterialContext == MaterialContext.TiledOutputMaterial);

        public bool isInputMaterial => !isOutputMaterial;

        protected TreeMaterial(int materialID, ResponsiveSettingsType settingsType) : base(settingsType)
        {
            _materialID = materialID;
        }
    }
}