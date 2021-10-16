/*using System;
using Appalachia.Simulation.Trees.Metadata.ResponsiveUI;
using Appalachia.Simulation.Trees.Runtime.Settings;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Metadata.Texturing.Materials.Base
{
    [Serializable]
    public abstract class TreeMaterial : ResponsiveSettings
    {
        [SerializeField]
        [HorizontalGroup("MAT", PaddingLeft = 0, PaddingRight = 10, MaxWidth = 100)]
        [InlineEditor(
            Expanded = true,
            DrawHeader = false,
            DrawPreview = true,
            PreviewHeight = 96,
            PreviewWidth = 96,
            ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden,
            DrawGUI = false
        ), PropertyOrder(-501)]
        protected Material _material;

        [Button(ButtonSizes.Small), EnableIf(nameof(_materialPresent))]
        [VerticalGroup("MAT/RIGHT"), PropertyOrder(-500)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        public void Select()
        {
            EditorGUIUtility.PingObject(_material);
        }

        private bool _materialPresent => _material != null;
        

        public Material material => _material;

        [SerializeField]
        [VerticalGroup("MAT/RIGHT")]
        [ReadOnly, LabelWidth(110), HideInInspector]
        private int _materialID;
        public int materialID => _materialID;
        
        public abstract MaterialContext MaterialContext { get; }

        public bool isOutputMaterial =>
            MaterialContext == MaterialContext.AtlasOutputMaterial ||
            MaterialContext == MaterialContext.TiledOutputMaterial;

        public bool isInputMaterial => !isOutputMaterial;

        protected TreeMaterial(int materialID, Material material, ResponsiveSettingsType settingsType) : base(settingsType)
        {
            this._materialID = materialID;
            this._material = material;
        }

        public void SetMaterial(string path)
        {
            _material = AssetDatabaseManager.LoadAssetAtPath<Material>(path);
        }
    }
}*/