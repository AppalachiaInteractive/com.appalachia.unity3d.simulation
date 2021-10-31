using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Extensions;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Core.Metadata.Density;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Core.Metadata.Materials
{
    [SmartLabel]
    public class
        PhysicMaterialWrapper : CategorizableAppalachiaObject<PhysicMaterialWrapper>
    {
        [SmartLabel]
        [SmartLabelChildren]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterial material;

        [SmartLabel] public DensityMetadata defaultDensity;

        [SmartLabel]
        [FormerlySerializedAs("gizmoColor")]
        public Color wireColor;

        [SmartLabel]
        [PropertyRange(0f, 1.0f)]
        public float meshTransparency = .1f;

        [SmartLabel] public Color labelBackgroundColor;

        [SmartLabel] public Color labelTextColor;

        [SmartLabel] public Material surface;

        public bool CanSearch => material == null;

        private void OnEnable()
        {
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
                Debug.LogWarning($"Need default density for material {name}", this);
            }
#endif
        }

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
    }
}
