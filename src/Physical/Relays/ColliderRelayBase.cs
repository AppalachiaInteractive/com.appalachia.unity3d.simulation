#region

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [ExecuteAlways]
    public abstract class ColliderRelayBase : MonoBehaviour
    {
        public Collider[] relayingColliders;

        private void Awake()
        {
            UpdateRelayingColliders();
        }

        private void Start()
        {
            UpdateRelayingColliders();
        }

        private void OnEnable()
        {
            UpdateRelayingColliders();
        }

        private void OnDisable()
        {
            relayingColliders = null;
        }

        [Button]
        private void UpdateRelayingColliders()
        {
            relayingColliders = GetColliders();
        }

        protected abstract Collider[] GetColliders();
    }
}
