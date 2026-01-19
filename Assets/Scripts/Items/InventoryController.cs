using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    private PlayerStats stats => GameManager.runStats;
    private List<ItemData> inventory = new();

    public void Pickup(ItemData item)
    {
        if (item == null) return;
        inventory.Add(item);
        item.Apply(stats);
    }
}
