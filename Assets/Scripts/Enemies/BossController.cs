using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public EnemyData enemy;

    public LayerMask obstacleMask;
    public int health = 100;

    private Rigidbody2D rb;
    private GameObject player;
    private Vector2 movement;
    private HitFeedback hf;

    private bool isDead = false;
    private float lastHit = 0f;
    public float hitCooldown = 0.3f;

    public static event Action OnBossKill;

    [Header("Falling Rocks Attack")]
    public ProjectileData rockProjectileData;
    public Transform rockContainer;
    public LayerMask groundLayer;
    public float attackRange = 10f;
    public int rocksPerWave = 3;
    public float rockAttackCooldown = 5f; // Cooldown entre les attaques de rochers
    private float lastRockAttack;

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
        if (isDead) return;

        // Suit le joueur continuellement
        Follow(enemy.moveSpeed);

        // Attaque de rochers périodique
        if (Time.time > lastRockAttack + rockAttackCooldown)
        {
            FallingRocksAttack();
            lastRockAttack = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (isDead) { rb.linearVelocity = Vector2.zero; return; }
        rb.linearVelocity = movement;
    }

    private bool IsPlayerInRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Follow(float speed)
    {
        if (player == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        movement = direction * speed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > lastHit + hitCooldown)
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(enemy.damage, transform.position);
            lastHit = Time.time;
        }
    }

    // ATTACK METHODS

    public void FallingRocksAttack()
    {
        StartCoroutine(SpawnRocksWave());
    }

    private IEnumerator SpawnRocksWave()
    {
        for (int i = 0; i < rocksPerWave; i++)
        {
            Vector2 randomPos = GetRandomGroundPosition();

            GameObject rock = Instantiate(rockProjectileData.prefab, Vector3.zero, Quaternion.identity, rockContainer);
            FallingRocksController rockCtrl = rock.GetComponent<FallingRocksController>();
            rockCtrl.Initialize(rockProjectileData, randomPos);

            yield return new WaitForSeconds(0.3f);
        }
    }

    private Vector2 GetRandomGroundPosition()
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * attackRange;
        Vector2 targetPos = (Vector2)transform.position + randomOffset;
        return targetPos;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        if (hf != null) hf.PlayHitEffect();

        if (health <= 0) { Die(); return; }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        OnBossKill?.Invoke();
        Destroy(gameObject);
        SceneManager.LoadScene("End");
    }
}