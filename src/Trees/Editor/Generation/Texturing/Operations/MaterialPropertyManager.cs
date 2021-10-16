#region

using System.Linq;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Transmission;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    public static class MaterialPropertyManager
    {
        public static void SetMaterialNonProportionalAreas(InputMaterialCache inputMaterialCache)
        {
            ResetMaterialAreas(inputMaterialCache);

            var total = 0.0f;

            for (var i = 0; i < inputMaterialCache.atlasInputMaterials.Count; i++)
            {
                total += 1.0f;
            }

            for (var i = 0; i < inputMaterialCache.atlasInputMaterials.Count; i++)
            {
                inputMaterialCache.atlasInputMaterials[i].proportionalArea = 1.0f / total;
            }
        }

        public static void SetMaterialProportionalAreas(
            InputMaterialCache inputMaterialCache,
            LODGenerationOutput mainOutput)
        {
            ResetMaterialAreas(inputMaterialCache);

            var atlasArea = 0f;

            foreach (var triangle in mainOutput.triangles)
            {
                if (triangle.inputMaterialID == -1)
                {
                    continue;
                }

                var area = triangle.Area(mainOutput.vertices);

                var material = inputMaterialCache.GetByMaterialID(triangle.inputMaterialID) as AtlasInputMaterial;
                material.proportionalArea += area;

                if (triangle.context != TreeMaterialUsage.Bark)
                {
                    atlasArea += area;
                }
            }

            NormalizeMaterialAreas(inputMaterialCache, atlasArea);
        }

        private static void ResetMaterialAreas(InputMaterialCache inputMaterialCache)
        {
            foreach (var material in inputMaterialCache.atlasInputMaterials)
            {
                material.proportionalArea = 0f;
            }
        }

        private static void NormalizeMaterialAreas(InputMaterialCache inputMaterialCache, float atlasArea)
        {
            foreach (var material in inputMaterialCache.atlasInputMaterials)
            {
                material.proportionalArea /= atlasArea;
            }
        }

        /*public static void PrepareMaterialProperties(
            bool dynamicEnabled,
            InputMaterialCache inputMaterialCache,
            OutputMaterialCache outputMaterialCache)
        {
            using (BUILD_TIME.MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
                if (dynamicEnabled)
                {
                    SetDynamicPrototypes(inputMaterialCache, outputMaterialCache);

                    CopyPrototypeMaterialProperties(outputMaterialCache);
                }

                outputMaterialCache.atlasOutputMaterial.FinalizeMaterial();
            }
        }*/

        public static void AssignTransmissionMaterialProperties(
            TransmissionSettings transmissionSettings,
            MaterialTransmissionValues transmission,
            InputMaterialCache inputMaterialCache,
            OutputMaterialCache outputMaterialCache,
            MaterialSettings materialSettings)
        {
            using (BUILD_TIME.MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
                if (outputMaterialCache.atlasOutputMaterial != null)
                {
                    /*AssignTransmissionMaterialProperties_Atlas(
                        DefaultMaterialResource.instance.material,
                        materialSettings
                    );
                }
                else
                {*/
                    AtlasInputMaterial transmissionTemplate = null;

                    foreach (var inputMaterial in inputMaterialCache.atlasInputMaterials)
                    {
                        if (inputMaterial.transmissionTemplate)
                        {
                            transmissionTemplate = inputMaterial;
                            break;
                        }
                    }

                    if (transmissionTemplate == null)
                    {
                        var targetHue = 110;
                        var bestDistance = int.MaxValue;

                        foreach (var inputMaterial in inputMaterialCache.atlasInputMaterials)
                        {
                            if (!inputMaterial.eligibleAsLeaf)
                            {
                                continue;
                            }

                            var leafAverage = TransmissionColorCalculator.GetAverageColor(
                                inputMaterial.material.primaryTexture(),
                                true,
                                true
                            );

                            Color.RGBToHSV(leafAverage, out var hue, out _, out _);

                            var distance = Mathf.Abs(hue - targetHue);

                            if (distance < bestDistance)
                            {
                                transmissionTemplate = inputMaterial;
                            }
                        }
                    }

                    if (transmissionTemplate == null)
                    {
                        transmissionTemplate = inputMaterialCache.atlasInputMaterials
                            .OrderByDescending(b => b.proportionalArea)
                            .FirstOrDefault(tm => tm.eligibleAsLeaf && (tm.material != null));
                    }

                    if (transmissionTemplate != null)
                    {
                        TransmissionColorCalculator.SetAutomaticTransmission(
                            transmissionTemplate.material.primaryTexture(),
                            transmission,
                            transmissionSettings
                        );
                    }

                    if (transmissionSettings.setTransmissionColorsAutomatically)
                    {
                        foreach (var material in outputMaterialCache.atlasOutputMaterial)
                        {
                            material.asset.SetColorIfNecessary(
                                TreeMaterialProperties.SubsurfaceColor,
                                transmission.automaticTransmissionColor
                            );
                        }
                    }
                    else
                    {
                        foreach (var material in outputMaterialCache.atlasOutputMaterial)
                        {
                            material.asset.SetColorIfNecessary(
                                TreeMaterialProperties.SubsurfaceColor,
                                transmissionSettings.transmissionColor
                            );
                        }
                    }

                    /*foreach (var material in outputMaterialCache.atlasOutputMaterial)
                    {
                        AssignTransmissionMaterialProperties_Atlas(material.asset, materialSettings);
                    }*/
                }
            }
        }

        public static void CopyLODMaterialSettings(OutputMaterialCache outputMaterialCache)
        {
            using (BUILD_TIME.MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
                {
                    var modelAom = outputMaterialCache.atlasOutputMaterial.First();

                    for (var i = 1; i < outputMaterialCache.atlasOutputMaterial.Count; i++)
                    {
                        var mat = outputMaterialCache.atlasOutputMaterial.GetMaterialElementByIndex(i);

                        CopyPrototypeMaterialProperties(mat.asset, modelAom.asset, false, false);
                    }
                }

                {
                    var modelSom = outputMaterialCache.shadowCasterOutputMaterial.First();

                    for (var i = 1; i < outputMaterialCache.shadowCasterOutputMaterial.Count; i++)
                    {
                        var mat = outputMaterialCache.shadowCasterOutputMaterial.GetMaterialElementByIndex(i);

                        CopyPrototypeMaterialProperties(mat.asset, modelSom.asset, false, false);
                    }
                }

                {
                    foreach (var tiledOutputMaterial in outputMaterialCache.tiledOutputMaterials)
                    {
                        var modelTom = tiledOutputMaterial.First();

                        for (var i = 1; i < tiledOutputMaterial.Count; i++)
                        {
                            var mat = tiledOutputMaterial.GetMaterialElementByIndex(i);

                            CopyPrototypeMaterialProperties(mat.asset, modelTom.asset, false, false);
                        }
                    }
                }
            }
        }

        /*public static void AssignTransmissionMaterialProperties_Atlas(
            Material material,
            MaterialSettings materialSettings)
        {
            material.SetFloatIfNecessary(TreeMaterialProperties.LeafTransmission, materialSettings.leafTransmission);

            material.SetFloatIfNecessary(
                TreeMaterialProperties.LeafTransmissionCutoff,
                materialSettings.leafTransmissionCutoff
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.OcclusionTransmissionDamping,
                materialSettings.occlusionTransmissionDamping
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.TranslucencyStrength,
                materialSettings.translucencyStrength
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.TranslucencyNormalDistortion,
                materialSettings.translucencyNormalDistortion
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.TranslucencyScatteringFalloff,
                materialSettings.translucencyScatteringFalloff
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.TranslucencyDirect,
                materialSettings.translucencyDirect
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.TranslucencyAmbient,
                materialSettings.translucencyAmbient
            );

            material.SetFloatIfNecessary(
                TreeMaterialProperties.TranslucencyShadow,
                materialSettings.translucencyShadow
            );
        }*/

        /*public static void AssignMaterialWindProperties(WindSettings windSettings, TreeMaterialCollection materials)
        {
            using (BUILD_TIME.MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
                if (materials.outputMaterialCache.tiledOutputMaterials.Count == 0)
                {
                    AssignMaterialWindProperties_Tiled(windSettings, DefaultMaterialResource.instance.material);
                }
                else
                {
                    foreach (var outputMaterial in materials.outputMaterialCache.tiledOutputMaterials)
                    {
                        foreach (var material in outputMaterial)
                        {
                            AssignMaterialWindProperties_Tiled(windSettings, material.asset);
                        }
                    }
                }

                if (materials.outputMaterialCache.atlasOutputMaterial == null)
                {
                    AssignMaterialWindProperties_Atlas(windSettings, DefaultMaterialResource.instance.material);
                }
                else
                {
                    foreach (var material in materials.outputMaterialCache.atlasOutputMaterial)
                    {
                        AssignMaterialWindProperties_Atlas(windSettings, material.asset);
                    }
                }
            }
        }*/

        /*
        public static void AssignMaterialWindProperties_Tiled(WindSettings windSettings, Material material)
        {
            material.SetFloatIfNecessary(TreeMaterialProperties.MotionAmplitude, windSettings.trunkMotionAmplitude);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionAmplitude2, windSettings.branchMotionAmplitude);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionAmplitude3, windSettings.leafFlutterAmplitude);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionSpeed, windSettings.trunkMotionSpeed);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionSpeed2, windSettings.branchMotionSpeed);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionSpeed3, windSettings.leafFlutterSpeed);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionScale, windSettings.trunkMotionScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionScale2, windSettings.branchMotionScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionScale3, windSettings.leafFlutterScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionVariation2, windSettings.branchMotionVariation);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionVertical2, windSettings.branchMotionVertical);
        }
        */

        /*public static void AssignMaterialWindProperties_Atlas(WindSettings windSettings, Material material)
        {
            material.SetFloatIfNecessary(TreeMaterialProperties.MotionAmplitude, windSettings.trunkMotionAmplitude);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionAmplitude2, windSettings.branchMotionAmplitude);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionAmplitude3, windSettings.leafFlutterAmplitude);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionSpeed, windSettings.trunkMotionSpeed);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionSpeed2, windSettings.branchMotionSpeed);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionSpeed3, windSettings.leafFlutterSpeed);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionScale, windSettings.trunkMotionScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionScale2, windSettings.branchMotionScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionScale3, windSettings.leafFlutterScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionVariation2, windSettings.branchMotionVariation);

            material.SetFloatIfNecessary(TreeMaterialProperties.MotionVertical2, windSettings.branchMotionVertical);

        }*/

        /*
        public static void AssignDefaultMaterialProperties(
            OutputMaterialCache outputMaterialCache,
            MaterialSettings materialSettings)
        {
            using (BUILD_TIME.MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
                foreach (var outputMaterial in outputMaterialCache.tiledOutputMaterials)
                {
                    outputMaterial.FinalizeMaterial();

                    outputMaterial.material.SetColorIfNecessary(
                        TreeMaterialProperties.Color,
                        materialSettings.trunkColor
                    );

                    outputMaterial.material.SetColorIfNecessary(
                        TreeMaterialProperties.Color3,
                        materialSettings.baseColor
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BumpScale,
                        materialSettings.trunkNormalScale
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.NormalScale,
                        materialSettings.trunkNormalScale
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BumpScale3,
                        materialSettings.baseNormalScale
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.NormalScale3,
                        materialSettings.baseNormalScale
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.Smoothness,
                        materialSettings.trunkSmoothness
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.Smoothness3,
                        materialSettings.baseSmoothness
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.TrunkVariation,
                        materialSettings.trunkVariation
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseBlendHeight,
                        materialSettings.baseBlendHeight
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseBlendAmount,
                        materialSettings.baseBlendAmount
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseBlendVariation,
                        materialSettings.baseBlendVariation
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.TrunkHeightOffset,
                        materialSettings.trunkHeightOffset
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.TrunkHeightRange,
                        materialSettings.trunkHeightRange
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseBlendHeightContrast,
                        materialSettings.baseBlendHeightContrast
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseHeightOffset,
                        materialSettings.baseHeightOffset
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseHeightRange,
                        materialSettings.baseHeightRange
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.BaseBlendNormals,
                        materialSettings.baseBlendNormals
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.Occlusion,
                        materialSettings.trunkTextureOcclusion
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.Occlusion3,
                        materialSettings.baseOcclusion
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.VertexOcclusion,
                        materialSettings.trunkVertexOcclusion
                    );

                    outputMaterial.material.SetFloatIfNecessary(
                        TreeMaterialProperties.EnableBase,
                        outputMaterial.HasTexture(TextureMap.Variant_Albedo) ? 1f : 0f
                    );
                }

                outputMaterialCache.atlasOutputMaterial.textureSet.UpdateAlphaTestReferenceValue(
                    1f - ((materialSettings.leafTransparency.x + materialSettings.leafTransparency.y) / 2f)
                );

                outputMaterialCache.atlasOutputMaterial.material.SetColorIfNecessary(
                    TreeMaterialProperties.Color,
                    materialSettings.leafColor
                );

                outputMaterialCache.atlasOutputMaterial.material.SetColorIfNecessary(
                    TreeMaterialProperties.Color3,
                    materialSettings.nonLeafColor
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.Saturation,
                    materialSettings.leafSaturation
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.Brightness,
                    materialSettings.leafBrightness
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.BumpScale,
                    materialSettings.leafNormalScale
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.NormalScale,
                    materialSettings.leafNormalScale
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.Smoothness,
                    materialSettings.leafSmoothness
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.CutoffLow,
                    materialSettings.leafTransparency.x
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.CutoffHigh,
                    materialSettings.leafTransparency.y
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.Occlusion,
                    materialSettings.leafTextureOcclusion
                );

                outputMaterialCache.atlasOutputMaterial.material.SetFloatIfNecessary(
                    TreeMaterialProperties.VertexOcclusion,
                    materialSettings.leafVertexOcclusion
                );
            }
        }
        */

        /*public static void AssignDefaultMaterialProperties(
            OutputMaterialCache outputMaterialCache,
            MaterialSettings materialSettings)
        {
            using (BUILD_TIME.MAT_PROP_MGR.AssignMaterialProperties.Auto())
            {
                if (outputMaterialCache.tiledOutputMaterials.Count == 0)
                {
                    AssignDefaultMaterialProperties_Tiled(DefaultMaterialResource.instance.material, materialSettings);
                }
                else
                {
                    foreach (var outputMaterial in outputMaterialCache.tiledOutputMaterials)
                    {
                        outputMaterial.FinalizeMaterial();

                        foreach (var material in outputMaterial)
                        {
                            AssignDefaultMaterialProperties_Tiled(material.asset, materialSettings);

                            material.asset.SetFloatIfNecessary(
                                TreeMaterialProperties.EnableBase,
                                outputMaterial.HasTexture(TextureMap.Variant_Albedo) ? 1f : 0f
                            );
                        }
                    }
                }

                if (outputMaterialCache.atlasOutputMaterial == null)
                {
                    AssignDefaultMaterialProperties_Atlas(DefaultMaterialResource.instance.material, materialSettings);
                }
                else
                {
                    outputMaterialCache.atlasOutputMaterial.textureSet.UpdateAlphaTestReferenceValue(
                        1f - ((materialSettings.leafTransparency.x + materialSettings.leafTransparency.y) / 2f)
                    );

                    foreach (var material in outputMaterialCache.atlasOutputMaterial)
                    {
                        AssignDefaultMaterialProperties_Atlas(material.asset, materialSettings);
                    }
                }
            }
        }*/

        /*
        public static void AssignDefaultMaterialProperties_Tiled(Material material, MaterialSettings materialSettings)
        {
            material.SetColorIfNecessary(TreeMaterialProperties.Color, materialSettings.trunkColor);

            material.SetColorIfNecessary(TreeMaterialProperties.Color3, materialSettings.baseColor);

            material.SetFloatIfNecessary(TreeMaterialProperties.BumpScale, materialSettings.trunkNormalScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.NormalScale, materialSettings.trunkNormalScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.BumpScale3, materialSettings.baseNormalScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.NormalScale3, materialSettings.baseNormalScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.Smoothness, materialSettings.trunkSmoothness);

            material.SetFloatIfNecessary(TreeMaterialProperties.Smoothness3, materialSettings.baseSmoothness);

            material.SetFloatIfNecessary(TreeMaterialProperties.TrunkVariation, materialSettings.trunkVariation);

            material.SetFloatIfNecessary(TreeMaterialProperties.BaseBlendHeight, materialSettings.baseBlendHeight);

            material.SetFloatIfNecessary(TreeMaterialProperties.BaseBlendAmount, materialSettings.baseBlendAmount);

            material.SetFloatIfNecessary(
                TreeMaterialProperties.BaseBlendVariation,
                materialSettings.baseBlendVariation
            );

            material.SetFloatIfNecessary(TreeMaterialProperties.TrunkHeightOffset, materialSettings.trunkHeightOffset);

            material.SetFloatIfNecessary(TreeMaterialProperties.TrunkHeightRange, materialSettings.trunkHeightRange);

            material.SetFloatIfNecessary(
                TreeMaterialProperties.BaseBlendHeightContrast,
                materialSettings.baseBlendHeightContrast
            );

            material.SetFloatIfNecessary(TreeMaterialProperties.BaseHeightOffset, materialSettings.baseHeightOffset);

            material.SetFloatIfNecessary(TreeMaterialProperties.BaseHeightRange, materialSettings.baseHeightRange);

            material.SetFloatIfNecessary(TreeMaterialProperties.BaseBlendNormals, materialSettings.baseBlendNormals);

            material.SetFloatIfNecessary(TreeMaterialProperties.Occlusion, materialSettings.trunkTextureOcclusion);

            material.SetFloatIfNecessary(TreeMaterialProperties.Occlusion3, materialSettings.baseOcclusion);

            material.SetFloatIfNecessary(TreeMaterialProperties.VertexOcclusion, materialSettings.trunkVertexOcclusion);

        }

        public static void AssignDefaultMaterialProperties_Atlas(Material material, MaterialSettings materialSettings)
        {
            material.SetColorIfNecessary(TreeMaterialProperties.Color, materialSettings.leafColor);

            material.SetColorIfNecessary(TreeMaterialProperties.Color3, materialSettings.nonLeafColor);

            material.SetFloatIfNecessary(TreeMaterialProperties.Saturation, materialSettings.leafSaturation);

            material.SetFloatIfNecessary(TreeMaterialProperties.Brightness, materialSettings.leafBrightness);

            material.SetFloatIfNecessary(TreeMaterialProperties.BumpScale, materialSettings.leafNormalScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.NormalScale, materialSettings.leafNormalScale);

            material.SetFloatIfNecessary(TreeMaterialProperties.Smoothness, materialSettings.leafSmoothness);

            material.SetFloatIfNecessary(TreeMaterialProperties.CutoffLow, materialSettings.leafTransparency.x);

            material.SetFloatIfNecessary(TreeMaterialProperties.CutoffHigh, materialSettings.leafTransparency.y);

            material.SetFloatIfNecessary(TreeMaterialProperties.Occlusion, materialSettings.leafTextureOcclusion);

            material.SetFloatIfNecessary(TreeMaterialProperties.VertexOcclusion, materialSettings.leafVertexOcclusion);

        }
        */

        /*private static void SetDynamicPrototypes(InputMaterialCache ic, OutputMaterialCache oc)
        {
            using (BUILD_TIME.MAT_PROP_MGR.SetDynamicPrototypes.Auto())
            {
                var aom = oc.atlasOutputMaterial;
                var aim = ic.atlasInputMaterials;

                if (aom.prototypeMaterial != null)
                {
                    var match = aim.First(a => a.material == aom.prototypeMaterial);

                    if (!match.eligibleAsLeaf && aim.Any(a => a.eligibleAsLeaf))
                    {
                        aom.prototypeMaterial = null;
                    }
                }

                var materialSorted = aim.Where(m => m.material.shader == aom.First().asset.shader)
                    .OrderByDescending(b => b.proportionalArea)
                    .ToArray();

                aom.prototypeMaterial = materialSorted.FirstOrDefault(tm => tm.eligibleAsLeaf && (tm.material != null))
                    ?.material;

                if (aom.prototypeMaterial == null)
                {
                    aom.prototypeMaterial = materialSorted.FirstOrDefault(tm => tm.material != null)?.material;
                }
            }

            foreach (var tom in oc.tiledOutputMaterials)
            {
                var tim = ic.tiledInputMaterials;

                tom.prototypeMaterial = tim.Where(m => m.material.shader == tom.First().asset.shader)
                    .FirstOrDefault(tm => tm.material != null)
                    .material;
            }
        }

        private static void CopyPrototypeMaterialProperties(OutputMaterialCache outputMaterialCache, bool force = false)
        {
            foreach (var mat in outputMaterialCache.tiledOutputMaterials)
            {
                foreach (var material in mat)
                {
                    CopyPrototypeMaterialProperties(material.asset, mat.prototypeMaterial, force);
                }
            }

            foreach (var material in outputMaterialCache.atlasOutputMaterial)
            {
                CopyPrototypeMaterialProperties(
                    material.asset,
                    outputMaterialCache.atlasOutputMaterial.prototypeMaterial,
                    force
                );
            }
        }
        */
        
        private static void CopyPrototypeMaterialProperties(Material target, Material prototype, bool force, bool resetTextures)
        {
            using (BUILD_TIME.MAT_PROP_MGR.CopyPrototypeMaterialProperties.Auto())
            {
                if ((target == null) || (prototype == null))
                {
                    return;
                }

                if (!force && (target == prototype))
                {
                    return;
                }

                target.CopyPropertiesFromMaterial(prototype);

                if (resetTextures)
                {
                    var textureProperties = target.GetTexturePropertyNames();

                    foreach (var textureProperty in textureProperties)
                    {
                        target.SetTexture(textureProperty, null);
                        target.SetTextureOffset(textureProperty, Vector2.zero);
                        target.SetTextureScale(textureProperty, Vector2.one);
                    }
                }
            }
        }
    }
}