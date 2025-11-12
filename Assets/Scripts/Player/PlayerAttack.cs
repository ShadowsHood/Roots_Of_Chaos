using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public ProjectileData projectileData;
    private PlayerController playerController;
    private Vector2 shootInput;
    private float lastFire;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void OnAttack(InputValue value)
    {
        shootInput = value.Get<Vector2>();
    }

    // void Update() {
    // }

    void FixedUpdate()
    {
        float fireRate = playerController.player.fireRate;
        if ((shootInput.x != 0 || shootInput.y != 0) && Time.time > lastFire + (1f/fireRate))
        {
            Shoot(shootInput);
            Debug.Log("Shoot!");
            lastFire = Time.time;
        }
    }

    void Shoot(Vector2 direction)
    {
        // instantiate projectile prefab depuis la data
        GameObject bullet = Instantiate(projectileData.prefab, transform.position, Quaternion.identity);
        ProjectileController projCtrl = bullet.GetComponent<ProjectileController>();
        // initialise le projectile avec data
        projCtrl.Initialize(projectileData, direction);
    }
}
