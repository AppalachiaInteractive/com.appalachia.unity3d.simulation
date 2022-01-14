using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input
{
    [Serializable]
    public class TiledInputMaterial : InputMaterial
    {
        public TiledInputMaterial(int materialID, Material material, ResponsiveSettingsType settingsType) :
            base(materialID, material, settingsType)
        {
        }

        #region Fields and Autoproperties

        private bool _eligibleAsBreak;

        #endregion

        public override bool eligibleAsBranch => true;
        public override bool eligibleAsBreak => _eligibleAsBreak;
        public override bool eligibleAsFrond => false;
        public override bool eligibleAsLeaf => false;

        public override MaterialContext MaterialContext => MaterialContext.TiledInputMaterial;

        public override Rect GetRect(Vector2 inputSize, Vector2 outputSize)
        {
            var scale = outputSize / inputSize;

            var rect = new Rect(0, 0, inputSize.x, inputSize.y);
            rect.width *= scale.x;
            rect.height *= scale.y;

            return rect;
        }

        public void SetEligibilityAsBreak(bool eligible)
        {
            _eligibleAsBreak = eligible;
        }
    }
}
