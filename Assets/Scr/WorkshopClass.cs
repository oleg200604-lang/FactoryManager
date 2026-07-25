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

    public void ProcessDay(ZoneClass zone, List<IProduct> factoryStorage)
    {
        if (demand == null || demand.product == null) return;

        ClothingClass targetProduct = demand.product as ClothingClass;
        if (targetProduct == null) return;

        demand.manufacturing++;

        if (demand.manufacturing < targetProduct.complexity) return;

        if (!TryConsumeResource(zone))
        {
            Debug.Log("Недостатньо ресурсів для завершення виробництва.");
            return;
        }

        demand.manufacturing = 0;

        ClothingClass finishedProduct = new ClothingClass
        {
            productType = targetProduct.productType,
            clothingType = targetProduct.clothingType,
            quality = quality,
            design = targetProduct.design,
            complexity = targetProduct.complexity
        };

        factoryStorage.Add(finishedProduct);
        Debug.Log($"Вироблено товар: {finishedProduct.clothingType} (якість {finishedProduct.quality}).");
    }

    private bool TryConsumeResource(ZoneClass zone)
    {
        if (need == null || need.product == null) return true;

        RawClass requiredResource = need.product as RawClass;
        if (requiredResource == null) return true;

        IProduct match = zone.materials.Find(m => m is RawClass raw && raw.resourceType == requiredResource.resourceType);

        if (match == null) return false;

        zone.materials.Remove(match);
        return true;
    }
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
    public int complexity;
}