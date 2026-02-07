using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;

using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private StatsManager stats => GameManager.runStats;

    [Header("Movement Feel")]
    public float acceleration = 16f;
    public float deceleration = 10f;
    private bool stunned = false;
    private Animator bodyAnimator;
    private Vector2 lastMoveDirection;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;
    public float flashSpeed = 15f;
    public float invincibilityMass = 10f;
    public bool invincible = false;
    private float minOpacity = 0.4f;
    private float maxOpacity = 0.6f;
    private float originalMass;

    private Rigidbody2D rbody;
    private SpriteRenderer[] spriteRenderers;
    private SpriteRenderer bodySr;
    private Vector2 moveInput;
    private HitFeedback hf;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.freezeRotation = true;
        hf = GetComponent<HitFeedback>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        bodySr = transform.Find("Body").GetComponent<SpriteRenderer>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        originalMass = rbody.mass;
    }

    // void Start()
    // {

    // }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (stats.Health <= 0) Die();
        // Animation
        bool isMoving = moveInput.magnitude > 0.01f;
        bodyAnimator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            lastMoveDirection = moveInput.normalized;

            bodyAnimator.SetFloat("moveX", lastMoveDirection.x);
            bodyAnimator.SetFloat("moveY", lastMoveDirection.y);

            if (lastMoveDirection.x != 0)
            {
                bodySr.flipX = lastMoveDirection.x < 0;
            }
        }
    }

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
        SoundManager.Instance.PlaySound3D("Hit", transform.position);

        stats.Health -= dmg;
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        stats.Corruption += stats.CorruptionHitPenalty / 100f;
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
            foreach (var sr in spriteRenderers)
                sr.color = Helpers.Opacity(sr.color, alpha);

            yield return null;
        }

        foreach (var sr in spriteRenderers)
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
        SoundManager.Instance.PlaySound3D("Die", transform.position);
        Debug.Log("Game Over");
        Destroy(gameObject);
        // yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("End");
    }
}
