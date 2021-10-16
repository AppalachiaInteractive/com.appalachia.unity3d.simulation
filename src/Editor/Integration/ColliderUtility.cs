using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Filtering;
using Appalachia.Editing.Core.Behaviours;
using Appalachia.Simulation.Core.Metadata.Materials;
using Appalachia.Simulation.Core.Selections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Integration
{
    public class ColliderUtility : EditorOnlyMonoBehaviour
    {
        [SmartLabel]
        [InlineButton(nameof(ApplyToAll))]
        public PhysicMaterialWrapper material;

        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection materialSelector;

        public override EditorOnlyExclusionStyle exclusionStyle =>
            EditorOnlyExclusionStyle.Component;

        private void ApplyToAll()
        {
            var colliders = _transform.FilterComponents<Collider>(true).NoTriggers().RunFilter();

            for (var i = 0; i < colliders.Length; i++)
            {
                colliders[i].sharedMaterial = material.material;
            }
        }

        protected override void Internal_OnEnable()
        {
            materialSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    material = mat;
                    ApplyToAll();
                }
            );
        }
    }
}
