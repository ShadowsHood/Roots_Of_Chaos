using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class WeightedItem
{
    public ItemData item;
    public int weight; // 100 = Commun, 10 = Rare, 1 = Légendaire
}

[CreateAssetMenu(fileName = "NewItemPool", menuName = "Data/ItemPool")]
public class ItemPool : ScriptableObject
{
    public List<WeightedItem> pool = new List<WeightedItem>();

    public ItemData GetRandomItem()
    {
        if (pool.Count == 0) return null;
        int totalWeight = 0;
        foreach (var i in pool) totalWeight += i.weight;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (var i in pool)
        {
            cursor += i.weight;
            if (roll < cursor) return i.item;
        }
        return pool[0].item;
    }

    [ContextMenu("Auto-Fill Pool from Folder")]
    private void AutoFill()
    {
        pool.Clear();
        string[] guids = AssetDatabase.FindAssets("t:ItemData");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);

            if (item != null)
            {
                pool.Add(new WeightedItem { item = item, weight = 50 });
            }
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"Pool filled with {pool.Count} items !");
    }
}