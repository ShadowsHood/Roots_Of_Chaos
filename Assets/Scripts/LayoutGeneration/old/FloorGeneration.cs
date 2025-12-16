using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloorGeneration : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject normalRoom;
    public GameObject startRoom;
    public GameObject bossRoom;
    public GameObject healRoom;
    public GameObject forgeRoom;
    public GameObject lightRoom;

    public float roomSpacing = 16f;

    private int[] floorPlan;
    private int floorPlanCount;

    private int minRooms = 7;
    private int maxRooms = 12;

    private int startIndex = 75;
    private int bossRoomIndex;
    private int healRoomIndex;
    private int forgeRoomIndex;
    private int lightRoomIndex;

    private List<int> endRooms;
    private Queue<int> cellQueue;

    private Transform roomsRoot;
    public GameObject player;
    private Room startRoomInstance;

    void Start()
    {
        SetupFloor();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            SetupFloor();
    }

    void SetupFloor()
    {
        // Create root if missing
        if (roomsRoot == null)
            roomsRoot = new GameObject("RoomsRoot").transform;

        // Clear existing rooms
        for (int i = roomsRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(roomsRoot.GetChild(i).gameObject);
        }

        // Reset
        floorPlan = new int[100];
        floorPlanCount = 0;
        cellQueue = new Queue<int>();
        endRooms = new List<int>();

        VisitCell(startIndex);
        GenerateFloor();
    }

    void GenerateFloor()
    {
        while (cellQueue.Count > 0 && floorPlanCount < maxRooms)
        {
            int index = cellQueue.Dequeue();
            int x = index % 10;

            bool created = false;

            if (x > 1) created |= VisitCell(index - 1);
            if (x < 9) created |= VisitCell(index + 1);
            if (index > 20) created |= VisitCell(index - 10);
            if (index < 70) created |= VisitCell(index + 10);

            if (!created)
                endRooms.Add(index);
        }

        if (floorPlanCount < minRooms)
        {
            SetupFloor();
            return;
        }

        AssignSpecialRooms();
        SpawnAllRooms();
    }

    void AssignSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;
        if (bossRoomIndex != -1)
            endRooms.RemoveAt(endRooms.Count - 1);

        lightRoomIndex = PickRandomEnd();
        forgeRoomIndex = PickRandomEnd();
        healRoomIndex = PickHealRoom();

        if (bossRoomIndex == -1 || forgeRoomIndex == -1 ||
            lightRoomIndex == -1 || healRoomIndex == -1)
        {
            SetupFloor();
        }
    }

    int PickRandomEnd()
    {
        if (endRooms.Count == 0) return -1;

        int id = Random.Range(0, endRooms.Count);
        int value = endRooms[id];
        endRooms.RemoveAt(id);
        return value;
    }

    int PickHealRoom()
    {
        for (int attempt = 0; attempt < 500; attempt++)
        {
            int index = Random.Range(0, 99);

            if (floorPlan[index] != 0) continue;

            if (IsAdjacent(index, bossRoomIndex)) continue;
            if (IsAdjacent(index, forgeRoomIndex)) continue;
            if (IsAdjacent(index, lightRoomIndex)) continue;

            return index;
        }

        return -1;
    }

    bool IsAdjacent(int a, int b)
    {
        if (b < 0) return false;

        return a == b + 1 || a == b - 1 || a == b + 10 || a == b - 10;
    }

    bool VisitCell(int index)
    {
        if (floorPlan[index] != 0)
            return false;

        if (GetNeighbourCount(index) > 1)
            return false;

        if (floorPlanCount >= maxRooms)
            return false;

        if (Random.value < 0.50f && index != startIndex)
            return false;

        floorPlan[index] = 1;
        floorPlanCount++;

        cellQueue.Enqueue(index);
        return true;
    }

    int GetNeighbourCount(int index)
    {
        int x = index % 10;
        int y = index / 10;

        int count = 0;

        if (x > 0) count += floorPlan[index - 1];
        if (x < 9) count += floorPlan[index + 1];
        if (y > 0) count += floorPlan[index - 10];
        if (y < 9) count += floorPlan[index + 10];

        return count;
    }

    void SpawnAllRooms()
    {
        for (int index = 0; index < floorPlan.Length; index++)
        {
            if (floorPlan[index] == 0 &&
                index != healRoomIndex) // Healroom is outside floorPlan
                continue;

            GameObject roomObj = SpawnRoom(index);
            Room room = roomObj.GetComponent<Room>();

            if (index == startIndex && room != null)
                startRoomInstance = room;
        }

        PositionPlayerAndCamera();
    }

    private GameObject SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 pos = new Vector2(x * roomSpacing, -y * roomSpacing);

        GameObject prefab = normalRoom;

        if (index == startIndex) prefab = startRoom;
        else if (index == bossRoomIndex) prefab = bossRoom;
        else if (index == healRoomIndex) prefab = healRoom;
        else if (index == forgeRoomIndex) prefab = forgeRoom;
        else if (index == lightRoomIndex) prefab = lightRoom;

        return Instantiate(prefab, pos, Quaternion.identity, roomsRoot);
    }

    void PositionPlayerAndCamera()
    {
        if (startRoomInstance == null) return;

        // Caméra
        if (startRoomInstance.cameraFocusPoint != null)
        {
            Camera.main.transform.position = new Vector2(
                startRoomInstance.cameraFocusPoint.position.x,
                startRoomInstance.cameraFocusPoint.position.y
            );
        }

        // Joueur
        // if (player != null && startRoomInstance.playerSpawnPoint != null)
        // {
        //     player.transform.position = startRoomInstance.playerSpawnPoint.position;
        // }
    }
}
