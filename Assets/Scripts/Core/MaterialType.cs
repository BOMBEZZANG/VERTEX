namespace VERTEX.Core
{
    public enum MaterialType
    {
        Air,
        Wood,
        Stone,
        Steel,
        Dirt,
        Coal,
        Iron
    }
    
    public enum LoadStatus
    {
        Safe,
        Moderate,
        Stressed,
        Critical
    }
    
    public enum ResourceTier
    {
        Tier1, // Wood (Surface)
        Tier2  // Stone, Coal, Iron (Shallow underground)
    }
}