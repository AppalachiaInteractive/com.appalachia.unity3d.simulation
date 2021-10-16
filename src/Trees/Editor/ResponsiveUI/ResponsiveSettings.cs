using System;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.ResponsiveUI
{
    [Serializable]
    public abstract class ResponsiveSettings : IResponsive
    {
        [FormerlySerializedAs("type")]
        [HideInInspector]
        public ResponsiveSettingsType settingsType;

        protected ResponsiveSettings(ResponsiveSettingsType settingsType)
        {
            this.settingsType = settingsType;
        }

        protected void MaterialGenerationChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.MaterialGeneration);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.MaterialGeneration);
            }
        }

        protected void MaterialPropertiesChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.MaterialProperty);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.MaterialProperty);
            }
        }

        protected void MeshSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Mesh);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Mesh);
            }
        }

        protected void DistributionSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
            }
            else  if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
            }
        }

        protected void GeometrySettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Geometry);
            }
        }


        protected void UVSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.UV);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.UV);
            }
        }

        protected void AmbientOcclusionSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.AmbientOcclusion);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else
            {
                BranchBuildRequestManager.SettingsChanged(SettingsUpdateTarget.AmbientOcclusion);
            }
        }

        protected void CollisionSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Collision);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
        }

        protected void ImpostorSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Impostor);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
        }


        protected void LevelOfDetailSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.LevelOfDetail);

            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
        }

        protected void VertexDataSettingsChanged()
        {
            if (settingsType == ResponsiveSettingsType.Tree)
            {
                TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
            else if (settingsType == ResponsiveSettingsType.Log)
            {
                LogBuildRequestManager.SettingsChanged(SettingsUpdateTarget.VertexData);
            }
        }

        public void UpdateSettingsType(ResponsiveSettingsType t)
        {
            settingsType = t;

            this.HandleResponsiveUpdate(t);
        }

        public virtual void CopySettingsTo(ResponsiveSettings t)
        {
            
        }
    }
}