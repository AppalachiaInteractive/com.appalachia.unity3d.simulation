namespace Appalachia.Simulation.Trees.Core.Settings
{
    public interface IResponsive
    {
        #if UNITY_EDITOR
        void UpdateSettingsType(ResponsiveSettingsType t);
        #endif
    }
}
