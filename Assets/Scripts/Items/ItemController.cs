using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemData item;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryController ic = collision.GetComponent<InventoryController>();
            if (ic != null)
            {
                ic.Pickup(item);
                Destroy(gameObject);
            }
        }
    }
}