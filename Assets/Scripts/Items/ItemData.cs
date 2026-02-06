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
    public ItemEffect[] effects;

    public void Apply(StatsManager stats)
    {
        foreach (var e in effects)
        {
            switch (e.type)
            {
                case EffectType.MaxHealth:
                    stats.MaxHealth += (int)e.amount;
                    break;
                case EffectType.Health:
                    stats.Health += (int)e.amount;
                    break;
                case EffectType.Damage:
                    stats.Damage += (int)e.amount;
                    break;
                case EffectType.Range:
                    stats.Range += (int)e.amount;
                    break;
                case EffectType.Speed:
                    stats.MoveSpeed += e.amount;
                    break;
                case EffectType.FireRate:
                    stats.FireRate += e.amount;
                    break;
                case EffectType.Corruption:
                    stats.Corruption += e.amount;
                    break;
                case EffectType.CorruptionGainRate:
                    stats.passiveGainRate += e.amount;
                    break;
                case EffectType.CorruptionHitPenalty:
                    stats.hitPenalty += e.amount;
                    break;
                case EffectType.SpreadAngle:
                    stats.SpreadAngle += e.amount;
                    break;
                case EffectType.ShotSpeed:
                    stats.ShotSpeed += e.amount;
                    break;
                case EffectType.Luck:
                    stats.Luck += (int)e.amount;
                    break;
            }
        }
    }
}
