using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance { get; private set; }
    public Cell cellPrefab;

    [Header("Sprites")]
    public Sprite start;
    public Sprite item;
    public Sprite forge;
    public Sprite heal;
    public Sprite boss;

    // Layout
    private float cellSize = 25f;
    private int mapWidth = 10;

    // Zoom settings
    public float normalScale = 1f;
    public float zoomedScale = 2.5f;
    public float zoomSpeed = 0.2f;

    private Transform minimapRoot;
    private List<Cell> spawnedCells = new List<Cell>();
    private PlayerInput playerInput;


    void Awake()
    {
        Instance = this;
        minimapRoot = GetComponent<RectTransform>();
    }

    private void Update()
    {
        bool tabPressed = Keyboard.current.tabKey.isPressed;
        minimapRoot.localScale = Vector3.Lerp(minimapRoot.localScale, Vector3.one * (tabPressed ? zoomedScale : normalScale), Time.deltaTime * zoomSpeed);
    }

    public void UpdateMinimap()
    {
        ClearCells();
        List<Vector2> positions = new();

        for (int index = 0; index < FloorMap.Instance.rooms.Length; index++)
        {
            RoomData data = FloorMap.Instance.rooms[index];
            if (!data.filled) continue;
            // if (!data.filled || !data.visited) continue;

            int x = index % mapWidth;
            int y = index / mapWidth;

            positions.Add(new Vector2(
                x * cellSize,
                -y * cellSize
            ));
        }

        if (positions.Count == 0)
            return;
        Vector2 min = positions[0];
        Vector2 max = positions[0];
        foreach (var p in positions)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Vector2 centerOffset = -(min + max) / 2f;
        int i = 0;
        for (int index = 0; index < FloorMap.Instance.rooms.Length; index++)
        {
            RoomData data = FloorMap.Instance.rooms[index];
            if (!data.filled) continue;
            // if (!data.filled || !data.visited) continue;

            SpawnCell(index, data.type, positions[i] + centerOffset);
            i++;
        }
    }

    void ClearCells()
    {
        foreach (var c in spawnedCells)
            if (c != null)
                Destroy(c.gameObject);

        spawnedCells.Clear();
    }

    void SpawnCell(int index, RoomType type, Vector2 pos)
    {
        Cell cell = Instantiate(cellPrefab, minimapRoot);
        RectTransform rt = cell.GetComponent<RectTransform>();

        rt.anchoredPosition = pos;
        rt.sizeDelta = Vector2.one * cellSize;
        rt.localScale = Vector3.one;

        cell.index = index;

        Sprite spr = GetSpriteForType(type);
        if (spr != null)
            cell.SetSpecialRoomSprite(spr);

        spawnedCells.Add(cell);
    }

    private Sprite GetSpriteForType(RoomType type)
    {
        // switch (type)
        // {
        //     case RoomType.Start: return start;
        //     case RoomType.Boss: return boss;
        //     case RoomType.Forge: return forge;
        //     case RoomType.Heal: return heal;
        //     case RoomType.Light: return item;
        //     case RoomType.Normal: default: return null;
        // }
        return type switch
        {
            RoomType.Start => start,
            RoomType.Boss => boss,
            RoomType.Forge => forge,
            RoomType.Heal => heal,
            RoomType.Light => item,
            _ => null
        };
    }
}
