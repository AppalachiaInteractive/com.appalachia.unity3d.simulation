using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Simulation.ReactionSystem.Cameras;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Terrains
{
    public class GrassDepressionCamera : ReactionSubsystemSingleCameraSingleLocation<GrassDepressionCamera>
    {
        #region Constants and Static Readonly

        private const string SYSTEM_NAME = "TERRAIN_GRASS_DEPRESSION";

        #endregion

        /// <inheritdoc />
        public override bool AutomaticRender => true;

        /// <inheritdoc />
        protected override string SubsystemName => SYSTEM_NAME;

        /// <inheritdoc />
        public override bool IsManualRenderingRequired(SubsystemCameraComponent cam)
        {
            return false;
        }

        /// <inheritdoc />
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

                terrainCenter.EnsureSubsystemIsAdded(this);
            }
        }

        /// <inheritdoc />
        protected override void OnInitializationComplete()
        {
        }

        /// <inheritdoc />
        protected override void OnInitializationStart()
        {
            var terrains = FindObjectsOfType<Terrain>();

            Group.lockSize = true;

            if (terrains.Length > 0)
            {
                var terrain = terrains[0];
                var terrainData = terrain.terrainData;

                Group.orthographicSize = (int)(terrainData.bounds.extents.x * terrain.transform.lossyScale.x);
            }
        }

        /// <inheritdoc />
        protected override void OnRenderComplete()
        {
        }
    }
}
