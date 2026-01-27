using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    private StatsManager stats => GameManager.runStats;
    [SerializeField] public List<ItemData> inventory = new();
    void Awake()
    {
        Instance = this;
    }

    public void Pickup(ItemData item)
    {
        // recalculate stats depending on previous items
        if (item == null) return;
        inventory.Add(item);
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        if (stats == null || GameManager.Instance?.playerData == null) return;

        int currentDamagedHealth = stats.MaxHealth - stats.Health;

        // Reset aux stats de base
        PlayerData baseData = GameManager.Instance.playerData;
        stats.MaxHealth = baseData.baseHealth;
        stats.Health = baseData.baseHealth;
        stats.MoveSpeed = baseData.moveSpeed;
        stats.FireRate = baseData.fireRate;
        // todo: reset other stats if needed

        foreach (var item in inventory)
        {
            item.Apply(stats);
        }

        int newHealth = Mathf.Clamp(stats.MaxHealth - currentDamagedHealth, 0, stats.MaxHealth);
        stats.Health = newHealth;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            RecalculateStats();
    }
#endif
}
