using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemData item;
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