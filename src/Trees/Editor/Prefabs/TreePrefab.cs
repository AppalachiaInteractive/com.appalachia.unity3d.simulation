#region

using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Appalachia.Simulation.Trees.Prefabs
{
    [Serializable]
    public class TreePrefab
    {
        public PrefabSetup setup;

        public int prefabID;

        public bool canMergeIntoTree;

        public List<TreePrefabLODCloningData> lods;

        public TreeComponentType type;

        public TreePrefab(int prefabID, PrefabSetup setup, TreeComponentType type)
        {
            this.prefabID = prefabID;
            this.setup = setup;
            this.type = type;

            Setup();
        }

        public int LODCount => lods?.Count ?? 0;

        public string guid => setup.guid;

        private void Setup()
        {
            var lodGroup = setup.prefab.GetComponentHereOrInChildren<LODGroup>();

            if (lodGroup)
            {
                lods = new List<TreePrefabLODCloningData>();

                var lodGroupLODs = lodGroup.GetLODs();

                for (var i = 0; i < lodGroupLODs.Length; i++)
                {
                    var lod = lodGroupLODs[i];

                    var cloneData = new TreePrefabLODCloningData(lod.renderers, type);

                    lods.Add(cloneData);
                }
            }
            else
            {
                var renderers = setup.prefab.GetComponentsInChildren<Renderer>();

                lods = new List<TreePrefabLODCloningData> {new TreePrefabLODCloningData(renderers, type)};
            }
        }

        public void SetAvailability()
        {
            canMergeIntoTree = lods.All(lod => lod.CanMergeIntoTree());
        }

        public IEnumerable<Object> GetExternalObjects()
        {
            return lods.SelectMany(l => l.GetExternalObjects()).Concat(new[] {setup.prefab});
        }

        public IEnumerable<Material> GetMaterials()
        {
            return lods.SelectMany(l => l.GetMaterials());
        }

        public TreePrefabLODCloningData GetLOD(int lodLevel)
        {
            var clamped = Mathf.Clamp(lodLevel, 0, lods.Count - 1);

            return lods[clamped];
        }

        public PrefabVertex ToVertex(
            ShapeData shape,
            Vector3 position,
            Vector3 normal,
            Quaternion rotation,
            float scale,
            float heightOffset,
            Matrix4x4 matrix)
        {
            var pf = new PrefabVertex() /*PrefabVertex.Get()*/;
            pf.Set(shape, position, normal, setup.prefab, prefabID, rotation, scale, heightOffset, matrix);

            return pf;
        }

        public PrefabVertex ToVertex(ShapeData shape, Vector3 position, float heightOffset)
        {
            return ToVertex(
                shape,
                position,
                Vector3.forward,
                MathUtils.QuaternionFromMatrix(shape.effectiveMatrix),
                shape.effectiveScale,
                heightOffset,
                shape.effectiveMatrix
            );
        }
    }
}