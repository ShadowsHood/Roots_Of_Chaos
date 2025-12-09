using UnityEngine;

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
public struct RoomData
{
    public bool filled;
    public RoomType type;
    public bool visited;
    public int enemiesLeft;

    public RoomData(bool filled)
    {
        this.filled = filled;
        this.type = RoomType.Normal;
        this.visited = false;
        this.enemiesLeft = 0;
    }

    public bool IsCleared()
    {
        return enemiesLeft <= 0;
    }
}
