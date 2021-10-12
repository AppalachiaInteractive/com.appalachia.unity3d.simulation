using System.Collections.Generic;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Simulation.ReactionSystem.Cameras;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Terrains
{
    public class GrassDepressionCamera : ReactionSubsystemSingleCameraSingleLocation
    {
        private const string _systemName = "TERRAIN_GRASS_DEPRESSION";

        protected override string SubsystemName => _systemName;

        public override bool AutomaticRender => true;

        public override bool IsManualRenderingRequired(SubsystemCameraComponent cam)
        {
            return false;
        }

        protected override void OnBeforeInitialization()
        {
            var terrains = FindObjectsOfType<Terrain>();

            for (var i = 0; i < terrains.Length; i++)
            {
                var otherTerrain = terrains[i];

                var terrainCenter = otherTerrain.GetComponent<ReactionSubsystemCenter>();

                if (terrainCenter == null)
                {
                    terrainCenter = otherTerrain.gameObject.AddComponent<ReactionSubsystemCenter>();
                }

                terrainCenter.ValidateSubsystems();

                terrainCenter.offset = Vector3.Scale(
                    otherTerrain.terrainData.bounds.center,
                    otherTerrain.transform.lossyScale
                );

                if (terrainCenter.subsystems == null)
                {
                    terrainCenter.subsystems = new List<ReactionSubsystemBase>();
                }

                if (!terrainCenter.subsystems.Contains(this))
                {
                    terrainCenter.subsystems.Add(this);
                }
            }
        }

        protected override void OnInitializationStart()
        {
            var terrains = FindObjectsOfType<Terrain>();

            Group.lockSize = true;

            if (terrains.Length > 0)
            {
                var terrain = terrains[0];
                var terrainData = terrain.terrainData;

                Group.orthographicSize =
                    (int) (terrainData.bounds.extents.x * terrain.transform.lossyScale.x);
            }
        }

        protected override void OnInitializationComplete()
        {
        }

        protected override void OnRenderComplete()
        {
        }
    }
}
