using UnityEngine;

public class LootController : MonoBehaviour
{
    private GameStats stats => GameManager.runtimeStats;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Debug.Log("Triggered");
            stats.Score++;
            Destroy(gameObject);
        }
    }
}
