using UnityEngine;
using System.Collections.Generic;

public enum RoomType
{
    None,
    Start,
    Normal,
    Boss,
    Heal,
    Forge,
    Light
}

[System.Serializable]
public class RoomData
{
    public bool filled;
    public RoomType type;
    public bool visited;
    public List<EnemySpawner.SpawnPointData> savedEnemies;

    public RoomData(bool filled)
    {
        this.filled = filled;
        this.type = RoomType.Normal;
        this.visited = false;
        this.savedEnemies = new List<EnemySpawner.SpawnPointData>();
    }

    public bool IsCleared() => visited && savedEnemies.Count == 0;
}
