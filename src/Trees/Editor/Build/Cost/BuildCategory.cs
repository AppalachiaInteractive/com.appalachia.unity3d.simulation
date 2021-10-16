namespace Appalachia.Simulation.Trees.Build.Cost
{
    public enum BuildCategory
    {
        None = 0,
        Distribution = 5,
        HighQualityGeometry = 10,
        LowQualityGeometry = 11,
        MaterialGeneration = 20,
        SavingTextures = 21,
        MaterialProperties = 30,
        AmbientOcclusion= 40,
        LevelsOfDetail = 50,
        VertexData = 80,
        //PrefabCreation = 90,
        UV = 100,
        Collision = 105,
        Mesh = 110,
        Impostor = 120,
    }
}
