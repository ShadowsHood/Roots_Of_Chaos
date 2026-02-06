using UnityEngine;

public class BossShoot : MonoBehaviour
{
    [Header("Shooting")]
    public ProjectileData projectileData;
    public Transform projectileRoot;
    public float shootCooldown = 2f;

    [Header("Prediction")]
    public bool predictPlayerMovement = true;
    public float predictionAmount = 0.5f;

    [Header("Pattern")] // ← Header doit être AVANT un champ, pas avant un enum
    public ShootPattern shootPattern = ShootPattern.EightDirections;

    public enum ShootPattern { TargetPlayer, FourDirections, EightDirections } // ← Enum après le champ

    private GameObject player;
    private Rigidbody2D playerRb;
    private float lastShot;

    // Les 4 directions cardinales
    private Vector2[] cardinalDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    // Les 8 directions (cardinales + diagonales)
    private Vector2[] eightDirections = new Vector2[]
    {
        Vector2.up,                              // Haut
        Vector2.down,                            // Bas
        Vector2.left,                            // Gauche
        Vector2.right,                           // Droite
        new Vector2(1, 1).normalized,            // Haut-Droite
        new Vector2(-1, 1).normalized,           // Haut-Gauche
        new Vector2(1, -1).normalized,           // Bas-Droite
        new Vector2(-1, -1).normalized           // Bas-Gauche
    };

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time > lastShot + shootCooldown)
        {
            Shoot();
            lastShot = Time.time;
        }
    }

    void Shoot()
    {
        Debug.Log($"Shooting with pattern: {shootPattern}");
        switch (shootPattern)
        {
            case ShootPattern.FourDirections:
            Debug.Log("4 directions");
                foreach (Vector2 direction in cardinalDirections) // ← Renommé
                    SpawnProjectile(direction);
                break;

            case ShootPattern.EightDirections:
                Debug.Log($"8 directions - Count: {eightDirections.Length}");
                foreach (Vector2 direction in eightDirections)
                {
                    Debug.Log($"Spawning projectile in direction: {direction}");
                    SpawnProjectile(direction);
                }
                break;

            case ShootPattern.TargetPlayer:

                Vector2 targetPos = player.transform.position;
                if (predictPlayerMovement && playerRb != null)
                {
                    targetPos += playerRb.linearVelocity * predictionAmount;
                }
                Vector2 playerDirection = (targetPos - (Vector2)transform.position).normalized; // ← Renommé
                SpawnProjectile(playerDirection);
                break;
        }
    }

    void SpawnProjectile(Vector2 direction) // ← Renommé
    {
        GameObject bullet = Instantiate(projectileData.prefab, transform.position, Quaternion.identity, projectileRoot);
        ProjectileController projCtrl = bullet.GetComponent<ProjectileController>();
        projCtrl.Initialize(projectileData, direction, Vector2.zero, true);
    }
}