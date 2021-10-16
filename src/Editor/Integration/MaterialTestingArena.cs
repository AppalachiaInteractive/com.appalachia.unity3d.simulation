using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Filtering;
using Appalachia.Editing.Core.Behaviours;
using Appalachia.Editing.Debugging.Testing;
using Appalachia.Simulation.Core.Metadata.Materials;
using Appalachia.Simulation.Core.Selections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Appalachia.Simulation.Integration
{
    public class MaterialTestingArena : EditorOnlyMonoBehaviour
    {
        [FoldoutGroup("Center")]
        [SmartLabel]
        public Transform[] center;

        [FoldoutGroup("Center")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper centerMaterial;

        [FoldoutGroup("Center")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection centerSelector;

        [FoldoutGroup("Floor")]
        [SmartLabel]
        public Transform[] floor;

        [FoldoutGroup("Floor")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper floorMaterial;

        [FoldoutGroup("Floor")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection floorSelector;

        [FoldoutGroup("Walls Low")]
        [SmartLabel]
        public Transform[] wallsLow;

        [FoldoutGroup("Walls Low")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper wallsLowMaterial;

        [FoldoutGroup("Walls Low")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection wallsLowSelector;

        [FoldoutGroup("Obstacles Low")]
        [SmartLabel]
        public Transform[] obstaclesLow;

        [FoldoutGroup("Obstacles Low")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper obstaclesLowMaterial;

        [FoldoutGroup("Obstacles Low")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection
            obstaclesLowSelector;

        [FoldoutGroup("Walls High")]
        [SmartLabel]
        public Transform[] wallsHigh;

        [FoldoutGroup("Walls High")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper wallsHighMaterial;

        [FoldoutGroup("Walls High")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection wallsHighSelector;

        [FoldoutGroup("Obstacles High")]
        [SmartLabel]
        public Transform[] obstaclesHigh;

        [FoldoutGroup("Obstacles High")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper obstaclesHighMaterial;

        [FoldoutGroup("Obstacles High")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection
            obstaclesHighSelector;

        [FoldoutGroup("Missile")]
        [SmartLabel]
        [ToggleLeft]
        public bool overrideMissile;

        [FoldoutGroup("Missile")]
        [ShowIf(nameof(overrideMissile))]
        [SmartLabel]
        public GameObject[] missiles;

        [FoldoutGroup("Missile")]
        [SmartLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public PhysicMaterialWrapper missileMaterial;

        [FoldoutGroup("Missile")]
        [ShowInInspector]
        [HideLabel]
        public PhysicMaterialLookupSelection missileSelector;

        [NonSerialized] private Transform[] _missiles;

        public override EditorOnlyExclusionStyle exclusionStyle =>
            EditorOnlyExclusionStyle.ObjectForceConflict;

        private void OnDisable()
        {
            Bazooka.instance.OnPreFire -= OnPreFire;
            Bazooka.instance.OnPostFire -= OnPostFire;
        }

        private void HandleApplication(
            Transform[] transforms,
            bool doColliders,
            bool doRenderers,
            PhysicMaterialWrapper wrapper)
        {
            if ((transforms == null) || (transforms.Length == 0) || (wrapper == null))
            {
                return;
            }

            for (var i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];

                if (doColliders)
                {
                    var colliders = t.FilterComponents<Collider>(true).NoTriggers().RunFilter();

                    for (var j = 0; j < colliders.Length; j++)
                    {
                        colliders[j].sharedMaterial = wrapper.material;
                    }
                }

                if (doRenderers)
                {
                    var renderers = t.FilterComponents<Renderer>(true).RunFilter();

                    for (var j = 0; j < renderers.Length; j++)
                    {
                        renderers[j].sharedMaterial = wrapper.surface;
                    }
                }
            }
        }

        [Button]
        private void ApplyAll()
        {
            Apply_Center();
            Apply_Floor();
            Apply_WallsLow();
            Apply_WallsHigh();
            Apply_ObstaclesLow();
            Apply_ObstaclesHigh();
            Apply_Missile();
        }

        private void Apply_Center()
        {
            HandleApplication(center, true, true, centerMaterial);
        }

        private void Apply_Floor()
        {
            HandleApplication(floor, true, true, floorMaterial);
        }

        private void Apply_WallsLow()
        {
            HandleApplication(wallsLow, true, true, wallsLowMaterial);
        }

        private void Apply_WallsHigh()
        {
            HandleApplication(wallsHigh, true, true, wallsHighMaterial);
        }

        private void Apply_ObstaclesLow()
        {
            HandleApplication(obstaclesLow, true, true, obstaclesLowMaterial);
        }

        private void Apply_ObstaclesHigh()
        {
            HandleApplication(obstaclesHigh, true, true, obstaclesHighMaterial);
        }

        private void Apply_Missile()
        {
            if ((_missiles == null) || (_missiles.Length == 0) || (_missiles[0] == null))
            {
                _missiles = new[] {Bazooka.instance.prefab.transform};
            }

            Bazooka.instance.material = missileMaterial.material;

            HandleApplication(_missiles, true, true, missileMaterial);
        }

        protected override void Internal_OnEnable()
        {
            centerSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    centerMaterial = mat;
                    Apply_Center();
                }
            );

            floorSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    floorMaterial = mat;
                    Apply_Floor();
                }
            );

            wallsLowSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    wallsLowMaterial = mat;
                    Apply_WallsLow();
                }
            );

            wallsHighSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    wallsHighMaterial = mat;
                    Apply_WallsHigh();
                }
            );

            obstaclesLowSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    obstaclesLowMaterial = mat;
                    Apply_ObstaclesLow();
                }
            );

            obstaclesHighSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    obstaclesHighMaterial = mat;
                    Apply_ObstaclesHigh();
                }
            );

            missileSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                mat =>
                {
                    missileMaterial = mat;
                    Apply_Missile();
                }
            );

            Bazooka.instance.OnPreFire += OnPreFire;
            Bazooka.instance.OnPostFire += OnPostFire;
        }

        private void OnPreFire(Bazooka b)
        {
            if (missiles.Length > 0)
            {
                var randomIndex = Random.Range(0, missiles.Length);

                b.prefab = missiles[randomIndex];
            }
        }

        private void OnPostFire(Bazooka b, GameObject missile)
        {
        }
    }
}
