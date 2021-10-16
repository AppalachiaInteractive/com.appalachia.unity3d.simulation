using Appalachia.Simulation.Trees.Build.Requests;

namespace Appalachia.Simulation.Trees.Build
{
    public static class BuildExtensions
    {
        public static BuildRequestLevel Max(this BuildRequestLevel b, BuildRequestLevel o)
        {
            if (b > o)
            {
                return b;
            }
            
            return o;
        }

     
    }
}