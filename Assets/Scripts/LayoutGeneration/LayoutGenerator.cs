using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    private int minRooms = 7;
    private int maxRooms = 15;

    private int[] floorPlan;
    private int floorPlanCount;

    private List<int> endRooms;
    private List<int> bigRoomIndexes;
    private Queue<int> cellQueue;

    private int startIndex;
    private int itemRoomIndex;
    private int forgeRoomIndex;
    private int healRoomIndex;
    private int bossRoomIndex;

    // private static readonly List<int[]> roomShapes = new()
    // {
    //     new int[]{-1 }, new int[]{1 }, new int[]{10 }, new int[]{-10 },
    //     new int[]{1,10}, new int[]{1,11}, new int[]{10,11},
    //     new int[]{9,10}, new int[]{-1,9}, new int[]{-1,10},
    //     new int[]{1,-10}, new int[]{1,-9}, new int[]{-9,-10},
    //     new int[]{-1,-10}, new int[]{-1,-11}, new int[]{-10,-11},
    //     new int[]{1,10,11}, new int[]{1,-9,-10}, new int[]{-1,9,10}, new int[]{-1,-10,-11}
    // };

    // ➤ C’est cette méthode que MapDisplay va appeler
    public LayoutData Generate(int startIndex)
    {
        this.startIndex = startIndex;

        bool valid = false;

        while (!valid)
        {
            ResetLayout();
            VisitCell(startIndex);
            GenerateFloor();

            valid =
                floorPlanCount >= minRooms &&
                bossRoomIndex != -1 &&
                itemRoomIndex != -1 &&
                forgeRoomIndex != -1 &&
                healRoomIndex != -1;
        }

        return new LayoutData
        {
            floorPlan = floorPlan,
            bigRoomIndexes = bigRoomIndexes,
            startIndex = startIndex,
            bossRoom = bossRoomIndex,
            itemRoom = itemRoomIndex,
            forgeRoom = forgeRoomIndex,
            healRoom = healRoomIndex
        };
    }

    private void ResetLayout()
    {
        floorPlan = new int[100];
        floorPlanCount = 0;

        endRooms = new();
        bigRoomIndexes = new();
        cellQueue = new();

        bossRoomIndex = itemRoomIndex = forgeRoomIndex = healRoomIndex = -1;
    }

    private void GenerateFloor()
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

        CleanEndRoomsList();
        SetupSpecialRooms();
    }

    private void CleanEndRoomsList()
    {
        endRooms.RemoveAll(i => bigRoomIndexes.Contains(i) || GetNeighbourCount(i) > 1);
    }

    private void SetupSpecialRooms()
    {
        bossRoomIndex = endRooms.Count > 0 ? endRooms[^1] : -1;
        if (bossRoomIndex != -1) endRooms.RemoveAt(endRooms.Count - 1);

        itemRoomIndex = RandomEndRoom();
        forgeRoomIndex = RandomEndRoom();
        healRoomIndex = PickHealRoom();
    }

    private int RandomEndRoom()
    {
        if (endRooms.Count == 0) return -1;
        int i = Random.Range(0, endRooms.Count);
        int v = endRooms[i];
        endRooms.RemoveAt(i);
        return v;
    }

    private int PickHealRoom()
    {
        int[] forbidden = { bossRoomIndex, itemRoomIndex, forgeRoomIndex };

        for (int i = 0; i < floorPlan.Length; i++)
        {
            if (floorPlan[i] != 0) continue;

            var neigh = GetNeighbours(i);
            if (neigh.Count != 1) continue;

            int n = neigh[0];
            if (forbidden.Contains(n)) continue;

            var second = GetNeighbours(n);
            if (second.Any(x => forbidden.Contains(x)))
                return i;
        }

        return -1;
    }

    private bool VisitCell(int index)
    {
        if (floorPlan[index] != 0 ||
            GetNeighbourCount(index) > 1 ||
            floorPlanCount > maxRooms)
            return false;

        // Try shape rooms
        // if (Random.value < 0.3f && index != startIndex)
        // {
        //     foreach (var shape in roomShapes.OrderBy(_ => Random.value))
        //     {
        //         if (TryPlaceRoom(index, shape))
        //             return true;
        //     }
        // }

        cellQueue.Enqueue(index);
        floorPlan[index] = 1;
        floorPlanCount++;
        return true;
    }

    // private bool TryPlaceRoom(int origin, int[] offsets)
    // {
    //     List<int> tiles = new() { origin };

    //     foreach (int off in offsets)
    //     {
    //         int id = origin + off;
    //         if (id < 0 || id >= floorPlan.Length || floorPlan[id] != 0)
    //             return false;

    //         tiles.Add(id);
    //     }

    //     foreach (int t in tiles)
    //     {
    //         floorPlan[t] = 1;
    //         floorPlanCount++;
    //         bigRoomIndexes.Add(t);
    //     }

    //     return true;
    // }

    private List<int> GetNeighbours(int index)
    {
        List<int> r = new();
        int x = index % 10;
        int y = index / 10;

        void Check(int idx)
        {
            if (idx >= 0 && idx < floorPlan.Length && floorPlan[idx] == 1)
                r.Add(idx);
        }

        if (x > 0) Check(index - 1);
        if (x < 9) Check(index + 1);
        if (y > 0) Check(index - 10);
        if (y < 9) Check(index + 10);

        return r;
    }

    private int GetNeighbourCount(int index)
    {
        int x = index % 10;
        int y = index / 10;
        int c = 0;

        if (x > 0) c += floorPlan[index - 1];
        if (x < 9) c += floorPlan[index + 1];
        if (y > 0) c += floorPlan[index - 10];
        if (y < 9) c += floorPlan[index + 10];

        return c;
    }
}

public class LayoutData
{
    public int[] floorPlan;
    public List<int> bigRoomIndexes;

    public int startIndex;
    public int itemRoom;
    public int forgeRoom;
    public int healRoom;
    public int bossRoom;
}
