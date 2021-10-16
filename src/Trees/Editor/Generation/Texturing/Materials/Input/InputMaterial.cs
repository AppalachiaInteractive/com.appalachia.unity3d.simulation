using System;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input
{
    [Serializable]
    public abstract class InputMaterial : TreeMaterial
    {
        [SerializeField, InlineProperty, HideLabel]
        private InputTextureSet _textures;

        public InputTextureSet textures => _textures;

        [SerializeField]
        [HorizontalGroup("MAT", PaddingLeft = 0, PaddingRight = 10, MaxWidth = 100)]
        [InlineEditor(
             Expanded = true,
             DrawHeader = false,
             DrawPreview = true,
             PreviewHeight = 96,
             PreviewWidth = 96,
             ObjectFieldMode = InlineEditorObjectFieldModes.Hidden,
             DrawGUI = false)]
         [PropertyOrder(-501)]
        protected Material _material;

        [Button(ButtonSizes.Small), EnableIf(nameof(_materialPresent))]
        [VerticalGroup("MAT/RIGHT"), PropertyOrder(-500)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        public void Select()
        {
            EditorGUIUtility.PingObject(_material);
        }

        [VerticalGroup("MAT/RIGHT")]
        [LabelWidth(150)]
        public MaterialColorStyle colorStyle = MaterialColorStyle.Unchanged;

        private bool showColorBoost => (colorStyle == MaterialColorStyle.GrayscaleBoosted) || (colorStyle == MaterialColorStyle.Boosted);
        
        [VerticalGroup("MAT/RIGHT")]
        [LabelWidth(150)]
        [PropertyRange(0f, 4f)]
        [ShowIf(nameof(showColorBoost))]
        public float colorBoost = 1.0f;
        
        private bool _materialPresent => _material != null;

        public Material material => _material;

        protected InputMaterial(int materialID, Material material, ResponsiveSettingsType settingsType) : base(materialID, settingsType)
        {
            _material = material;
            _textures = TextureExtractor.GetInputTextureSet(material);
        }

        public abstract bool eligibleAsBranch { get; }
        public abstract bool eligibleAsBreak { get; }
        public abstract bool eligibleAsFrond { get; }
        public abstract bool eligibleAsLeaf { get; }

        [Button(ButtonSizes.Small), EnableIf(nameof(_materialPresent))]
        [VerticalGroup("MAT/RIGHT"), PropertyOrder(-499)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        public void UpdateTextures()
        {
            using (BUILD_TIME.INPUT_MAT.UpdateTextures.Auto())
            {
                _textures = TextureExtractor.GetInputTextureSet(material);
            }
        }

        public abstract Rect GetRect(Vector2 inputSize, Vector2 outputSize);
    }
}