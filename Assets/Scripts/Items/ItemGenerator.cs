using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public ItemData[] items;
    public GameObject itemPrefab;

    public ItemData GetRandomItem()
    {
        if (items.Length == 0) return null;
        int index = Random.Range(0, items.Length);
        return items[index];
    }

    void Start()
    {
        if (items != null)
        {
            ItemData itemData = GetRandomItem();
            GameObject itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            ItemController itemCtrl = itemInstance.GetComponent<ItemController>();
            itemCtrl.SetItemData(itemData);
        }
    }

}