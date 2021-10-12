using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class
        WaterPhysicsCoefficients : SelfSavingSingletonScriptableObject<WaterPhysicsCoefficients>
    {
        [InlineProperty]
        [SmartLabel]
        public WaterPhysicsCoefficentData data = new(1.0f);

        protected override void WhenEnabled()
        {
            if (data == default)
            {
                data = new WaterPhysicsCoefficentData(1.0f);
            }

            data.ConfirmRanges();
        }
    }
}
