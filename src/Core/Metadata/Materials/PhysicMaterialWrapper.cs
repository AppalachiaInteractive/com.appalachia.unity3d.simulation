using Appalachia.Core.Collections.Extensions;
using Appalachia.Core.Editing.AssetDB;
using Appalachia.Core.Editing.Attributes;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Physical.Density;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Physical
{
    [SmartLabel]
    public class PhysicMaterialWrapper : SelfCategorizingAndSavingScriptableObject<PhysicMaterialWrapper>
    {
        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (defaultDensity == null)
            {
                defaultDensity = AssetDatabaseHelper.FindAssets<DensityMetadata>().FirstOrDefault_NoAlloc(dm => dm.materialWrapper == this);
            }
            if (defaultDensity == null)
            {
                Debug.LogWarning($"Need default density for material {name}", this);
            }
            #endif
        }

        
        [SmartLabel, SmartLabelChildren, InlineEditor(InlineEditorObjectFieldModes.Boxed)] 
        public PhysicMaterial material;

        [SmartLabel] 
        public DensityMetadata defaultDensity;
        
        [SmartLabel]
        [FormerlySerializedAs("gizmoColor")] 
        public Color wireColor;
        
        [SmartLabel, PropertyRange(0f, 1.0f)]
        public float meshTransparency = .1f;
        
        [SmartLabel]
        public Color labelBackgroundColor;
        
        [SmartLabel]
        public Color labelTextColor;

        [SmartLabel]
        public Material surface;

        public bool CanSearch => material == null;
        
        [Button, EnableIf(nameof(CanSearch))]
        public void Search()
        {
            var materials = AssetDatabaseHelper.FindAssets<PhysicMaterial>();

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
            
            var surfaces = AssetDatabaseHelper.FindAssets<Material>();
            
            var foundSurface = surfaces.FirstOrDefault_NoAlloc(m => m.name == $"physics_{name}");
            
            if (foundSurface != null)
            {
                surface = foundSurface;
            }
            else
            {
                foundSurface = surfaces.FirstOrDefault_NoAlloc(m => m.name.StartsWith("physics_") && m.name.EndsWith(name));
                
                if (foundSurface != null)
                {
                    surface = foundSurface;
                }
            }
        }
    }
}
