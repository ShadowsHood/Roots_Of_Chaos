using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    [Header("Number of enemies to spawn")]
    public int spawnMin = 3;
    public int spawnMax = 6;

    public bool spawnOnStart = true;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        if (spawnOnStart)
            SpawnAll();
    }

    public void SpawnAll()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: Aucun enemyPrefab assigné !");
            return;
        }

        List<Vector3> spawnPositions = new List<Vector3>();
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                spawnPositions.Add(worldPos);
            }
        }

        if (spawnPositions.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: aucune tile trouvée pour le spawn.");
            return;
        }

        int spawnCount = Random.Range(spawnMin, spawnMax + 1);
        spawnCount = Mathf.Min(spawnCount, spawnPositions.Count);
        ShuffleList(spawnPositions);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, spawnPositions[i], Quaternion.identity, transform);
        }

        tilemap.ClearAllTiles();
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
