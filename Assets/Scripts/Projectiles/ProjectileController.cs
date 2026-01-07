using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private ProjectileData projectile;
    private Rigidbody2D rb;
    private Transform visualChild;

    private float height = 0.5f;
    private float verticalVelocity = 0f;
    private float gravity = -20f;
    private float timer = 0f;
    private float straightDuration;

    public LayerMask wallLayer;

    public void Initialize(ProjectileData data, Vector2 dir, Vector2 playerVelocity)
    {
        projectile = data;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (transform.childCount > 0) visualChild = transform.GetChild(0);

        // 1. FORCE CARDINAL DIRECTION (No diagonals)
        Vector2 shootDir = Vector2.zero;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            shootDir = new Vector2(Mathf.Sign(dir.x), 0); // Horizontal
        else
            shootDir = new Vector2(0, Mathf.Sign(dir.y)); // Vertical

        // 2. ISOLATE LATERAL MOMENTUM
        // sideDir is perpendicular to shootDir
        Vector2 sideDir = new Vector2(-shootDir.y, shootDir.x);
        float lateralSpeed = Vector2.Dot(playerVelocity, sideDir);
        Vector2 lateralMomentum = sideDir * (lateralSpeed * 0.5f);

        // 3. COMBINE
        Vector2 finalVelocity = (shootDir * projectile.speed) + lateralMomentum;
        rb.linearVelocity = finalVelocity;

        // 4. ROTATION
        float angle = Mathf.Atan2(finalVelocity.y, finalVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // 5. RANGE & LIFETIME
        float playerRange = GameManager.runtimeStats.Range;
        float totalLifetime = playerRange * 0.25f;
        straightDuration = totalLifetime * 0.7f;

        Destroy(gameObject, totalLifetime);
    }

    void Update()
    {
        if (visualChild == null) return;

        timer += Time.deltaTime;

        // 4. VISUAL BULLET DROP (Fake Height)
        if (timer > straightDuration)
        {
            verticalVelocity += gravity * Time.deltaTime;
            height += verticalVelocity * Time.deltaTime;
        }

        // Apply height only to the child sprite's local Y position
        float visualY = Mathf.Max(height, -0.5f);
        visualChild.localPosition = new Vector3(0, visualY, 0);

        // Death when hitting the ground
        if (height < -0.3f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Wall detection by Layer
        if (((1 << col.gameObject.layer) & wallLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // Enemy detection by Tag
        if (col.CompareTag(projectile.targetTag))
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null) enemy.Die();

            Destroy(gameObject);
        }
    }
}