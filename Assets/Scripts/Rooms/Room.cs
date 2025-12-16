using UnityEngine;

public class Room : MonoBehaviour
{
    public int mapIndex;
    public Transform cameraFocusPoint;
    public Transform center;

    [Header("Doors")]
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;
    [Header("Door Spawn Points")]
    public GameObject spawnDoorNorth;
    public GameObject spawnDoorSouth;
    public GameObject spawnDoorEast;
    public GameObject spawnDoorWest;

    private bool visited = false;
    private EnemySpawner enemySpawner;

    public void Awake()
    {
        enemySpawner = GetComponentInChildren<EnemySpawner>();
    }

    public void UpdateDoors()
    {
        FloorMap map = FloorMap.Instance;
        int width = 10;
        bool north = HasRoom(mapIndex - width);
        bool south = HasRoom(mapIndex + width);
        bool east = (mapIndex % width != width - 1) && HasRoom(mapIndex + 1);
        bool west = (mapIndex % width != 0) && HasRoom(mapIndex - 1);

        SetDoorActive(doorNorth, north);
        SetDoorActive(doorSouth, south);
        SetDoorActive(doorEast, east);
        SetDoorActive(doorWest, west);

        // if (!map.rooms[mapIndex].IsCleared())
        //     CloseAllDoors();
        // else
        //     OpenAllDoors();
    }

    private bool HasRoom(int index)
    {
        if (index < 0 || index >= FloorMap.Instance.rooms.Length)
            return false;

        return FloorMap.Instance.rooms[index].filled;
    }

    private void SetDoorActive(GameObject door, bool state)
    {
        if (door != null)
            door.SetActive(state);
    }

    public Transform GetSpawnPointFrom(DoorDirection entryDir)
    {
        return entryDir switch
        {
            DoorDirection.North => spawnDoorSouth.transform,
            DoorDirection.South => spawnDoorNorth.transform,
            DoorDirection.East => spawnDoorWest.transform,
            DoorDirection.West => spawnDoorEast.transform,
            _ => null
        };
    }

    public void Enter()
    {
        if (!visited)
        {
            visited = true;
            enemySpawner.Init();
        }

        foreach (var door in GetComponentsInChildren<DoorController>())
            door.Lock(1f);
    }
}
