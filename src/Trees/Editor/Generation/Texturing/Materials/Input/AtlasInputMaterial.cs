using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input
{
    [Serializable]
    public class AtlasInputMaterial : InputMaterial
    {
        [SerializeField, HideInInspector] private bool _eligibleAsBreak;
        [SerializeField, HideInInspector] private bool _eligibleAsFrond;
        [SerializeField, HideInInspector] private bool _eligibleAsLeaf;

        [VerticalGroup("MAT/RIGHT")]
        [LabelWidth(150)]
        public bool transmissionTemplate = true;
        
        [VerticalGroup("MAT/RIGHT")]
        [LabelWidth(150)]
        public bool eligibleForDeadTrees;

        [VerticalGroup("MAT/RIGHT")]
        [LabelWidth(150)]
        public bool eligibleForLiveTrees = true;
        
        [VerticalGroup("MAT/RIGHT")]
        [ReadOnly, LabelWidth(150)]
        public float proportionalArea;
        
        [FormerlySerializedAs("uvRect")]
        [HideInInspector]
        public Rect atlasUVRect;
        
        [FormerlySerializedAs("packedRect")]
        [HideInInspector]
        public Rect atlasPackedUVRect;


        public override bool eligibleAsBranch => false;
        
        public override bool eligibleAsBreak => false;
        public override bool eligibleAsFrond => _eligibleAsFrond;
        public override bool eligibleAsLeaf => _eligibleAsLeaf;
        
        public override Rect GetRect(Vector2 inputSize, Vector2 outputSize)
        {
            return atlasPackedUVRect;
        }


        public override MaterialContext MaterialContext => MaterialContext.AtlasInputMaterial;

        public void SetEligibilityAsFrond(bool eligible)
        {
            _eligibleAsFrond = eligible;
        }

        public void SetEligibilityAsLeaf(bool eligible)
        {
            _eligibleAsLeaf = eligible;
        }

        public AtlasInputMaterial(int materialID, Material material, ResponsiveSettingsType settingsType) : base(materialID, material, settingsType)
        {
        }
    }
}