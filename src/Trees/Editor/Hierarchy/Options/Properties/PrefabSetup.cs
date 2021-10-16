#region

using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Hierarchy.Options.Properties
{
    [Serializable]
    public class PrefabSetup : ResponsiveSettings, ICloneable<PrefabSetup>
    {
        [SerializeField, HideInInspector] private string _guid;

        [PropertyTooltip("Prefab to use.")]
        [TreePropertySimple, AssetsOnly, PropertyOrder(0)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        [OnValueChanged(nameof(RecalculateGuid))]
        public GameObject prefab;

        [PropertyTooltip("Position offset for the prefab.")]
        [TreePropertySimple, AssetsOnly, PropertyOrder(0)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector3 positionOffset;

        [PropertyTooltip("Rotation offset for the prefab.")]
        [TreePropertySimple, AssetsOnly, PropertyOrder(0)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Quaternion rotationOffset = Quaternion.identity;
                        
        [PropertyTooltip("Should a collider be added to the prefab?")]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public boolTree addCollider = TreeProperty.New(true);
        
        [PropertyTooltip("The radius of the added collider.")]
        [PropertyRange(0f, 2f)]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree colliderRadius = TreeProperty.New(0.50f);

        public string guid
        {
            get
            {
                if (prefab == null)
                {
                    _guid = null;
                }
                else if (_guid == null)
                {
                    AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(prefab, out _guid, out long _);
                }

                return _guid;
            }
        }

        private void RecalculateGuid()
        {
            if (prefab != null)
            {
                AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(prefab, out _guid, out long _);
            }
        }

        public PrefabSetup Clone()
        {
            return new PrefabSetup(settingsType)
            {
                _guid = _guid,
                positionOffset = positionOffset,
                prefab = prefab,
                rotationOffset = rotationOffset
            };
        }

        public PrefabSetup(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
    }
}
