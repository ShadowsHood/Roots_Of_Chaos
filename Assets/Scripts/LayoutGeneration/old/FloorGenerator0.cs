using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class FloorGenerator0 : MonoBehaviour
{
    private int minRooms = 7;
    private int maxRooms = 15;

    private int startIndex = 75;
    private int bossRoomIndex;
    private int healRoomIndex;
    private int forgeRoomIndex;
    private int lightRoomIndex;

    private List<int> endRooms;
    private Queue<int> cellQueue;

    private RoomSpawner roomSpawner;

    void Awake()
    {
        roomSpawner = GetComponent<RoomSpawner>();
    }

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
        // Reset
        for (int i = 0; i < 100; i++)
            FloorMap.Instance.rooms[i] = new RoomData(false);

        endRooms = new List<int>();
        cellQueue = new Queue<int>();

        VisitCell(startIndex);
        GenerateFloor();

        roomSpawner.SpawnRooms();
    }

    void GenerateFloor()
    {
        int count = 1;

        while (cellQueue.Count > 0 && count < maxRooms)
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

            count++;
        }

        if (count < minRooms)
        {
            SetupFloor();
            return;
        }

        AssignSpecialRooms();
        ApplyRoomTypes();
    }

    void AssignSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;
        if (bossRoomIndex != -1)
            endRooms.RemoveAt(endRooms.Count - 1);

        lightRoomIndex = PickRandomEnd();
        forgeRoomIndex = PickRandomEnd();
        healRoomIndex = PickHealRoom();
    }

    int PickRandomEnd()
    {
        if (endRooms.Count == 0) return -1;
        int id = Random.Range(0, endRooms.Count);
        int val = endRooms[id];
        endRooms.RemoveAt(id);
        return val;
    }

    int PickHealRoom()
    {
        for (int attempt = 0; attempt < 200; attempt++)
        {
            int index = Random.Range(0, 99);
            if (!FloorMap.Instance.rooms[index].filled) continue;
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
        if (FloorMap.Instance.rooms[index].filled)
            return false;
        if (GetNeighbourCount(index) > 1)
            return false;

        int y = index / 10;
        float branchProbability = Mathf.Lerp(0.2f, 0.8f, 1f - (y / 9f));
        if (index != startIndex && Random.value > branchProbability)
            return false;

        FloorMap.Instance.rooms[index] = new RoomData(true);

        cellQueue.Enqueue(index);
        return true;
    }

    int GetNeighbourCount(int index)
    {
        int x = index % 10;
        int y = index / 10;
        int count = 0;

        if (x > 0 && FloorMap.Instance.rooms[index - 1].filled) count++;
        if (x < 9 && FloorMap.Instance.rooms[index + 1].filled) count++;
        if (y > 0 && FloorMap.Instance.rooms[index - 10].filled) count++;
        if (y < 9 && FloorMap.Instance.rooms[index + 10].filled) count++;
        return count;
    }

    void ApplyRoomTypes()
    {
        FloorMap.Instance.rooms[startIndex].type = RoomType.Start;
        FloorMap.Instance.rooms[bossRoomIndex].type = RoomType.Boss;
        FloorMap.Instance.rooms[healRoomIndex].type = RoomType.Heal;
        FloorMap.Instance.rooms[forgeRoomIndex].type = RoomType.Forge;
        FloorMap.Instance.rooms[lightRoomIndex].type = RoomType.Light;

        for (int i = 0; i < 100; i++)
        {
            if (FloorMap.Instance.rooms[i].filled && FloorMap.Instance.rooms[i].type == RoomType.None)
                FloorMap.Instance.rooms[i].type = RoomType.Normal;
        }
    }
}
