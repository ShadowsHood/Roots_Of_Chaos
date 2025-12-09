using UnityEngine;
using System.Collections.Generic;

public class MapDisplay : MonoBehaviour
{
    [Header("Prefabs")]
    public Cell cellPrefab;

    [Header("Sprites")]
    public Sprite start;
    public Sprite item;
    public Sprite forge;
    public Sprite heal;
    public Sprite boss;

    private float cellSize = 0.5f;

    private Transform mapRoot;

    private List<Cell> spawnedCells = new List<Cell>();

    void Awake()
    {
        mapRoot = new GameObject("mapRoot").transform;
    }

    public void SpawnRooms()
    {
        ClearRooms();

        for (int index = 0; index < FloorMap.Instance.rooms.Length; index++)
        {
            RoomData data = FloorMap.Instance.rooms[index];
            if (!data.filled) continue;
            SpawnCell(index, data.type);
        }
    }

    void ClearRooms()
    {
        foreach (var c in spawnedCells)
        {
            if (c != null)
                Destroy(c.gameObject);
        }
        spawnedCells.Clear();
    }

    private void SpawnCell(int index, RoomType type)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 pos = new Vector2(x * cellSize, -y * cellSize);
        Cell c = Instantiate(cellPrefab, pos, Quaternion.identity, mapRoot);
        c.index = index;

        Sprite spr = GetSpriteForType(type);
        if (spr != null)
            c.SetSpecialRoomSprite(spr);

        spawnedCells.Add(c);
    }

    private Sprite GetSpriteForType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start: return start;
            case RoomType.Boss: return boss;
            case RoomType.Forge: return forge;
            case RoomType.Heal: return heal;
            case RoomType.Light: return item;
            case RoomType.Normal: default: return null;
        }
    }
}
