using UnityEngine;
using System;

public enum EffectType
{
    Health,
    MaxHealth,
    Damage,
    Range,
    Speed,
    FireRate,
    Corruption
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
                case EffectType.Health:
                    stats.Health += (int)e.amount;
                    break;
                case EffectType.MaxHealth:
                    stats.MaxHealth += (int)e.amount;
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
            }
        }
    }
}
