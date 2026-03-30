using UnityEngine;

public class ItemGeneratorOld : MonoBehaviour
{
    public ItemData[] items;
    public GameObject itemPrefab;

    private Transform itemRoot;

    void Awake()
    {
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent != null)
        {
            itemRoot = itemsParent.transform;
        }
    }

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
            GameObject itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity, itemRoot);
            ItemController itemCtrl = itemInstance.GetComponent<ItemController>();
            itemCtrl.SetItemData(itemData);
        }
    }

}