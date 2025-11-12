using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameStats", menuName = "Data/GameStats")]
public class GameStats : ScriptableObject
{
    [Header("Stats")]
    private int maxHealth = 8;
    private int score = 0;
    private int health;
    private float moveSpeed;
    private float fireRate;

    public event Action OnHealthChanged;
    public event Action OnScoreChanged;

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = Mathf.Min(8, value);
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
        set => moveSpeed = Mathf.Max(0, value);
    }

    public float FireRate
    {
        get => fireRate;
        set => fireRate = Mathf.Max(0.1f, value);
    }

    public int Score
    {
        get => score;
        set
        {
            int newScore = Mathf.Max(0, value);
            if (score != newScore)
            {
                score = newScore;
                OnScoreChanged?.Invoke();
            }
        }
    }
}
