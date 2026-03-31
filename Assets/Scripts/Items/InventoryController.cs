using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    [SerializeField] public List<ItemData> inventory = new();
    void Awake()
    {
        Instance = this;
    }

    public void Pickup(ItemData item, bool wasPurchased)
    {
        if (item == null) return;
        if (wasPurchased) item.RecordSacrifice(GameManager.runStats);
        item.ApplyOneShotEffects(GameManager.runStats, wasPurchased);
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

        // Sacrifices reset
        currentStats.MaxHealth -= currentStats.maxHealthSacrifice;
        currentStats.Damage -= currentStats.damageSacrifice;
        currentStats.MoveSpeed -= currentStats.speedSacrifice;
        currentStats.Range -= currentStats.rangeSacrifice;
        currentStats.FireRate -= currentStats.fireRateSacrifice;
        currentStats.CorruptionGainRate -= currentStats.corruptionGainRateSacrifice;
        currentStats.CorruptionHitPenalty -= currentStats.corruptionHitPenaltySacrifice;
        currentStats.SpreadAngle -= currentStats.spreadAngleSacrifice;
        currentStats.ShotSpeed -= currentStats.shotSpeedSacrifice;
        currentStats.Luck -= currentStats.luckSacrifice;

        foreach (var item in inventory)
        {
            if (item != null) item.ApplyPermanentBuffs(currentStats);
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
