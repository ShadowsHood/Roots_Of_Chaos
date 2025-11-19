using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{

    private int[] floorPlan;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<int> endRooms;

    private int bossRoomIndex;
    private int healRoomIndex;
    private int forgeRoomIndex;
    private int lightRoomIndex;

    public Cell cellPrefab;
    private float cellSize;
    private Queue<int> cellQueue;
    private List<Cell> spawnedCells;

    [Header("Sprite references")]
    [SerializeField] private Sprite light;
    [SerializeField] private Sprite forge;
    [SerializeField] private Sprite heal;
    [SerializeField] private Sprite boss;
    void Start()
    {
        minRooms = 7;
        maxRooms = 12;
        cellSize = 0.5f;
        spawnedCells = new List<Cell>();

        SetupFloor();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SetupFloor();
        }
    }

    void SetupFloor()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            Destroy(spawnedCells[i].gameObject);
        }
        spawnedCells.Clear();
        floorPlan = new int[100];
        floorPlanCount = default;
        cellQueue = new Queue<int>();
        endRooms = new List<int>();

        VisitCell(75);
        GenerateFloor();
    }

    void GenerateFloor()
    {
        while (cellQueue.Count > 0 && floorPlanCount < maxRooms)
        {
            int currentIndex = cellQueue.Dequeue();
            int x = currentIndex % 10;
            bool created = false;

            if (x > 1) created |= VisitCell(currentIndex - 1);
            if (x < 9) created |= VisitCell(currentIndex + 1);
            if (currentIndex > 20) created |= VisitCell(currentIndex - 10);
            if (currentIndex < 70) created |= VisitCell(currentIndex + 10);

            if (created == false)
                endRooms.Add(currentIndex);
        }

        if (floorPlanCount < minRooms)
        {
            SetupFloor();
            return;
        }

        SetupSpecialRooms();
        // UpdateSpecialRoomVisuals();
    }

    void SetupSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;
        if (bossRoomIndex != -1)
        {
            endRooms.RemoveAt(endRooms.Count - 1);
        }

        lightRoomIndex = RandomEndRoom();
        forgeRoomIndex = RandomEndRoom();
        healRoomIndex = PickHealRoom();

        if (lightRoomIndex == -1 || forgeRoomIndex == -1 || healRoomIndex == -1 || bossRoomIndex == -1)
        {
            SetupFloor();
            return;
        }

        SpawnRoom(healRoomIndex);
        UpdateSpecialRoomVisuals();
    }

    void UpdateSpecialRoomVisuals()
    {
        foreach (var cell in spawnedCells)
        {
            var sprite = cell.index switch
            {
                var index when index == bossRoomIndex => boss,
                var index when index == lightRoomIndex => light,
                var index when index == forgeRoomIndex => forge,
                var index when index == healRoomIndex => heal,
                _ => null
            };

            if (sprite != null)
                cell.SetSpecialRoomSprite(sprite);
        }
    }

    int RandomEndRoom()
    {
        if (endRooms.Count == 0) return -1;

        int randomRoom = Random.Range(0, endRooms.Count);
        int index = endRooms[randomRoom];
        endRooms.RemoveAt(randomRoom);
        return index;
    }

    int PickHealRoom()
    {
        for (int attempt = 0; attempt < 900; attempt++)
        {
            int x = Mathf.FloorToInt(Random.Range(0f, 1f) * 9) + 1;
            int y = Mathf.FloorToInt(Random.Range(0f, 1f) * 8) + 2;

            int index = y * 10 + x;

            if (floorPlan[index] != 0)
                continue;
            if (bossRoomIndex == index - 1 || bossRoomIndex == index + 1 ||
               bossRoomIndex == index - 10 || bossRoomIndex == index + 10)
                continue;
            if (index - 1 < 0 || index + 1 > floorPlan.Length || index - 10 < 0 || index + 10 > floorPlan.Length)
                continue;
            int neighbours = GetNeighbourCount(index);
            if (neighbours >= 3 || (attempt > 300 && neighbours >= 2) || (attempt > 600 && neighbours >= 1))
                return index;
        }
        return -1;
    }

    private int GetNeighbourCount(int index)
    {
        return floorPlan[index - 1] +
               floorPlan[index + 1] +
               floorPlan[index - 10] +
               floorPlan[index + 10];
    }

    private bool VisitCell(int index)
    {
        if (floorPlan[index] != 0 ||
            GetNeighbourCount(index) > 1 ||
            floorPlanCount > maxRooms ||
            Random.value < 0.5f
        )
            return false;

        cellQueue.Enqueue(index);
        floorPlan[index] = 1;
        floorPlanCount++;
        SpawnRoom(index);
        return true;
    }

    private void SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;
        Vector2 pos = new Vector2(x * cellSize, -y * cellSize);

        Cell newCell = Instantiate(cellPrefab, pos, Quaternion.identity);
        newCell.index = index;
        newCell.value = 1;

        spawnedCells.Add(newCell);
    }
}
