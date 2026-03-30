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
        if (item == null) return;
        item.ApplyInstantEffects(GameManager.runStats);
        inventory.Add(item);
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        if (GameManager.Instance == null || GameManager.runStats == null || GameManager.Instance.playerData == null) return;
        StatsManager currentStats = GameManager.runStats;

        int missingHealth = currentStats.MaxHealth - currentStats.Health;
        PlayerData baseData = GameManager.Instance.playerData;

        // Stats reset
        currentStats.MaxHealth = baseData.baseHealth;
        currentStats.Damage = baseData.damage;
        currentStats.MoveSpeed = baseData.moveSpeed;
        currentStats.Range = baseData.range;
        currentStats.FireRate = baseData.fireRate;
        currentStats.CorruptionGainRate = baseData.corruptionGainRate;
        currentStats.CorruptionHitPenalty = baseData.corruptionHitPenalty;
        currentStats.SpreadAngle = baseData.spreadAngle;
        currentStats.ShotSpeed = baseData.shotSpeed;
        currentStats.Luck = baseData.luck;

        foreach (var item in inventory)
        {
            if (item != null) item.ApplyPermanentEffects(currentStats);
        }

        currentStats.Health = Mathf.Clamp(currentStats.MaxHealth - missingHealth, 1, currentStats.MaxHealth);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && GameManager.Instance != null)
            RecalculateStats();
    }
#endif
}
