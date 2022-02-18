#region

using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [ExecuteAlways]
    public abstract class ColliderRelayBase : AppalachiaBehaviour<ColliderRelayBase>
    {
        #region Fields and Autoproperties

        public Collider[] relayingColliders;

        #endregion

        protected abstract Collider[] GetColliders();

        /// <inheritdoc />
        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            relayingColliders = null;
        }

        /// <inheritdoc />
        protected override async AppaTask WhenEnabled()
        {
            await base.WhenEnabled();

            using (_PRF_WhenEnabled.Auto())
            {
                UpdateRelayingColliders();
            }
        }

        [Button]
        private void UpdateRelayingColliders()
        {
            relayingColliders = GetColliders();
        }
    }
}
