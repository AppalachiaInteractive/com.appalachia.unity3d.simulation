using System;
using Appalachia.Core.Objects.Layers;
using Appalachia.Simulation.Trees.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Global", "Control the generation of all tree species.", TitleAlignments.Split)]
    public class TreeGlobalSettings : SingletonAppalachiaTreeObject<TreeGlobalSettings>
    {
        #region Fields and Autoproperties

        [TitleGroup("Layers")]
        [BoxGroup("Layers/Trees")]
        public LayerSelection treeLayer;

        [BoxGroup("Layers/Interactions")]
        public LayerSelection interactionLayer;

        [BoxGroup("Layers/Interactions")]
        [PropertyRange(.1f, 1.0f)]
        public float trunkCutColliderRadiusAdditive = .25f;

        [BoxGroup("Layers/Logs")]
        public LayerSelection logLayer;

        [BoxGroup("Physics")] public PhysicMaterial woodMaterial;

        [BoxGroup("Physics")] public PhysicMaterial fruitMaterial;

        [BoxGroup("Physics")] public PhysicMaterial mushroomMaterial;

        #endregion
    }
}
