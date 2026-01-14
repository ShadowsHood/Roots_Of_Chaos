using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private PlayerStats stats => GameManager.runtimeStats;
    public PlayerData player;

    [Header("Movement Feel")]
    public float acceleration = 16f;
    public float deceleration = 10f;
    private Rigidbody2D rbody;
    private Vector2 moveInput;
    private HitFeedback hitFeedback;

    public static event Action OnTakeDamage;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.freezeRotation = true;

        hitFeedback = GetComponent<HitFeedback>();
    }

    void Start()
    {
        stats.MaxHealth = player.baseHealth;
        stats.Health = stats.MaxHealth;
        stats.MoveSpeed = player.moveSpeed;
        stats.FireRate = player.fireRate;

        OnTakeDamage += () =>
        {
            stats.Corruption += stats.hitPenalty / 100f;
            hitFeedback.PlayHitEffect();
        };
    }

    void OnDestroy()
    {
        OnTakeDamage -= () =>
        {
            stats.Corruption += stats.hitPenalty / 100f;
            hitFeedback.PlayHitEffect();
        };
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // void Update() {
    // }

    void FixedUpdate()
    {
        // Debug.Log("Speed : " + player.moveSpeed);
        // rbody.linearVelocity = moveInput * stats.MoveSpeed;
        Vector2 targetVelocity = moveInput * stats.MoveSpeed;
        float lerp = (moveInput.magnitude > 0) ? acceleration : deceleration;
        rbody.linearVelocity = Vector2.Lerp(rbody.linearVelocity, targetVelocity, lerp * Time.fixedDeltaTime);
    }

    // void LateUpdate()
    // {
    // }

    public void TakeDamage(int dmg)
    {
        stats.Health -= dmg;
        OnTakeDamage?.Invoke();

        if (stats.Health <= 0)
        {
            Die();
        }
    }
    public void Heal(int healAmount)
    {
        stats.Health = Mathf.Min(stats.MaxHealth, stats.Health + healAmount);
    }

    void Die()
    {
        Debug.Log("Game Over");
        Destroy(gameObject);
    }
}
