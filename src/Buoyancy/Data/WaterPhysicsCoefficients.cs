using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class WaterPhysicsCoefficients : SingletonAppalachiaObject<WaterPhysicsCoefficients>
    {
        #region Fields and Autoproperties

        [InlineProperty]
        [SmartLabel]
        public WaterPhysicsCoefficentData data = new(1.0f);

        #endregion

        /// <inheritdoc />
        protected override async AppaTask WhenEnabled()
        {
            await base.WhenEnabled();

            using (_PRF_WhenEnabled.Auto())
            {
                if (data == default)
                {
                    data = new WaterPhysicsCoefficentData(1.0f);
                }

                data.ConfirmRanges();
            }
        }
    }
}
