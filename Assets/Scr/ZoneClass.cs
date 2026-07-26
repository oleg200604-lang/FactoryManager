using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    None, Workshop, Warehouse, Development
}

[System.Serializable]
public class ZoneClass
{
    public ZoneType type;
    public List<CellScr> cells = new List<CellScr>();

    [Header("Майстерня (лише для типу Workshop)")]
    [SerializeReference, TypeSelector(typeof(IProductSettings))]
    public IProductSettings inputSettings;
    public int inputQuality;
    public int inputAmount = 1;

    [SerializeReference, TypeSelector(typeof(IProductSettings))]
    public IProductSettings outputSettings;
    public int outputQuality = 1;
    public int outputAmount = 1;
    public int complexity = 3;
    [HideInInspector] public int progress;

    [Header("Склад (лише для типу Warehouse)")]
    public int capacityBonus = 10;

    public void ProcessDay(Storage storage, int capacity)
    {
        if (type != ZoneType.Workshop || outputSettings == null) return;

        if (progress < complexity)
        {
            progress++;
            Debug.Log($"[Майстерня] Виготовлення: {progress}/{complexity}.");
            if (progress < complexity) return;
        }

        if (inputSettings != null && !storage.HasEnough(inputSettings, inputQuality, inputAmount))
        {
            Debug.Log($"[Майстерня] Чекаємо на сировину: {inputSettings.Describe()} x{inputAmount}.");
            return;
        }

        if (storage.TotalCount() + outputAmount > capacity)
        {
            Debug.Log("[Майстерня] Склад повний — готовий товар чекає місця.");
            return;
        }

        if (inputSettings != null)
            storage.TryRemove(inputSettings, inputQuality, inputAmount);

        storage.TryAdd(outputSettings, outputQuality, outputAmount, capacity);
        Debug.Log($"[Майстерня] Вироблено: {outputSettings.Describe()} x{outputAmount}.");

        progress = 0;
    }
}