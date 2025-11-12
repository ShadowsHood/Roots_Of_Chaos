using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Wander,
    Follow,
    // Scared
}

public class EnemyController : MonoBehaviour
{
    public EnemyData enemy;
    public EnemyState currentState = EnemyState.Wander;
    private bool isDead = false;
    private float currentSpeed;

    private bool chooseDir = false;
    private Vector3 randomDir;
    private GameObject player;
    private float lastHit = 0f;
    public float hitCooldown = 0.3f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentSpeed = enemy.moveSpeed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > lastHit + hitCooldown)
        {
            Debug.Log("Hit");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(enemy.damage);
            lastHit = Time.time;
        }
    }

    void Update()
    {
        if (isDead) return;

        currentState = IsPlayerInRange(enemy.detectionRange) ? EnemyState.Follow : EnemyState.Wander;
        currentSpeed = currentState == EnemyState.Follow ? enemy.moveSpeed : enemy.moveSpeed * 0.7f;

        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private IEnumerator ChooseDirection()
    {
        chooseDir = true;

        // nouvelle direction aléatoire en 2D
        float angle = Random.Range(0f, 360f);
        randomDir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

        yield return new WaitForSeconds(Random.Range(2f, 5f));
        chooseDir = false;
    }

    void Wander()
    {
        if (!chooseDir)
            StartCoroutine(ChooseDirection());

        transform.position += randomDir.normalized * currentSpeed * Time.deltaTime;

        // Limites de l'écran
        Vector3 pos = transform.position;
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Ajuste la position pour rester dans l'écran
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        transform.position = pos;
    }

    void Follow()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.transform.position,
            currentSpeed * Time.deltaTime
        );

        // Limites de l'écran
        Vector3 pos = transform.position;
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Ajuste la position pour rester dans l'écran
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        transform.position = pos;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Destroy(gameObject);
    }
}
