using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Prefabs (Tilemaps)")]
    public GameObject normalRoom;
    public GameObject startRoom;
    public GameObject bossRoom;
    public GameObject healRoom;
    public GameObject forgeRoom;
    public GameObject lightRoom;

    [Header("Positioning")]
    private float hSpacing = 15f * 1.5f;
    private float vSpacing = 9f * 1.5f;

    private Transform roomsRoot;
    public GameObject player;
    private Room startRoomInstance;

    void Awake()
    {
        roomsRoot = new GameObject("RoomsRoot").transform;
    }

    public void SpawnRooms()
    {
        // Reset
        for (int i = roomsRoot.childCount - 1; i >= 0; i--)
            Destroy(roomsRoot.GetChild(i).gameObject);

        for (int index = 0; index < FloorMap.Instance.rooms.Length; index++)
        {
            RoomData data = FloorMap.Instance.rooms[index];
            if (!data.filled) continue;

            SpawnRoom(index, data.type);
        }

        UpdateAllDoors();
        CameraController.Instance.transform.position = startRoomInstance.cameraFocusPoint.position;
        RoomManager.Instance.MoveToRoom(startRoomInstance, startRoomInstance.mapIndex, null);
    }

    private void SpawnRoom(int index, RoomType type)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 pos = new Vector2(x * hSpacing, -y * vSpacing);
        GameObject prefab = GetPrefab(type);

        if (prefab == null)
        {
            Debug.LogWarning("No prefab for type: " + type);
            return;
        }

        GameObject instance = Instantiate(prefab, pos, Quaternion.identity, roomsRoot);
        Room roomInfo = instance.GetComponent<Room>();
        if (roomInfo != null)
        {
            roomInfo.mapIndex = index;
            RoomManager.Instance.AddRoom(roomInfo);
            if (type == RoomType.Start)
                startRoomInstance = roomInfo;
        }
    }

    private GameObject GetPrefab(RoomType type)
    {
        switch (type)
        {
            default:
            case RoomType.Normal: return normalRoom;
            case RoomType.Start: return startRoom;
            case RoomType.Boss: return bossRoom;
            case RoomType.Heal: return healRoom;
            case RoomType.Forge: return forgeRoom;
            case RoomType.Light: return lightRoom;
        }
    }

    void UpdateAllDoors()
    {
        Room[] all = roomsRoot.GetComponentsInChildren<Room>();
        foreach (var room in all)
        {
            room.UpdateDoors();
        }
    }

}
