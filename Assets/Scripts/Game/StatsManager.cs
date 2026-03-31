using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StatsManager", menuName = "Data/StatsManager")]
public class StatsManager : ScriptableObject
{
    [Header("Stats")]
    public int maxHealth = 8;
    public int health;
    public float moveSpeed;
    public float fireRate;
    public int damage = 1;
    public int range = 4;
    public float spreadAngle = 5f;
    public float shotSpeed = 5f;
    public int luck = 0;

    [Header("Corruption")]
    public float corruption = 0.0f;
    public float corruptionGainRate = 1f;
    public float corruptionHitPenalty = 5f;
    public float corruptionDamageInterval = 1.0f;

    [Header("RecordedSacrifices")]
    public int damageSacrifice;
    public int maxHealthSacrifice;
    public float speedSacrifice;
    public int rangeSacrifice;
    public float fireRateSacrifice;
    public float corruptionGainRateSacrifice;
    public float corruptionHitPenaltySacrifice;
    public float spreadAngleSacrifice;
    public float shotSpeedSacrifice;
    public int luckSacrifice;

    public event Action OnHealthChanged;
    public event Action OnMaxHealthChanged;
    public event Action OnMoveSpeedChanged;
    public event Action OnFireRateChanged;
    public event Action OnDamageChanged;
    public event Action OnRangeChanged;
    public event Action OnCorruptionChanged;
    public event Action OnSpreadAngleChanged;
    public event Action OnShotSpeedChanged;
    public event Action OnLuckChanged;
    public event Action OnCorruptionGainRateChanged;
    public event Action OnCorruptionHitPenaltyChanged;

    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            int newMaxHealth = Mathf.Min(8, value);
            if (maxHealth != newMaxHealth)
            {
                maxHealth = newMaxHealth;
                OnMaxHealthChanged?.Invoke();
            }
        }
    }
    public int Health
    {
        get => health;
        set
        {
            int newHealth = Mathf.Clamp(value, 0, maxHealth);
            if (health != newHealth)
            {
                health = newHealth;
                OnHealthChanged?.Invoke();
            }
        }
    }
    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            float newmoveSpeed = Mathf.Max(0, value);
            if (moveSpeed != newmoveSpeed)
            {
                moveSpeed = newmoveSpeed;
                OnMoveSpeedChanged?.Invoke();
            }
        }
    }

    public float FireRate
    {
        get => fireRate;
        set
        {
            float newFireRate = Mathf.Max(0.1f, value);
            if (fireRate != newFireRate)
            {
                fireRate = newFireRate;
                OnFireRateChanged?.Invoke();
            }
        }
    }

    public int Damage
    {
        get => damage;
        set
        {
            int newDamage = Mathf.Max(1, value);
            if (damage != newDamage)
            {
                damage = newDamage;
                OnDamageChanged?.Invoke();
            }
        }
    }

    public int Range
    {
        get => range;
        set
        {
            int newRange = Mathf.Max(1, value);
            if (range != newRange)
            {
                range = newRange;
                OnRangeChanged?.Invoke();
            }
        }
    }

    public float Corruption
    {
        get => corruption;
        set
        {
            float newCorruption = Mathf.Clamp01(value);
            if (corruption != newCorruption)
            {
                corruption = newCorruption;
                OnCorruptionChanged?.Invoke();
            }
        }
    }

    public float SpreadAngle
    {
        get => spreadAngle;
        set
        {
            float newSpreadAngle = Mathf.Max(0, value);
            if (spreadAngle != newSpreadAngle)
            {
                spreadAngle = newSpreadAngle;
                OnSpreadAngleChanged?.Invoke();
            }
        }
    }

    public float ShotSpeed
    {
        get => shotSpeed;
        set
        {
            float newShotSpeed = Mathf.Max(0, value);
            if (shotSpeed != newShotSpeed)
            {
                shotSpeed = newShotSpeed;
                OnShotSpeedChanged?.Invoke();
            }
        }
    }

    public int Luck
    {
        get => luck;
        set
        {
            int newLuck = Mathf.Max(0, value);
            if (luck != newLuck)
            {
                luck = newLuck;
                OnLuckChanged?.Invoke();
            }
        }
    }

    public float CorruptionGainRate
    {
        get => corruptionGainRate;
        set
        {
            float newGainRate = Mathf.Max(0, value);
            if (corruptionGainRate != newGainRate)
            {
                corruptionGainRate = newGainRate;
                OnCorruptionGainRateChanged?.Invoke();
            }
        }
    }

    public float CorruptionHitPenalty
    {
        get => corruptionHitPenalty;
        set
        {
            float newHitPenalty = Mathf.Max(0, value);
            if (corruptionHitPenalty != newHitPenalty)
            {
                corruptionHitPenalty = newHitPenalty;
                OnCorruptionHitPenaltyChanged?.Invoke();
            }
        }
    }

    public float CorruptionDamageInterval
    {
        get => corruptionDamageInterval;
    }

    public bool IsCorrupted => corruption >= 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        maxHealth = Mathf.Min(8, maxHealth);
        health = Mathf.Clamp(health, 0, maxHealth);
        moveSpeed = Mathf.Max(0, moveSpeed);
        fireRate = Mathf.Max(0.1f, fireRate);
        damage = Mathf.Max(1, damage);
        range = Mathf.Max(1, range);
        corruption = Mathf.Clamp01(corruption);
        spreadAngle = Mathf.Max(0, spreadAngle);
        shotSpeed = Mathf.Max(0, shotSpeed);
        luck = Mathf.Max(0, luck);
        corruptionGainRate = Mathf.Max(0, corruptionGainRate);
        corruptionHitPenalty = Mathf.Max(0, corruptionHitPenalty);

        OnHealthChanged?.Invoke();
        OnMaxHealthChanged?.Invoke();
        OnMoveSpeedChanged?.Invoke();
        OnFireRateChanged?.Invoke();
        OnDamageChanged?.Invoke();
        OnRangeChanged?.Invoke();
        OnCorruptionChanged?.Invoke();
        OnSpreadAngleChanged?.Invoke();
        OnShotSpeedChanged?.Invoke();
        OnLuckChanged?.Invoke();
        OnCorruptionGainRateChanged?.Invoke();
        OnCorruptionHitPenaltyChanged?.Invoke();
    }
#endif
}
