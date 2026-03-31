using UnityEngine;
using System.Collections;
using System;

public enum EnemyState
{
    Wander,
    Follow,
    // Scared
}

public class EnemyController : MonoBehaviour
{
    public EnemyData enemy;
    public LayerMask obstacleMask;
    public EnemyState currentState = EnemyState.Wander;
    public bool isActive = false;
    private int health;

    private Rigidbody2D rb;
    private GameObject player;
    private Vector2 movement;
    private HitFeedback hf;

    private bool isDead = false;
    private bool chooseDir = false;
    private float lastHit = 0f;
    public float hitCooldown = 0.3f;

    private bool stunned = false;

    public static event Action OnEnemyKill;


    void Start()
    {
        health = enemy.maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        hf = GetComponent<HitFeedback>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        if (isDead || !isActive) return;

        currentState = IsPlayerInRange(enemy.detectionRange) ? EnemyState.Follow : EnemyState.Wander;
        float speed = currentState == EnemyState.Follow ? enemy.moveSpeed : enemy.moveSpeed * 0.7f;

        switch (currentState)
        {
            case EnemyState.Wander: Wander(speed); break;
            case EnemyState.Follow: Follow(speed); break;
        }
    }

    void FixedUpdate()
    {
        if (isDead || !isActive) { rb.linearVelocity = Vector2.zero; return; }
        if (!stunned)
        {
            rb.linearVelocity = movement;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 randomDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        movement = randomDir * (enemy.moveSpeed * 0.7f);

        yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));
        chooseDir = false;
    }

    void Wander(float speed)
    {
        if (!chooseDir)
            StartCoroutine(ChooseDirection());

        if (movement != Vector2.zero)
            movement = movement.normalized * speed;
    }

    void Follow(float speed)
    {
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        movement = direction * speed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleMask) != 0)
        {
            if (currentState == EnemyState.Wander)
            {
                StopCoroutine(ChooseDirection());
                chooseDir = false;
                movement = Vector2.zero;
            }
        }

        if (collision.gameObject.CompareTag("Player") && Time.time > lastHit + hitCooldown)
        {
            if (PlayerController.Instance != null) PlayerController.Instance.TakeDamage(enemy.damage, transform.position);
            lastHit = Time.time;

            Vector2 recoilDir = (transform.position - collision.transform.position).normalized;
            StartCoroutine(KnockbackRoutine(recoilDir));
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        if (hf != null) hf.PlayHitEffect();

        if (health <= 0) { Die(); return; }

        Vector2 knockbackDir = (transform.position - player.transform.position).normalized;
        StartCoroutine(KnockbackRoutine(knockbackDir));
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        OnEnemyKill?.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator KnockbackRoutine(Vector2 dir)
    {
        stunned = true;
        movement = Vector2.zero;
        hf.Knockback(dir);
        yield return new WaitForSeconds(0.2f);
        stunned = false;
    }
}
