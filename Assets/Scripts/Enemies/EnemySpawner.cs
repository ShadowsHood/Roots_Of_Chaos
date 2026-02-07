using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    // public EnemyData[] enemyTypes; //TODO
    public float activationDelay = 1.0f;

    [Header("Number of enemies to spawn")]
    public int spawnMin = 3;
    public int spawnMax = 6;

    private Tilemap tilemap;
    private Room room;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        room = GetComponentInParent<Room>();
    }

    public void GenerateEnemies(RoomData data)
    {
        data.savedEnemies.Clear();
        List<Vector3> spawnPositions = new List<Vector3>();
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
                spawnPositions.Add(tilemap.GetCellCenterWorld(pos));
        }

        if (spawnPositions.Count == 0) return;

        int spawnCount = Random.Range(spawnMin, Mathf.Min(spawnMax, spawnPositions.Count) + 1);
        ShuffleList(spawnPositions);
        data.activeEnemies = spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            data.savedEnemies.Add(new SpawnPointData
            {
                enemyType = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                position = spawnPositions[i]
            });
        }
        tilemap.ClearAllTiles();
    }

    public void Spawn(RoomData data)
    {
        ClearEnemies();
        foreach (var spawnData in data.savedEnemies)
        {
            GameObject enemy = Instantiate(spawnData.enemyType, spawnData.position, Quaternion.identity, transform);
            // GameObject enemy = Instantiate(spawnData.enemyType.prefab, spawnData.position, Quaternion.identity, transform);//TODO
            StartCoroutine(ActivateEnemyAfterDelay(enemy));
        }
        // Debug.Log("Active Enemies: " + data.activeEnemies);
    }

    public void ClearEnemies()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator ActivateEnemyAfterDelay(GameObject enemy)
    {
        var controller = enemy.GetComponent<EnemyController>();
        // if (controller != null) controller = enemy.GetComponent<BossController>();

        if (controller != null) controller.isActive = false;
        var collider = enemy.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);

        yield return new WaitForSeconds(activationDelay);

        if (enemy != null && controller != null) controller.isActive = true;
        if (enemy != null && collider != null) collider.enabled = true;
        if (enemy != null && sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }



    [System.Serializable]
    public class SpawnPointData
    {
        public GameObject enemyType;
        // public EnemyData enemyType; //TODO
        public Vector3 position;
    }
    void ShuffleList(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
