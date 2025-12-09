using UnityEngine;

public class FloorMap : MonoBehaviour
{
    public static FloorMap Instance;

    public RoomData[] rooms = new RoomData[100];

    void Awake()
    {
        Instance = this;
    }

    public RoomData GetRoom(int index)
    {
        return rooms[index];
    }

    public void SetRoom(int index, RoomData data)
    {
        rooms[index] = data;
    }
}
