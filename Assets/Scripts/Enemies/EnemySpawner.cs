using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
            // SpawnEnemy();
            SpawnAll();
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner : Aucun enemyPrefab assigné dans l'inspector !");
            return;
        }

        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    public void SpawnAll()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemyGroupSpawner : Aucun enemyPrefab assigné !");
            return;
        }

        // Pour chaque enfant → spawn un ennemi
        foreach (Transform spawnPoint in transform)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
