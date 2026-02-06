using UnityEngine;
using System.Collections;

public class EnemyShoot : MonoBehaviour
{
    [Header("Shooting")]
    public ProjectileData projectileData;
    public Transform projectileRoot;
    public float shootCooldown = 2f;
    public float shootRange = 8f;

    [Header("Prediction")]
    public bool predictPlayerMovement = true;
    public float predictionAmount = 0.5f; // Ajuste pour viser devant le joueur

    private EnemyController enemyController;
    private GameObject player;
    private Rigidbody2D playerRb;
    private float lastShot;

    void Awake()
    {
        GameObject projectiles = GameObject.Find("Projectiles");
        if (projectiles != null)
        {
            projectileRoot = projectiles.transform;
        }
    }

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || enemyController.currentState != EnemyState.Follow)
            return;

        float distToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Tirer si le joueur est dans la portée et cooldown écoulé
        if (distToPlayer <= shootRange && Time.time > lastShot + shootCooldown)
        {
            if (HasLineOfSight())
            {
                Shoot();
                lastShot = Time.time;
            }
        }
    }

    bool HasLineOfSight()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, shootRange, enemyController.obstacleMask);

        // Si on ne touche rien, on a line of sight
        return hit.collider == null;
    }

    void Shoot()
    {
        Vector2 targetPos = player.transform.position;

        // Prédiction du mouvement du joueur
        if (predictPlayerMovement && playerRb != null)
        {
            targetPos += playerRb.linearVelocity * predictionAmount;
        }

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        GameObject bullet = Instantiate(projectileData.prefab, transform.position, Quaternion.identity, projectileRoot);
        ProjectileController projCtrl = bullet.GetComponent<ProjectileController>();

        // L'ennemi ne bouge pas donc velocity = Vector2.zero (ou utilise le rb de l'ennemi si tu veux)
        projCtrl.Initialize(projectileData, direction, Vector2.zero);
    }

    // Debug visuel dans l'éditeur
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}