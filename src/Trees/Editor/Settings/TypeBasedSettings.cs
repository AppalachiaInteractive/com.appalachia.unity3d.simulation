using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Core.Serialization;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;

namespace Appalachia.Simulation.Trees.Settings
{
    [CallStaticConstructorInEditor]
    public abstract class TypeBasedSettings<T> : ResponsiveAppalachiaObject
        where T : TypeBasedSettings<T>
    {
        static TypeBasedSettings()
        {
            DefaultMaterialResource.InstanceAvailable += i => _defaultMaterialResource = i;
            DefaultShaderResource.InstanceAvailable += i => _defaultShaderResource = i;
            TreeGlobalSettings.InstanceAvailable += i => _treeGlobalSettings = i;
        }

        #region Static Fields and Autoproperties

        protected static DefaultMaterialResource _defaultMaterialResource;
        protected static DefaultShaderResource _defaultShaderResource;
        protected static TreeGlobalSettings _treeGlobalSettings;

        #endregion

        public virtual void CopySettingsTo(T t)
        {
        }

        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
            this.HandleResponsiveUpdate(t);
        }
    }
}
