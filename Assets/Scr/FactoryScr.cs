using System.Collections.Generic;
using UnityEngine;

public class FactoryScr : MonoBehaviour
{
    public Storage storage = new Storage();
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

    private int CalculateCapacity()
    {
        int capacity = storage.baseCapacity;

        foreach (ZoneClass zone in zones)
        {
            if (zone.type == ZoneType.Warehouse)
                capacity += zone.capacityBonus;
        }

        return capacity;
    }

    private void ProcessProduction()
    {
        int capacity = CalculateCapacity();

        foreach (ZoneClass zone in zones)
        {
            zone.ProcessDay(storage, capacity);
        }
    }

    private void LogStorageSummary()
    {
        storage.LogSummary(CalculateCapacity());
    }
}