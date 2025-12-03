using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{

    private int[] floorPlan;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    private List<int> endRooms;

    private int startIndex;
    private int bossRoomIndex;
    private int healRoomIndex;
    private int forgeRoomIndex;
    private int itemRoomIndex;

    private List<int> bigRoomIndexes;
    public Cell cellPrefab;
    private float cellSize;
    private Queue<int> cellQueue;
    private List<Cell> spawnedCells;

    [Header("Sprite references")]
    [SerializeField] private Sprite start;
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite forge;
    [SerializeField] private Sprite heal;
    [SerializeField] private Sprite boss;

    [Header("Room Variations")]
    [SerializeField] private Sprite largeRoom;
    [SerializeField] private Sprite LRoom;
    [SerializeField] private Sprite verticalRoom;
    [SerializeField] private Sprite horizontalRoom;

    private static readonly List<int[]> roomShapes = new()
    {
        new int[]{-1 },
        new int[]{1 },

        new int[]{10 },
        new int[]{-10 },

        new int[]{1,10},
        new int[]{1,11},
        new int[]{10,11},

        new int[]{9,10},
        new int[]{-1,9},
        new int[]{-1,10},

        new int[]{1,-10},
        new int[]{1,-9},
        new int[]{-9,-10},

        new int[]{-1,-10},
        new int[]{-1,-11},
        new int[]{-10,-11},

        new int[]{1,10,11},
        new int[]{1,-9,-10},
        new int[]{-1,9,10},
        new int[]{-1,-10,-11}
    };

    void Start()
    {
        minRooms = 7;
        maxRooms = 15;
        cellSize = 0.5f;
        spawnedCells = new List<Cell>();
        startIndex = 75;

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
        bigRoomIndexes = new List<int>();

        VisitCell(startIndex);
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

        CleanEndRoomsList();

        SetupSpecialRooms();

        Debug.Log($"Big room indexes : {string.Join(", ", bigRoomIndexes)}");
        // UpdateSpecialRoomVisuals();
    }

    void CleanEndRoomsList()
    {
        endRooms.RemoveAll(item => bigRoomIndexes.Contains(item) || GetNeighbourCount(item) > 1);
    }

    void SetupSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;
        if (bossRoomIndex != -1)
        {
            endRooms.RemoveAt(endRooms.Count - 1);
        }

        itemRoomIndex = RandomEndRoom();
        forgeRoomIndex = RandomEndRoom();
        healRoomIndex = PickHealRoom();

        if (itemRoomIndex == -1 || forgeRoomIndex == -1 || healRoomIndex == -1 || bossRoomIndex == -1)
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
                var index when index == startIndex => start,
                var index when index == bossRoomIndex => boss,
                var index when index == itemRoomIndex => item,
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
        int[] specialRooms = new int[] { bossRoomIndex, itemRoomIndex, forgeRoomIndex };

        for (int index = 0; index < floorPlan.Length; index++)
        {
            if (floorPlan[index] != 0)
                continue;

            List<int> neighbours = GetNeighboursIndex(index);
            if (neighbours.Count != 1)
                continue;

            int neighbour = neighbours[0];
            if (specialRooms.Contains(neighbour))
                continue;

            List<int> secondNeighbours = GetNeighboursIndex(neighbour);

            bool touchesSpecial = secondNeighbours.Exists(n =>
                specialRooms.Contains(n)
            );

            if (touchesSpecial)
                return index;
        }
        return -1;
    }

    List<int> GetNeighboursIndex(int index)
    {
        List<int> result = new List<int>();

        int x = index % 10;
        int y = index / 10;

        void AddIfValid(int idx)
        {
            if (idx >= 0 && idx < floorPlan.Length && floorPlan[idx] == 1)
                result.Add(idx);
        }

        if (x > 0) AddIfValid(index - 1);
        if (x < 9) AddIfValid(index + 1);
        if (y > 0) AddIfValid(index - 10);
        if (y < 9) AddIfValid(index + 10);

        return result;
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
        int x = index % 10;
        int y = index / 10;
        float verticalProb = 1f - (y / 9f);
        float centerProb = 1f - (Mathf.Abs(x - 5) / 5f);
        float chance = verticalProb * centerProb;

        if (floorPlan[index] != 0 ||
            GetNeighbourCount(index) > 1 ||
            floorPlanCount > maxRooms ||
            Random.value > chance)
            return false;

        if (Random.value < 0.3f && index != startIndex)
        {
            foreach (var shape in roomShapes.OrderBy(_ => Random.value))
            {
                if (TryPlaceRoom(index, shape))
                    return true;
            }
        }

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

    private bool TryPlaceRoom(int originIndex, int[] offsets)
    {
        List<int> currentRoomIndexes = new List<int>() { originIndex };
        foreach (int offset in offsets)
        {
            int checkedIndex = originIndex + offset;
            if (checkedIndex - 10 < 0 || checkedIndex + 10 >= floorPlan.Length || floorPlan[checkedIndex] != 0)
            {
                return false;
            }
            if (checkedIndex == originIndex) continue;
            if (checkedIndex % 10 == 0) continue;

            currentRoomIndexes.Add(checkedIndex);
        }

        if (currentRoomIndexes.Count == 1) return false;

        foreach (int index in currentRoomIndexes)
        {
            floorPlan[index] = 1;
            floorPlanCount++;
            // cellQueue.Enqueue(index);
            bigRoomIndexes.Add(index);
        }

        SpawnLargeRoom(currentRoomIndexes);

        return true;
    }

    private void SpawnLargeRoom(List<int> largeRoomIndexes)
    {
        Cell newCell = null;

        int combinedX = default;
        int combinedY = default;
        float offset = cellSize / 2f;
        // int minX = int.MaxValue;
        // int maxX = int.MinValue;
        // int minY = int.MaxValue;
        // int maxY = int.MinValue;

        for (int i = 0; i < largeRoomIndexes.Count; i++)
        {
            int index = largeRoomIndexes[i];
            int x = index % 10;
            int y = index / 10;

            combinedX += x;
            combinedY += y;
            // minX = Mathf.Min(minX, x);
            // maxX = Mathf.Max(maxX, x);
            // minY = Mathf.Min(minY, y);
            // maxY = Mathf.Max(maxY, y);
        }

        // float centerX = ((minX + maxX + 1) / 2f) * cellSize;
        // float centerY = -((minY + maxY + 1) / 2f) * cellSize;

        // Cell cell = Instantiate(cellPrefab, new Vector2(centerX, centerY), Quaternion.identity);

        // if (largeRoomIndexes.Count == 4)
        //     cell.SetRoomSprite(largeRoom);
        // else if (largeRoomIndexes.Count == 3)
        // {
        //     cell.SetRoomSprite(LRoom);
        //     cell.RotateCell(largeRoomIndexes);
        // }
        // else if (largeRoomIndexes.Count == 2)
        // {
        //     bool vertical = Mathf.Abs((largeRoomIndexes[0] / 10) - (largeRoomIndexes[1] / 10)) == 1;
        //     cell.SetRoomSprite(vertical ? verticalRoom : horizontalRoom);
        // }

        // spawnedCells.Add(cell);

        if (largeRoomIndexes.Count == 4)
        {
            Vector2 position = new Vector2((combinedX / 4f) * cellSize + offset, -(combinedY / 4f) * cellSize - offset);

            newCell = Instantiate(cellPrefab, position, Quaternion.identity);
            newCell.SetRoomSprite(largeRoom);
        }
        else if (largeRoomIndexes.Count == 3)
        {
            Vector2 position = new Vector2((combinedX / 3f) * cellSize, -(combinedY / 3f) * cellSize);
            newCell = Instantiate(cellPrefab, position, Quaternion.identity);
            newCell.SetRoomSprite(LRoom);
            newCell.RotateCell(largeRoomIndexes);
        }
        else if (largeRoomIndexes.Count == 2)
        {
            if (largeRoomIndexes[0] + 10 == largeRoomIndexes[1] || largeRoomIndexes[0] - 10 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2((combinedX / 2f) * cellSize + offset, -(combinedY / 2f) * cellSize - offset);
                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.SetRoomSprite(verticalRoom);
            }
            else if (largeRoomIndexes[0] + 1 == largeRoomIndexes[1] || largeRoomIndexes[0] - 1 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2((combinedX / 2f) * cellSize + offset, -(combinedY / 2f) * cellSize - offset);
                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.SetRoomSprite(horizontalRoom);
            }
        }

        spawnedCells.Add(newCell);
    }
}
