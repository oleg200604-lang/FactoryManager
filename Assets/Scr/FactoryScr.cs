using System.Collections.Generic;
using UnityEngine;

public class FactoryScr : MonoBehaviour
{
    public List<IProduct> storage;
    public List<IBuilding> buildings;
    public List<ZoneClass> zones = new List<ZoneClass>();

    [SerializeField] private TimeScr timeScr;

    private void OnEnable()
    {
        if (timeScr != null)
        {
            timeScr.OnNewDay += ProcessProduction;
            timeScr.OnNewMonth += LogStorageSummary;
        }
    }

    private void OnDisable()
    {
        if (timeScr != null)
        {
            timeScr.OnNewDay -= ProcessProduction;
            timeScr.OnNewMonth -= LogStorageSummary;
        }
    }

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

    private void ProcessProduction()
    {
        foreach (ZoneClass zone in zones)
        {
            if (zone.type != ZoneType.Workshop) continue;

            foreach (CellScr cell in zone.cells)
            {
                if (cell.buld is WorkshopClass workshop)
                {
                    workshop.ProcessDay(zone, storage);
                }
            }
        }
    }

    private void LogStorageSummary()
    {
        Debug.Log($"Склад: {storage.Count} товар(ів).");
    }

    [ContextMenu("Тест: Налаштувати тестове виробництво")]
    private void SetupTestProduction()
    {
        foreach (ZoneClass zone in zones)
        {
            if (zone.type != ZoneType.Workshop) continue;

            foreach (CellScr cell in zone.cells)
            {
                if (cell.buld is WorkshopClass workshop)
                {
                    workshop.need = new ManufacturingClass
                    {
                        product = new RawClass { resourceType = ResourceType.Fabrics, quality = 1 },
                        manufacturing = 0
                    };

                    workshop.demand = new ManufacturingClass
                    {
                        product = new ClothingClass { clothingType = ClothingType.Casual, complexity = 3, quality = 1 },
                        manufacturing = 0
                    };

                    zone.TryAddMaterial(new RawClass { resourceType = ResourceType.Fabrics, quality = 1 });

                    Debug.Log("Тестове виробництво налаштовано.");
                    return;
                }
            }
        }

        Debug.Log("Не знайдено жодної майстерні у зонах типу Workshop.");
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