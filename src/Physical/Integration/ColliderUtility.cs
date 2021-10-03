using Appalachia.Core.Editing;
using Appalachia.Core.Editing.Attributes;
using Appalachia.Core.Editing.Behaviours;
using Appalachia.Core.Editing.Coloring;
using Appalachia.Filtering;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Integration
{
    public class ColliderUtility : EditorOnlyMonoBehaviour
    {
        [SmartLabel, InlineButton(nameof(ApplyToAll))] 
        public PhysicMaterialWrapper material;

        private void ApplyToAll()
        {
            var colliders = _transform.FilterComponents<Collider>(true).NoTriggers().RunFilter();

            for (var i = 0; i < colliders.Length; i++)
            {
                colliders[i].sharedMaterial = material.material;
            }
        }

        [ShowInInspector, HideLabel]
        public CollectionButtonSelector<PhysicsMaterials, PhysicMaterialWrapper>  materialSelector;

        public override EditorOnlyExclusionStyle exclusionStyle => EditorOnlyExclusionStyle.Component;

        protected override void Internal_OnEnable()
        {
            materialSelector = new CollectionButtonSelector<PhysicsMaterials, PhysicMaterialWrapper>(
                PhysicsMaterials.instance,
                (mat) =>
                {
                    material = mat;
                    ApplyToAll();
                },
                ColorPrefs.Instance.PhysicMaterialSelectorButton,
                ColorPrefs.Instance.PhysicMaterialSelectorColorDrop
            );
        }
    }
}
