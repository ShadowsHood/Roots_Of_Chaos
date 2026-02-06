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
        if (GameManager.Instance == null || GameManager.runStats == null || GameManager.Instance.playerData == null)
        {
            return;
        }
        StatsManager currentStats = GameManager.runStats;

        int currentDamagedHealth = currentStats.MaxHealth - currentStats.Health;

        // Reset aux stats de base
        PlayerData baseData = GameManager.Instance.playerData;
        currentStats.MaxHealth = baseData.baseHealth;
        currentStats.Health = baseData.baseHealth;
        currentStats.MoveSpeed = baseData.moveSpeed;
        currentStats.FireRate = baseData.fireRate;
        currentStats.Damage = baseData.damage;
        currentStats.Range = baseData.range;

        foreach (var item in inventory)
        {
            if (item != null) item.Apply(currentStats);
        }

        int newHealth = Mathf.Clamp(currentStats.MaxHealth - currentDamagedHealth, 1, currentStats.MaxHealth);
        currentStats.Health = newHealth;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && GameManager.Instance != null)
            RecalculateStats();
    }
#endif
}
