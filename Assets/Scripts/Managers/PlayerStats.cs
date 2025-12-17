using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Stats")]
    private int maxHealth = 8;
    // private int score = 0;
    private int health;
    private float moveSpeed;
    private float fireRate;

    private int damage = 1;
    private int range = 4;
    private float corruption = 0.0f;

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
}
