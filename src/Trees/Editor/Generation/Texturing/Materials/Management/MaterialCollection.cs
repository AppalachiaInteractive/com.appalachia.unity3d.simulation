using System;
using System.Linq;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management
{
    [Serializable]
    public abstract class MaterialCollection<T> : TypeBasedSettings<T>
        where T : TypeBasedSettings<T>
    {
        #region Fields and Autoproperties

        [InlineProperty, HideLabel]
        [TabGroup("Input Materials", Paddingless = true)]
        public InputMaterialCache inputMaterialCache;

        [InlineProperty, HideLabel]
        [TabGroup("Output Materials", Paddingless = true)]
        public OutputMaterialCache outputMaterialCache;

        [HideInInspector] public ResponsiveSettingsType settingsType;
        [HideInInspector] public string hash;

        [SerializeField, HideInInspector]
        protected IDIncrementer _ids;

        #endregion

        public string CalculateHash()
        {
            using (BUILD_TIME.TREE_MAT_COLL.CalculateHash.Auto())
            {
                return inputMaterialCache.CalculateHash() + outputMaterialCache.CalculateHash();
            }
        }

        public InputMaterial FindInputMaterialForOutputMaterial(TiledOutputMaterial m)
        {
            return inputMaterialCache.GetByMaterialID(m.inputMaterialID);
        }

        public OutputMaterial GetOutputMaterialByInputID(int materialID)
        {
            var mat = inputMaterialCache.GetByMaterialID(materialID);

            if (mat.MaterialContext == MaterialContext.AtlasInputMaterial)
            {
                return outputMaterialCache.atlasOutputMaterial;
            }

            foreach (var output in outputMaterialCache.tiledOutputMaterials)
            {
                if (output.inputMaterialID == materialID)
                {
                    return output;
                }
            }

            return null;
        }

        public bool RequiresUpdate(MaterialSettings settings, out string currentHash)
        {
            using (BUILD_TIME.TREE_MAT_COLL.RequiresUpdate.Auto())
            {
                if (string.IsNullOrWhiteSpace(hash))
                {
                    currentHash = CalculateHash();
                    return true;
                }

                currentHash = CalculateHash();

                if (hash != currentHash)
                {
                    return true;
                }

                /*if (settings.forceRegenerateMaterials)
                {
                    return true;
                }*/

                if (outputMaterialCache.atlasOutputMaterial == null)
                {
                    return true;
                }

                if (outputMaterialCache.atlasOutputMaterial.RequiresUpdate)
                {
                    return true;
                }

                if (outputMaterialCache.tiledOutputMaterials == null)
                {
                    return true;
                }

                if (outputMaterialCache.tiledOutputMaterials.Any(n => n.RequiresUpdate))
                {
                    return true;
                }

                if ((inputMaterialCache.atlasInputMaterials.Count > 0) &&
                    inputMaterialCache.atlasInputMaterials.All(om => om.proportionalArea == 0))
                {
                    return true;
                }

                return false;
            }
        }

        public void UpdateMaterialNames(NameBasis nameBasis)
        {
            var count = 0;

            foreach (var material in outputMaterialCache.tiledOutputMaterials)
            {
                var inputMaterial = inputMaterialCache.GetByMaterialID(material.inputMaterialID);

                count = 0;

                foreach (var mat in material)
                {
                    mat.asset.name = ZString.Format(
                        "{0}_tiled_{1}_LOD{2}",
                        nameBasis.safeName,
                        inputMaterial.material.name,
                        count
                    );
                    count += 1;
                }
            }

            count = 0;
            foreach (var mat in outputMaterialCache.atlasOutputMaterial)
            {
                mat.asset.name = ZString.Format("{0}_atlas_LOD{1}", nameBasis.safeName, count);
                count += 1;
            }

            count = 0;
            foreach (var mat in outputMaterialCache.shadowCasterOutputMaterial)
            {
                if ((mat != null) && (mat.asset != null))
                {
                    mat.asset.name = ZString.Format("{0}_shadow_LOD{1}", nameBasis.safeName, count);
                    count += 1;
                }
            }
        }

        protected void Initialize(ResponsiveSettingsType type)
        {
            _ids = new IDIncrementer(true);
            inputMaterialCache = new InputMaterialCache();
            outputMaterialCache = new OutputMaterialCache(_ids, type);
            settingsType = type;
        }
    }
}
