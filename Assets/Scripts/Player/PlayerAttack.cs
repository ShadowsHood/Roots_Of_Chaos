using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public ProjectileData projectileData;
    public Transform projectileRoot;
    private PlayerController playerController;
    private Rigidbody2D rbody;
    private Vector2 shootInput;
    private float lastFire;
    private Vector2 lastShootDirection;
    private Animator headAnimator;
    private SpriteRenderer headSr;

    void Awake()
    {
        headAnimator = transform.Find("Head").GetComponent<Animator>();
        headSr = transform.Find("Head").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rbody = GetComponent<Rigidbody2D>();
    }

    void OnAttack(InputValue value)
    {
        shootInput = value.Get<Vector2>();
        // Animation
        bool isShooting = shootInput.magnitude > 0.01f;
        headAnimator.SetBool("isShooting", isShooting);

        if (isShooting)
        {
            lastShootDirection = shootInput.normalized;
            headAnimator.SetFloat("shootX", shootInput.x);
            headAnimator.SetFloat("shootY", shootInput.y);

            if (lastShootDirection.x != 0)
            {
                headSr.flipX = lastShootDirection.x < 0;
            }
        }

    }

    // void Update() {
    // }

    void FixedUpdate()
    {
        if ((shootInput.x != 0 || shootInput.y != 0) && Time.time > lastFire + (1f / GameManager.runStats.fireRate) && playerController.invincible == false)
        {
            Shoot(shootInput);
            // Debug.Log("Shoot!");
            lastFire = Time.time;
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(projectileData.prefab, transform.position, Quaternion.identity, projectileRoot);
        ProjectileController projCtrl = bullet.GetComponent<ProjectileController>();
        Vector2 velocity = rbody.linearVelocity;
        projCtrl.Initialize(projectileData, direction, velocity);
    }
}
