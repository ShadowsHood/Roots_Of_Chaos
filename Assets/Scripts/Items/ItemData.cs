using UnityEngine;
using System;
using System.Diagnostics;

public enum EffectType
{
    Health,
    MaxHealth,
    Damage,
    Range,
    Speed,
    FireRate,
    Corruption,
    CorruptionGainRate,
    CorruptionHitPenalty,
    SpreadAngle,
    ShotSpeed,
    Luck
}

[Serializable]
public class ItemEffect
{
    public EffectType type;
    public float amount;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemEffect[] buffs;
    public ItemEffect[] nerfs;

    public void ApplyOneShotEffects(StatsManager stats, bool isShopPurchase)
    {
        foreach (var e in buffs)
        {
            if (e.type == EffectType.Health) stats.Health += (int)e.amount;
        }

        if (isShopPurchase && nerfs != null)
        {
            foreach (var n in nerfs)
            {
                switch (n.type)
                {
                    case EffectType.MaxHealth: stats.MaxHealth -= (int)n.amount; break;
                    case EffectType.Damage: stats.Damage -= (int)n.amount; break;
                    case EffectType.Speed: stats.MoveSpeed -= n.amount; break;
                    case EffectType.Range: stats.Range -= (int)n.amount; break;
                    case EffectType.FireRate: stats.FireRate -= n.amount; break;
                    case EffectType.CorruptionGainRate: stats.CorruptionGainRate -= n.amount; break;
                    case EffectType.CorruptionHitPenalty: stats.CorruptionHitPenalty -= n.amount; break;
                    case EffectType.SpreadAngle: stats.SpreadAngle -= n.amount; break;
                    case EffectType.ShotSpeed: stats.ShotSpeed -= n.amount; break;
                    case EffectType.Luck: stats.Luck -= (int)n.amount; break;
                    case EffectType.Health: stats.Health -= (int)n.amount; break;
                }
            }
        }
    }

    public void ApplyPermanentBuffs(StatsManager stats)
    {
        if (buffs == null) return;
        foreach (var e in buffs)
        {
            switch (e.type)
            {
                case EffectType.MaxHealth: stats.MaxHealth += (int)e.amount; break;
                case EffectType.Damage: stats.Damage += (int)e.amount; break;
                case EffectType.Speed: stats.MoveSpeed += e.amount; break;
                case EffectType.Range: stats.Range += (int)e.amount; break;
                case EffectType.FireRate: stats.FireRate += e.amount; break;
                case EffectType.CorruptionGainRate: stats.CorruptionGainRate += e.amount; break;
                case EffectType.CorruptionHitPenalty: stats.CorruptionHitPenalty += e.amount; break;
                case EffectType.SpreadAngle: stats.SpreadAngle += e.amount; break;
                case EffectType.ShotSpeed: stats.ShotSpeed += e.amount; break;
                case EffectType.Luck: stats.Luck += (int)e.amount; break;
            }
        }
    }

    public void RecordSacrifice(StatsManager stats)
    {
        if (nerfs == null) return;
        foreach (var n in nerfs)
        {
            switch (n.type)
            {
                case EffectType.MaxHealth: stats.maxHealthSacrifice += (int)n.amount; break;
                case EffectType.Damage: stats.damageSacrifice += (int)n.amount; break;
                case EffectType.Speed: stats.speedSacrifice += n.amount; break;
                case EffectType.Range: stats.rangeSacrifice += (int)n.amount; break;
                case EffectType.FireRate: stats.fireRateSacrifice += n.amount; break;
                case EffectType.CorruptionGainRate: stats.corruptionGainRateSacrifice += n.amount; break;
                case EffectType.CorruptionHitPenalty: stats.corruptionHitPenaltySacrifice += n.amount; break;
                case EffectType.SpreadAngle: stats.spreadAngleSacrifice += n.amount; break;
                case EffectType.ShotSpeed: stats.shotSpeedSacrifice += n.amount; break;
                case EffectType.Luck: stats.luckSacrifice += (int)n.amount; break;
            }
        }
    }
}
