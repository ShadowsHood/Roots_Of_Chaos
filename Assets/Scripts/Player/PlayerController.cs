using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private GameStats stats => GameManager.runtimeStats;
    public PlayerData player;
    private Rigidbody2D rbody;
    private Vector2 moveInput;
    private HitFeedback hitFeedback;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.freezeRotation = true;

        hitFeedback = GetComponent<HitFeedback>();
    }

    void Start()
    {
        stats.MaxHealth = player.baseHealth;
        stats.Health = stats.MaxHealth;
        stats.MoveSpeed = player.moveSpeed;
        stats.FireRate = player.fireRate;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // void Update() {
    // }

    void FixedUpdate()
    {
        // Debug.Log("Speed : " + player.moveSpeed);
        rbody.linearVelocity = moveInput * stats.MoveSpeed;
    }

    // void LateUpdate()
    // {
    // }

    public void TakeDamage(int dmg)
    {
        stats.Health -= dmg;
        hitFeedback.PlayHitEffect();
        if (stats.Health <= 0)
        {
            Die();
        }
    }
    public void Heal(int healAmount)
    {
        stats.Health = Mathf.Min(stats.MaxHealth, stats.Health + healAmount);
    }

    void Die()
    {
        Debug.Log("Game Over");
        Destroy(gameObject);
    }
}
