using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private ProjectileData projectile;
    private Rigidbody2D rb;
    private Transform visualChild;

    private float height = 0.5f;
    private float verticalVelocity = 0f;
    private float gravity = -10f;
    private float timer = 0f;
    private float straightDuration;
    private float totalLifetime;

    [SerializeField][Range(0.1f, 5f)] private float curveSpeed = 1.0f;
    [SerializeField] private float floatIntensity = 0.1f; // Réduit un peu car le Perlin Noise cumule vite
    [SerializeField] private float floatFrequency = 1f;   // Vitesse de l'oscillation (vaguelette)
    private float noiseSeed; // Pour que chaque larme ondule différemment

    private Vector2 shotVelocity;
    private Vector2 targetInertia;
    private Vector2 sideDir; // Stocké pour le flottement

    public LayerMask wallLayer;

    public void Initialize(ProjectileData data, Vector2 dir, Vector2 playerVelocity)
    {
        projectile = data;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (transform.childCount > 0) visualChild = transform.GetChild(0);

        // 1. DIRECTION CARDINAL
        Vector2 shootDir = (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            ? new Vector2(Mathf.Sign(dir.x), 0)
            : new Vector2(0, Mathf.Sign(dir.y));

        shotVelocity = shootDir * projectile.speed;

        // 2. INERTIA
        sideDir = new Vector2(-shootDir.y, shootDir.x);
        float lateralSpeed = Vector2.Dot(playerVelocity, sideDir);

        // On garde un petit offset de départ, mais moins violent
        float randomOffset = Random.Range(-0.2f, 0.2f);
        targetInertia = sideDir * ((lateralSpeed * 0.5f) + randomOffset);

        // Seed unique pour le bruit de flottement
        noiseSeed = Random.Range(0f, 100f);

        // 3. LIFETIME
        float playerRange = GameManager.runtimeStats.Range;
        totalLifetime = playerRange * 0.25f;
        straightDuration = totalLifetime * 0.7f;

        // 4. ROTATION INIT
        float initialAngle = Mathf.Atan2(shotVelocity.y, shotVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, initialAngle - 90f);

        Destroy(gameObject, totalLifetime);
    }

    void FixedUpdate()
    {
        // 5. LINEAR CURVE WITH TEMPS
        float t = Mathf.Clamp01(timer * curveSpeed);

        // --- CALCUL DU FLOTTEMENT (PERLIN NOISE) ---
        // On génère une valeur entre -1 et 1 qui évolue fluidement avec le temps
        float noise = Mathf.PerlinNoise(noiseSeed, timer * floatFrequency) * 2f - 1f;
        Vector2 floatOffset = sideDir * (noise * floatIntensity);
        // -------------------------------------------

        Vector2 currentInertia = targetInertia * t;
        Vector2 moveVelocity = shotVelocity + currentInertia + floatOffset;

        // DROP
        if (timer > straightDuration)
        {
            moveVelocity *= 0.8f;
        }
        rb.linearVelocity = moveVelocity;

        // 6. ROTATION
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Update()
    {
        if (visualChild == null) return;
        timer += Time.deltaTime;

        // VISUAL DROP
        if (timer > straightDuration)
        {
            float dropGravity = gravity * 3f;
            verticalVelocity += dropGravity * Time.deltaTime;
            height += verticalVelocity * Time.deltaTime;
        }

        float visualY = Mathf.Max(height, -0.5f);
        visualChild.position = transform.position + new Vector3(0, visualY, 0);

        if (height < -0.2f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallLayer) != 0) { Destroy(gameObject); return; }
        if (col.CompareTag(projectile.targetTag))
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null) enemy.Die();
            Destroy(gameObject);
        }
    }
}