public interface IProductSettings
{
    string Describe();
}

public enum ClothingType
{
    None, Casual, Formal, Swimwear, Work, Winter
}

public enum ResourceType
{
    None, Machines, Cotton, Wool
}

[System.Serializable]
public class ClothingSettings : IProductSettings
{
    public ClothingType clothingType;
    public ResourceType material;
    public int design;

    public string Describe() => $"{clothingType} (одяг, {material})";
}

[System.Serializable]
public class RawMaterialSettings : IProductSettings
{
    public ResourceType resourceType;

    public string Describe() => $"{resourceType} (сировина)";
}