using System;
using Appalachia.Base.Scriptables;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class WaterPhysicsCoefficients : SelfSavingSingletonScriptableObject<WaterPhysicsCoefficients>
    {
        [InlineProperty, SmartLabel]
        public WaterPhysicsCoefficentData data = new WaterPhysicsCoefficentData(1.0f);

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
