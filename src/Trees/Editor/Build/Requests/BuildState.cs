namespace Appalachia.Simulation.Trees.Build.Requests
{
    public enum BuildState
    {
        Cancelled = -200,
        Disabled = -100,
        Default = 0,
        Full = 100,
        ForceFull = 200,
    }
}