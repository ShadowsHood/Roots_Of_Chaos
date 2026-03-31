using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Prefabs (Tilemaps)")]
    public GameObject[] normalRoom;
    public GameObject[] startRoom;
    public GameObject[] bossRoom;
    public GameObject[] healRoom;
    public GameObject[] forgeRoom;
    public GameObject[] lightRoom;

    [Header("Positioning")]
    private float hSpacing = 15f * 1.5f;
    private float vSpacing = 9f * 1.5f;

    private Transform roomsRoot;
    private Room startRoomInstance;

    void Awake()
    {
        roomsRoot = new GameObject("RoomsRoot").transform;
    }

    public void SpawnRooms()
    {
        RoomManager.Instance.allRooms.Clear();

        // Reset
        foreach (Transform child in roomsRoot)
        {
            Destroy(child.gameObject);
        }

        for (int index = 0; index < FloorMap.Instance.rooms.Length; index++)
        {
            RoomData data = FloorMap.Instance.rooms[index];
            if (!data.filled) continue;

            SpawnRoom(index, data.type);
        }

        UpdateAllDoors();

        if (startRoomInstance != null)
        {
            CameraController.Instance.transform.position = startRoomInstance.cameraFocusPoint.position;
            RoomManager.Instance.MoveToRoom(startRoomInstance, startRoomInstance.mapIndex, null);
        }
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
            RoomData data = FloorMap.Instance.rooms[index];

            EnemySpawner eSpawner = instance.GetComponentInChildren<EnemySpawner>();
            if (eSpawner != null)
            {
                eSpawner.GenerateEnemies(data);
            }

            if (type == RoomType.Light || type == RoomType.Heal || type == RoomType.Forge)
            {
                ShopRoom sRoom = instance.GetComponent<ShopRoom>();
                if (sRoom != null) sRoom.GenerateShop();
            }
            if (type == RoomType.Start)
                startRoomInstance = roomInfo;
        }
    }

    private GameObject GetPrefab(RoomType type)
    {
        switch (type)
        {
            default:
            case RoomType.Normal: return GetRandomRoom(RoomType.Normal);
            case RoomType.Start: return GetRandomRoom(RoomType.Start);
            case RoomType.Boss: return GetRandomRoom(RoomType.Boss);
            case RoomType.Heal: return GetRandomRoom(RoomType.Heal);
            case RoomType.Forge: return GetRandomRoom(RoomType.Forge);
            case RoomType.Light: return GetRandomRoom(RoomType.Light);
        }
    }

    private GameObject GetRandomRoom(RoomType type)
    {
        switch (type)
        {
            case RoomType.Normal:
                if (normalRoom.Length == 0) return null;
                int index = Random.Range(0, normalRoom.Length);
                return normalRoom[index];
            case RoomType.Start:
                if (startRoom.Length == 0) return null;
                index = Random.Range(0, startRoom.Length);
                return startRoom[index];
            case RoomType.Boss:
                if (bossRoom.Length == 0) return null;
                index = Random.Range(0, bossRoom.Length);
                return bossRoom[index];
            case RoomType.Heal:
                if (healRoom.Length == 0) return null;
                index = Random.Range(0, healRoom.Length);
                return healRoom[index];
            case RoomType.Forge:
                if (forgeRoom.Length == 0) return null;
                index = Random.Range(0, forgeRoom.Length);
                return forgeRoom[index];
            case RoomType.Light:
                if (lightRoom.Length == 0) return null;
                index = Random.Range(0, lightRoom.Length);
                return lightRoom[index];
            default:
                return null;
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

    public void ClearAllRooms()
    {
        foreach (Transform child in roomsRoot)
        {
            Destroy(child.gameObject);
        }
    }

}
