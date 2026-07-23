using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProduct
{

}
public interface IBuilding
{

}
public enum BuildingType
{
    None, Workshopm, Composition, Development
}
public enum ProductType
{
    None, Product, Resource
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

public class ManufacturingClass
{
    public IProduct product;
    public int manufacturing;

}
public class RawClass : IProduct
{
    public ProductType productType;
    public int quality;
}

public class ClothingClass : IProduct
{
    public ClothingType clothingType;
    public int quality;
    public int design;
}