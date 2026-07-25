using System.Collections.Generic;
using UnityEngine;

public class FactoryScr : MonoBehaviour
{
    public IProduct[] storage;
    public IBuilding[] buildings;
    public List<ZoneClass> zones = new List<ZoneClass>();

    public bool IsCellInAnyZone(CellScr cell)
    {
        foreach (ZoneClass zone in zones)
        {
            if (zone.cells.Contains(cell))
                return true;
        }
        return false;
    }

    public void RegisterZone(ZoneClass zone)
    {
        if (zone == null || zone.cells.Count == 0) return;

        zones.Add(zone);
        Debug.Log($"Зону типу {zone.type} з {zone.cells.Count} клітинок додано до FactoryScr.");
    }
}

[System.Serializable]
public class ZoneClass
{
    public ZoneType type;
    public List<CellScr> cells = new List<CellScr>();
    public List<IProduct> materials = new List<IProduct>();

    public bool TryAddMaterial(IProduct product)
    {
        if (!IsAllowedMaterial(product))
        {
            Debug.Log($"Матеріал {product.GetType().Name} не підходить для зони типу {type}.");
            return false;
        }

        materials.Add(product);
        Debug.Log($"Матеріал {product.GetType().Name} додано до зони {type}.");
        return true;
    }

    private bool IsAllowedMaterial(IProduct product)
    {
        switch (type)
        {
            case ZoneType.Workshop:
                return product is RawClass;
            case ZoneType.Warehouse:
                return product is ClothingClass;
            default:
                return false;
        }
    }
}