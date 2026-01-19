using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private PlayerStats stats => GameManager.runStats;
    public PlayerData player;

    [Header("Movement Feel")]
    public float acceleration = 16f;
    public float deceleration = 10f;
    private bool stunned = false;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;
    public float flashSpeed = 15f;
    public float invincibilityMass = 10f;
    public bool invincible = false;
    private float minOpacity = 0.4f;
    private float maxOpacity = 0.6f;
    private float originalMass;

    private Rigidbody2D rbody;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private HitFeedback hf;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.freezeRotation = true;
        hf = GetComponent<HitFeedback>();
        sr = GetComponent<SpriteRenderer>();
        originalMass = rbody.mass;
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
        if (invincible) return;

        stats.Health -= dmg;
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        stats.Corruption += stats.hitPenalty / 100f;
        hf.PlayHitEffect();
        StartCoroutine(KnockbackRoutine(knockbackDir));
        StartCoroutine(InvincibilityRoutine());

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

    private IEnumerator InvincibilityRoutine()
    {
        invincible = true;
        rbody.mass = invincibilityMass;

        float timer = 0;
        while (timer < invincibilityDuration)
        {
            timer += Time.deltaTime;
            float wave = Mathf.Sin(timer * flashSpeed);
            float inter = (wave + 1f) / 2f;
            float alpha = Mathf.Lerp(minOpacity, maxOpacity, inter);
            sr.color = Helpers.Opacity(sr.color, alpha);

            yield return null;
        }

        sr.color = Helpers.Opacity(sr.color, 1.0f);
        rbody.mass = originalMass;
        invincible = false;
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
