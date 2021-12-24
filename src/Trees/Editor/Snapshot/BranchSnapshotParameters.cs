using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Snapshot
{
    [Serializable]
    public class BranchSnapshotParameters : TypeBasedSettings<BranchSnapshotParameters>
    {
        #region Fields and Autoproperties

        [PropertyOrder(-50)] public bool active;

        [PropertyOrder(-50)] public bool equalizeAmbientOcclusion = true;

        [PropertyOrder(-50)] public bool equalizeSmoothness = true;

        [PropertyOrder(-50)] public bool equalizeTranslucency;

        [TabGroup("Tabs", "Camera", true, Paddingless = true), DisableIf(nameof(lockCameraPerspective))]
        public bool focusOnTrunk = true;

        [BoxGroup("Tabs/Camera")]
        public bool lockCameraPerspective;

        [PropertyOrder(-50)] public bool locked;

        [BoxGroup("Tabs/Camera/Rotation"), DisableIf(nameof(lockCameraPerspective))]
        public bool lockHorizontalRotation;

        [BoxGroup("Tabs/Camera/Rotation"), DisableIf(nameof(lockCameraPerspective))]
        public bool lockVerticalRotation = true;

        [TabGroup("Tabs", "Camera", true, Paddingless = true), DisableIf(nameof(lockCameraPerspective))]
        public bool orthographic = true;

        [TabGroup("Tabs", "General", true, Paddingless = true)]
        public BranchOutputMaterial branchOutputMaterial;

        [TabGroup("Tabs", "Lighting", true, Paddingless = true)]
        public Color ambientColor = new Color(.95f, .95f, .95f, 0);

        [TabGroup("Tabs", "Camera", true, Paddingless = true), PropertyRange(15f, 60f),
         HideIf(nameof(orthographic)), DisableIf(nameof(lockCameraPerspective))]
        public float fieldOfView = 30f;

        [BoxGroup("Tabs/Camera/Position"), ShowIf(nameof(lockCameraPerspective)), ReadOnly]
        public float lockedCameraDistance;

        [BoxGroup("Tabs/Camera/Position"), ShowIf(nameof(lockCameraPerspective)), ReadOnly]
        public float lockedCameraSize;

        [BoxGroup("Tabs/Lighting/Primary"), PropertyRange(0.1f, 3f)]
        public float primaryIntensity = 1.0f;

        [BoxGroup("Tabs/Lighting/Secondary"), PropertyRange(0.1f, 3f)]
        public float secondaryIntensity = 1.0f;

        [BoxGroup("Tabs/Camera/Position")]
        [PropertyRange(0f, 2f), DisableIf(nameof(lockCameraPerspective))]
        public float translationDistance = 1f;

        [BoxGroup("Tabs/Camera/Position"), DisableIf(nameof(lockCameraPerspective))]
        [PropertyRange(-1f, 1f)]
        public float translationXOffset;

        [BoxGroup("Tabs/Camera/Position"), DisableIf(nameof(lockCameraPerspective))]
        [PropertyRange(-1f, 1f)]
        public float translationYOffset;

        [TitleGroup("Branch Snapshot Information", Alignment = TitleAlignments.Centered)]
        [TabGroup("Tabs", "General", Paddingless = true)]
        [PropertyOrder(0)]
        public NameBasis nameBasis;

        [BoxGroup("Tabs/Camera/Position"), ShowIf(nameof(lockCameraPerspective)), ReadOnly]
        public Quaternion lockedCameraRotation = Quaternion.identity;

        [BoxGroup("Tabs/Camera/Position"), ShowIf(nameof(lockCameraPerspective)), ReadOnly]
        public Quaternion lockedModelRotation = Quaternion.identity;

        [BoxGroup("Tabs/Lighting/Primary")]
        public Quaternion primaryRotation = Quaternion.Euler(0f, 180f, 0f);

        [BoxGroup("Tabs/Lighting/Secondary")]
        public Quaternion secondaryRotation = Quaternion.Euler(0f, 0f, 0f);

        [TabGroup("Tabs", "Rendering", true, Paddingless = true)]
        public TextureSize textureSize = TextureSize.k1024;

        [HideInInspector] public Vector2 rotationOffset;

        [BoxGroup("Tabs/Camera/Position"), ShowIf(nameof(lockCameraPerspective)), ReadOnly]
        public Vector3 lockedCameraPosition = Vector3.zero;

        [BoxGroup("Tabs/Camera/Position"), ShowIf(nameof(lockCameraPerspective)), ReadOnly]
        public Vector3 lockedModelPosition = Vector3.zero;

        #endregion

        public Vector2 textureSizeV2 => new Vector2((int) textureSize, (int) textureSize);

        public static BranchSnapshotParameters Create(string folder, NameBasis nameBasis, int index)
        {
            var assetName = ZString.Format("{0}_{1}", nameBasis.FileNameSO("snapshot"), index);
            var instance = LoadOrCreateNew<BranchSnapshotParameters>(folder, assetName);

            return instance;
        }

        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
            branchOutputMaterial.settingsType = t;
        }

        public void CopySettings(BranchSnapshotParameters model)
        {
            locked = model.locked;
            lockCameraPerspective = model.lockCameraPerspective;
            lockedCameraPosition = model.lockedCameraPosition;
            lockedCameraRotation = model.lockedCameraRotation;
            lockedModelPosition = model.lockedModelPosition;
            lockedModelRotation = model.lockedModelRotation;
            rotationOffset = model.rotationOffset;
            focusOnTrunk = model.focusOnTrunk;
            orthographic = model.orthographic;
            fieldOfView = model.fieldOfView;
            translationXOffset = model.translationXOffset;
            translationYOffset = model.translationYOffset;
            translationDistance = model.translationDistance;
            lockHorizontalRotation = model.lockHorizontalRotation;
            lockVerticalRotation = model.lockVerticalRotation;
            ambientColor = model.ambientColor;
            primaryIntensity = model.primaryIntensity;
            primaryRotation = model.primaryRotation;
            secondaryIntensity = model.secondaryIntensity;
            secondaryRotation = model.secondaryRotation;
            textureSize = model.textureSize;
        }

        [BoxGroup("Scene Control", Order = -100), DisableIf(nameof(lockCameraPerspective))]
        [Button]
        public void ResetCamera()
        {
            focusOnTrunk = true;
            orthographic = true;
            fieldOfView = 30f;
            rotationOffset = Vector2.zero;
            translationDistance = 1f;
            translationXOffset = 0.0f;
            translationYOffset = 0.0f;
            translationXOffset = 0.0f;
            lockHorizontalRotation = false;
            lockVerticalRotation = true;
            lockCameraPerspective = false;
        }

        [BoxGroup("Scene Control", Order = -100)]
        [Button]
        public void ResetLighting()
        {
            primaryRotation = Quaternion.Euler(0f, 0f, 0);
            primaryIntensity = 1.0f;
            secondaryRotation = Quaternion.Euler(0f, 180f, 0);
            secondaryIntensity = 1.0f;

            ambientColor = new Color(.95f, .95f, .95f, 0);
        }
    }
}
