using System.Collections.Generic;
using UnityEngine;

public class FactoryScr : MonoBehaviour
{
    [SerializeReference, TypeSelector(typeof(IProduct))]
    public List<IProduct> storageProduct = new List<IProduct>();
    public List<int> storage;
    [SerializeReference, TypeSelector(typeof(IBuilding))]
    public List<IBuilding> buildings = new List<IBuilding>();

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

    public ZoneClass GetZoneForCell(CellScr cell)
    {
        foreach (ZoneClass zone in zones)
        {
            if (zone.cells.Contains(cell))
                return zone;
        }
        return null;
    }

    public bool IsCellInAnyZone(CellScr cell)
    {
        return GetZoneForCell(cell) != null;
    }

    public void RegisterZone(ZoneClass zone)
    {
        if (zone == null || zone.cells.Count == 0) return;

        zones.Add(zone);
        Debug.Log($"Зону типу {zone.type} з {zone.cells.Count} клітинок додано до FactoryScr.");

        if (zone.type == ZoneType.Workshop)
        {
            foreach (CellScr cell in zone.cells)
            {
                if (cell.buld is WorkshopClass workshop)
                    AutoSetupWorkshop(workshop);
            }
        }
    }

    public void AutoSetupWorkshop(WorkshopClass workshop)
    {
        if (workshop.demand != null && workshop.demand.product != null) return;

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

        Debug.Log("Майстерню автоматично налаштовано на виробництво.");
    }

    private void ProcessProduction()
    {
        foreach (ZoneClass zone in zones)
        {
            if (zone.type != ZoneType.Workshop) continue;

            foreach (CellScr cell in zone.cells)
            {
                if (cell.buld is WorkshopClass workshop)
                    workshop.ProcessDay(zone, storageProduct);
            }
        }
    }

    private void LogStorageSummary()
    {
        if (storageProduct.Count == 0)
        {
            Debug.Log("Склад: порожньо.");
            return;
        }

        Dictionary<string, int> counts = new Dictionary<string, int>();

        foreach (IProduct product in storageProduct)
        {
            string label = product.Describe();
            counts.TryGetValue(label, out int current);
            counts[label] = current + 1;
        }

        string summary = $"Склад ({storageProduct.Count} товар(ів)):";
        foreach (var pair in counts)
        {
            summary += $"\n— {pair.Key}: {pair.Value}";
        }

        Debug.Log(summary);
    }
}

[System.Serializable]
public class ZoneClass
{
    public ZoneType type;
    public List<CellScr> cells = new List<CellScr>();

    [SerializeReference, TypeSelector(typeof(IProduct))]
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