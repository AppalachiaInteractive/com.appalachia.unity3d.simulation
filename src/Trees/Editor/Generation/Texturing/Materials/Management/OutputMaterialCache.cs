#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management
{
    [Serializable]
    public class OutputMaterialCache
    {
        [SerializeField]
        [TabGroup("Atlas", Paddingless = true)]
        [HideLabel, InlineProperty, PropertyOrder(-1100)]
        [ShowIf(nameof(_showAtlasOutputMaterial))]
        private AtlasOutputMaterial _atlasOutputMaterial;

        [SerializeField]
        [TabGroup("Tiled", Paddingless = true)]
        [InlineProperty, PropertyOrder(-900)]
        [ListDrawerSettings(
            Expanded = true,
            DraggableItems = false,
            IsReadOnly = true,
            HideAddButton = true,
            HideRemoveButton = true,
            NumberOfItemsPerPage = 1,
            ShowPaging = true
        )]
        [ShowIf(nameof(_showTiledOutputMaterial))]
        private List<TiledOutputMaterial> _tiledOutputMaterials;

        [SerializeField]
        [TabGroup("Shadow", Paddingless = true)]
        [HideLabel, InlineProperty, PropertyOrder(-700)]
        [ShowIf(nameof(_showShadowCasterOutputMaterial))]
        private ShadowCasterOutputMaterial _shadowCasterOutputMaterial;

        public OutputMaterialCache(IDIncrementer ids, ResponsiveSettingsType type)
        {
            _tiledOutputMaterials = new List<TiledOutputMaterial>();

            _atlasOutputMaterial = new AtlasOutputMaterial(ids.GetNextIdAndIncrement(), type);
        }

        private bool _showAtlasOutputMaterial => _atlasOutputMaterial != null;
        private bool _showTiledOutputMaterial => _tiledOutputMaterials.Count > 0;
        
        private bool _showShadowCasterOutputMaterial => _shadowCasterOutputMaterial != null;

        public AtlasOutputMaterial atlasOutputMaterial => _atlasOutputMaterial;

        public IReadOnlyList<TiledOutputMaterial> tiledOutputMaterials => _tiledOutputMaterials;

        public ShadowCasterOutputMaterial shadowCasterOutputMaterial => _shadowCasterOutputMaterial;


        public void Reset()
        {
            _atlasOutputMaterial = null;
            _tiledOutputMaterials.Clear();
            _shadowCasterOutputMaterial = null;
        }

        
        public void Update(IDIncrementer ids, InputMaterialCache inputs, ResponsiveSettingsType type, int lodCount, bool shadowCaster)
        {
            using (BUILD_TIME.TREE_MAT_CACHE.Update.Auto())
            {
                try
                {
                    CreateOutputMaterials(ids, inputs.tiledInputMaterials, type, lodCount, shadowCaster);

                    for (var i = _tiledOutputMaterials.Count - 1; i >= 0; i--)
                    {
                        var om = _tiledOutputMaterials[i];

                        var imi = om.inputMaterialID;

                        if (!inputs.materialsByID.ContainsKey(imi))
                        {
                            _tiledOutputMaterials.RemoveAt(i);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppaLog.Error(ex);
                    throw;
                }
            }
        }

        private void CreateOutputMaterials(
            IDIncrementer ids, 
            IEnumerable<TiledInputMaterial> tiledInputMaterials, 
            ResponsiveSettingsType type, 
            int lodCount,
            bool shadowCaster)
        {
            if (_atlasOutputMaterial == null)
            {
                _atlasOutputMaterial = new AtlasOutputMaterial(ids.GetNextIdAndIncrement(), type);
            }
            
            _atlasOutputMaterial.EnsureCreated(lodCount);
            
            if (shadowCaster)
            {
                if (_shadowCasterOutputMaterial == null)
                {
                    _shadowCasterOutputMaterial = new ShadowCasterOutputMaterial(ids.GetNextIdAndIncrement());
                }
                
                _shadowCasterOutputMaterial.EnsureCreated(lodCount);
            }

            if (_tiledOutputMaterials == null)
            {
                _tiledOutputMaterials = new List<TiledOutputMaterial>();
            }

            foreach (var tiledInputMaterial in tiledInputMaterials)
            {
                var found = false;

                foreach (var tiledOutputMaterial in _tiledOutputMaterials)
                {
                    if (tiledOutputMaterial.inputMaterialID == tiledInputMaterial.materialID)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                var newTiledOutputMaterial = new TiledOutputMaterial(
                    ids.GetNextIdAndIncrement(),
                    tiledInputMaterial.materialID,
                    new UVScale(), type
                );

                _tiledOutputMaterials.Add(newTiledOutputMaterial);
            }

            foreach (var tiledMat in _tiledOutputMaterials)
            {
                tiledMat.EnsureCreated(lodCount);
            }
        }
        
        public string CalculateHash()
        {
            using (BUILD_TIME.TREE_MAT_COLL.CalculateHash.Auto())
            {
                var materials = tiledOutputMaterials.SelectMany(m => m.Select(e => e.asset))
                    .Concat(atlasOutputMaterial.Select(a => a.asset));

                var builder = new StringBuilder();

                foreach (var material in materials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    builder.Append(material.name);
                    builder.Append(material.shader.name);
                    builder.Append(material.GetInstanceID());

                    foreach (var prop in material.GetTexturePropertyNames())
                    {
                        var tex = material.GetTexture(prop) as Texture2D;
                        if (tex == null)
                        {
                            continue;
                        }

                        var path = AssetDatabaseManager.GetAssetPath(tex);

                        if (path.StartsWith("Assets"))
                        {
                            path = path.Replace("Assets", Application.dataPath);
                        }

                        if (path.StartsWith("Assets"))
                        {
                            path = path.Replace("Assets", Application.dataPath);
                        }

                        builder.Append(tex.name);
                        builder.Append(tex.GetInstanceID());
                        builder.Append(AppaFile.GetLastWriteTime(path).ToString("s"));
                        builder.Append(AppaFile.ReadAllText($"{path}.meta"));
                    }
                }

                return builder.ToString().GetHashCode().ToString();
            }
        }
    }
}
