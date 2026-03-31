using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopRoom : MonoBehaviour
{
    public ItemPool[] pools;
    public GameObject itemPrefab;
    public Transform[] itemSpawnPoints;
    public int minItems = 2;
    public int maxItems = 4;
    public bool isFreeShop = false;

    private Transform itemRoot;

    void Awake()
    {
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent != null) itemRoot = itemsParent.transform;
    }

    public void GenerateShop()
    {
        int luckShift = GameManager.runStats.Luck / 15;
        int itemsNumber = Random.Range(minItems + luckShift, maxItems + luckShift + 1);
        itemsNumber = Mathf.Clamp(itemsNumber, minItems, itemSpawnPoints.Length);

        List<ItemData> chosenItems = new List<ItemData>();
        List<Transform> availablePoints = new List<Transform>(itemSpawnPoints);
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
                float finalPrice = isFreeShop ? 0 : Mathf.Clamp(basePrice + rand, 0.1f, 0.5f);
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

        int luck = GameManager.runStats.Luck;

        foreach (var wi in pool)
        {
            if (!excludes.Contains(wi.item))
            {
                float weightModifier = 1f;
                if (wi.weight < 40)
                {
                    weightModifier += luck * 1.5f;
                }
                else if (wi.weight >= 80)
                {
                    weightModifier = 1f / (1f + (luck * 0.1f));
                }
                validItems.Add(wi);
                // arrondi a l'int
                totalWeight += Mathf.RoundToInt(wi.weight * weightModifier);
            }
        }

        if (validItems.Count == 0) return null;

        float roll = Random.Range(0f, totalWeight);
        float cursor = 0;

        foreach (var wi in validItems)
        {
            float m = 1f;
            if (wi.weight < 40) m += luck * 1.5f;
            else if (wi.weight >= 80) m = 1f / (1f + (luck * 0.1f));

            cursor += wi.weight * m;
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