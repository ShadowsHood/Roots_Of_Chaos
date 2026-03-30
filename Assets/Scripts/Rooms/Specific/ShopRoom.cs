using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopRoom : MonoBehaviour
{
    public ItemPool[] pools;
    public GameObject itemPrefab;
    public Transform[] spawnPoints;
    public int minItems = 2;
    public int maxItems = 4;

    private Transform itemRoot;

    void Awake()
    {
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent != null) itemRoot = itemsParent.transform;
    }

    public void GenerateShop()
    {
        int itemsNumber = Random.Range(minItems, Mathf.Min(maxItems, spawnPoints.Length) + 1);

        List<ItemData> chosenItems = new List<ItemData>();
        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        ShuffleList(availablePoints);

        List<WeightedItem> mergedPool = new List<WeightedItem>();
        foreach (var p in pools)
        {
            mergedPool.AddRange(p.pool);
        }

        for (int i = 0; i < itemsNumber; i++)
        {
            WeightedItem selectedWI = GetRandomWeightedItem(mergedPool, chosenItems);
            if (selectedWI != null && selectedWI.item != null)
            {
                ItemData newItem = selectedWI.item;
                chosenItems.Add(newItem);

                GameObject instance = Instantiate(itemPrefab, availablePoints[i].position, Quaternion.identity, itemRoot);

                // Corruption price calculation
                // Inverse : 1 -> 0.5 | 100 -> 0.1
                float rarityFactor = (selectedWI.weight - 1f) / 99f;
                float basePrice = Mathf.Lerp(0.5f, 0.1f, rarityFactor);
                float rand = Random.Range(0.01f, 0.05f) * (Random.value > 0.5f ? 1 : -1);
                float finalPrice = Mathf.Clamp(basePrice + rand, 0.1f, 0.5f);
                instance.GetComponent<ItemController>().SetItemData(newItem, finalPrice);
            }
            else
            {
                Debug.LogWarning("No valid item found for the shop.");
            }
        }
    }

    private WeightedItem GetRandomWeightedItem(List<WeightedItem> pool, List<ItemData> excludes)
    {
        int totalWeight = 0;
        List<WeightedItem> validItems = new List<WeightedItem>();

        foreach (var wi in pool)
        {
            if (!excludes.Contains(wi.item))
            {
                validItems.Add(wi);
                totalWeight += wi.weight;
            }
        }

        if (validItems.Count == 0) return null;

        int roll = Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (var wi in validItems)
        {
            cursor += wi.weight;
            if (roll <= cursor) return wi;
        }
        return validItems[0];
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}