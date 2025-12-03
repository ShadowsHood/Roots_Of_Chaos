using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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

    [Header("Large Rooms")]
    public Sprite largeRoom;
    public Sprite LRoom;
    public Sprite verticalRoom;
    public Sprite horizontalRoom;

    private float cellSize = 0.5f;

    private List<Cell> spawnedCells = new();
    private LayoutGenerator generator = new();
    private LayoutData layout;

    public int startIndex = 75;

    void Start()
    {
        GenerateAndDisplay();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            GenerateAndDisplay();
    }

    void GenerateAndDisplay()
    {
        ClearDisplay();

        layout = generator.Generate(startIndex);

        DisplayRooms();
        DisplayLargeRooms();
        ApplySpecialSprites();
    }

    void ClearDisplay()
    {
        foreach (var c in spawnedCells)
            Destroy(c.gameObject);

        spawnedCells.Clear();
    }

    void DisplayRooms()
    {
        for (int i = 0; i < layout.floorPlan.Length; i++)
        {
            if (layout.floorPlan[i] == 0) continue;
            // if (layout.bigRoomIndexes.Contains(i)) continue; // handled separately

            SpawnCell(i, cellPrefab);
        }
    }

    void SpawnCell(int index, Cell prefab)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 pos = new Vector2(x * cellSize, -y * cellSize);
        Cell c = Instantiate(prefab, pos, Quaternion.identity);
        c.index = index;

        spawnedCells.Add(c);
    }

    void DisplayLargeRooms()
    {
        // Tu pourras reprendre la logique de SpawnLargeRoom ici
    }

    void ApplySpecialSprites()
    {
        foreach (var c in spawnedCells)
        {
            Sprite spr = null;

            if (c.index == layout.startIndex) spr = start;
            else if (c.index == layout.itemRoom) spr = item;
            else if (c.index == layout.forgeRoom) spr = forge;
            else if (c.index == layout.healRoom) spr = heal;
            else if (c.index == layout.bossRoom) spr = boss;

            if (spr != null)
                c.SetSpecialRoomSprite(spr);
        }
    }
}
