namespace Appalachia.Simulation.Physical.Sampling
{
    public interface ITrilinearSampler<T>
        where T : struct
    {
        T Sample(TrilinearSamples<T> samples);
    }
}
