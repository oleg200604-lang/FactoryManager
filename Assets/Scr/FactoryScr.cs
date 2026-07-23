using System.Collections.Generic;
using UnityEngine;

public class FactoryScr : MonoBehaviour
{
    public IProduct[] storage;
    public List<ZoneClass> zones = new List<ZoneClass>();

    public void RegisterZone(ZoneClass zone)
    {
        if (zone == null || zone.cells.Count == 0) return;

        zones.Add(zone);
        Debug.Log($"Зону з {zone.cells.Count} клітинок додано до FactoryScr.");
    }
}

[System.Serializable]
public class ZoneClass
{
    public List<CellScr> cells = new List<CellScr>();
    public IBuilding buildings;
}