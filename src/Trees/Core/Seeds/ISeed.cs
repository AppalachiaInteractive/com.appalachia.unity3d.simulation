namespace Appalachia.Simulation.Trees.Core.Seeds
{
    public interface ISeed
    {
        float RandomValue(float min = 0f, float max = 1f);
        
        int RandomValue(int min, int max);
        
        void Reset();
        
        float Noise2(float x, float y, float scale);
        
        float Noise2(float x, float y, float scaleX, float scaleY);
        
        float Noise3(float x, float y, float z, float scale, float offset);
        
        float Noise3(float x, float y, float z, float scaleX, float scaleY, float scaleZ, float offset);
    }
}
