using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloorGenerator : MonoBehaviour
{

    [Header("Special Rooms")]
    private int minRooms = 7;
    private int maxRooms = 15;

    private int startIndex = 75;
    private int bossRoomIndex;
    private int healRoomIndex;
    private int forgeRoomIndex;
    private int lightRoomIndex;

    private List<int> mainPath = new List<int>();
    private List<int> endRooms = new List<int>();

    private RoomSpawner roomSpawner;
    private MinimapController minimap;
    private RoomManager roomManager;


    void Awake()
    {
        roomSpawner = GetComponent<RoomSpawner>();
        minimap = GetComponent<MinimapController>();
        roomManager = GetComponent<RoomManager>();
    }

    // void Start()
    // {

    // }

    // void Update()
    // {
    //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //         SetupFloor();
    // }

    public void SetupFloor()
    {
        // Reset
        for (int i = 0; i < 100; i++)
            FloorMap.Instance.rooms[i] = new RoomData(false);

        mainPath.Clear();
        endRooms.Clear();

        GenerateFloor();

        AssignSpecialRooms();
        ApplyRoomTypes();

        roomSpawner.SpawnRooms();
        // minimap.UpdateMinimap();
        roomManager.currentRoomIndex = startIndex;
    }

    void GenerateFloor()
    {
        CreateMainPath(startIndex);
        CreateAllBranches();

        ParseEndRooms();

        if (endRooms.Count < 3 || CountFilledRooms() < minRooms)
        {
            // Debug.Log("Regenerating (not enough end rooms or rooms)...");
            SetupFloor();
        }
    }




    void CreateMainPath(int start)
    {
        int length = Random.Range(3, 7);

        int index = start;
        SetFilled(index);
        mainPath.Add(index);

        for (int i = 0; i < length; i++)
        {
            if (CountFilledRooms() > maxRooms) break;
            int up = index - 10;
            if (up < 0) break;

            index = up;
            SetFilled(index);
            mainPath.Add(index);
        }
    }

    void CreateAllBranches()
    {
        int total = mainPath.Count;

        for (int i = 0; i < total; i++)
        {
            int index = mainPath[i];

            float t = (float)i / (total - 1);
            float chance = Mathf.Lerp(0.05f, 0.70f, t);

            TrySpawnBranchesFrom(index, chance);
        }
    }



    void TrySpawnBranchesFrom(int index, float chance)
    {
        int x = index % 10;
        int y = index / 10;
        (int offset, bool allowed)[] neighbours = {
            (-1, x > 0),
            (+1, x < 9),
            (-10, y > 0),
            (+10, y < 9)
        };

        foreach (var n in neighbours)
        {
            if (CountFilledRooms() > maxRooms) break;

            if (!n.allowed) continue;
            int next = index + n.offset;
            if (!IsValid(next)) continue;
            if (IsFilled(next)) continue;

            if (Random.value < chance)
                CreateBranch(next);
        }
    }



    void CreateBranch(int start)
    {
        int length = Random.Range(2, 5);
        int index = start;

        SetFilled(index);

        for (int i = 0; i < length; i++)
        {
            if (CountFilledRooms() > maxRooms) break;
            int x = index % 10;
            int y = index / 10;
            (int offset, bool allowed)[] dirs = {
                (-1, x > 0),
                (+1, x < 9),
                (-10, y > 0),
                (+10, y < 9)
            };

            var possibleDirs = new List<int>();
            foreach (var d in dirs)
                if (d.allowed && !IsFilled(index + d.offset))
                    possibleDirs.Add(d.offset);
            if (possibleDirs.Count == 0) break;
            int dir = possibleDirs[Random.Range(0, possibleDirs.Count)];
            int next = index + dir;

            SetFilled(next);
            index = next;

            if (Random.value < 0.25f)
                TrySpawnBranchesFrom(index, 0.4f);
        }
    }

    void ParseEndRooms()
    {
        endRooms.Clear();

        for (int i = 0; i < 100; i++)
        {
            if (!IsFilled(i)) continue;
            if (i == startIndex) continue;

            int neighbours = GetNeighbourCount(i);
            if (neighbours == 1)
                endRooms.Add(i);
        }
    }

    void AssignSpecialRooms()
    {
        if (endRooms.Count == 0) return;

        bossRoomIndex = TakeRandomEnd();
        lightRoomIndex = TakeRandomEnd();
        forgeRoomIndex = TakeRandomEnd();
        healRoomIndex = PickHealRoom();

        if (bossRoomIndex < 0 || lightRoomIndex < 0 || forgeRoomIndex < 0 || healRoomIndex < 0)
        {
            Debug.Log("Special rooms missing, regenerating floor...");
            SetupFloor();
        }

        if (healRoomIndex >= 0)
            SetFilled(healRoomIndex);
    }

    int TakeRandomEnd()
    {
        if (endRooms.Count == 0) return -1;

        int id = Random.Range(0, endRooms.Count);
        int val = endRooms[id];
        endRooms.RemoveAt(id);
        return val;
    }

    int PickHealRoom()
    {
        List<int> possiblePos = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            if (IsFilled(i)) continue;
            if (GetNeighbourCount(i) != 1) continue;
            if (IsAdjacent(i, bossRoomIndex) || IsAdjacent(i, forgeRoomIndex) || IsAdjacent(i, lightRoomIndex))
                continue;

            int x = i % 10;
            int y = i / 10;
            int[] offsets = { -1, +1, -10, +10 };
            foreach (int offset in offsets)
            {
                int neighbor = i + offset;
                if (!IsValid(neighbor)) continue;
                if (!IsFilled(neighbor)) continue;

                RoomType type = FloorMap.Instance.rooms[neighbor].type;
                if (type == RoomType.Normal)
                {
                    if (IsAdjacent(neighbor, bossRoomIndex) || IsAdjacent(neighbor, forgeRoomIndex) || IsAdjacent(neighbor, lightRoomIndex))
                    {
                        possiblePos.Add(i);
                        break;
                    }
                }
            }
        }

        if (possiblePos.Count == 0) return -1;

        return possiblePos[Random.Range(0, possiblePos.Count)];
    }

    void ApplyRoomTypes()
    {
        FloorMap.Instance.rooms[startIndex].type = RoomType.Start;

        if (bossRoomIndex >= 0) FloorMap.Instance.rooms[bossRoomIndex].type = RoomType.Boss;
        if (healRoomIndex >= 0) FloorMap.Instance.rooms[healRoomIndex].type = RoomType.Heal;
        if (forgeRoomIndex >= 0) FloorMap.Instance.rooms[forgeRoomIndex].type = RoomType.Forge;
        if (lightRoomIndex >= 0) FloorMap.Instance.rooms[lightRoomIndex].type = RoomType.Light;

        for (int i = 0; i < 100; i++)
        {
            if (IsFilled(i) && FloorMap.Instance.rooms[i].type == RoomType.None)
                FloorMap.Instance.rooms[i].type = RoomType.Normal;
        }
    }




    bool IsValid(int index) => index >= 0 && index < 100;

    bool IsFilled(int index) => FloorMap.Instance.rooms[index].filled;

    void SetFilled(int index) => FloorMap.Instance.rooms[index].filled = true;

    int CountFilledRooms()
    {
        int result = 0;
        for (int i = 0; i < 100; i++)
            if (IsFilled(i)) result++;
        return result;
    }

    int GetNeighbourCount(int index)
    {
        int count = 0;

        int x = index % 10;
        int y = index / 10;

        if (x > 0 && IsFilled(index - 1)) count++;
        if (x < 9 && IsFilled(index + 1)) count++;
        if (y > 0 && IsFilled(index - 10)) count++;
        if (y < 9 && IsFilled(index + 10)) count++;

        return count;
    }

    bool IsAdjacent(int a, int b)
    {
        if (b < 0) return false;
        int ax = a % 10;
        int ay = a / 10;
        int bx = b % 10;
        int by = b / 10;

        return
            (ax == bx && Mathf.Abs(ay - by) == 1) ||
            (ay == by && Mathf.Abs(ax - bx) == 1);
    }
}
