using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProduct
{
    string Describe();
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
        Debug.Log($"[{zone.type}] Виготовлення: {demand.manufacturing}/{targetProduct.complexity} ({targetProduct.clothingType}).");

        if (demand.manufacturing < targetProduct.complexity) return;

        TryConsumeResource(zone);

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
        Debug.Log($"[{zone.type}] Вироблено товар: {finishedProduct.clothingType} (якість {finishedProduct.quality}). Всього на складі: {factoryStorage.Count}.");
    }

    private void TryConsumeResource(ZoneClass zone)
    {
        if (need == null || need.product == null) return;

        RawClass requiredResource = need.product as RawClass;
        if (requiredResource == null) return;

        IProduct match = zone.materials.Find(m => m is RawClass raw && raw.resourceType == requiredResource.resourceType);

        if (match == null)
        {
            Debug.Log($"[{zone.type}] Ресурсу {requiredResource.resourceType} не вистачало — автоматично поповнено.");
            return;
        }

        zone.materials.Remove(match);
        Debug.Log($"[{zone.type}] Витрачено ресурс: {requiredResource.resourceType}.");
    }
}

[System.Serializable]
public class WarehouseClass : IBuilding
{
    public int capacity;

    [SerializeReference, TypeSelector(typeof(IProduct))]
    public List<IProduct> storedProducts = new List<IProduct>();
}

[System.Serializable]
public class DevelopmentClass : IBuilding
{
    public List<ClothingType> unlockedVariants = new List<ClothingType>();
}

[System.Serializable]
public class ManufacturingClass
{
    [SerializeReference, TypeSelector(typeof(IProduct))]
    public IProduct product;
    public int manufacturing;
}

[System.Serializable]
public class RawClass : IProduct
{
    public ProductType productType;
    public ResourceType resourceType;
    public int quality;

    public string Describe() => $"{resourceType} (сировина, якість {quality})";
}

[System.Serializable]
public class ClothingClass : IProduct
{
    public ProductType productType;
    public ClothingType clothingType;

    [SerializeReference, TypeSelector(typeof(IProduct))]
    public IProduct need;
    public int quality;
    public int design;
    public int complexity;

    public string Describe() => $"{clothingType} (одяг, якість {quality})";
}