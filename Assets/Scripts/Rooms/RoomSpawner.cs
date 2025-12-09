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
    public float roomSpacing = 16f;

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

        PositionPlayerAndCamera();
    }

    private void SpawnRoom(int index, RoomType type)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 pos = new Vector2(x * roomSpacing, -y * roomSpacing);
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

    void PositionPlayerAndCamera()
    {
        if (startRoomInstance == null) return;

        // Caméra
        if (startRoomInstance.cameraFocusPoint != null)
        {
            Camera.main.transform.position = new Vector3(
                startRoomInstance.cameraFocusPoint.position.x,
                startRoomInstance.cameraFocusPoint.position.y,
                -10f
            );
        }

        // Joueur
        if (player != null && startRoomInstance.playerSpawnPoint != null)
        {
            Instantiate(player, startRoomInstance.playerSpawnPoint.position, Quaternion.identity);
            // player.transform.position = startRoomInstance.playerSpawnPoint.position;
        }
    }

}
