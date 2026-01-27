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

    [Header("Corruption")]
    public float corruption = 0.0f;
    public float passiveGainRate = 1f;
    public float hitPenalty = 5f;

    public event Action OnHealthChanged;
    public event Action OnMaxHealthChanged;
    public event Action OnMoveSpeedChanged;
    public event Action OnFireRateChanged;
    public event Action OnDamageChanged;
    public event Action OnRangeChanged;
    public event Action OnCorruptionChanged;

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

        OnHealthChanged?.Invoke();
        OnMaxHealthChanged?.Invoke();
        OnMoveSpeedChanged?.Invoke();
        OnFireRateChanged?.Invoke();
        OnDamageChanged?.Invoke();
        OnRangeChanged?.Invoke();
        OnCorruptionChanged?.Invoke();
    }
#endif
}
