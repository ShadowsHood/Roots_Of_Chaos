using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    private ProjectileData projectile;
    private Vector2 direction;
    private Rigidbody2D rb;

    // void Start()
    // {
    //     StartCoroutine(DeathDelay());
    // }
    public void Initialize(ProjectileData data, Vector2 dir)
    {
        projectile = data;
        direction = dir.normalized;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.linearVelocity = direction * projectile.speed;

        // rotation visuelle du projectile
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // destruction automatique
        Destroy(gameObject, projectile.lifetime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (projectile == null) return;
        if (col.CompareTag(projectile.targetTag))
        {
            Debug.Log($"Projectile hit {projectile.targetTag}");

            // applique les dégâts si possible
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.Die();

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     transform.Translate(direction.normalized * speed * Time.deltaTime);
    // }

    // IEnumerator DeathDelay()
    // {
    //     yield return new WaitForSeconds(projectile.lifetime);
    //     Destroy(gameObject);
    // }
}
