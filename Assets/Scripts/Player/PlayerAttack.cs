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

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rbody = GetComponent<Rigidbody2D>();
    }

    void OnAttack(InputValue value)
    {
        shootInput = value.Get<Vector2>();
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
