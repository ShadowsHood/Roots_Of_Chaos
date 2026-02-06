using UnityEngine;

public class ProjectilePlayerController : MonoBehaviour
{
    private ProjectileData projectile;
    private Rigidbody2D rb;
    private Transform visualChild;

    private float height = 0.2f;
    private float verticalVelocity = 0f;
    private float gravity = -10f;
    private float timer = 0f;
    private float straightDuration;
    private float totalLifetime;
    private float spreadAngle;
    private float playerRange;
    private float shotSpeed;
    private int damage;

    public LayerMask wallLayer;

    void Awake()
    {
        spreadAngle = GameManager.runStats.SpreadAngle;
        playerRange = GameManager.runStats.Range;
        shotSpeed = GameManager.runStats.ShotSpeed;
        damage = GameManager.runStats.Damage;
    }

    public void Initialize(ProjectileData data, Vector2 dir, Vector2 playerVelocity)
    {
        projectile = data;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (transform.childCount > 0) visualChild = transform.GetChild(0);

        // 1. DIRECTION
        Vector2 shootDir = (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))


            ? new Vector2(Mathf.Sign(dir.x), 0)
            : new Vector2(0, Mathf.Sign(dir.y));

        // 2. INERTIA
        Vector2 sideDir = new Vector2(-shootDir.y, shootDir.x);
        float lateralSpeed = Vector2.Dot(playerVelocity, sideDir);
        Vector2 inertia = sideDir * (lateralSpeed * 0.5f) * 0.5f;

        // 3. VELOCITY + SPREAD
        float randomOffset = Random.Range(-spreadAngle, spreadAngle);
        Vector2 spreadDir = Quaternion.Euler(0, 0, randomOffset) * shootDir;

        rb.linearVelocity = (spreadDir * shotSpeed) + inertia;

        // 4. LIFETIME + ROTATION
        totalLifetime = playerRange / shotSpeed;
        straightDuration = totalLifetime * 0.7f;

        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Destroy(gameObject, totalLifetime);
    }

    void Update()
    {
        if (visualChild == null) return;
        timer += Time.deltaTime;

        // 5. DROP
        if (timer > straightDuration)
        {
            rb.linearVelocity *= (1f - Time.deltaTime * 5f);

            float dropGravity = gravity * 3f;
            verticalVelocity += dropGravity * Time.deltaTime;
            height += verticalVelocity * Time.deltaTime;
        }

        // 6. MOVEMENT
        float visualY = Mathf.Max(height, -0.5f);
        Vector3 pos = transform.position;
        visualChild.position = new Vector3(pos.x, pos.y + visualY, pos.z);

        if (height < -0.2f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallLayer) != 0) { Destroy(gameObject); return; }
        if (col.CompareTag(projectile.targetTag))
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                // Sinon essaie BossController
                BossController boss = col.GetComponent<BossController>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
    }
}