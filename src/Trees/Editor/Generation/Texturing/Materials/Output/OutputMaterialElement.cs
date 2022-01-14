using System;
using System.Runtime.Serialization;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public sealed class OutputMaterialElement : IDeserializationCallback
    {
        public OutputMaterialElement(OutputShaderSelection selectedShader)
        {
            _selectedShader = selectedShader;
            _asset = new Material(_selectedShader.shader);
        }

        #region Fields and Autoproperties

        [SerializeField, HideInInspector]
        private bool _atlas;

        [SerializeField, ShowInInspector]
        [PropertyOrder(0)]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        private Material _asset;

        [PropertyOrder(-100)]
        [SerializeField, InlineProperty, HideLabel]
        private OutputShaderSelection _selectedShader;

        #endregion

        [ShowInInspector /*, InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)*/]
        public Material asset => _asset;

        public OutputShaderSelection selectedShader => _selectedShader;

        private bool _materialPresent => _asset != null;

        public void FinalizeMaterial(bool atlas)
        {
            asset.shader = selectedShader.shader;

            selectedShader.materialShader.FinalizeSettings(asset, atlas);
        }

        [Button(ButtonSizes.Small), EnableIf(nameof(_materialPresent))]
        [ /*VerticalGroup("MAT/RIGHT"),*/ PropertyOrder(-50)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        public void Select()
        {
            EditorGUIUtility.PingObject(_asset);
        }

        public void SetMaterial(string path, bool atlas)
        {
            _atlas = atlas;
            _asset = AssetDatabaseManager.LoadAssetAtPath<Material>(path);
        }

        public void SetMaterial(Material mat, bool atlas)
        {
            _atlas = atlas;
            _asset = mat;
        }

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            _selectedShader.OnShaderChanged += () => FinalizeMaterial(_atlas);
        }

        #endregion
    }
}
