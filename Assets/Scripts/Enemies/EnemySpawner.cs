using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    // public EnemyData[] enemyTypes; //TODO
    public float activationDelay = 1.0f;
    public Transform[] spawnPoints;

    [Header("Number of enemies to spawn")]
    public int spawnMin = 3;
    public int spawnMax = 6;

    // private Tilemap tilemap;
    private Room room;

    private void Awake()
    {
        // tilemap = GetComponent<Tilemap>();
        room = GetComponentInParent<Room>();
    }

    public void GenerateEnemies(RoomData data)
    {
        data.savedEnemies.Clear();
        if (spawnPoints.Length == 0) return;

        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        ShuffleList(availablePoints);

        int spawnCount = Random.Range(spawnMin, Mathf.Min(spawnMax, availablePoints.Count) + 1);
        data.activeEnemies = spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            data.savedEnemies.Add(new SpawnPointData
            {
                enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                position = availablePoints[i].position
            });
        }
    }

    public void Spawn(RoomData data)
    {
        ClearEnemies();
        foreach (var spawnData in data.savedEnemies)
        {
            GameObject enemy = Instantiate(spawnData.enemyPrefab, spawnData.position, Quaternion.identity, transform);
            // GameObject enemy = Instantiate(spawnData.enemyType.prefab, spawnData.position, Quaternion.identity, transform);//TODO
            StartCoroutine(ActivateEnemyAfterDelay(enemy));
        }
        // Debug.Log("Active Enemies: " + data.activeEnemies);
    }

    public void ClearEnemies()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
    }

    private IEnumerator ActivateEnemyAfterDelay(GameObject enemy)
    {
        if (enemy == null) yield break;

        // Get enemy controller OR boss controller
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        BossController bossController = enemy.GetComponent<BossController>();

        if (enemyController != null) enemyController.isActive = false;
        if (bossController != null) bossController.isActive = false;

        var enemyCollider = enemy.GetComponent<Collider2D>();
        if (enemyCollider != null) enemyCollider.enabled = false;
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);

        yield return new WaitForSeconds(activationDelay);

        if (enemyController != null) enemyController.isActive = true;
        if (bossController != null) bossController.isActive = true;

        if (enemyCollider != null) enemyCollider.enabled = true;
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }



    [System.Serializable]
    public class SpawnPointData
    {
        public GameObject enemyPrefab;
        public Vector3 position; // Utilise Vector3, pas Transform !
    }
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
