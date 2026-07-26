using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Storage
{
    public int baseCapacity = 20;

    [SerializeReference]
    public List<ProductStock> products = new List<ProductStock>();

    public int TotalCount()
    {
        int total = 0;
        foreach (ProductStock stock in products) total += stock.amount;
        return total;
    }

    public bool HasEnough(IProductSettings settings, int quality, int amount)
    {
        ProductStock stock = FindStock(settings, quality);
        return stock != null && stock.amount >= amount;
    }

    public bool TryAdd(IProductSettings settings, int quality, int amount, int capacity)
    {
        if (TotalCount() + amount > capacity)
        {
            Debug.Log($"Склад переповнений (максимум {capacity}).");
            return false;
        }

        ProductStock stock = FindStock(settings, quality);

        if (stock == null)
        {
            stock = new ProductStock { settings = settings, quality = quality, amount = 0 };
            products.Add(stock);
        }

        stock.amount += amount;
        return true;
    }

    public bool TryRemove(IProductSettings settings, int quality, int amount)
    {
        ProductStock stock = FindStock(settings, quality);

        if (stock == null || stock.amount < amount) return false;

        stock.amount -= amount;
        return true;
    }

    private ProductStock FindStock(IProductSettings settings, int quality)
    {
        string label = settings?.Describe() ?? "Невідомо";
        return products.Find(p => p.quality == quality && (p.settings?.Describe() ?? "Невідомо") == label);
    }

    public void LogSummary(int capacity)
    {
        int total = TotalCount();

        if (total == 0)
        {
            Debug.Log($"Склад: порожньо (0/{capacity}).");
            return;
        }

        string summary = $"Склад ({total}/{capacity}):";
        foreach (ProductStock stock in products)
        {
            if (stock.amount <= 0) continue;
            summary += $"\n— {stock.settings?.Describe() ?? "Невідомо"} (якість {stock.quality}): {stock.amount}";
        }

        Debug.Log(summary);
    }
}

[System.Serializable]
public class ProductStock
{
    [SerializeReference, TypeSelector(typeof(IProductSettings))]
    public IProductSettings settings;
    public int quality;
    public int amount;
}