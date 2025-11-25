using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
            Spawn();
    }

    public void Spawn()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("playerSpawner : Aucun playerPrefab assigné dans l'inspector !");
            return;
        }

        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }
}
