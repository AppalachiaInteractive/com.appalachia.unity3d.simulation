using System;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.Selections.State;

namespace Appalachia.Simulation.Trees.Build.RequestManagers
{
    public static class TreeBuildRequestManager
    {
        private static TreeDataContainer CTX => TreeSpeciesEditorSelection.instance.tree.selection.selected;
        
        /// <summary>
        /// Builds all individuals for the species, resetting assets beforehand.
        /// </summary>
        public static void ForceFull()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.ForceFull;            
            CTX.PushBuildRequestLevelAll(BuildRequestLevel.FinalPass);
        }
        
        /// <summary>
        /// Builds all individuals for the species, only what needs to be built.
        /// </summary>
        public static void Full()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Full;
            CTX.PushBuildRequestLevelAll(BuildRequestLevel.FinalPass);
        }

        /// <summary>
        /// Builds only the current stage, only what needs to be built.
        /// </summary>
        public static void Default()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Default;
            CTX.PushBuildRequestLevelAll(BuildRequestLevel.FinalPass);
        }
        
        public static void TextureOnly()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Default;
            CTX.PushBuildRequestLevel(BuildRequestLevel.None, BuildRequestLevel.FinalPass, BuildRequestLevel.FinalPass);
        }     
        
        public static void CollidersOnly()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Default;
            CTX.PushBuildRequestLevel(collision: BuildRequestLevel.FinalPass);
        }  
        
        public static void ImpostorsOnly()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Default;
            CTX.PushBuildRequestLevel(impostors: BuildRequestLevel.FinalPass);
        }
        
        private static void MaterialGenerationChanged()
        {
            CTX.BuildMaterialGeneration(BuildRequestLevel.FinalPass)
                .BuildMaterialProperties(BuildRequestLevel.FinalPass);
        }
        
        private static void MaterialPropertiesChanged()
        {
            CTX.BuildMaterialProperties(BuildRequestLevel.FinalPass);
        }

        private static void DistributionSettingsChanged()
        {
            CTX.BuildDistribution(BuildRequestLevel.FinalPass)
                /*.BuildMaterialProperties(BuildRequestLevel.FinalPass)*/
                .BuildStage(BuildRequestLevel.FinalPass);
        }
        
        private static void GeometrySettingsChanged()
        {
            CTX.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage(BuildRequestLevel.FinalPass)
                /*.BuildLowQualityGeometry(BuildRequestLevel.InitialPass)
                .BuildUv(BuildRequestLevel.InitialPass)*/;
        }
        
        private static void MeshSettingsChanged()
        {        
            CTX.BuildDistribution(BuildRequestLevel.FinalPass).BuildStage(BuildRequestLevel.FinalPass)
                /*.BuildLowQualityGeometry(BuildRequestLevel.InitialPass)
                .BuildUv(BuildRequestLevel.InitialPass)*/;
        }

        
        private static void AmbientOcclusionSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage(BuildRequestLevel.FinalPass);
        }

        private static void LevelOfDetailSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage(BuildRequestLevel.FinalPass);
        }

        private static void VertexDataSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildMaterialProperties(BuildRequestLevel.FinalPass)
                .BuildStage(BuildRequestLevel.FinalPass);
        }
        
        private static void ImpostorSettingsChanged()
        {
            CTX.BuildImpostor(BuildRequestLevel.FinalPass);
        }
        
        private static void UVSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage(BuildRequestLevel.FinalPass);
        }
        
        private static void CollisionSettingsChanged()
        {
            CTX.BuildColliders(BuildRequestLevel.FinalPass);
        }

        public static void SettingsChanged(SettingsUpdateTarget type)
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            
            if (CTX.buildState != BuildState.Disabled)
            {
                CTX.buildState = BuildState.Default;
            }
            
            switch (type)
            {
                case SettingsUpdateTarget.MaterialGeneration:
                    MaterialGenerationChanged();
                    break;
                case SettingsUpdateTarget.MaterialProperty:
                    MaterialPropertiesChanged();
                    break;
                case SettingsUpdateTarget.Geometry:
                    GeometrySettingsChanged();
                    break;
                case SettingsUpdateTarget.Mesh:
                    MeshSettingsChanged();
                    break;
                case SettingsUpdateTarget.Distribution:
                    DistributionSettingsChanged();
                    break;
                case SettingsUpdateTarget.UV:
                    UVSettingsChanged();
                    break;
                case SettingsUpdateTarget.Collision:
                    CollisionSettingsChanged();
                    break;
                case SettingsUpdateTarget.AmbientOcclusion:
                    AmbientOcclusionSettingsChanged();
                    break;
                case SettingsUpdateTarget.LevelOfDetail:
                    LevelOfDetailSettingsChanged();
                    break;
                case SettingsUpdateTarget.VertexData:
                    VertexDataSettingsChanged();
                    break;
                case SettingsUpdateTarget.Impostor:
                    ImpostorSettingsChanged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}