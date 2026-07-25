using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProduct
{

}

public interface IBuilding
{

}
public enum ZoneType
{
    None, Workshop, Warehouse, Development
}
public enum BuildingType
{
    None, Workshopm, Composition, Development
}
public enum ProductType
{
    None, Product, Resource
}
public enum ResourceType
{
    None, Machines, Fabrics
}
public enum ClothingType
{
    None, Casual, Formal, Swimwear, Work, Winter
}

[System.Serializable]
public class WorkshopClass : IBuilding
{
    public int quality;
    public ProductType[] equipment;
    public ManufacturingClass need;
    public ManufacturingClass demand;
}

[System.Serializable]
public class WarehouseClass : IBuilding
{
    public int capacity;
    public List<IProduct> storedProducts = new List<IProduct>();
}

[System.Serializable]
public class DevelopmentClass : IBuilding
{
    public List<ClothingType> unlockedVariants = new List<ClothingType>();
}

public class ManufacturingClass
{
    public IProduct product;
    public int manufacturing;

}
public class RawClass : IProduct
{
    public ProductType productType;
    public ResourceType resourceType;
    public int quality;
}

public class ClothingClass : IProduct
{
    public ProductType productType;
    public ClothingType clothingType;
    public IProduct need;
    public int quality;
    public int design;
}