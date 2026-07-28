using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    None, Workshop, Warehouse, Development
}

[System.Serializable]
public class Recipe
{
    public string recipeName = "Новий рецепт";

    [SerializeReference, TypeSelector(typeof(IProductSettings))]
    public IProductSettings inputSettings;
    public int inputQuality;
    public int inputAmount = 1;

    [SerializeReference, TypeSelector(typeof(IProductSettings))]
    public IProductSettings outputSettings;
    public int outputQuality = 1;
    public int outputAmount = 1;
    public int complexity = 3;
}

[System.Serializable]
public class ZoneClass
{
    public ZoneType type;
    public List<CellScr> cells = new List<CellScr>();

    [Header("Майстерня (лише для типу Workshop)")]
    public List<Recipe> recipes = new List<Recipe>();

    [HideInInspector] public int activeRecipeIndex = -1;
    [HideInInspector] public int progress;

    [Header("Склад (лише для типу Warehouse)")]
    [Tooltip("Бонус місткості = кількість клітинок зони × Y")]
    public int capacityMultiplier = 5;

    public int ProductionPower => Mathf.Max(1, cells.Count / 2);
    public int CapacityBonus => cells.Count * capacityMultiplier;

    public void ProcessDay(Storage storage, int capacity)
    {
        if (type != ZoneType.Workshop || recipes.Count == 0) return;

        if (activeRecipeIndex < 0)
        {
            activeRecipeIndex = FindReadyRecipe(storage);

            if (activeRecipeIndex < 0)
            {
                Debug.Log("[Майстерня] Немає сировини для жодного з рецептів.");
                return;
            }

            progress = 0;
        }

        Recipe recipe = recipes[activeRecipeIndex];

        progress += ProductionPower;
        Debug.Log($"[Майстерня] {recipe.recipeName}: {progress}/{recipe.complexity} (сила {ProductionPower}).");

        if (progress < recipe.complexity) return;

        if (recipe.inputSettings != null && !storage.HasEnough(recipe.inputSettings, recipe.inputQuality, recipe.inputAmount))
        {
            Debug.Log($"[Майстерня] Сировина закінчилась під час виробництва ({recipe.inputSettings.Describe()}). Чекаємо.");
            return;
        }

        if (storage.TotalCount() + recipe.outputAmount > capacity)
        {
            Debug.Log("[Майстерня] Склад повний — готовий товар чекає місця.");
            return;
        }

        if (recipe.inputSettings != null)
            storage.TryRemove(recipe.inputSettings, recipe.inputQuality, recipe.inputAmount);

        storage.TryAdd(recipe.outputSettings, recipe.outputQuality, recipe.outputAmount, capacity);
        Debug.Log($"[Майстерня] Вироблено: {recipe.outputSettings.Describe()} x{recipe.outputAmount}.");

        progress = 0;
        activeRecipeIndex = -1;
    }

    private int FindReadyRecipe(Storage storage)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            Recipe recipe = recipes[i];
            if (recipe.outputSettings == null) continue;

            if (recipe.inputSettings == null || storage.HasEnough(recipe.inputSettings, recipe.inputQuality, recipe.inputAmount))
                return i;
        }

        return -1;
    }
}