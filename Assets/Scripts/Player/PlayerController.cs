using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private PlayerStats stats => GameManager.runtimeStats;
    public PlayerData player;

    [Header("Movement Feel")]
    public float acceleration = 16f;
    public float deceleration = 10f;
    private bool stunned = false;
    private Rigidbody2D rbody;
    private Vector2 moveInput;
    private HitFeedback hf;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.freezeRotation = true;
        hf = GetComponent<HitFeedback>();
    }

    void Start()
    {
        stats.MaxHealth = player.baseHealth;
        stats.Health = stats.MaxHealth;
        stats.MoveSpeed = player.moveSpeed;
        stats.FireRate = player.fireRate;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // void Update() {
    // }

    void FixedUpdate()
    {
        if (stunned) return;
        Vector2 targetVelocity = moveInput * stats.MoveSpeed;
        float lerp = (moveInput.magnitude > 0) ? acceleration : deceleration;
        rbody.linearVelocity = Vector2.Lerp(rbody.linearVelocity, targetVelocity, lerp * Time.fixedDeltaTime);
    }

    // void LateUpdate()
    // {
    // }

    public void TakeDamage(int dmg, Vector2 attackerPosition)
    {
        stats.Health -= dmg;
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        stats.Corruption += stats.hitPenalty / 100f;
        hf.PlayHitEffect();
        StartCoroutine(KnockbackRoutine(knockbackDir));

        if (stats.Health <= 0) Die();
    }

    private IEnumerator KnockbackRoutine(Vector2 dir)
    {
        stunned = true;
        // movement = Vector2.zero;
        hf.Knockback(dir);
        yield return new WaitForSeconds(0.2f);
        stunned = false;
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
