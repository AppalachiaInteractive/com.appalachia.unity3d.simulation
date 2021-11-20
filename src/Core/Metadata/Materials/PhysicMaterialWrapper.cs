using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Core.Metadata.Materials
{
    [SmartLabel]
    public class PhysicMaterialWrapper : CategorizableAppalachiaObject
    {
        #region Fields and Autoproperties

        [SmartLabel] public Color labelBackgroundColor;

        [SmartLabel] public Color labelTextColor;

        [SmartLabel]
        [FormerlySerializedAs("gizmoColor")]
        public Color wireColor;

        [SmartLabel] public DensityMetadata defaultDensity;

        [SmartLabel]
        [PropertyRange(0f, 1.0f)]
        public float meshTransparency = .1f;

        [SmartLabel] public Material surface;

        [SmartLabel]
        [SmartLabelChildren]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterial material;

        #endregion

        public bool CanSearch => material == null;

        #region Event Functions

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            if (defaultDensity == null)
            {
                defaultDensity = AssetDatabaseManager.FindAssets<DensityMetadata>()
                                                     .FirstOrDefault_NoAlloc(
                                                          dm => dm.materialWrapper == this
                                                      );
            }

            if (defaultDensity == null)
            {
                AppaLog.Warn($"Need default density for material {name}", this);
            }
#endif
        }

        #endregion

#if UNITY_EDITOR
        [Button]
        [EnableIf(nameof(CanSearch))]
        public void Search()
        {
            var materials = AssetDatabaseManager.FindAssets<PhysicMaterial>();

            var foundMaterial = materials.FirstOrDefault_NoAlloc(m => m.name == name);

            if (foundMaterial != null)
            {
                material = foundMaterial;
            }
            else
            {
                foundMaterial = materials.FirstOrDefault_NoAlloc(m => m.name.StartsWith(name));

                if (foundMaterial != null)
                {
                    material = foundMaterial;
                }
                else
                {
                    foundMaterial = materials.FirstOrDefault_NoAlloc(m => name.StartsWith(m.name));

                    if (foundMaterial != null)
                    {
                        material = foundMaterial;
                    }
                }
            }

            var surfaces = AssetDatabaseManager.FindAssets<Material>();

            var foundSurface = surfaces.FirstOrDefault_NoAlloc(m => m.name == $"physics_{name}");

            if (foundSurface != null)
            {
                surface = foundSurface;
            }
            else
            {
                foundSurface = surfaces.FirstOrDefault_NoAlloc(
                    m => m.name.StartsWith("physics_") && m.name.EndsWith(name)
                );

                if (foundSurface != null)
                {
                    surface = foundSurface;
                }
            }
        }
#endif
    }
}
