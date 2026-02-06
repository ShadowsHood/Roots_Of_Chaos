using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemData item;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void SetItemData(ItemData newItem)
    {
        item = newItem;
        if (sr != null && item != null)
        {
            sr.sprite = item.icon;
        }
    }

    // void Start()
    // {
    // }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (InventoryController.Instance != null)
            {
                InventoryController.Instance.Pickup(item);
                Destroy(gameObject);
            }
        }
    }

}